using JakeDrinkStore.DataAccess;
using JakeDrinkStore.DataAccess.Repository;
using JakeDrinkStore.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using JakeDrinkStore.Utility;

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
    builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Add Identity Services to the application
// AddDefaultTokenProviders for Custom Identity, not required in AddDefaultIdentity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Register IEmailSender for Custom Identity as a Singleton service
builder.Services.AddSingleton<IEmailSender, EmailSender>();

// Register IUnitOfWork to the container
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

// MapRazorPage function for Identity Pages
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
