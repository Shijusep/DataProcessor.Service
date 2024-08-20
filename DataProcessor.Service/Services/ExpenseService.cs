using DataProcessor.Service.Interface;
using DataProcessor.Service.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace DataProcessor.Service.Services;

public class ExpenseService : IExpenseService
{
    //private readonly decimal _salesTaxRate = 0.10m;
    private readonly decimal _salesTaxRate;

    public ExpenseService(IOptions<SalesTaxOptions> salesTaxOptions)=> _salesTaxRate = salesTaxOptions.Value.Rate;
    /// <summary>
    /// ProcessExpense
    /// </summary>
    /// <param name="inputText"></param>
    /// <returns></returns>
    public (ExpenseModel expense, ErrorResponse error) ProcessExpense(string inputText)
    {
        string xmlContent = ExtractXmlContent(inputText);

        if (string.IsNullOrEmpty(xmlContent))
        {
            return (null, new ErrorResponse { Message = "No valid XML content found." });
        }

        return ParseXml(xmlContent);
    }
    /// <summary>
    /// ExtractXmlContent
    /// </summary>
    /// <param name="inputText"></param>
    /// <returns></returns>
    private string ExtractXmlContent(string inputText)
    {
        var matches = Regex.Matches(inputText, @"<[^>]+>.*?</[^>]+>", RegexOptions.Singleline);
        var xmlContent = string.Join(Environment.NewLine, matches);
        return !string.IsNullOrEmpty(xmlContent) ? $"<root>{xmlContent}</root>" : null;
    }
    /// <summary>
    /// ParseXml
    /// </summary>
    /// <param name="xmlContent"></param>
    /// <returns></returns>
    private (ExpenseModel expense, ErrorResponse error) ParseXml(string xmlContent)
    {
        var expense = new ExpenseModel();
        ErrorResponse error = null;

        try
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            var expenseNode = xmlDoc.SelectSingleNode("//expense");
            if (expenseNode == null)
            {
                error = new ErrorResponse { Message = "Missing <expense> tag." };
                return (null, error);
            }

            var costCentreNode = xmlDoc.SelectSingleNode("//cost_centre");
            var totalNode = xmlDoc.SelectSingleNode("//total");
            var paymentMethodNode = xmlDoc.SelectSingleNode("//payment_method");
            var vendorNode = xmlDoc.SelectSingleNode("//vendor");
            var descriptionNode = xmlDoc.SelectSingleNode("//description");
            var dateNode = xmlDoc.SelectSingleNode("//date");

            if (totalNode == null)
            {
                error = new ErrorResponse { Message = "Missing <total> tag." };
                return (null, error);
            }

            expense.CostCentre = costCentreNode?.InnerText ?? "UNKNOWN";
            if (!decimal.TryParse(totalNode.InnerText, out decimal total))
            {
                error = new ErrorResponse { Message = "Invalid total amount." };
                return (null, error);
            }
            expense.Total = total;
            expense.PaymentMethod = paymentMethodNode?.InnerText;
            expense.Vendor = vendorNode?.InnerText;
            expense.Description = descriptionNode?.InnerText;

            if (dateNode != null && DateTime.TryParse(dateNode.InnerText, out DateTime parsedDate))
            {
                expense.Date = parsedDate;
            }

            expense.SalesTax = total * _salesTaxRate;
            expense.TotalExcludingTax = total - expense.SalesTax;

            ExtractUnknownTags(expenseNode, expense.UnknownTags);
        }
        catch (XmlException)
        {
            error = new ErrorResponse { Message = "XML processing error." };
        }

        return (expense, error);
    }
    /// <summary>
    /// ExtractUnknownTags
    /// </summary>
    /// <param name="node"></param>
    /// <param name="unknownTags"></param>
    private void ExtractUnknownTags(XmlNode node, Dictionary<string, string> unknownTags)
    {
        foreach (XmlNode childNode in node.ChildNodes)
        {
            if (childNode.NodeType == XmlNodeType.Text || IsKnownTag(childNode.Name))
            {
                continue;
            }

            unknownTags[childNode.Name] = childNode.InnerText;

            if (childNode.HasChildNodes)
            {
                ExtractUnknownTags(childNode, unknownTags);
            }
        }
    }
    /// <summary>
    /// IsKnownTag
    /// </summary>
    /// <param name="tagName"></param>
    /// <returns></returns>
    private bool IsKnownTag(string tagName)
    {
        var knownTags = new HashSet<string> { "cost_centre", "total", "payment_method", "vendor", "description", "date" };
        return knownTags.Contains(tagName);
    }
}

