using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using spotiq_backend.DataAccess;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Serialization;
using spotiq_backend.Workers;
using static System.Net.WebRequestMethods;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<PollServiceConsumer>();
builder.Services.AddScoped<IPollService, PollService>();

builder.Services.AddCors();

builder.Services.AddDbContext<SpotiqContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("SpotiqContext") ?? throw new InvalidOperationException("Connection string 'SpotiqContext' not found.")));

//Azure
//if (builder.Environment.IsDevelopment())
//{
//    builder.Services.AddDbContext<SpotiqContext>(options =>
//        options.UseSqlServer(builder.Configuration.GetConnectionString("SpotiqContext") ?? throw new InvalidOperationException("Connection string 'SpotiqContext' not found.")));
//}
//if (builder.Environment.IsProduction())
//{
//    builder.Services.AddDbContext<SpotiqContext>(options =>
//        options.UseSqlServer(builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING") ?? throw new InvalidOperationException("Connection string 'SpotiqContext' not found.")));
//}

builder.Services.AddControllers();
 //AddJsonOptions gjør det mulig å bruke EF's linq med navigasjonsobjekter i web-api.
// Uten dette får vi feil: System.Text.Json.JsonException: A possible object cycle was detected which is not supported...osv
builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo // Denne må stå til 'v1', ellers laster ikke Swagger!
    {
        Version = "v0.2",
        Title = "SpotiQ",
        Description = "An ASP.NET Core Web API letting venue guests queue " +
        "Spotify tracks to a host's Spotify player (mobile, desktop app and web)",
        
    });

    // using System.Reflection;
    String xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

WebApplication app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
  //  app.UseSwagger();
  //  app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(c => { c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); });

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
