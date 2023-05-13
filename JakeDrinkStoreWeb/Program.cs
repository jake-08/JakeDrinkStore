using JakeDrinkStore.DataAccess;
using JakeDrinkStore.DataAccess.Repository;
using JakeDrinkStore.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using JakeDrinkStore.Utility;
using Stripe;
using JakeDrinkStore.DataAccess.DbInitializer;

var builder = WebApplication.CreateBuilder(args);

/* Add services to the container.*/

// Configure MVC features to use Controller and Views 
builder.Services.AddControllersWithViews();

// To ignore Object Cycles
builder.Services.AddMvc().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// To allow Razor Page Refresh
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

// Add Database Connection from appsettings.json and add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection"), o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "drinkstore"))
);

// Configure Stripe Settings
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));

// Add Identity Services to the application
// AddDefaultTokenProviders for Custom Identity, not required in AddDefaultIdentity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Register IEmailSender for Custom Identity as a Singleton service
builder.Services.AddSingleton<IEmailSender, EmailSender>();

// Register IUnitOfWork to the container
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register IDbInitializer 
builder.Services.AddScoped<IDbInitializer, DbInitializer>();   

// Change Default Login Paths to Identity Paths
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

// Add Session for Shopping Cart 
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add Facebook Login
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = builder.Configuration.GetSection("Facebook:AppId").Get<string>();
    options.AppSecret = builder.Configuration.GetSection("Facebook:AppSecret").Get<string>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Add Stripe Api Key to the Pipeline 
StripeConfiguration.ApiKey = builder.Configuration.GetSection("StripeSettings:SecretKey").Get<string>();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Use Session in the application
app.UseSession();

// Call DbInitilizer method
SeedDatabase();

// MapRazorPage function for Identity Pages
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();


void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
         var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>(); 
         dbInitializer.Initialize();
    }
}