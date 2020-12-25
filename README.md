# Performance Dashboard Readme

This is a web application that shows daily test results on a line chart. 

## Requirements

*  .NET Framework 4.7.2 or later
*  (**for server**) Internet Information Services to be enabled (via "Turn Windows features on or off")
*  (**for server**) SQL Server or SQL Express

## How to Use

1. On the server machine where you'd like to host the live dashboard web application, run the Performance Dashboard installer (see [**Releases page**](https://code.siemens.com/hakan.yildizhan/performance-dashboard/-/releases) to download the .msi setup file)
2. Integrate Performance Dashboard components into your C# test project. You can do this in two ways:
	* Add Siemens.Sirius.Integration.Performance.dll (found [here](https://code.siemens.com/hakan.yildizhan/performance-dashboard/-/tree/master/nuget/lib/net45)) as a project reference
	* Download the .nupkg offline nuget package (see [**Releases page**](https://code.siemens.com/hakan.yildizhan/performance-dashboard/-/releases) and install it for your project via Nuget Package Library
Note that the client library depends on the **EntityFramework** library, for which you also need to add a reference to your project. (Nuget package installation will automatically do this for you) 

3. In your C# test project, add an App.config file with a connection string with name "**PerformanceDashboardContext**" as shown below:
```xml
	<connectionStrings>
		<add name="PerformanceDashboardContext" connectionString=" Data Source=[YOUR_DATA_SOURCE];Initial Catalog=PerformanceDashboard;MultipleActiveResultSets=true;[YOUR_SECURITY/LOGIN_INFO];" providerName="System.Data.SqlClient" />
	</connectionStrings>
```
4. Create your custom measurement, using the `Integration.Framework.Performance.Model.Measurement` interface:
```csharp
using Integration.Framework.Performance.Model;

class MyMeasurement : Measurement
{
     /// <summary>
     /// Name of the test scenario you want to display on the dashboard.
     /// </summary>
     public string ScenarioName => "MyTestScenario";

     /// <summary>
     /// Full name of the executed method which you are measuring the duration of, using i.e. PerformanceSuite library.
     /// </summary>
     public string FunctionName => "";

     /// <summary>
     /// A KPI value you can set for comparison.
     /// </summary>
     public double ReferenceValue => 2.5d;

     /// <summary>
     /// Actual measured value. This can be set later, after the measurement is complete.
     /// </summary>
     public double ActualValue { get; set; } // set after measurement
}

...

// initialize it in your code:
Measurement myMeasurement = new MyMeasurement();

// after running your measurements, set the actual measurement value
myMeasurement.ActualValue = 2;
```

5. Create an instance of `TestInfo` to provide other neccessary information about your test:
```csharp
using Integration.Framework.Performance.Model;

TestInfo testInfo = new TestInfo()
{
	// Test configuration to be displayed on the dashboard
    Configuration = "MyDeviceConfiguration",
	
	// Name of the test scenario
    Name = "MyTestScenario",

	// Unique identifier for this test
	// optional for DB-logging; could be used for file logging purposes
    TestRunIdentifier = "MyDeviceConfiguration_MyTestScenario"
};
```

6. Initialize your `DBContext`, by either instantiating it or registering it on your IoC container:
```csharp
using Integration.Framework.Performance.Entity;

PerformanceDashboardContext dbContext = new PerformanceDashboardContext();

// or
_serviceProvider.Register<PerformanceDashboardContext>();
```

7. Then, initialize your `IResultLogger`, again, by either instantiating it or registering it on your IoC container:
```csharp
using Integration.Framework.Performance.Model;

IResultLogger logger = new DBLogger(dbContext);

// or
_serviceProvider.Register<IResultLogger, DBLogger>();
```

8. Finally, pass in your `Measurement` list & your `TestInfo` to the `IResultLogger.LogResults` method in order to insert test results into the database:
```csharp
using Integration.Framework.Performance.Model;

IList<Measurement> myMeasurements = new List<MyMeasurement>() { myMeasurement };
logger.LogResults(myMeasurements, testInfo);

// or
IResultLogger logger => _serviceProvider?.Resolve<IResultLogger>();
logger.LogResults(myMeasurements, testInfo);
```

