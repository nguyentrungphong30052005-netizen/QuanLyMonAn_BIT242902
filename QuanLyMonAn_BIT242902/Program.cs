using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using QuanLyMonAn_BIT242902.Data;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình đa ngôn ngữ (Localization)
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// 2. Đăng ký DbContext với chuỗi kết nối SQL Server (.\SQLEXPRESS)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }
    ));

builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

var app = builder.Build();

// 3. Tự động khởi tạo Database và các bảng ngay khi ứng dụng chạy lần đầu
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

// 4. Cấu hình ngôn ngữ hỗ trợ (Mặc định Tiếng Việt / Tiếng Anh)
var supportedCultures = new[]
{
    new CultureInfo("vi-VN"),
    new CultureInfo("en-US")
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("vi-VN"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dishes}/{action=Index}/{id?}");

app.Run();