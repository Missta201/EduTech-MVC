using EduTech;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using EduTech.Models;
using EduTech.Services;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<EduTechDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserService, UserService>();
// Adds the Identity system, including the default UI, and configures the user type as IdentityUser
builder.Services.AddDefaultIdentity<ApplicationUser>(
    options =>
    {
        options.SignIn.RequireConfirmedAccount = true;
        options.Lockout.AllowedForNewUsers = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireDigit = true;
    }
    )
    .AddEntityFrameworkStores<EduTechDbContext>(); // Configures Identity to store its data in EF Core

// Configures policies
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("IsAdmin", policy => policy.RequireClaim("UserType", UserTypes.Admin))
    .AddPolicy("IsScheduler", policy => policy.RequireClaim("UserType", UserTypes.Scheduler))
    .AddPolicy("IsLecturer", policy => policy.RequireClaim("UserType", UserTypes.Lecturer))
    .AddPolicy("IsStudent", policy => policy.RequireClaim("UserType", UserTypes.Student))
    .AddPolicy("IsAdminOrScheduler", policy =>
        policy.RequireClaim("UserType", UserTypes.Admin, UserTypes.Scheduler))
    .AddPolicy("CanManageClasses", policy => policy.RequireClaim("UserType", UserTypes.Admin, UserTypes.Scheduler))
    .AddPolicy("CanViewStudentsLectures", policy => policy.RequireClaim("UserType", UserTypes.Admin, UserTypes.Scheduler))
    .AddPolicy("CanManageStudentsLectures", policy => policy.RequireClaim("UserType", UserTypes.Admin, UserTypes.Scheduler))
    .AddPolicy("CanDeleteStudentsLectures", policy => policy.RequireClaim("UserType", UserTypes.Admin));
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

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

app.MapControllerRoute(
    name: "dashboard",
    pattern: "dashboard/{action=Index}",
    defaults: new { controller = "Dashboard" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
    
app.MapRazorPages();
app.Run();
