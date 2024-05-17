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
	public class InmueblesController : Controller
	{
		private readonly DataContext contexto;

		public InmueblesController(DataContext contexto)
		{
			this.contexto = contexto;
		}

		// GET: api/<controller>
		[HttpGet ("Get")]
		public async Task<IActionResult> Get()
		{     
    var user = HttpContext.User;

    var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

  
 if (int.TryParse(userIdClaim, out int Id))
   {
    var inmueblesDelPropietario = contexto.Inmuebles
	    .Include (i=> i.InmuebleTipo)
        .Include (i=> i.Propietario)
        .Where(i => i.PropietarioId == Id)
		
        .ToList();

    return Ok(inmueblesDelPropietario);
   }
    else
    {
        return BadRequest("Invalid user ID format in token");
    }
}

		// GET api/<controller>/5
		[HttpGet("GetInmuebles")]
		public async Task<IActionResult> GetInmuebles()
		{
			   var user = HttpContext.User;

    var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

  
 if (int.TryParse(userIdClaim, out int Id))
   {
    var inmueblesDelPropietario = contexto.Inmuebles
	    .Include (i=> i.InmuebleTipo)
        .Where(i => i.PropietarioId == Id)
		
        .ToList();

    return Ok(inmueblesDelPropietario);
   }
    else
    {
        return BadRequest("Invalid user ID format in token");
    }
		}

		// POST api/<controller>
		[HttpPost("Post")]
		public async Task<IActionResult> Post([FromBody] Inmueble entidad)

		{   Console.WriteLine("carajo");
			try
			{
				if (ModelState.IsValid)
				{   Console.WriteLine("carajo");
					entidad.PropietarioId = contexto.Propietarios.Single(e => e.Email == User.Identity.Name).Id;
					contexto.Inmuebles.Add(entidad);
					contexto.SaveChanges();
					return CreatedAtAction(nameof(Get), new { id = entidad.Id }, entidad);
				}
				return BadRequest();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// PUT api/<controller>/5
		[HttpPut("{id}")]
		public async Task<IActionResult> Put(int id, Inmueble entidad)
		{
			try
			{
				if (ModelState.IsValid && contexto.Inmuebles.AsNoTracking().Include(e => e.Propietario).FirstOrDefault(e => e.Id == id && e.Propietario.Email == User.Identity.Name) != null)
				{
					entidad.Id = id;
					contexto.Inmuebles.Update(entidad);
					contexto.SaveChanges();
					return Ok(entidad);
				}
				return BadRequest();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// DELETE api/<controller>/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				var entidad = contexto.Inmuebles.Include(e => e.Propietario).FirstOrDefault(e => e.Id == id && e.Propietario.Email == User.Identity.Name);
				if (entidad != null)
				{
					contexto.Inmuebles.Remove(entidad);
					contexto.SaveChanges();
					return Ok();
				}
				return BadRequest();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// DELETE api/<controller>/5
		[HttpDelete("BajaLogica/{id}")]
		public async Task<IActionResult> BajaLogica(int id)
		{
			try
			{
				var entidad = contexto.Inmuebles.Include(e => e.Propietario).FirstOrDefault(e => e.Id == id && e.Propietario.Email == User.Identity.Name);
				if (entidad != null)
				{
					//entidad.Superficie = -1;//cambiar por estado = 0
					contexto.Inmuebles.Update(entidad);
					contexto.SaveChanges();
					return Ok();
				}
				return BadRequest();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
/*
{
    "Direccion":"Pucara",
    "InmuebleTipoId":3,
    "CantidadAmbientes":10,
    "Suspendido": 0,
    "Disponible":1,
    "PrecioBase":70000,
    "Uso":"Residencial"
   
}
http://localhost:5000/api/Inmuebles/Post

http://localhost:5000/api/Propietarios/login?Usuario=luzza@hotmail.com&Clave=luzza@hotmail.com

*/