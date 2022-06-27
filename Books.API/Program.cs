using Books.API.Context;
using Books.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();

var connectionString = builder.Configuration["ConnectionStrings:BooksDBConnectionString"];

builder.Services.AddDbContext<BookContext>(o => o.UseSqlServer(connectionString));

builder.Services.AddScoped<IBookRepository, BooksRepository>();

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
