using DTS.Common.Utility;
using DTS.DataAccess;
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
    options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection"))); 

// builder.Services.AddDefaultIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.ConfigureApplicationCookie(options =>
{
    
    options.LoginPath = $"/";
    options.LogoutPath = $"/Account/Logout";
    options.AccessDeniedPath = $"/Account/AccessDenied";
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

// app.MapControllerRoute(
//     name: "default",
//     pattern: "{area=User}/{controller=Document}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Account}/{controller=Document}/{action=Index}/{id?}");
 

app.Run();