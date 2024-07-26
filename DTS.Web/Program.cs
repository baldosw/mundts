using System.Configuration;
using DTS.Common;
using DTS.Common.Utility;
using DTS.DataAccess;
using DTS.Web.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);
 
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddMemoryCache();


builder.Services.AddCors(options =>  
{  
    options.AddPolicy(name: "MyPolicy",  
        policy =>  
        {  
            policy.WithOrigins("https://localhost:5097")  
                .WithMethods("POST", "PUT", "DELETE", "GET");  
        });  
}); 

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connection"))); 

// builder.Services.AddDefaultIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedEmail = true; // Require email confirmation
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddTokenProvider("Default", typeof(EmailTwoFactorAuthentication<IdentityUser>));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Configure cookie options if needed
        options.Cookie.HttpOnly = true;
        // Other configuration options...
    });

builder.Services.AddHttpContextAccessor();
 
builder.Services.ConfigureApplicationCookie(options =>
{
    
    options.LoginPath = $"/";
    options.LogoutPath = $"/Account/Logout";
    options.AccessDeniedPath = $"/AccessDenied";
});

builder.Services.AddScoped<IEmailSender, EmailSender>();
var app = builder.Build();
 
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseDeveloperExceptionPage();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    
}

app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.UseMiddleware<RequestCultureMiddleware>();

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{area=User}/{controller=Document}/{action=Index}/{id?}");



app.MapControllerRoute(
    name: "default",
    pattern: "{area=Account}/{controller=Document}/{action=Index}/{id?}");
 

app.Run();