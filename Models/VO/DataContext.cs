
using InmobiliariaGutierrez.Models.VO;
using Microsoft.EntityFrameworkCore;

using Laboratorio_3.Models.VO;


namespace Laboratorio_3.Models

{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{

		}
		public DbSet<Propietario> Propietarios { get; set; }
	     public DbSet<Inquilino> Inquilinos { get; set; }
		public DbSet<Inmueble> Inmuebles { get; set; }
		public DbSet<Inmuebletipos> Inmuebletipos { get; set; }
		public DbSet<pagos> Pagos { get; set; }

		public DbSet<Contrato> Contratos { get; set; }
		//public DbSet<Coordenada> Coordenadas { get; set; }
		 protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<pagos>().HasNoKey().ToTable("Pagos");
    }
		
	
	}
}