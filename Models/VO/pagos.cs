using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaGutierrez.Models.VO;

public class pagos
{
  
    [ForeignKey(nameof(Contrato))]
    public int ContratoId { get; set; }
    public Contrato? Contrato { get; set; }
    public  int NumeroPago { get; set; }
    public  DateTime? FechaPago { get; set; }
	public DateTime Fecha { get; set; }
	
    public decimal Importe { get; set; }
       public override string ToString()
        {
            return
                   $"Contrato ID: {ContratoId}, " +
                   $"Numero de Pago: {NumeroPago}, " +
                   $"Fecha de Pago: {FechaPago?.ToString("yyyy-MM-dd") ?? "N/A"}, " +
                   $"Fecha: {Fecha.ToString("yyyy-MM-dd")}, " +
                   $"Importe: {Importe:C}";
        }


    
}

