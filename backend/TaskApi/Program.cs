using TaskApi.Data;
using TaskApi.Interfaces;
using Microsoft.EntityFrameworkCore;
using TaskApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Add Entity Framework Core with SQLite
builder.Services.AddDbContext<TaskDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=tasks.db"));
builder.Services.AddScoped<ITaskService, TaskService>();

// Add CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add logging
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.MapControllers();

// Apply migrations and create database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();
