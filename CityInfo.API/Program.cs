using Microsoft.AspNetCore.Mvc.Formatters;

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


var app = builder.Build();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseStatusCodePages();

app.MapGet("/", () => "Hello World!");

app.Run();
