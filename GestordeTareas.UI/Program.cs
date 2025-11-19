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
<<<<<<< HEAD
using GestordeTareas.UI.profiles;
=======
using GestordeTareas.BL.Services;
>>>>>>> 6f3904e1ef769397f15b3f638d17e031b23152fb

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("GestordeTareasUIContextConnection") ?? throw new InvalidOperationException("Connection string 'GestordeTareasUIContextConnection' not found.");

builder.Services.AddDbContext<ContextoBD>(options => options.UseSqlServer(connectionString));
<<<<<<< HEAD
builder.Services.AddDbContext<GestordeTareasUIContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<GestordeTareasUIContext>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddAutoMapper(AutoMapperRegistry.GetProfiles());
 //para los perfiles en otros proyectos
var configuration = builder.Configuration;
=======
//builder.Services.AddDbContext<GestordeTareasUIContext>(options => options.UseSqlServer(connectionString));
//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<GestordeTareasUIContext>();
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
>>>>>>> 6f3904e1ef769397f15b3f638d17e031b23152fb

//REGISTRO DE INTERFACES
// DAL
builder.Services.AddScoped<IProyectoDAL, ProyectoDAL>();
builder.Services.AddScoped<ITareaDAL, TareaDAL>();
builder.Services.AddScoped<ICargoDAL, CargoDAL>();
builder.Services.AddScoped<IProyectoUsuarioDAL, ProyectoUsuarioDAL>();
builder.Services.AddScoped<ICategoriaDAL, CategoriaDAL>();
builder.Services.AddScoped<IPrioridadDAL, PrioridadDAL>();
builder.Services.AddScoped<IEstadoTareaDAL, EstadoTareaDAL>();
builder.Services.AddScoped<IUsuarioDAL, UsuarioDAL>();
builder.Services.AddScoped<IInvitacionProyectoDAL, InvitacionProyectoDAL>();
builder.Services.AddScoped<ISeguridadService, SeguridadService>();

// REGISTRO DE BL
builder.Services.AddControllersWithViews();
// BL
builder.Services.AddScoped<ProyectoBL>();
builder.Services.AddScoped<TareaBL>();
builder.Services.AddScoped<CargoBL>();
builder.Services.AddScoped<ProyectoUsuarioBL>();
builder.Services.AddScoped<CategoriaBL>();
builder.Services.AddScoped<PrioridadBL>();
builder.Services.AddScoped<EstadoTareaBL>();
builder.Services.AddScoped<UsuarioBL>();
builder.Services.AddScoped<InvitacionProyectoBL>();

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
// configurar la autenticaci�n
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).
//    AddCookie((options) =>
//    {

//        options.LoginPath = new PathString("/Usuario/Login");
//        options.AccessDeniedPath = new PathString("/home/index");
//        options.ExpireTimeSpan = TimeSpan.FromHours(8);
//        options.SlidingExpiration = true;


//    });

var googleConfig = builder.Configuration.GetSection("Authentication:Google");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "Google";
})
.AddCookie(options =>
{
    options.LoginPath = "/Usuario/Login";      // <-- tu ruta
    options.AccessDeniedPath = "/Home/Index";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
})
.AddCookie("External")
.AddGoogle("Google", googleOptions =>
{
    googleOptions.ClientId = googleConfig["ClientId"];
    googleOptions.ClientSecret = googleConfig["ClientSecret"];
    googleOptions.CallbackPath = "/signin-google";
    googleOptions.SignInScheme = "External";

    googleOptions.Scope.Add("profile");
    googleOptions.Scope.Add("email");
    googleOptions.SaveTokens = true;
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

app.UseRouting();

app.UseAuthentication(); // poner en uso la autenticaci�n 

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();