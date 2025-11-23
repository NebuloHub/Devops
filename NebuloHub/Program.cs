using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NebuloHub.Application.Swagger;
using NebuloHub.Application.UseCase;
using NebuloHub.Application.Validators;
using NebuloHub.Domain.Entity;
using NebuloHub.Infraestructure.Context;
using NebuloHub.Infraestructure.Repositores;
using NebuloHub.Infraestructure.Repositories;
using NebuloHub.Services;
using OpenTelemetry.Trace;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// CONFIGURAÇÕES DE JWT
// ===============================
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// ===============================
// BANCO DE DADOS
// ===============================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);


// ===============================
// CONTROLLERS + JSON
// ===============================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

// ===============================
// DEPENDÊNCIAS
// ===============================
builder.Services.AddScoped<IRepository<Avaliacao>, Repository<Avaliacao>>();
builder.Services.AddScoped<IRepository<Habilidade>, Repository<Habilidade>>();
builder.Services.AddScoped<IRepository<Possui>, Repository<Possui>>();
builder.Services.AddScoped<IRepository<Startup>, Repository<Startup>>();
builder.Services.AddScoped<IRepository<Usuario>, Repository<Usuario>>();
builder.Services.AddScoped<StartupProcedureRepository>();

// Validators
builder.Services.AddScoped<CreateAvaliacaoRequestValidator>();
builder.Services.AddScoped<CreateHabilidadeRequestValidator>();
builder.Services.AddScoped<CreatePossuiRequestValidator>();
builder.Services.AddScoped<CreateStartupRequestValidator>();
builder.Services.AddScoped<CreateUsuarioRequestValidator>();

// UseCases
builder.Services.AddScoped<AvaliacaoUseCase>();
builder.Services.AddScoped<HabilidadeUseCase>();
builder.Services.AddScoped<PossuiUseCase>();
builder.Services.AddScoped<StartupUseCase>();
builder.Services.AddScoped<UsuarioUseCase>();
builder.Services.AddScoped<AnaliseStartupUseCase>();

// ===============================
// BEHAVIOR
// ===============================
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// ===============================
// VERSIONAMENTO
// ===============================
builder.Services.AddApiVersioning(o =>
{
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.DefaultApiVersion = new ApiVersion(1, 0);
});

builder.Services.AddVersionedApiExplorer(setup =>
{
    setup.GroupNameFormat = "'v'VVV";
    setup.SubstituteApiVersionInUrl = true;
});

// ===============================
// SWAGGER
// ===============================
builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<SwaggerDefaultValues>();

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT desta forma: Bearer {seu_token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();

// ===============================
// HEALTH CHECKS
// ===============================

// URL do health para ambiente Azure (HEALTHCHECK_URL)
var healthUrl = builder.Configuration["HEALTHCHECK_URL"] ?? "/health";

builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), name: "sqlserver");

builder.Services.AddHealthChecksUI(opt =>
{
    opt.SetEvaluationTimeInSeconds(10);
    opt.MaximumHistoryEntriesPerEndpoint(60);
    opt.AddHealthCheckEndpoint("API Health", healthUrl);
}).AddInMemoryStorage();

// ===============================
// JWT AUTHENTICATION
// ===============================
builder.Services.AddSingleton<TokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ===============================
// OPEN TELEMETRY
// ===============================
builder.Services.AddOpenTelemetry()
    .WithTracing(t =>
    {
        t.AddAspNetCoreInstrumentation()
         .AddHttpClientInstrumentation()
         .AddSource("Microsoft.EntityFrameworkCore")
         .AddConsoleExporter();
    });

// ===============================
// APP
// ===============================
var app = builder.Build();
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                $"NebuloHub API - {description.GroupName.ToUpper()}");
        }
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
});

app.Run();

// Necessário para WebApplicationFactory
public partial class Program { }
