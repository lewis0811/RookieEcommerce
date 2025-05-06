using RookieEcommerce.Api;
using RookieEcommerce.Api.Middleware;
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

// Retrieve BaseAddress from configuration
//var baseAddress = builder.Configuration.GetValue<string>("HttpClient:BaseAddress");

// Register CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Services.AddHostedService<Worker>();

var app = builder.Build();

// Register error handling middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

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

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();