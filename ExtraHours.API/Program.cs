using Microsoft.EntityFrameworkCore;
using ExtraHours.API.Data;
using ExtraHours.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using ExtraHours.API.Services;
using ExtraHours.API.Repositories;
using ExtraHours.API.Interfaces;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// 💼 Servicios de la aplicación
builder.Services.AddScoped<IExtraHourService, ExtraHourService>();
builder.Services.AddScoped<IExtraHourRepository, ExtraHourRepository>();
builder.Services.AddScoped<IExtraHourRequestService, ExtraHourRequestService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IExtraHourRequestRepository, ExtraHourRequestRepository>();
builder.Services.AddHttpContextAccessor(); // ✅ Necesario para acceder al usuario autenticado

// 🔗 Configuración de conexión a base de datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 🌐 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// 🎯 Controladores con configuración unificada
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// 📘 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "ExtraHours API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingresa el token JWT con el prefijo Bearer. Ejemplo: 'Bearer TU_TOKEN'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// 🔐 Autenticación y autorización
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(UserRole.Admin.ToString()));
    options.AddPolicy("ManagerOrAdmin", policy => policy.RequireRole(UserRole.Manager.ToString(), UserRole.Admin.ToString()));
});

// 📎 Límite de tamaño para archivos
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 5 * 1024 * 1024; // 5MB
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();

        // 🚀 Aplicar migraciones pendientes ANTES de intentar acceder a cualquier tabla
        // Esto asegura que las tablas existan cuando intentas inicializar datos.
        context.Database.Migrate();

        // 🧪 Carga inicial de datos (Seed Data)
        // Ahora puedes estar seguro de que las tablas como 'Departments' existen.
        if (!context.Departments.Any(d => d.Name == "Administración"))
            context.Departments.Add(new Department { Name = "Administración", Employees = 0, Status = "Activo" });

        if (!context.Departments.Any(d => d.Name == "Ventas"))
            context.Departments.Add(new Department { Name = "Ventas", Employees = 0, Status = "Activo" });

        // Guarda los cambios después de añadir los departamentos si quieres consultarlos inmediatamente.
        // A menudo es una buena práctica guardar todas las inicializaciones al final, pero para datos relacionales, 
        // podrías necesitar guardar las entidades padre primero.
        context.SaveChanges();

        var administracion = context.Departments.FirstOrDefault(d => d.Name == "Administración");
        var ventas = context.Departments.FirstOrDefault(d => d.Name == "Ventas");

        if (administracion == null || ventas == null)
            throw new Exception("No se pudieron obtener los departamentos requeridos para la inicialización de usuarios.");

        if (!context.Users.Any(u => u.Email == "admin@ejemplo.com"))
        {
            context.Users.Add(new User
            {
                Username = "admin",
                Email = "admin@ejemplo.com",
                FirstName = "Administrador",
                LastName = "Principal",
                Role = UserRole.Admin,
                Department = administracion,
                Position = "Administrador General",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123")
            });
        }

        if (!context.Users.Any(u => u.Email == "empleado1@ejemplo.com"))
        {
            context.Users.Add(new User
            {
                Username = "empleado1",
                Email = "empleado1@ejemplo.com",
                FirstName = "Vero",
                LastName = "Morante",
                Role = UserRole.Employee,
                Department = ventas,
                Position = "Vendedora",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("empleado123")
            });
        }

        context.SaveChanges(); // Guarda todos los cambios al final de la inicialización
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al migrar o inicializar la base de datos.");
        // Dependiendo de tu estrategia de despliegue, podrías querer re-lanzar la excepción
        // o simplemente registrarla y dejar que la aplicación continúe si el error no es crítico para el inicio.
        // Para la creación/inicialización de la base de datos, suele ser crítico.
        throw; // Re-lanzar para asegurar que la aplicación falle si la configuración de la DB falla
    }
}