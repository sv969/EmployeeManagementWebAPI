using SampleWebAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Configure services directly
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions => sqlOptions.EnableRetryOnFailure() 
    ));
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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Employee API V1");
        c.RoutePrefix = "EmployeeManagement"; // Swagger UI at root: http://localhost:5001/
    });
}
app.UseCors("AllowAll");
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();

// 4️⃣ Run the app
app.Run();
