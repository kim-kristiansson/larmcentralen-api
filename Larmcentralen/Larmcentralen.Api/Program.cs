using System.Text.Json.Serialization;
using Larmcentralen.Api.Services;
using Larmcentralen.Application.Interfaces;
using Larmcentralen.Application.Services;
using Larmcentralen.Domain.Interfaces;
using Larmcentralen.Infrastructure.Data;
using Larmcentralen.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<IAlarmRepository, AlarmRepository>();
builder.Services.AddScoped<ISolutionRepository, SolutionRepository>();

// Services
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<IAlarmService, AlarmService>();
builder.Services.AddScoped<ISolutionService, SolutionService>();
builder.Services.AddScoped<ISharePointSyncService, SharePointSyncService>();
builder.Services.AddScoped<SharePointUploadService>();
builder.Services.AddScoped<ExportService>();
builder.Services.AddScoped<SmsAlarmService>();

// Configurations
builder.Services.Configure<EmailOptions>(
    builder.Configuration.GetSection("Email"));
builder.Services.Configure<SharePointOptions>(
    builder.Configuration.GetSection("SharePoint"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();