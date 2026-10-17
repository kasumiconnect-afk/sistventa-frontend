using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SistVenta.Api.Data;
using SistVenta.Api.DTOs;
using SistVenta.Api.Helpers;
using SistVenta.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SistVenta.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Correo == dto.Correo && u.Estado_id == 1);

        // Registrar intento de login
        var ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
        await RegistrarIntentoLogin(dto.Correo, false, ip);

        if (usuario == null)
        {
            return Ok(new AuthResponseDto
            {
                Exitoso = false,
                Mensaje = "Credenciales inválidas"
            });
        }

        if (!PasswordHelper.VerifyPassword(dto.Contrasena, usuario.Contrasena))
        {
            return Ok(new AuthResponseDto
            {
                Exitoso = false,
                Mensaje = "Credenciales inválidas"
            });
        }

        // Actualizar registro de login exitoso
        await ActualizarIntentoLoginExitoso(dto.Correo, ip);

        // Cargar permisos del usuario
        var permisos = await ObtenerPermisosUsuario(usuario.IdRol);

        // Generar Token JWT
        var token = GenerateJwtToken(usuario);

        return Ok(new AuthResponseDto
        {
            Exitoso = true,
            Mensaje = "Login exitoso",
            IdUsuario = usuario.IdUsuario,
            Nombres = usuario.Nombres,
            Apellidos = usuario.Apellidos,
            Correo = usuario.Correo,
            IdRol = usuario.IdRol,
            RolNombre = usuario.Rol?.Nombre,
            NombreCompleto = usuario.NombreCompleto,
            Token = token,
            Expiracion = DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60")),
            Permisos = permisos
        });
    }

    private string GenerateJwtToken(Usuario usuario)
    {
        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];
        var expiryMinutes = int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Correo),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, usuario.NombreCompleto),
            new Claim(ClaimTypes.Role, usuario.Rol?.Nombre ?? "Usuario"),
            new Claim("IdRol", usuario.IdRol.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        try
        {
            // Validar el token expirado
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtKey = _configuration["Jwt:Key"];
            
            var principal = tokenHandler.ValidateToken(dto.Token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, // Permitir tokens expirados para refresh
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
            }, out _);

            var userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return BadRequest("Token inválido");
            }

            // En una implementación completa, aquí validarías el refresh token en la base de datos
            // Por ahora, generamos un nuevo token
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.IdUsuario == int.Parse(userIdClaim));

            if (usuario == null)
            {
                return BadRequest("Usuario no encontrado");
            }

            // Cargar permisos del usuario
            var permisos = await ObtenerPermisosUsuario(usuario.IdRol);

            var newToken = GenerateJwtToken(usuario);
            var newRefreshToken = GenerateRefreshToken();

            return Ok(new AuthResponseDto
            {
                Exitoso = true,
                Mensaje = "Token refrescado exitosamente",
                IdUsuario = usuario.IdUsuario,
                Nombres = usuario.Nombres,
                Apellidos = usuario.Apellidos,
                Correo = usuario.Correo,
                IdRol = usuario.IdRol,
                RolNombre = usuario.Rol?.Nombre,
                NombreCompleto = usuario.NombreCompleto,
                Token = newToken,
                RefreshToken = newRefreshToken,
                Expiracion = DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:ExpiryMinutes"] ?? "60")),
                Permisos = permisos
            });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al refrescar token: {ex.Message}");
        }
    }

    private async Task RegistrarIntentoLogin(string correo, bool exitoso, string ip)
    {
        var log = new LogLogin
        {
            Correo = correo,
            Exitoso = exitoso,
            IP = ip,
            Fecha = DateTime.Now
        };
        _context.LogLogins.Add(log);
        await _context.SaveChangesAsync();
    }

    private async Task ActualizarIntentoLoginExitoso(string correo, string ip)
    {
        var ultimoLog = await _context.LogLogins
            .Where(l => l.Correo == correo && !l.Exitoso)
            .OrderByDescending(l => l.Fecha)
            .FirstOrDefaultAsync();

        if (ultimoLog != null)
        {
            ultimoLog.Exitoso = true;
            await _context.SaveChangesAsync();
        }
    }

    private async Task<List<string>> ObtenerPermisosUsuario(int idRol)
    {
        try
        {
            Console.WriteLine($"🔍 Cargando permisos para rol ID: {idRol}");

            var permisos = await _context.RolPermisos
                .Include(rp => rp.Permiso)
                .Where(rp => rp.IdRol == idRol && rp.Estado_id == 1)
                .Select(rp => rp.Permiso!.Codigo)
                .ToListAsync();

            Console.WriteLine($"✅ Permisos cargados: {permisos.Count} permisos encontrados");
            Console.WriteLine($"📋 Permisos: {string.Join(", ", permisos)}");

            return permisos;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al cargar permisos: {ex.Message}");
            Console.WriteLine($"🔍 Stack trace: {ex.StackTrace}");
            return new List<string>();
        }
    }
}
