using Microsoft.EntityFrameworkCore;
using SistVenta.Api.Models;

namespace SistVenta.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Catálogos
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Proveedor> Proveedores { get; set; }
    public DbSet<Rol> Roles { get; set; }
    public DbSet<Tienda> Tiendas { get; set; }
    public DbSet<TipoComprobante> TiposComprobante { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Estado> Estados { get; set; }
    public DbSet<Permiso> Permisos { get; set; }
    public DbSet<Modulo> Modulos { get; set; }
    public DbSet<RolPermiso> RolPermisos { get; set; }
    public DbSet<LogLogin> LogLogins { get; set; }
    public DbSet<Entrega> Entregas { get; set; }

    // Operaciones
    public DbSet<Compra> Compras { get; set; }
    public DbSet<DetalleCompra> DetallesCompra { get; set; }
    public DbSet<Venta> Ventas { get; set; }
    public DbSet<DetalleVenta> DetallesVenta { get; set; }
    public DbSet<Cotizacion> Cotizaciones { get; set; }
    public DbSet<DetalleCotizacion> DetallesCotizacion { get; set; }
    public DbSet<MetodoPagoVenta> MetodosPagoVenta { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurar nombres de tablas para coincidir con la base de datos existente
        modelBuilder.Entity<Producto>().ToTable("producto");
        modelBuilder.Entity<Categoria>().ToTable("categoria");
        modelBuilder.Entity<Cliente>().ToTable("cliente");
        modelBuilder.Entity<Proveedor>().ToTable("proveedor");
        modelBuilder.Entity<Rol>().ToTable("rol");
        modelBuilder.Entity<Tienda>().ToTable("tienda");
        modelBuilder.Entity<TipoComprobante>().ToTable("tipocomprobante");
        modelBuilder.Entity<Usuario>().ToTable("usuario");
        modelBuilder.Entity<Estado>().ToTable("estado");
        modelBuilder.Entity<Permiso>().ToTable("permiso");
        modelBuilder.Entity<Modulo>().ToTable("modulo");
        modelBuilder.Entity<RolPermiso>().ToTable("rolpermiso");
        modelBuilder.Entity<LogLogin>().ToTable("loglogin");
        modelBuilder.Entity<Entrega>().ToTable("entrega");
        modelBuilder.Entity<Compra>().ToTable("compra");
        modelBuilder.Entity<DetalleCompra>().ToTable("detallecompra");
        modelBuilder.Entity<Venta>().ToTable("venta");
        modelBuilder.Entity<DetalleVenta>().ToTable("detalle_venta");
        modelBuilder.Entity<Cotizacion>().ToTable("cotizacion");
        modelBuilder.Entity<DetalleCotizacion>().ToTable("detalle_cotizacion");
        modelBuilder.Entity<MetodoPagoVenta>().ToTable("metodo_pago_venta");

        // Configurar relación entre MetodoPagoVenta y Venta
        modelBuilder.Entity<MetodoPagoVenta>()
            .HasOne(mp => mp.Venta)
            .WithMany(v => v.MetodosPago)
            .HasForeignKey(mp => mp.IdVenta)
            .OnDelete(DeleteBehavior.Cascade);

        // Configurar nombre de columna para clave primaria de MetodoPagoVenta
        modelBuilder.Entity<MetodoPagoVenta>()
            .Property(mp => mp.IdMetodoPago)
            .HasColumnName("IdMetodoPago");

        // Configurar relación entre Venta y Entrega
        modelBuilder.Entity<Venta>()
            .HasOne(v => v.Entrega)
            .WithMany()
            .HasForeignKey(v => v.IdEntrega)
            .OnDelete(DeleteBehavior.Restrict);

        // Configurar relación entre Cotizacion y Entrega
        modelBuilder.Entity<Cotizacion>()
            .HasOne(c => c.Entrega)
            .WithMany()
            .HasForeignKey(c => c.IdEntrega)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
