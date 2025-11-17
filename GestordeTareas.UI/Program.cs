using Microsoft.AspNetCore.Authentication.Google;
using GestordeTareas.UI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using GestordeTareas.BL;
using GestordeTareas.DAL;
using GestordeTareas.DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using System.Reflection;
using GestordeTareas.BL.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("GestordeTareasUIContextConnection") ?? throw new InvalidOperationException("Connection string 'GestordeTareasUIContextConnection' not found.");

builder.Services.AddDbContext<ContextoBD>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<GestordeTareasUIContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<GestordeTareasUIContext>();
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//REGISTRO DE INTERFACES
builder.Services.AddScoped<IUsuarioDAL, UsuarioDAL>();
builder.Services.AddScoped<ISeguridadService, SeguridadService>();

// REGISTRO DE BL
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<UsuarioBL>();
builder.Services.AddScoped<ProyectoBL>();
builder.Services.AddScoped<TareaBL>();
builder.Services.AddScoped<CargoBL>();
builder.Services.AddScoped<ProyectoUsuarioBL>();
builder.Services.AddScoped<ElegirTareaBL>();
builder.Services.AddScoped<CategoriaBL>();
builder.Services.AddScoped<PrioridadBL>();
builder.Services.AddScoped<EstadoTareaBL>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
// configurar la autenticaci�n
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).
    AddCookie((options) =>
    {

        options.LoginPath = new PathString("/Usuario/Login");
        options.AccessDeniedPath = new PathString("/home/index");
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;


    });

var app = builder.Build();

var googleConfig = builder.Configuration.GetSection("Authentication:Google");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie() // cookie principal para tu app
.AddCookie("External") // cookie temporal usada por proveedores externos
.AddGoogle("Google", googleOptions =>
{
    googleOptions.ClientId = googleConfig["ClientId"];
    googleOptions.ClientSecret = googleConfig["ClientSecret"];
    googleOptions.CallbackPath = "/signin-google";
    googleOptions.SignInScheme = "External"; // IMPORTANT: almacena el principal en la cookie "External"
});


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

app.UseAuthentication(); // poner en uso la autenticaci�n 

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();