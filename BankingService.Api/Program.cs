using BankingService.Api.Configuration;
using BankingService.Core.API.Interfaces;
using BankingService.Core.Services;
using BankingService.Core.SPI.Interfaces;
using BankingService.Infra.Database.Services;
using BankingService.Infra.Database.SPI.Interfaces;
using BankingService.Infra.FileSystem.Adapters;
using BankingService.Infra.FileSystem.API.Interfaces;
using BankingService.Infra.FileSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

builder.Services.AddScoped<IFileSystemService, FileSystemService>();
builder.Services.AddScoped<IFileSystemServiceForFileDB, FileSystemAdapterDatabase>();
builder.Services.AddScoped<IFileSystemServiceForCore, FileSystemAdapterCore>();
builder.Services.AddScoped<IBankDatabaseConfiguration, DatabaseConfiguration>();
builder.Services.AddScoped<IBankDatabaseService, BankDatabaseService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IImportService, ImportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();
// https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/iis/hosting-bundle?view=aspnetcore-8.0#install-the-net-core-hosting-bundle