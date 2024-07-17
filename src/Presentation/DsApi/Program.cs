using Ds.Application;
using Ds.Data;
using System.Text.Json.Serialization;
using System.Text.Json;
using QuestPDF.Infrastructure;
using DsApi.CustomMiddlewares;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.Converters.Add(new JsonConverterDateTime());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddScoped<IRepository>(provider => 
    new Repository(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICommonService, CommonService>();


var CorsConfig = builder.Configuration.GetSection("Cors").Get<CorsPolicy>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsConfig.Policy,
        builder => builder
            .WithOrigins(CorsConfig.Origins)
            .AllowAnyHeader()
            .AllowAnyMethod());
});


var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(CorsConfig.Policy);
app.UseHttpsRedirection();
app.MapControllers();
app.Run();