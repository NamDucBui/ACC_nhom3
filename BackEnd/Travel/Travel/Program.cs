using Microsoft.EntityFrameworkCore;
using Travel.Data;
using Travel.Services;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure; // Thêm nếu cần

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));

// Đăng ký HttpClient cho Telegram service
builder.Services.AddHttpClient();

// Đăng ký TelegramNotificationService
builder.Services.AddScoped<TelegramNotificationService>();

// Đăng ký CrawlerService
builder.Services.AddScoped<CrawlerService>();
// Thêm cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Thêm middleware CORS trước UseAuthorization
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();

app.Run();
