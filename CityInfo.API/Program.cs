using Asp.Versioning;
using CityInfo.API.DbContexts;
using CityInfo.API.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;

Log.Logger = new LoggerConfiguration()
	.MinimumLevel.Debug()
	.WriteTo.Console()
	.WriteTo.File("logs/cityinfo.txt", rollingInterval: RollingInterval.Day)
	.CreateLogger();

var builder = WebApplication.CreateBuilder(args);
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers()
	.AddXmlDataContractSerializerFormatters();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

//var apiVersions = new[] { "v1", "v2" };
//foreach (var apiVersion in apiVersions)
//{
//	builder.Services.AddOpenApi(apiVersion);
//}

builder.Services.AddOpenApi("v1", options =>
{
	options.AddDocumentTransformer((document, context, cancellationToken) =>
	{
		document.Info = new()
		{
			Title = "City Info API",
			Version = context.DocumentName,
			Description = "Through this API you can access cities and their points of interest.",
		};

		return Task.CompletedTask;
	});
});
builder.Services.AddOpenApi("v2", options =>
{
	options.AddDocumentTransformer((document, context, cancellationToken) =>
	{
		document.Info = new()
		{
			Title = "City Info API",
			Version = context.DocumentName,
			Description = "Through this API you can access cities and their points of interest.",
		};

		return Task.CompletedTask;
	});
});
// builder.Services.AddOpenApi("v2");

builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();

builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

#if DEBUG
builder.Services.AddTransient<IMailService, LocalMailService>();
#else
builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

builder.Services.AddDbContext<CityInfoContext>(dbContextOptions =>
	dbContextOptions.UseSqlite(
		builder.Configuration["ConnectionStrings:CityInfoDBConnectionString"]
			?? throw new InvalidOperationException()));

builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();

builder.Services.AddScoped<IPointOfInterestService, PointOfInterestService>();

builder.Services.AddAutoMapper(config => { },
	AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthentication("Bearer")
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new()
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = builder.Configuration["Authentication:Issuer"],
			ValidAudience = builder.Configuration["Authentication:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(
				Convert.FromBase64String(builder.Configuration["Authentication:SecretForKey"]!))
		};
	});

builder.Services.AddAuthorizationBuilder()
	.AddPolicy("MustBeFromAntwerp", policy =>
	{
		policy.RequireAuthenticatedUser();
		policy.RequireClaim("city", "Antwerp");
	});

builder.Services.AddApiVersioning(setupAction =>
{
	setupAction.ReportApiVersions = true;
	setupAction.AssumeDefaultVersionWhenUnspecified = true;
	setupAction.DefaultApiVersion = new ApiVersion(1, 0);
}).AddMvc()
.AddApiExplorer(setupAction =>
{
	setupAction.SubstituteApiVersionInUrl = true;
	setupAction.GroupNameFormat = "'v'V";
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi("/openapi/{documentName}.json");
	app.MapScalarApiReference(options =>
	{
		options.WithTitle("City Info API")
			.WithTheme(ScalarTheme.Solarized)
			.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
			.AddPreferredSecuritySchemes("Bearer");

	});
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

//app.Run(async (context) =>
//{
//    await context.Response.WriteAsync("Hello World!");
//});

app.Run();
