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
	public class ContratoController : Controller
	{
		private readonly DataContext contexto;

		public ContratoController(DataContext contexto)
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
[HttpGet("inmueble/{id}")]
        public async Task<IActionResult> GetContratoPorInmueble(int id)
        {Console.WriteLine("caca");
            try
            {
                var usuario = User.Identity.Name;

                var propietario = await contexto.Propietarios
                    .FirstOrDefaultAsync(p => p.Email == usuario);

                if (propietario == null)
                {
                    return NotFound("Propietario no encontrado");
                }

                var contrato = await contexto.Contratos
                    .Include(c => c.Inquilino)
                    .Include(c => c.Inmueble) // Incluye el objeto Inmueble en la consulta
                    .FirstOrDefaultAsync(c => c.InmuebleId == id && c.Estado == true);

                if (contrato == null)
                {
                    return NotFound("Contrato no encontrado");
                }

                return Ok(contrato);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al obtener el contrato: {ex.Message}");
            }
        }
	}
}
