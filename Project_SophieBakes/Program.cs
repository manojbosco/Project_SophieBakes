using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project_SophieBakes.Data;
using Project_SophieBakes.Models;
using Project_SophieBakes.Services;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// ✅ Register services
builder.Services.AddControllersWithViews().AddNewtonsoftJson();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure Entity Framework with MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), 
        new MySqlServerVersion(new Version(8, 0, 21))));

// Configure authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireDeliveryRole", policy => policy.RequireRole("DeliveryBoy"));
});

// ✅ Fix: Register UserManager & SignInManager
builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<SignInManager<User>>();
builder.Services.AddScoped<CartService>();

// ✅ Configure Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// ✅ Fix: Run Seeding Without Await
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    SeedRolesAndUsers(serviceProvider).GetAwaiter().GetResult();
}

// Check if DB is connected
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    Console.WriteLine(context.Database.CanConnect() ? "Connected to Database" : "Database Connection Failed");
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Use attribute routing for CheckoutController
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "OrderConfirmation",
    pattern: "OrderConfirmation/{action=Index}/{orderId?}");

app.Run();

/// ✅ **Seed roles and delivery boy user**
async Task SeedRolesAndUsers(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

    string deliveryBoyRole = "DeliveryBoy";

    if (!await roleManager.RoleExistsAsync(deliveryBoyRole))
    {
        await roleManager.CreateAsync(new IdentityRole(deliveryBoyRole));
    }

    string deliveryEmail = "delivery1@example.com";
    string deliveryPassword = "Delivery@123";

    if (await userManager.FindByEmailAsync(deliveryEmail) == null)
    {
        var deliveryBoy = new User
        {
            UserName = "delivery1",
            Email = deliveryEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(deliveryBoy, deliveryPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(deliveryBoy, deliveryBoyRole);
        }
        else
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"Error: {error.Description}");
            }
        }
    }
}
