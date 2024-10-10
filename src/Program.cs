using Maersk.FbM.OCT.BusinessLogic;
using Maersk.FbM.OCT.Extensions;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

var serviceName = builder.Configuration.GetValue<string>("AppSettings:Application")!;
var serviceVersion = VersionExtensions.GetInformationalVersion();
var otlpEndpoint = builder.Configuration.GetValue<string>("Otlp:Endpoint")!;

// Add services to the container.
builder.Services.AddLogging(ConfigureResource, otlpEndpoint);
builder.Services.AddOpenTelemetry(ConfigureResource, otlpEndpoint);
builder.Services.AddScoped<IWeatherService, ApiWeatherGovService>();
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddApiHealthChecks();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();

var app = builder.Build();

try
{
    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    else
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapVersion();
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthorization();
    app.MapRazorPages();
    app.MapHealthChecks("/healthz");
    app.UseLoggingMiddleware();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Unhandled exception");
    throw;
}

return;

void ConfigureResource(ResourceBuilder r) 
    => r.AddService(
        serviceName: serviceName,
        serviceVersion: serviceVersion, 
        serviceInstanceId: Environment.MachineName);