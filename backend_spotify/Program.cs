using DotNetEnv;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Đọc biến môi trường từ file .env
Env.Load();
var port = Environment.GetEnvironmentVariable("PORT") ?? "5025"; // Sử dụng cổng 5025

// Thêm dịch vụ CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .WithOrigins("https://student-management-se100.onrender.com");
        });
});

#region Đây là phần kết nối CSDL

// Lấy chuỗi kết nối từ file .env
var neonConnectionString = Env.GetString("POSTGRES_DATABASE_URL");
var supabaseConnectionString = Env.GetString("SUPABASE_DATABASE_URL");

// Đăng ký các DbContext
builder.Services.AddDbContext<NeonDbContext>(options =>
    options.UseNpgsql(neonConnectionString));

#endregion

#region Phần kết nối Swagger
// Cấu hình các dịch vụ
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion

var app = builder.Build();

// Sử dụng CORS
app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction()) // Hiển thị Swagger cả trong môi trường production
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.RoutePrefix = string.Empty; // Để hiển thị Swagger tại root URL
    });
}

// Lắng nghe cổng 5025
app.Urls.Add($"http://localhost:{port}");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
