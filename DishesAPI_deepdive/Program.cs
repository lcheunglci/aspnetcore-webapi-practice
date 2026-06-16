using DishesAPI.DbContexts;
using DishesAPI.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails();

builder.Services.AddValidation();

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

builder.Services.AddAuthorizationBuilder()
	.AddPolicy("RequireAdminFromBelgium", policy =>
		policy.RequireAuthenticatedUser()
		.RequireRole("admin")
		.RequireClaim("country", "Belgium"));

// connection string from appSettings
builder.Services.AddDbContext<DishesDbContext>(o => o.UseSqlite(
	builder.Configuration["ConnectionStrings:DishesDBConnectionString"]));

builder.Services.AddOpenApi("v1", options =>
{
	options.AddDocumentTransformer((document, context, cancellationToken) =>
	{
		document.Info = new OpenApiInfo
		{
			Title = "DishesAPI",
			Version = "1",
			Description = "An API for managing dishes and their ingredients."
		};
		document.Components ??= new OpenApiComponents();
		document.Components.SecuritySchemes ??=
			new Dictionary<string, IOpenApiSecurityScheme>();
		document.Components.SecuritySchemes["Bearer"] =
			new OpenApiSecurityScheme
			{
				Type = SecuritySchemeType.Http,
				Scheme = "bearer",
				BearerFormat = "JWT",
				Description = "Enter a valid JWT bearer token."
			};
		document.Security ??= [];
		document.Security.Add(new OpenApiSecurityRequirement
		{
			[new OpenApiSecuritySchemeReference("Bearer")] =
				new List<string>()
		});

		var paths = document.Paths.ToDictionary(
			p => p.Key.Replace("{version}", document.Info.Version),
			p => p.Value);
		document.Paths.Clear();
		foreach (var (key, value) in paths)
		{
			document.Paths.Add(key, value);
		}

		return Task.CompletedTask;
	});

});

builder.Services.AddOpenApi("v2", options =>
{
	options.AddDocumentTransformer((document, context, cancellationToken) =>
	{
		document.Info = new OpenApiInfo
		{
			Title = "DishesAPI",
			Version = "2",
			Description = "An API for managing dishes and their ingredients."
		};
		document.Components ??= new OpenApiComponents();
		document.Components.SecuritySchemes ??=
			new Dictionary<string, IOpenApiSecurityScheme>();
		document.Components.SecuritySchemes["Bearer"] =
			new OpenApiSecurityScheme
			{
				Type = SecuritySchemeType.Http,
				Scheme = "bearer",
				BearerFormat = "JWT",
				Description = "Enter a valid JWT bearer token."
			};
		document.Security ??= [];
		document.Security.Add(new OpenApiSecurityRequirement
		{
			[new OpenApiSecuritySchemeReference("Bearer")] =
				new List<string>()
		});

		var paths = document.Paths.ToDictionary(
			p => p.Key.Replace("{version}", document.Info.Version),
			p => p.Value);
		document.Paths.Clear();
		foreach (var (key, value) in paths)
		{
			document.Paths.Add(key, value);
		}

		return Task.CompletedTask;
	});

});

builder.Services.AddApiVersioning(options =>
{
	options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
	options.AssumeDefaultVersionWhenUnspecified = true;
	options.ReportApiVersions = true;
	options.ApiVersionReader = new Asp.Versioning.UrlSegmentApiVersionReader();
});

builder.Services.RegisterAllEndpoints();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler();
}

app.UseHttpsRedirection();

app.UseStatusCodePages();

// openapi/v1.json
app.MapOpenApi();

if (app.Environment.IsDevelopment())
{
	app.MapScalarApiReference();
}

// For demo purposes, temp disable authNZ
//app.UseAuthentication();
//app.UseAuthorization();

app.MapGet("/testerror", () =>
{
	throw new NotImplementedException();
});

// app.RegisterDishesEndpoints();
// app.RegisterIngredientsEndpoints();
app.MapEndpoints();

// recreate & migrate the database on each run, for demo purposes
using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
	var context = serviceScope.ServiceProvider.GetRequiredService<DishesDbContext>();
	context.Database.EnsureDeleted();
	context.Database.Migrate();
}

app.Run();
