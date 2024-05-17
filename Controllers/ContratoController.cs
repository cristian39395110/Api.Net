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
{     
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


}
}
