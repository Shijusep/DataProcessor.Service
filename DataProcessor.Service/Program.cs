
using DataProcessor.Service.Interface;
using DataProcessor.Service.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace DataProcessor.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Configure SalesTaxOptions from the appsettings.json file
            builder.Services.Configure<SalesTaxOptions>(builder.Configuration.GetSection("SalesTax"));

            builder.Services.AddControllers();
            // Add XML formatters
            builder.Services.AddControllers()
                .AddXmlSerializerFormatters();

            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register MediatR for CQRS, specifying the assembly where your handlers are located
            builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

            // Register your services
            builder.Services.AddScoped<IExpenseService, ExpenseService>();
           
            // Register API versioning
            builder.Services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });

            // Configure logging
            builder.Logging.ClearProviders(); // Clears the default logging providers
            builder.Logging.AddConsole();     // Adds Console logging
            builder.Logging.AddDebug();       // Adds Debug logging
            
            // Build the app
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
