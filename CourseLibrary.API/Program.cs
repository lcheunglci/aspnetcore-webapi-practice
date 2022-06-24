using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();
builder.Services.AddDbContext<CourseLibraryContext>(options =>
{
    options.UseSqlServer(
        @"Server=(localdb)\mssqllocaldb;Database=CourseLibraryDB;Trusted_Connection=True;");
});

// register PropertyMappingService
builder.Services.AddTransient<IPropertyMappingService, PropertyMappingService>();

// register PropertyCheckerService
builder.Services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddResponseCaching();

builder.Services.AddControllers(setupAction =>
{
    setupAction.ReturnHttpNotAcceptable = true;

    setupAction.CacheProfiles.Add("240SecondsCacheProfile", new CacheProfile()
    {
        Duration = 240
    });
    //setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
})
    .AddNewtonsoftJson(setupAction =>
    {
        setupAction.SerializerSettings.ContractResolver =
            new CamelCasePropertyNamesContractResolver();

    })
    .AddXmlDataContractSerializerFormatters()
    .ConfigureApiBehaviorOptions(setupAction =>
    {
        setupAction.InvalidModelStateResponseFactory = context =>
        {
            // create a problem details object
            var problemDetailFactory = context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
            var problemDetails = problemDetailFactory.CreateValidationProblemDetails(
                context.HttpContext,
                context.ModelState);

            // add additional info not added by default
            problemDetails.Detail = "See the errors field for details.";
            problemDetails.Instance = context.HttpContext.Request.Path;


            // find out which status code to use
            var actionExecutionContext = context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

            // if there are ModelState errors and all arguments were correctly found/parsed
            // we're dealing with validation errors

            if ((context.ModelState.ErrorCount > 0) &&
                (actionExecutionContext?.ActionArguments.Count == context.ActionDescriptor.Parameters.Count))
            {
                problemDetails.Type = "https://courselibrary.com/modelvalidationproblem";
                problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                problemDetails.Title = "One or more validation errors occurred.";

                return new UnprocessableEntityObjectResult(problemDetails)
                {
                    ContentTypes = { "application/problem+json" }
                };
            };

            // if one of the arguments wasn't correctly found / couldn't be parsed
            // we are dealing with null/unparseable input

            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Title = "One or more errors on input occurred.";
            return new BadRequestObjectResult(problemDetails)
            {
                ContentTypes = { "application/problem+json" }
            };
        };
    });



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MvcOptions>(config =>
{
    //config.InputFormatters.OfType<NewtonsoftJsonInputFormatter>()
    //.First(f => !(f is NewtonsoftJsonPatchInputFormatter))
    //.SupportedMediaTypes.Add("application/vnd.marvin.hateoas+json");

    var newtonsoftJsonOutputFormatter = config.OutputFormatters.OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

    if (newtonsoftJsonOutputFormatter != null)
    {
        newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.marvin.hateoas+json");
    }
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetService<CourseLibraryContext>();
        // for demo purposes, delete the database & migrate on startup so 
        // we can start with a clean slate
        context?.Database.EnsureDeleted();
        context?.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler(appBuilder =>
    {
        appBuilder.Run(async context =>
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
        });
    });
}

app.UseResponseCaching();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    app.MapControllers();
});

app.Run();
