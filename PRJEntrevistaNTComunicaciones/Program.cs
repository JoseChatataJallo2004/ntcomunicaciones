using PRJEntrevistaNTComunicaciones.CapaDatos;
using PRJEntrevistaNTComunicaciones.CapaNegocio;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddScoped<FirmasDAO>();
builder.Services.AddScoped<Firmas_CN>();

builder.Services.AddScoped<Firmas_CN>(provider => new Firmas_CN(builder.Configuration));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Firmas}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
