# Expense Management API

This project is a .NET Core Web API for processing expense data embedded in text. It extracts the necessary data, calculates sales tax, and provides the extracted and calculated data via a RESTful API.
Access the API via Swagger:
After running the API, you can access the Swagger UI by navigating to the following URL in your web browser:http://localhost:5253/swagger/index.html

Input string : "Hi Patricia,\nPlease create an expense claim for the below. Relevant details are marked up as requested…\n<expense><cost_centre>DEV632</cost_centre><total>35,000</total><payment_method>personal card</payment_method></expense>\nFrom: William Steele\nSent: Friday, 16 June 2022 10:32 AM\nTo: Maria Washington\nSubject: test\nHi Maria,\nPlease create a reservation for 10 at the <vendor>Seaside Steakhouse</vendor> for our <description>development team’s project end celebration</description> on <date>27 April 2022</date> at 7.30pm.\nRegards,\nWilliam"
