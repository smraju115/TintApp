using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Newtonsoft.Json;
using TintApp.Data;
using TintApp.Models;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services with custom ApplicationUser and ApplicationRole
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";   // Login Page
    options.LogoutPath = "/Account/Logout"; // Logout Page
    options.AccessDeniedPath = "/Account/AccessDenied"; // Access Denied Page
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); //  after 30 min session expire
    options.SlidingExpiration = false; //   autologout when user/admin inactive

    
});

//Ratelimit
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("BookingLimiter", config =>
    {
        config.Window = TimeSpan.FromMinutes(1);
        config.PermitLimit = 5; // Allow max 5 requests per minute
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 2;
    });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

// Call the SeedData method during application startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
    await SeedData.Initialize(services, userManager, roleManager);
}

//new add
app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseHsts();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
