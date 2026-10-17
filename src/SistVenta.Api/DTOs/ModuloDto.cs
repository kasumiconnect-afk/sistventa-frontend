namespace SistVenta.Api.DTOs;

public class ModuloDto
{
    public int IdModulo { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Icono { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int Orden { get; set; }
    public int Estado_id { get; set; }
    public DateTime Fecha { get; set; }
}

public class CreateModuloDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Icono { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int Orden { get; set; }
}

public class UpdateModuloDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Icono { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int Orden { get; set; }
}
