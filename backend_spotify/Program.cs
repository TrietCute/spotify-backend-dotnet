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
                  .WithOrigins("https://spotify-backend-dotnet.onrender.com");
        });
});

#region Đây là phần kết nối CSDL
// Lấy chuỗi kết nối từ file .env
var neonConnectionString = Env.GetString("POSTGRES_DATABASE_URL");
// Đăng ký các DbContext
builder.Services.AddDbContext<NeonDbContext>(options =>
    options.UseNpgsql(neonConnectionString));
#endregion

#region Phần kết nối Swagger
// Cấu hình các dịch vụ
builder.Services.AddSingleton<OtpService>(); // Thêm OtpService
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c=>
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Spotify Backend Remake", // Đặt tên mới cho Swagger
        Version = "v1",
        Description = "This API is used for managing music data.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Le Dang Thuong",
            Email = "ledangthuongsp@gmail.com",
            Url = new Uri("https://ledangthuongsp.github.io/ThuongProfile")
        }
    })
);

#endregion

var app = builder.Build();
// Tự động migrate database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<NeonDbContext>();
    dbContext.Database.Migrate();
}
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
app.Urls.Add($"http://*:{port}");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
