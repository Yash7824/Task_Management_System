using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Task_Managament_System.DL;
using Task_Managament_System.Repositories;
using Task_Management_System.BL;
using Task_Management_System.Constants;
using Task_Management_System.DL;
using Task_Management_System.Models;
using Task_Management_System.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.Configure<AppSettingsModel>(builder.Configuration.GetSection("AppSettings"));
var serviceProvider = builder.Services.BuildServiceProvider();
var options = serviceProvider.GetRequiredService<IOptions<AppSettingsModel>>();
TaskConstant.Initialize(options);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var issuer = TaskConstant.JWT_Issuer;
        var audience = TaskConstant.JWT_Audience;
        var key = TaskConstant.JWT_Key;

        if (issuer != null && audience != null && key != null)
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true, 
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };


            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($" Authentication failed: {context.Exception.Message}");
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    Console.WriteLine(" Token successfully validated.");
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    Console.WriteLine(" Challenge triggered (401 Unauthorized).");
                    return Task.CompletedTask;
                }
            };
        }
        else
        {
            throw new ApplicationException("JWT configuration values are missing.");
        }
    });


builder.Services.AddAuthorization();

builder.Services.AddScoped<IUserRepository, UserDL>();
builder.Services.AddScoped<ITokenRepository, LoginBL>();
builder.Services.AddScoped<ITaskRepository, TaskDL>();


var app = builder.Build();

app.Use(async (context, next) =>
{
    Console.WriteLine($"Request Path: {context.Request.Path}");
    Console.WriteLine($"Authorization Header: {context.Request.Headers["Authorization"]}");
    await next();
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
