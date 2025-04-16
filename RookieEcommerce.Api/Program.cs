using Microsoft.AspNetCore.Builder;
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
    app.UseSwaggerUI(options =>
    {
        var groupNames = app.DescribeApiVersions().Select(description => description.GroupName);

        // Build a swagger endpoint for each discovered API version
        foreach (var groupName in groupNames)
        {
            options.SwaggerEndpoint($"/swagger/{groupName}/swagger.json", groupName.ToUpperInvariant());
        }
    });
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
