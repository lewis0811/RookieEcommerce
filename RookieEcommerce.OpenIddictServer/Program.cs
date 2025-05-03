using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quartz;
using RookieEcommerce.Domain.Entities;
using RookieEcommerce.Infrastructure;
using RookieEcommerce.OpenIddictServer;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

// Add  builder.Services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// --- Retrieve Connection Strings ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// --- Add DbContext, Database Provider, OpenIddict ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.UseOpenIddict();
});

// Register the Identity services.
builder.Services.AddIdentity<Customer, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.AddQuartz(options =>
{
    options.UseSimpleTypeLoader();
    options.UseInMemoryStore();
});

builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

builder.Services.AddOpenIddict()
   // Register the OpenIddict core components.
   .AddCore(options =>
   {
       // Configure OpenIddict to use the Entity Framework Core stores and models.
       // Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.
       options.UseEntityFrameworkCore()
              .UseDbContext<ApplicationDbContext>();

       // Enable Quartz.NET integration.
       options.UseQuartz();
   })

   // Register the OpenIddict client components.
   .AddClient(options =>
   {
       options.AllowAuthorizationCodeFlow();

       options.AddDevelopmentEncryptionCertificate()
              .AddDevelopmentSigningCertificate();

       options.UseAspNetCore()
              .EnableStatusCodePagesIntegration()
              .EnableRedirectionEndpointPassthrough();

       options.UseSystemNetHttp()
              .SetProductInformation(typeof(Program).Assembly);

       options.UseWebProviders()
           .AddGitHub(options =>
           {
               options.SetClientId(builder.Configuration["Github:ClientId"]!)
                       .SetClientSecret(builder.Configuration["Github:ClientSecret"]!)
                       .SetRedirectUri(builder.Configuration["Github:RedirectUri"]!);
           });
   })

   // Register the OpenIddict server
   // components.
   .AddServer(options =>
   {
       options.SetAuthorizationEndpointUris("connect/authorize")
              .SetEndSessionEndpointUris("connect/logout")
              .SetTokenEndpointUris("connect/token")
              .SetUserInfoEndpointUris("connect/userinfo");

       options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);

       options.AllowAuthorizationCodeFlow();

       // Register the signing and encryption credentials.
       options.AddDevelopmentEncryptionCertificate()
              .AddDevelopmentSigningCertificate();

       // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
       options.UseAspNetCore()
              .EnableAuthorizationEndpointPassthrough()
              .EnableEndSessionEndpointPassthrough()
              .EnableTokenEndpointPassthrough()
              .EnableUserInfoEndpointPassthrough()
              .EnableStatusCodePagesIntegration();
   })

   // Register the OpenIddict validation components.
   .AddValidation(options =>
   {
       // Import the configuration from the local OpenIddict server instance.
       options.UseLocalServer();

       // Register the ASP.NET Core host.
       options.UseAspNetCore();
   });

// Register the worker responsible for seeding the database.
builder.Services.AddHostedService<Worker>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

await app.RunAsync();