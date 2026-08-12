using EmployeeManagement;
using EmployeeManagement.DataAccess.Entities;
using EmployeeManagement.Middleware;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
});
builder.Services.AddAutoMapper(config => { },
    AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient();
builder.Services.RegisterBusinessServices();
builder.Services.RegisterDataServices(builder.Configuration);

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:5001";
        options.TokenValidationParameters.ValidateAudience = false;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBeAdmin", policy =>
        policy.RequireRole("Admin"));
});

var app = builder.Build();

app.UseMiddleware<EmployeeManagementSecurityHeadersMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Minimal API endpoints
app.MapGet("/api/courses", async (
    EmployeeManagement.DataAccess.Services.IEmployeeManagementRepository repository) =>
{
    var courses = await repository.GetCoursesAsync();
    return Results.Ok(courses.Select(c => new { c.Id, c.Title }));
});

app.MapPost("/api/courses", async (
    EmployeeManagement.DataAccess.Services.IEmployeeManagementRepository repository,
    CourseForCreationDto courseForCreation) =>
{
    if (string.IsNullOrWhiteSpace(courseForCreation.Title))
    {
        return Results.ValidationProblem(
            new Dictionary<string, string[]>
            {
                { "Title", new[] { "The Title field is required." } }
            });
    }

    var course = new EmployeeManagement.DataAccess.Entities.Course(
        courseForCreation.Title);
    repository.AddCourse(course);
    await repository.SaveChangesAsync();

    return Results.Created($"/api/courses/{course.Id}",
        new { course.Id, course.Title });
});

app.Run();


public record CourseForCreationDto(string? Title);
