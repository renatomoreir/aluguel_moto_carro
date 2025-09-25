using Microsoft.EntityFrameworkCore;
using Mottu.Infrastructure.Data;
using Mottu.Domain.Interfaces;
using Mottu.Infrastructure.Repositories;
using Mottu.Application.Interfaces;
using Mottu.Application.Services;
using Mottu.Infrastructure.Messaging;
using Mottu.API.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Verificar se deve habilitar banco de dados
var enableDatabase = builder.Configuration.GetValue<bool>("Features:EnableDatabase", true);
if (enableDatabase)
{
    // Configure Entity Framework with PostgreSQL
    builder.Services.AddDbContext<MottuDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
}
else
{
    // Configure In-Memory Database para testes
    builder.Services.AddDbContext<MottuDbContext>(options =>
        options.UseInMemoryDatabase("MottuTestDb"));
}

// Register repositories
builder.Services.AddScoped<IMotoRepository, MotoRepository>();
builder.Services.AddScoped<IEntregadorRepository, EntregadorRepository>();
builder.Services.AddScoped<ILocacaoRepository, LocacaoRepository>();

// Verificar se deve habilitar RabbitMQ
var enableRabbitMQ = builder.Configuration.GetValue<bool>("Features:EnableRabbitMQ", true);
if (enableRabbitMQ)
{
    // Register messaging services
    builder.Services.AddSingleton<IMessagePublisher, RabbitMQPublisher>();
    builder.Services.AddHostedService<MotoConsumer>();
}
else
{
    // Register optional messaging service para testes
    builder.Services.AddSingleton<IMessagePublisher, RabbitMQPublisherOptional>();
}

// Register services
builder.Services.AddScoped<IMotoService, MotoService>();
builder.Services.AddScoped<IEntregadorService, EntregadorService>();
builder.Services.AddScoped<ILocacaoService, LocacaoService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Mottu  API", 
        Version = "v1.0.0",
        Description = "API para gerenciamento de aluguel de motos e entregadores - Desafio  Mottu",
        Contact = new()
        {
            Name = "Mottu ",
            Email = "contato@mottu.com"
        }
    });

    // Incluir comentários XML
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Configurar exemplos de schema
    c.SchemaFilter<SchemaFilter>();
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run("http://0.0.0.0:5000");

