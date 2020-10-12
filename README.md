# Performance Dashboard Readme

This is a web application that shows daily test results on a line chart. 

## Requirements

*  .NET Framework 4.7.2 or later
*  Internet Information Services to be enabled (via "Turn Windows features on or off")
*  SQL Server or SQL Express 

## Installation

*  Restore database "PerformanceDashboard" using the .bak file under setup/database folder
*  Update the project name on database with this script: UPDATE Configuration SET Value = 'Your Project Name'
*  Unzip the website files under setup/website & deploy the website on IIS with an App Pool with name "PerformanceDashboard"
*  Modify the connection string in web.config file with the server address of your database
*  In your test project, use the Entity classes & DashboardContext class under PerformanceDashboard.Entity project to interact with the database. Note that your test project has to have EntityFramework nuget library & an app.config file that includes "entityFramework" and "conectionStrings" tags found in the web.config file of PerformanceDashboard web project.