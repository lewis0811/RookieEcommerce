using Microsoft.EntityFrameworkCore;
using RookieEcommerce.Api;
using RookieEcommerce.Application;
using RookieEcommerce.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// --- Add services to the container by calling extension methods ---

// Register services from Infrastructure layer (DbContext, Redis Cache)
builder.Services.AddInfrastructureService(builder.Configuration);

// Register services from Application layer (Validators, MediatR, etc.)
builder.Services.AddApplicationService();

// Register Identity/Auth related services
builder.Services.AddIdentityServices(builder.Configuration);

// Register API specific services (Controllers, Swagger, etc.)
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseAuthentication();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
