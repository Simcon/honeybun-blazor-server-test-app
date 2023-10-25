using HoneybunBlazorServerExample.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

// *** START INTEGRATING HONEYBUN AUTH

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .Build();

IdentityModelEventSource.ShowPII = true; // dev only

var settings = new HoneybunSettings();
config.GetSection("Honeybun").Bind(settings);
var branding = new Branding();
config.GetSection("Branding").Bind(branding);

builder.Services.AddSingleton(settings);
builder.Services.AddSingleton(branding);

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.Secure = CookieSecurePolicy.Always;
});

builder.Services
    .AddAuthentication(opt =>
    {
        opt.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opt.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, opt =>
    {
        opt.RequireHttpsMetadata = false;
        opt.Authority = settings.Authority;
        opt.ClientId = settings.ClientId;
        opt.ClientSecret = settings.ClientSecret;
        opt.ResponseType = OpenIdConnectResponseType.Code;
        opt.UseTokenLifetime = true;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = ClaimTypes.Email,
            ValidateIssuer = false,
            ValidIssuers = new[]
            {
                settings.Authority
            }          
        };
        opt.Events = new OpenIdConnectEvents
        {
            OnAccessDenied = context =>
            {
                context.HandleResponse();
                context.Response.Redirect("/");
                return Task.CompletedTask;
            }
        };
    });

// *** END INTEGRATING HONEYBUN AUTH

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

// *** START INTEGRATING HONEYBUN AUTH

app.UseAuthentication();
app.UseAuthorization();

app.UseCookiePolicy();

// *** END INTEGRATING HONEYBUN AUTH

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
