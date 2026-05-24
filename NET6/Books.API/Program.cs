using Books.API.Context;
using Books.API.Services;
using Microsoft.EntityFrameworkCore;


// throttle the thread pool (set available threads to amount of processors)
ThreadPool.SetMaxThreads(Environment.ProcessorCount, Environment.ProcessorCount);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration["ConnectionStrings:BooksDBConnectionString"];

builder.Services.AddDbContext<BookContext>(o => o.UseSqlServer(connectionString));


builder.Services.AddHttpClient();
builder.Services.AddScoped<IBooksRepository, BooksRepository>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.Configure<ApplicationBuilder>(config =>
{

    if (!builder.Environment.IsProduction())
    {
        config.UseDeveloperExceptionPage();
    }

    config.UseHttpsRedirection();

});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
