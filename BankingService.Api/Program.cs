using BankingService.Api.Configuration;
using BankingService.Api.MapperProfile;
using BankingService.Api.Middlewares;
using BankingService.Core.API.Interfaces;
using BankingService.Core.API.MapperProfile;
using BankingService.Core.Services;
using BankingService.Core.SPI.Interfaces;
using BankingService.Core.SPI.MapperProfile;
using BankingService.Infra.Database.API.Interfaces;
using BankingService.Infra.Database.Services;
using BankingService.Infra.Database.SPI.Interfaces;
using BankingService.Infra.FileSystem.Adapters;
using BankingService.SharedTechnical;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/docs/configure-and-customize-swaggergen.md#add-security-definitions-and-requirements-for-bearer-authentication
    options.AddSecurityDefinition("BearerAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="BearerAuth"
                }
            },
            Array.Empty<string>()
        }
    });
});

var allowSpecificOrigin = builder.Configuration.GetSection("AllowSpecificOrigin").Value ?? "";

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins(allowSpecificOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddAutoMapper(
    typeof(CoreApiProfile),
    typeof(CoreSpiProfile),
    typeof(AspApiProfile));

// TODO use module pattern to register services
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IFileSystemServiceForFileDB, FileSystemAdapter>();
builder.Services.AddScoped<IFileSystemServiceForCore, FileSystemAdapter>();
builder.Services.AddScoped<IBankDatabaseConfiguration, DatabaseConfiguration>();
builder.Services.AddScoped<IImportConfiguration, ImportConfiguration>();
builder.Services.AddScoped<IBankDatabaseService, BankDatabaseService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IImportService, ImportService>();
builder.Services.AddScoped<IMaintenanceService, MaintenanceService>();
builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<ExceptionMiddleware>();

// Authentication
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(jwtOptions =>
    {
        //jwtOptions.Authority = "https://{--your-authority--}";
        //jwtOptions.Audience = "https://{--your-audience--}";
        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidAudiences = ["BankingService"],
            ValidIssuers = ["BankingService"],

            //// Specify the key used to sign the token
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("authentication_is_not_really_enabled_this_is_just_an_example")),
            RequireSignedTokens = true,

            //// Ensure token's expiration mgt
            RequireExpirationTime = true,
            ValidateLifetime = true,
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseMiddleware<ExceptionMiddleware>();

app.Run();
// https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/hosting-bundle?view=aspnetcore-8.0#install-the-net-core-hosting-bundle