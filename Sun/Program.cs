using Microsoft.AspNetCore.Identity.UI.Services;
using MySql.Data.MySqlClient;
using Sun.Repository;
using Sun.Services;

var builder = WebApplication.CreateBuilder(args);

// Добавление строки подключения для MySQL
//builder.Services.AddSingleton<MySqlConnection>(options => new MySqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрация UserRepository в DI
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IRoleRepository, RoleRepository>();
//builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IEmailSender, EmailSender>(); 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Устанавливаем тайм-аут сессии
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddAuthentication("Sun")
    .AddCookie("Sun", options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        // options.AccessDeniedPath = "/Account/AccessDenied"; // Путь для страницы отказа в доступе
        options.SlidingExpiration = true;
    });

var app = builder.Build();

app.UseSession();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
