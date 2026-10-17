namespace SistVenta.Api.DTOs;

public class PermisoDto
{
    public int IdPermiso { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int IdModulo { get; set; }
    public string ModuloNombre { get; set; } = string.Empty;
    public int Estado_id { get; set; }
    public DateTime Fecha { get; set; }
}

public class CreatePermisoDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int IdModulo { get; set; }
}

public class UpdatePermisoDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int IdModulo { get; set; }
}
