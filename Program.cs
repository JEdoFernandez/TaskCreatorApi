using Microsoft.EntityFrameworkCore;
using TaskCreatorAPI.Data;
using TaskCreatorAPI.Services;
using TaskCreatorAPI.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Conexión a base de datos - SQLite con ruta absoluta para debugging
builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Ruta absoluta para evitar problemas de permisos
    var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "TaskCreatorDB.db");
    Console.WriteLine($"📁 Database path: {dbPath}");
    
    options.UseSqlite($"Data Source={dbPath}");
    options.EnableSensitiveDataLogging(); // Para ver queries SQL
    options.EnableDetailedErrors(); // Para errores detallados
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// Repositorios
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<TareaRepository>();
builder.Services.AddScoped<TareaPublicaRepository>();
builder.Services.AddScoped<TareaPublicaCompletadaRepository>();

// Servicios
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<TareaService>();
builder.Services.AddScoped<TareaPublicaService>();
builder.Services.AddScoped<TareaPublicaCompletadaService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger con autenticación JWT
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

// JWT
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

// Autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

var app = builder.Build();

// CORS
app.UseCors("AllowAll");

// Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// 🔍 DEBUG DETALLADO DE LA BASE DE DATOS
Console.WriteLine("🚀 Iniciando aplicación TaskCreatorAPI");
Console.WriteLine("🔍 Debug: Información del sistema...");
Console.WriteLine($"📁 Directorio actual: {Directory.GetCurrentDirectory()}");
Console.WriteLine($"📁 Usuario: {Environment.UserName}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    Console.WriteLine("🔍 Debug: Obteniendo información de la base de datos...");
    
    try
    {
        var connection = db.Database.GetDbConnection();
        var dataSource = connection.DataSource;
        var fullPath = Path.GetFullPath(dataSource);
        
        Console.WriteLine($"📁 DataSource: {dataSource}");
        Console.WriteLine($"📁 Full Path: {fullPath}");
        Console.WriteLine($"📁 Directory: {Path.GetDirectoryName(fullPath)}");
        Console.WriteLine($"📁 File Exists: {File.Exists(fullPath)}");
        
        // Verificar si el directorio existe
        var directory = Path.GetDirectoryName(fullPath);
        if (!Directory.Exists(directory) && directory != null)
        {
            Console.WriteLine($"📁 Creando directorio: {directory}");
            Directory.CreateDirectory(directory);
        }
        
        // Verificar permisos de escritura
        try
        {
            var testFile = Path.Combine(directory ?? Directory.GetCurrentDirectory(), "test_write.txt");
            File.WriteAllText(testFile, "test");
            File.Delete(testFile);
            Console.WriteLine("✅ Permisos de escritura: OK");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Permisos de escritura: ERROR - {ex.Message}");
        }
        
        Console.WriteLine("🛠️ Intentando EnsureCreated...");
        var created = db.Database.EnsureCreated();
        
        Console.WriteLine($"✅ EnsureCreated result: {created}");
        Console.WriteLine($"📁 File exists after EnsureCreated: {File.Exists(fullPath)}");
        
        if (File.Exists(fullPath))
        {
            var fileInfo = new FileInfo(fullPath);
            Console.WriteLine($"📁 File size: {fileInfo.Length} bytes");
            Console.WriteLine($"📁 Created: {fileInfo.CreationTime}");
            
            // Verificar que las tablas se crearon
            try
            {
                var usuarioCount = db.Usuarios.Count();
                Console.WriteLine($"✅ Tabla Usuarios: OK ({usuarioCount} registros)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error accediendo a tabla Usuarios: {ex.Message}");
                
                // Intentar crear tablas manualmente
                Console.WriteLine("🛠️ Intentando crear tablas manualmente...");
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
                    Console.WriteLine("✅ Tabla Usuarios creada manualmente");
                }
                catch (Exception manualEx)
                {
                    Console.WriteLine($"❌ Error creando tabla manualmente: {manualEx.Message}");
                }
            }
        }
        else
        {
            Console.WriteLine("❌ El archivo de base de datos NO se creó");
            
            // Intentar crear el archivo manualmente
            try
            {
                Console.WriteLine("🛠️ Creando archivo de BD manualmente...");
                File.WriteAllBytes(fullPath, new byte[0]);
                Console.WriteLine("✅ Archivo creado manualmente");
                
                // Intentar EnsureCreated again
                created = db.Database.EnsureCreated();
                Console.WriteLine($"✅ EnsureCreated después de creación manual: {created}");
            }
            catch (Exception fileEx)
            {
                Console.WriteLine($"❌ Error creando archivo manualmente: {fileEx.Message}");
                
                // Probar con ruta alternativa
                var altPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TaskCreatorDB.db");
                Console.WriteLine($"🔄 Probando ruta alternativa: {altPath}");
                
                try
                {
                    var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                    optionsBuilder.UseSqlite($"Data Source={altPath}");
                    
                    using var tempDb = new AppDbContext(optionsBuilder.Options);
                    tempDb.Database.EnsureCreated();
                    Console.WriteLine($"✅ Base de datos creada en ubicación alternativa: {altPath}");
                }
                catch (Exception altEx)
                {
                    Console.WriteLine($"❌ Error en ubicación alternativa: {altEx.Message}");
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"💥 ERROR: {ex.Message}");
        Console.WriteLine($"💥 StackTrace: {ex.StackTrace}");
        
        if (ex.InnerException != null)
        {
            Console.WriteLine($"💥 Inner Exception: {ex.InnerException.Message}");
        }
    }
}

app.MapControllers();

// Middleware para mostrar información de la BD en cada request (solo desarrollo)
if (app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "TaskCreatorDB.db");
        context.Response.Headers.Append("X-DB-Path", dbPath);
        context.Response.Headers.Append("X-DB-Exists", File.Exists(dbPath).ToString());
        
        await next();
    });
}

Console.WriteLine("🎯 Aplicación iniciada. Presiona Ctrl+C para detener.");
app.Run();