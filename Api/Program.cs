using Api.Services;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var benefitsConfig = new BenefitsConfiguration();
// Load configuration into a strongly-typed object
builder.Configuration.GetSection("BenefitsConfiguration").Bind(benefitsConfig);

// Load configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json");
// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// Register EmployeeService with benefitsConfig
builder.Services.AddScoped<IEmployeeService>(provider => new EmployeeService(Options.Create(benefitsConfig)));
builder.Services.AddScoped<IDependentService, DependentService>();

builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Employee Benefit Cost Calculation Api",
        Description = "Api to support employee benefit cost calculations"
    });
});

var allowLocalhost = "allow localhost";
builder.Services.AddCors(options =>
{
    options.AddPolicy(allowLocalhost,
        policy => { policy.WithOrigins("http://localhost:3000", "http://localhost"); });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HttpsRedirection should preced Cors in request processing pipeline
app.UseHttpsRedirection();

app.UseCors(allowLocalhost);

app.UseAuthorization();

app.MapControllers();

app.Run();

// Necessary to make the class accessible from ApiTests project
public partial class Program { }