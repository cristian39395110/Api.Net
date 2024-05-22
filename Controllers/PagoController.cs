using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Laboratorio_3.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Laboratorio_3.Models.VO;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Laboratorio_3.Controllers
{
	[Route("api/[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[ApiController]
	public class PagoController : Controller
	{
		private readonly DataContext contexto;

		public PagoController(DataContext contexto)
		{
			this.contexto = contexto;
		}

		// GET: api/<controller>

[HttpGet("GetContrato")]
 // Requires valid JWT token for access
public IActionResult GetContrato(int id)
{     Console.WriteLine("hola");
    var user = HttpContext.User;

    var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
Console.WriteLine("hola");
      var inmueblesDelPropietario = contexto.Contratos
	    .Include (i=> i.Inquilino)
		.Include (i=> i.Inmueble)
        .Where(i => i.InmuebleId == id)
		
        .ToList();

    return Ok(inmueblesDelPropietario);

}
[HttpGet("pago/{id}")]
        public async Task<IActionResult> GetPago(int id)
        {Console.WriteLine("caca");
            try
            {
               /*
                var usuario = User.Identity.Name;

                var propietario = await contexto.Propietarios
                    .FirstOrDefaultAsync(p => p.Email == usuario);

                if (propietario == null)
                {
                    return NotFound("Propietario no encontrado");
                }
*/
               
    var pagosContrato = contexto.Pagos
	   
        
        .Where(i => i.ContratoId == id)
		
        .ToList();

    return Ok(pagosContrato);

            }
            catch (Exception ex)
            {
                return BadRequest($"Error al obtener el contrato: {ex.Message}");
            }
        }
	}
}
