using Bina.Data;
using Bina.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.EntityFrameworkCore;

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

// In ConfigureServices FirebaseCloud
builder.Services.AddSingleton<FirebaseCloud>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    var firebaseConfig = configuration.GetSection("Firebase");

    return new FirebaseCloud(
        firebaseConfig["apiKey"],
        firebaseConfig["storageBucket"],
        "", // authEmail not use in config
        ""  // authPassword not use in config
    );
});




builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(60);
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

    endpoints.MapControllerRoute(
        name: "upload",
        pattern: "{controller=Upload}/{action=Index}");
});



app.Run();
