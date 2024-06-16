using Identity.DbContexts;
using Identity.Middlewares;
using Identity.Repositories.Users;
using Identity.Jwt.Interfaces;
using Identity.Jwt.Services;
using Microsoft.EntityFrameworkCore;
using Identity.Services.Emails.Interfaces;
using Identity.Emails.Services;
using Api.OptionsSetup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Identity.Services.UnitOfWorks;
using Identity.Repositories.Roles;
using Identity.Services.Interfaces;
using Identity.Services.Hashing;
using Identity.Services.Emails.Options;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Auth:Issuer"]!,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)),
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(SD.Policy.Admin, policy =>
   policy.RequireRole(SD.Policy.Role.Admin))
    .AddPolicy(SD.Policy.User, policy =>
   policy.RequireRole(SD.Policy.Role.User))
    .AddPolicy(SD.Policy.UserAndAdmin, policy =>
    policy.RequireRole(SD.Policy.Role.User, SD.Policy.Role.Admin));

builder.Services.ConfigureOptions<JwtOptionsSetup>();
builder.Services.ConfigureOptions<JwtBearerOptionsSetup>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(SD.DefaultCorsPolicyName,
        builder => builder
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());
});

builder.Services.AddOptions<EmailOptions>()
          .Bind(builder.Configuration.GetSection(SD.EmailSectionKey))
          .ValidateDataAnnotations();

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("UserDB");

builder.Services.AddDbContext<UserDbContext>(x => x.UseSqlServer(connectionString));
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IRoleRepository, RoleRepository>();
builder.Services.AddTransient<IJwtManager, JwtManager>();
builder.Services.AddTransient<IHashingService, HashingService>();
builder.Services.AddTransient<IJwtBlacklistManager, JwtBlacklistManager>();
builder.Services.AddTransient<IIdentityEmailService, IdentityEmailService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseCors(SD.DefaultCorsPolicyName);

//app.UseMiddleware<JwtBlackListMiddleware>();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();


app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();