using Asp.Versioning;
using Library.API.Contexts;
using Library.API.DocumentTransformers;
using Library.API.Endpoints;
using Library.API.Services;
using Library.API.Transformers;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
	.AddXmlDataContractSerializerFormatters();

builder.Services.AddProblemDetails();

builder.Services.AddAuthentication()
	.AddJwtBearer();

builder.Services.AddAuthorization();

builder.Services.AddApiVersioning(
	options =>
	{
		options.DefaultApiVersion = new ApiVersion(1, 0);
		options.AssumeDefaultVersionWhenUnspecified = true;
		options.ReportApiVersions = true;
		options.ApiVersionReader = new UrlSegmentApiVersionReader();
	}).AddMvc()
	.AddApiExplorer(options =>
	{
		options.GroupNameFormat = "'v'VVV";
		options.SubstituteApiVersionInUrl = true;
	});

// builder.Services.AddOpenApi("library-api");
builder.Services.AddOpenApi("v1", options =>
{
	options.AddDocumentTransformer<AddGeneralInformationTransformer>();
	options.AddDocumentTransformer<AddSecurityDescriptionTransformer>();
	options.AddDocumentTransformer<AddDefaultResponseTypeTransformer>();
	options.AddDocumentTransformer<RemoveInternalOperationsTransformer>();
	options.AddOperationTransformer<ResponseDescriptionOperationTransformer>();
	options.AddSchemaTransformer<AddSchemaExamplesTransformer>();
});

builder.Services.AddOpenApi("v2", options =>
{
	options.AddDocumentTransformer<AddGeneralInformationTransformer>();
	options.AddDocumentTransformer<AddSecurityDescriptionTransformer>();
	options.AddDocumentTransformer<AddDefaultResponseTypeTransformer>();
	options.AddDocumentTransformer<RemoveInternalOperationsTransformer>();
	options.AddOperationTransformer<ResponseDescriptionOperationTransformer>();
	options.AddSchemaTransformer<AddSchemaExamplesTransformer>();
});

builder.Services.AddDbContext<LibraryContext>(
	dbContextOptions => dbContextOptions.UseSqlite(
		builder.Configuration["ConnectionStrings:LibraryDBConnectionString"]));

builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();

builder.Services.AddAutoMapper(config => { },
	AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapOpenApi("/openapi/{documentName}.json");
// app.MapOpenApi();

app.MapScalarApiReference(options =>
{
	options
		.WithTitle("Library API")
		.WithTheme(ScalarTheme.DeepSpace)
		.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.MapControllers();

app.MapBookEndpoints();

app.Run();
