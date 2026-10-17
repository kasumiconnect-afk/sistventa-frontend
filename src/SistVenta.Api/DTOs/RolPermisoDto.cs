namespace SistVenta.Api.DTOs;

public class RolPermisoDto
{
    public int IdRolPermiso { get; set; }
    public int IdRol { get; set; }
    public string RolNombre { get; set; } = string.Empty;
    public int IdPermiso { get; set; }
    public string PermisoCodigo { get; set; } = string.Empty;
    public string PermisoDescripcion { get; set; } = string.Empty;
    public int Estado_id { get; set; }
    public DateTime Fecha { get; set; }
}

public class CreateRolPermisoDto
{
    public int IdRol { get; set; }
    public int IdPermiso { get; set; }
}
