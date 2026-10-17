using Microsoft.EntityFrameworkCore;
using SistVenta.Api.Data;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

// Configure EF Core with MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "SistVentaSecretKey2024!ForJWTTokenGeneration";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "SistVentaAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "SistVentaClient";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero // Eliminar el desfase de tiempo
    };
});

builder.Services.AddAuthorization();

// Add CORS for frontend
var frontendUrl = builder.Configuration["FrontendUrl"] ?? "http://localhost:5173";
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:5174",
                "http://localhost:5175",
                "http://192.168.0.95:5173",
                "http://192.168.0.95:5174",
                "http://192.168.0.95:5175",
                "http://192.168.56.1:5173",
                "http://192.168.56.1:5174",
                "http://192.168.56.1:5175",
                "http://192.168.43.113:5173",
                "http://192.168.43.113:5174",
                "http://192.168.43.113:5175",
                "http://192.168.1.7:5173",
                "http://192.168.1.7:5174",
                "http://192.168.1.7:5175",
                "http://192.168.1.8:5173",
                "http://192.168.1.8:5174",
                "http://192.168.1.8:5175",
                "http://192.168.1.9:5173",
                "http://192.168.1.9:5174",
                "http://192.168.1.9:5175",
                frontendUrl // Production URL from configuration
            )
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials()
                  .WithExposedHeaders("X-Total-Count", "X-Page-Size"); // Exponer headers personalizados
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
// app.UseHttpsRedirection(); // Deshabilitado para desarrollo local
app.UseCors("AllowReactApp");
app.UseAuthentication(); // Importante: debe estar antes de UseAuthorization
app.UseAuthorization();
app.MapControllers();

// Escuchar en todas las interfaces de red para acceso desde celular
app.Run("http://0.0.0.0:5164");

