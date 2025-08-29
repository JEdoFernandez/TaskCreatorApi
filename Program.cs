using Microsoft.EntityFrameworkCore;
using TaskCreatorAPI.Data;
using TaskCreatorAPI.Services;
using TaskCreatorAPI.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuración de la base de datos SQLite con ruta absoluta
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "TaskCreatorDB.db");
    options.UseSqlite($"Data Source={dbPath}");
    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
});

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// Registro de repositorios
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<TareaRepository>();
builder.Services.AddScoped<TareaPublicaRepository>();
builder.Services.AddScoped<TareaPublicaCompletadaRepository>();

// Registro de servicios
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<TareaService>();
builder.Services.AddScoped<TareaPublicaService>();
builder.Services.AddScoped<TareaPublicaCompletadaService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuración de Swagger con autenticación JWT
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskCreator API", Version = "v1" });

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Introduce tu token JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

// Configuración de autenticación JWT
var jwtConfig = builder.Configuration.GetSection("JwtSettings");
var claveSecreta = jwtConfig["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtConfig["Issuer"],
        ValidAudience = jwtConfig["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(claveSecreta!))
    };
});

// Configuración de autorización con roles
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

var app = builder.Build();

// Middleware de CORS
app.UseCors("AllowAll");

// Middleware de Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Inicialización de la base de datos al iniciar la aplicación
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    try
    {
        // Obtener la ruta completa de la base de datos
        var dbPath = db.Database.GetDbConnection().DataSource;
        var fullPath = Path.GetFullPath(dbPath);
        
        // Asegurar que el directorio existe
        var directory = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(directory) && directory != null)
        {
            Directory.CreateDirectory(directory);
        }
        
        // Crear la base de datos y tablas si no existen
        var created = db.Database.EnsureCreated();
        
        if (created)
        {
            Console.WriteLine("Base de datos y tablas creadas exitosamente");
        }
        else
        {
            Console.WriteLine("Base de datos ya existe");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error inicializando la base de datos: {ex.Message}");
        
        // Fallback: intentar crear tablas manualmente si EnsureCreated falla
        try
        {
            db.Database.ExecuteSqlRaw(@"
                CREATE TABLE IF NOT EXISTS Usuarios (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Nombre TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    Contraseña TEXT NOT NULL,
                    FechaRegistro TEXT NOT NULL,
                    Activo INTEGER NOT NULL,
                    Rol TEXT NOT NULL
                )");
                
            Console.WriteLine("Tabla Usuarios creada manualmente");
        }
        catch (Exception manualEx)
        {
            Console.WriteLine($"Error creando tabla manualmente: {manualEx.Message}");
        }
    }
}

// Mapeo de controladores
app.MapControllers();

app.Run();