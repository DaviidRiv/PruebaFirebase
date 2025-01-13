using PruebaFireBase.Interfaces;
using PruebaFireBase.Models.Configs;
using PruebaFireBase.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Tiempo de expiración
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Firebase Services
builder.Services.Configure<FirebaseConfig>(builder.Configuration.GetSection("Firebase"));
builder.Services.AddScoped<IPhoneRepository, PhoneRepository>();

//Firebase Auth
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddHttpClient();

// Dropbox Services
builder.Services.Configure<DropboxConfig>(builder.Configuration.GetSection("Dropbox"));
builder.Services.AddScoped<IPhoneStorageRepository, PhoneRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
