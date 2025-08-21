using Microsoft.EntityFrameworkCore;
using Travel.Data;
using Travel.Services;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// DbContext (Pomelo MySQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

// HttpClient + Services
builder.Services.AddHttpClient();
builder.Services.AddScoped<TelegramNotificationService>();
builder.Services.AddScoped<CrawlerService>();

// CORS: ĐỊNH NGHĨA RÕ ORIGIN CHO DEV
const string FrontendPolicy = "FrontendDev";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: FrontendPolicy, policy =>
    {
        policy
            // Chỉ rõ các origin dev bạn dùng:
            .WithOrigins(
                "http://localhost:3000",
                "http://127.0.0.1:3000",
                "http://localhost:5173",
                "http://127.0.0.1:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
        // Nếu DÙNG COOKIE/TOKEN QUA CREDENTIALS thì bật dòng dưới
        // và KHÔNG được AllowAnyOrigin:
        // .AllowCredentials();
    });

    // Tuỳ chọn: policy mở rộng trong nội bộ máy (chỉ dùng khi thật cần)
    options.AddPolicy("AllowLocalhostAll", policy =>
    {
        policy
            .SetIsOriginAllowed(origin =>
                origin.StartsWith("http://localhost:") ||
                origin.StartsWith("http://127.0.0.1:"))
            .AllowAnyHeader()
            .AllowAnyMethod();
        // .AllowCredentials(); // bật nếu cần cookie
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ⚠️ ĐẶT CORS SỚM HƠN HTTPS REDIRECT để tránh mất header khi 307 redirect
app.UseCors(FrontendPolicy);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS redirect (giữ sau UseCors để tránh lỗi CORS trên redirect)
app.UseHttpsRedirection();

// Nếu có auth:
// app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
