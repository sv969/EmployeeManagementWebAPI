using SampleWebAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using SampleWebAPI.Repositories;


var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Configure services directly
builder.Services.AddControllers();
builder.Services.AddMemoryCache();

builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
    opt.ApiVersionReader = new UrlSegmentApiVersionReader();
});

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "My Employee Service API",
        Version = "v1"
    });
});
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions => sqlOptions.EnableRetryOnFailure() 
    ));
builder.Services.AddScoped<DapperDBContext>();
builder.Services.AddScoped<IEmployeeReadRepository, EmployeeReadRepository>();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowAll", policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

// 2️⃣ Build the app
var app = builder.Build();

// 3️⃣ Configure middleware directly
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Employee Service API V1");
        c.RoutePrefix = "EmployeeManagement"; // Swagger UI at root: http://localhost:5001/
    });
}
app.UseCors("AllowAll");
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();

// 4️⃣ Run the app
app.Run();
