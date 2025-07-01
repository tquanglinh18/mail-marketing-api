using Amazon.S3;
using mail_marketing_api.Data;
using mail_marketing_api.Services;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration; // Lấy IConfiguration

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                      });
});

var connectionString = builder.Configuration.GetConnectionString("SqlServerInfo");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'SqlServerInfo' not found.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

var dbChecker = new CheckConnectDb(configuration);

dbChecker.TestConnection();

builder.Services.AddControllers();

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();

builder.Services.AddScoped<IS3Service, S3Service>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<ICampaignService, CampaignService>();
builder.Services.AddScoped<IRecipientService, RecipientService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();

