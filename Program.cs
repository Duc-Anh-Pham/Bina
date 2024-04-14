using Bina.Data;
using Firebase.Storage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(config =>
{
    config.AddConsole()
          .AddDebug()
          .AddEventSourceLogger();
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})

.AddCookie()

//Login Google OAuth
.AddGoogle(GoogleDefaults.AuthenticationScheme, option =>
{
    option.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
    option.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
})

//Login Microsoft OAuth
.AddMicrosoftAccount(MicrosoftAccountDefaults.AuthenticationScheme, option =>
{
    option.ClientId = builder.Configuration.GetSection("MicrosoftKeys:ClientId").Value;
    option.ClientSecret = builder.Configuration.GetSection("MicrosoftKeys:ClientSecret").Value;
});

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<Ft1Context>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllersWithViews();

builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(60);
});

builder.Services.AddSingleton<FirebaseStorage>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var firebaseConfig = config.GetSection("Firebase");
    return new FirebaseStorage(
        firebaseConfig["Bucket"],
        new FirebaseStorageOptions
        {
            AuthTokenAsyncFactory = () => Task.FromResult(firebaseConfig["ApiKey"]),
            ThrowOnCancel = true // optionally
        });
});


var app = builder.Build();

app.Logger.LogInformation("Application has started.");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
// Create the directory if it doesn't exist
Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars"));

// Then, use the static file provider
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars")),
    RequestPath = "/uploads/avatars"
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Logins}/{action=Login}/{id?}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});



app.Run();
