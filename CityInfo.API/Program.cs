using CityInfo.API.Context;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("initialize main");

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddControllers();
    builder.Services.AddMvc().AddMvcOptions(
        options =>
        {
            options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
        });
    //.AddNewtonsoftJson(
    //    options =>
    //    {
    //        if (options.SerializerSettings.ContractResolver != null)
    //        {
    //            var castedResolver = options.SerializerSettings.ContractResolver as DefaultContractResolver;
    //            castedResolver.NamingStrategy = null;
    //        }
    //    });


    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();

#if DEBUG
    builder.Services.AddTransient<IMailService, LocalMailService>();
#else
    builder.Services.AddTransient<IMailService, CloudMailService>();
#endif 
    string ConnectionString = @"Server=(localdb)\mssqllocaldb;RelationalDatabaseFacadeExtensions=CityInfoDB, Trused_Connection = true;";
    builder.Services.AddDbContext<CityInfoContext>(o => o.UseSqlServer());


    var app = builder.Build();

    app.UseRouting();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });

    app.UseStatusCodePages();

    app.MapGet("/", () => "Hello World!");

    app.Run();
}
catch (Exception ex)
{
    // NLog: catch setup errors
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}