using YINSA_INTRANET.Servicios;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using YINSA_INTRANET.Filtros;
using Serilog;
using Serilog.Events;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<AutorizacionFacturasFilter>();
builder.Services.AddScoped<CrearFacturasProveedorFilter>();
builder.Services.AddTransient<IApiService, ApiService>();
builder.Services.AddTransient<IRutasService, RutasService>();
builder.Services.AddTransient<IArchivosService, ArchivosService>();
builder.Services.AddTransient<IEmpleadosService, EmpleadosService>();
builder.Services.AddTransient<ISociosService, SociosService>();
builder.Services.AddTransient<IDocumentosService, DocumentosService>();
builder.Services.AddTransient<IReportesService, ReportesService>();
builder.Services.AddTransient<IUsuariosService, UsuariosService>();
builder.Services.AddTransient<HashService>();

//builder.Services.AddSession(options =>
//{
//	options.IdleTimeout = TimeSpan.Zero;
//});
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

//ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
//ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => false;

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
			.AddCookie(options =>
			{
				options.CookieManager = new ChunkingCookieManager();

				options.Cookie.HttpOnly = true;
				options.ExpireTimeSpan = TimeSpan.FromMinutes(30);//de 5 a 20min

				options.LoginPath = "/Usuarios/Login";
				options.AccessDeniedPath = "/Home/NoEncontrado";
				options.SlidingExpiration = true;
				options.Cookie.SameSite = SameSiteMode.None;

				options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
			});

builder.Services.AddAuthorization(options =>
{   //options.AddPolicy("SuperAdmin", policy => policy.RequireClaim("AdminType","Super")) 
	options.AddPolicy("Master", policy => policy.RequireRole("master")); //policy.RequireRole("1")
	options.AddPolicy("Admin", policy => policy.RequireRole("master", "admin"));
	options.AddPolicy("Empleado", policy => policy.RequireRole("master", "admin", "empleado", "contador"));
	options.AddPolicy("Contador", policy => policy.RequireRole("contador"));
	options.AddPolicy("Socio", policy => policy.RequireRole("socio"));
	options.AddPolicy("AutorizarFctProv", policy => policy.RequireClaim("authFct"));
	options.AddPolicy("SubirFctProv", policy => policy.RequireClaim("subirFct"));
});

//builder.Services.AddLogging(options =>
//{
//	options.AddSerilog()
//	.AddFile("Logs/Log-{Date}.txt")
//	.SetMinimumLevel(LogLevel.Error);
//});
builder.Services.AddLogging(options =>
{
	options.AddSerilog()
		   .AddFile("Logs/Log-{Date}.txt",LogLevel.Error);  		  
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

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Usuarios}/{action=Login}");

RotativaConfiguration.Setup(app.Environment.WebRootPath, "../Rotativa");


app.Run();
