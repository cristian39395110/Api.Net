

using System.ComponentModel.DataAnnotations.Schema;
using InmobiliariaGutierrez.Models.VO;

namespace Laboratorio_3.Models.VO;

public class Inmueble
{
	public int? Id { get; set; }

	[ForeignKey(nameof(Propietario))]
	public int? PropietarioId { get; set; }

	public Propietario? Propietario { get; set; }
	public string? Direccion { get; set; }
	[ForeignKey(nameof(Inmuebletipos))]
	public int? InmuebleTipoId { get; set; } // Cambiado

	public Inmuebletipos? InmuebleTipo { get; set; } // Agregado

	public int CantidadAmbientes { get; set; }
	public bool? Suspendido { get; set; }
	public bool? Disponible { get; set; }
	public decimal? PrecioBase { get; set; }
	
	 public string? Imagen { get; set; } 

	public string? Uso { get; set; }


	public override string ToString()
	{
		return $" Id:{Id} , PropietarioId: {PropietarioId}, Dirección: {Direccion}, Tipo: {InmuebleTipoId}, Cantidad de Ambientes: {CantidadAmbientes}, Uso: {Uso}, Precio: {PrecioBase}";
	}
}


