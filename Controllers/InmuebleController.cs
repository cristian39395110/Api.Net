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
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using System.Text.RegularExpressions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Laboratorio_3.Controllers
{
	[Route("api/[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[ApiController]
	public class InmueblesController : Controller
	{
		 private readonly DataContext contexto;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public InmueblesController(DataContext contexto, IWebHostEnvironment webHostEnvironment)
        {
            this.contexto = contexto;
            _webHostEnvironment = webHostEnvironment;
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
	 
        .Where(i => i.PropietarioId == Id)
		
        .ToList();

    return Ok(inmueblesDelPropietario);
   }
    else
    {
        return BadRequest("Invalid user ID format in token");
    }
		}

[HttpPost("cargar")]
public async Task<IActionResult> Cargar([FromForm] IFormFile imagen, [FromForm] string inmueble)
{
    Console.WriteLine(inmueble);
    try
    {
        // Deserializa el JSON al objeto Inmueble
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var inmuebleObject = JsonSerializer.Deserialize<Inmueble>(inmueble, options);
        
        Console.WriteLine(inmuebleObject.ToString());

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (imagen != null) 
        {
            var uploadsRootFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsRootFolder))
            {
                Directory.CreateDirectory(uploadsRootFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
            var filePath = Path.Combine(uploadsRootFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imagen.CopyToAsync(fileStream);
            }

            inmuebleObject.Imagen = Path.Combine("uploads", uniqueFileName);
        }

        var user = HttpContext.User;
        var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(userIdClaim, out int propietarioId))
        {
            inmuebleObject.PropietarioId = propietarioId;
            contexto.Inmuebles.Add(inmuebleObject);
            await contexto.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = inmuebleObject.Id }, inmuebleObject);
        }
        else
        {
            return BadRequest("Invalid user ID format in token");
        }
    }
    catch (Exception ex)
    {
        return BadRequest(ex.Message);
    }
}

        // Otros métodos del controlador...



		// PUT api/<controller>/5
[HttpPut("PutInmueble")]
public async Task<IActionResult> PutInmueble([FromBody] Inmueble entidad)
{   Console.WriteLine("loco");
    var user = HttpContext.User;
    var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

    if (int.TryParse(userIdClaim, out int id))
    {
        try
        {    entidad.PropietarioId=id; 
            contexto.Inmuebles.Update(entidad);
            await contexto.SaveChangesAsync();
            return Ok(entidad);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    return BadRequest("Invalid user ID or missing claim");
}

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