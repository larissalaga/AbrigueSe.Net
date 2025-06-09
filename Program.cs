using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using AbrigueSe.Configuration;
using AbrigueSe.Data;
using AbrigueSe.Mappings;
using AbrigueSe.Repositories.Implementations;
using AbrigueSe.Repositories.Interfaces;
using Microsoft.OpenApi.Models; // Para OpenApiInfo
using System.Reflection; // Para XmlComments
using AbrigueSe.MlModels; // Adicionar using para GenerativeAIService
using AspNetCoreRateLimit; // Adicionado para Rate Limiting

var builder = WebApplication.CreateBuilder(args);
// Update the code to fix the error
builder.Services.AddControllers()
    .AddNewtonsoftJson(); // Adicionado para melhor manipulação de DTOs com herança (HATEOAS)


// Add services to the container.
// Initialize ConfigurationManager with the configuration
ConfigManager.Instance.Initialize(builder.Configuration);

// Registrando o AutoMapper com o perfil de mapeamento
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson(); // Adicionado para melhor manipulação de DTOs com herança (HATEOAS)

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Abrigue-Se API",
        Description = "API RESTful para o sistema Abrigue-Se, uma plataforma para gerenciamento de abrigos e assistência em eventos extremos.",
        Contact = new OpenApiContact
        {
            Name = "Equipe Abrigue-Se",
            Email = "contato@abriguese.com.br", // Exemplo
            Url = new Uri("https://abriguese.com.br") // Exemplo
        },
        License = new OpenApiLicense
        {
            Name = "MIT License", // Exemplo
            Url = new Uri("https://opensource.org/licenses/MIT") // Exemplo
        }
    });

    // Configurar Swagger para usar o arquivo XML de comentários
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseOracle(ConfigManager.Instance.GetConnectionString("OracleConnection"));
});

// Registering repositories
builder.Services.AddScoped<IAbrigoRepository, AbrigoRepository>();
builder.Services.AddScoped<IPessoaRepository, PessoaRepository>(); // Novo
builder.Services.AddScoped<IRecursoRepository, RecursoRepository>(); // Novo
builder.Services.AddScoped<ICheckInRepository, CheckInRepository>(); // Alterado de IAbrigoPessoaRepository, AbrigoPessoaRepository
builder.Services.AddScoped<IEstoqueRecursoRepository, EstoqueRecursoRepository>(); // Novo
builder.Services.AddScoped<ITipoUsuarioRepository, TipoUsuarioRepository>(); // Novo
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>(); // Novo
builder.Services.AddScoped<IPaisRepository, PaisRepository>(); // Novo
builder.Services.AddScoped<IEstadoRepository, EstadoRepository>(); // Novo
builder.Services.AddScoped<ICidadeRepository, CidadeRepository>(); // Novo
builder.Services.AddScoped<IEnderecoRepository, EnderecoRepository>(); // Novo

// Register Generative AI service
builder.Services.AddSingleton<GenerativeAIService>(); // Registrado como Singleton conforme exemplo anterior, pode ser Scoped se fizer sentido.
//builder.Services.AddScoped<MlService>(); // Se existir outro serviço de ML
//builder.Services.AddHostedService<MlInitializationService>(); // Se existir um serviço de inicialização

builder.Services.AddDataProtection();
builder.Services.AddSingleton(ConfigManager.Instance);

// Configurações do Rate Limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
builder.Services.AddInMemoryRateLimiting();

// --- Google Authentication configuration ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    // Force account selection every time
    options.Events.OnRedirectToAuthorizationEndpoint = context =>
    {
        var prompt = "select_account";
        var redirectUri = context.RedirectUri;
        // Add prompt=select_account to the query string
        if (!redirectUri.Contains("prompt="))
        {
            redirectUri += (redirectUri.Contains("?") ? "&" : "?") + "prompt=" + prompt;
        }
        context.Response.Redirect(redirectUri);
        return Task.CompletedTask;
    };
});
// --- End Google Authentication configuration ---

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();

    // Serve static files from StaticFiles folder
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles")),
        RequestPath = ""
    });

    // Require authentication for Swagger UI and Swagger JSON
    app.UseWhen(
        context => context.Request.Path.StartsWithSegments("/swagger"),
        appBuilder =>
        {
            appBuilder.UseAuthentication();
            appBuilder.UseAuthorization();
            appBuilder.Use(async (context, next) =>
            {
                if (!context.User.Identity?.IsAuthenticated ?? true)
                {
                    await context.ChallengeAsync(GoogleDefaults.AuthenticationScheme);
                    return;
                }
                await next();
            });

            // Place SwaggerUI here so it's protected
            appBuilder.UseSwaggerUI(options =>
            {
                options.InjectJavascript("/swagger-user.js");
            });
        });
}

app.UseHttpsRedirection();

app.UseAuthentication(); // <-- Add this line before UseAuthorization
app.UseAuthorization();

// Adiciona o middleware do Rate Limiting
app.UseIpRateLimiting();

app.MapControllers();

app.Run();

public partial class Program { }