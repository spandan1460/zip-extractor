# Logging

This template uses Microsoft logger on par with OpenTelemetry to provide a consistent logging experience across the FbM stack.  
* https://learn.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line
* https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-7.0

# Using Microsoft Logger

## Setting Up Logger

1. Add an `Action<ResourceBuilder>` method to `Program.cs`. This method is used to build a resource that represents the identity and attributes of the service you are monitoring. Resources are used in observability tools to help distinguish and categorize telemetry data

   ```csharp
   void ConfigureResource(ResourceBuilder r) 
    => r.AddService(
        serviceName: serviceName,
        serviceVersion: serviceVersion, 
        serviceInstanceId: Environment.MachineName);
   ```
   
2. Configure log sinks to utilize OpenTelemetry exporter

   ```csharp
        builder
            .ClearProviders()
            .AddOpenTelemetry(options =>
            {
                var resourceBuilder = ResourceBuilder.CreateDefault();
                configureResource(resourceBuilder);
                options.IncludeScopes = true;
                options.SetResourceBuilder(resourceBuilder);
                options.AddOtlpExporter(otlpOptions => otlpOptions.Endpoint = new Uri(otlpEndpoint));
            });
   ```

3. In your code, import the necessary namespaces:

   ```csharp
   using Microsoft.Extensions.Logging;
   ```

4. Create an instance of the `ILogger` interface using Dependency Injection (DI). For example, in a controller or service class:

   ```csharp
   private readonly ILogger<MyClass> _logger;

   public MyClass(ILogger<MyClass> logger)
   {
       _logger = logger;
   }
   ```

## Using Structured Logging

**Structured logging is a recommended practice for capturing logs with context and information that can be easily analyzed.** Use the `LogInformation`, `LogWarning`, `LogError`, and other logging methods to log structured information. Here's an example of structured logging:

```csharp
_logger.LogInformation("User {UserId} authenticated successfully.", userId);
```

In this example, `{UserId}` is a placeholder for the actual user Id. The `userId` variable will be associated with the logged message, making it easier to filter and analyze logs.

***Please do not use string interpolation on logs.*** This is not recommended:

```csharp
_logger.LogInformation($"User {userId} authenticated successfully.");
```

## Log Levels

- `LogInformation`: Use this to log general information.
- `LogWarning`: Use this to log non-critical issues.
- `LogError`: Use this to log errors.
- `LogCritical`: Use this to log critical issues that require immediate attention.

## Logging Best Practices

- Avoid using interpolated strings for logs, as structured logging provides better context and flexibility for analysis.
- Include relevant information and context in your logs.
- Use appropriate log levels to categorize log entries correctly.

## CorrelationId / RequestId values in logs:

What is important across the FbM stack is we use the very same correlation technique across all services, hence the guidance here:

If the caller provides X-MAERSK-RID it will appear as the correlationId/requestId in the logs, the format is such that this id should be chained across service calls.

If no X-MAERSK-RID value is supplied, the current request `TraceId` will be added to the logs as the correlationId/requestId.

See 
* [src/Maersk/FbM/OCT/Extensions/HttpContextExtensions.cs](./src/Maersk/FbM/OCT/Extensions/HttpContextExtensions.cs) - retrieves or adds requestId from HttpContext

When you call other services, add the parameter X-MAERSK-RID to the HTTPS request so that the calling service also absorbs your present RID value.

```csharp
// Making a remote service call to another micro-service ensure the X-MAERSK-RID is provided to chain the RID's together.
string rid = context.GetOrAddRid();
request.Headers.Add(HttpContextExtensions.HeaderName, rid);
```


## Add common context markers to every log entry

Every log should have both client name and requestId, this is done by adding a middleware to the pipeline.

1. Create a custom middleware to intercept incoming requests and extract the client information and X-MAERSK-RID from the request.

   See
   * [src/Maersk/FbM/OCT/Middlewares/LoggingMiddleware.cs](./src/Maersk/FbM/OCT/Middlewares/LoggingMiddleware.cs) - middleware that adds client name and requestId to the log entry

2. Create a filter to add common markers only where they are needed.
   
   ```csharp
   private static bool ShouldUseLoggingMiddleware(HttpContext context)
    {
        var path = context.Request.Path.Value;
        return !string.IsNullOrEmpty(path) &&
               !path.Contains("health", StringComparison.InvariantCultureIgnoreCase) &&
               !path.Contains("version", StringComparison.InvariantCultureIgnoreCase);
    }
   ```

3. Register the custom middleware in your `Progam.cs`.

    ```csharp
    app.UseLoggingMiddleware();
    ```


