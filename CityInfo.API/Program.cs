var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMvc();

var app = builder.Build();

app.UseMvc();

app.MapGet("/", () => "Hello World!");

app.Run();
