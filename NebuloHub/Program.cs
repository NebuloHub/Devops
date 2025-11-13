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
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// JWT
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// Configuração do banco Oracle
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleNebuloHub")));

// Adiciona controladores e configura JsonOptions
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();

// Registro de dependências
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


// Configuração do comportamento API
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Versionamento
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

// Swagger
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

// HealthChecks
builder.Services.AddHealthChecks()
    .AddOracle(builder.Configuration.GetConnectionString("OracleNebuloHub"), name: "oracle");

builder.Services.AddHealthChecksUI(opt =>
{
    opt.SetEvaluationTimeInSeconds(10);
    opt.MaximumHistoryEntriesPerEndpoint(60);
    opt.AddHealthCheckEndpoint("API Health", "/health");
}).AddInMemoryStorage();

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerGenOptions>();
builder.Services.AddSingleton<TokenService>();

// JWT Authentication
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

var app = builder.Build();
var versionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in versionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                $"Web API - {description.GroupName.ToUpper()}");
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

// Para o WebApplicationFactory nos testes
public partial class Program { }