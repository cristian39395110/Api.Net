using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaGutierrez.Models.VO;
/*Cuando el inquilino viene a alquilar un local se lo entrevista
solicitando sus datos personales. ABM inquilino. DNI, nombre
completo y datos de contacto.*/
public class Inquilino
{
    
	public int Id { get; set; }

    [Required(ErrorMessage="Campo DNI obligatorio.")]
    public String? DNI {get;set;}

    [Required(ErrorMessage="Campo DNI obligatorio.")]
	public String? Nombre { get; set; }

    [Required(ErrorMessage="Campo APellido obligatorio.")]
    public String? Apellido { get; set; }
    
   
    [Required(ErrorMessage = "El teléfono es obligatorio"),StringLength(15, ErrorMessage = "El teléfono no puede tener más de 15 caracteres")]
   // [RegularExpression(@"^[0-9]+$", ErrorMessage = "El teléfono solo puede contener números")]
    public String? Telefono { get; set; }
    [Required, EmailAddress]
    public String? Email { get; set; }
    [Required(ErrorMessage="Campo Domicilio obligatorio.")]
    public String? Domicilio { get; set; }
    [NotMapped]
    public List<Contrato> ListaContratos{get;set;}

    public Inquilino(){
        ListaContratos = new List<Contrato>();
    }
    
      public override string ToString()
        {  string? listaContratosStr = ListaContratos != null ? ListaContratos.ToString() : "Ningún inmueble asignado.";
            return $"Id: {Id}, DNI: {DNI}, Nombre: {Nombre}, Apellido: {Apellido}, Telefono: {Telefono}, Email: {Email}, Domicilio: {Domicilio}, ListaContratos: {ListaContratos?.Count}";
        }
}

    /*El inquilino puede terminar antes el contrato si lo desea, pero
pagando una multa. En caso de terminar el alquiler, se debe
actualizar la fecha de fin del contrato y calcular la multa. Si se
cumplió menos de la mitad del tiempo original de alquiler, deberá
pagar 2 (dos) meses extra de alquiler. Caso contrario, sólo uno.
Además, se debe revisar que no deba meses de alquiler. El sistema
no registra el pago de la multa, pero sí debe informar ese valor.*/


