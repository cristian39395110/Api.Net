using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Api.Net.Controllers;
using Api.Net.Models;
using Dotnet.Models.VO;
using InmobiliariaGutierrez.Models.VO;
using Laboratorio_3.Models.VO;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MimeKit;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Laboratorio_3.Models
{
	[Route("api/[controller]")]
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[ApiController]
	public class PropietariosController : ControllerBase  //
	{
		private readonly DataContext contexto;
		private readonly IConfiguration config;
		private readonly IWebHostEnvironment environment;
			
		
		

		public PropietariosController(DataContext contexto, IConfiguration config, IWebHostEnvironment env)
		{
			this.contexto = contexto;
			this.config = config;
			environment = env;
			
		
		}
		// GET: api/<controller>
		[HttpGet]
		public async Task<ActionResult<Propietario>> Get()
		{
			try
			{
				/*contexto.Inmuebles
					.Include(x => x.Duenio)
					.Where(x => x.Duenio.Nombre == "")//.ToList() => lista de inmuebles
					.Select(x => x.Duenio)
					.ToList();//lista de propietarios*/
				var usuario = User.Identity.Name;
				/*contexto.Contratos.Include(x => x.Inquilino).Include(x => x.Inmueble).ThenInclude(x => x.Duenio)
					.Where(c => c.Inmueble.Duenio.Email....);*/
				/*var res = contexto.Propietarios.Select(x => new { x.Nombre, x.Apellido, x.Email })
					.SingleOrDefault(x => x.Email == usuario);*/
				return await contexto.Propietarios.SingleOrDefaultAsync(x => x.Email == usuario);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// GET api/<controller>/5
		[HttpGet("GetPropietario")]
		public async Task<IActionResult> GetPropietario()
		{
             var user = HttpContext.User;

    var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

  
 if (int.TryParse(userIdClaim, out int Id))
   {
   try
			{
				var entidad = await contexto.Propietarios.SingleOrDefaultAsync(x => x.Id == Id);
				return entidad != null ? Ok(entidad) : NotFound();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}

   }
    else
    {
        return BadRequest("Invalid user ID format in token");
    }


			
		}

		// GET api/<controller>/token
			[HttpGet("token")]
			public async Task<IActionResult> Token()
			{
				try
				{ //este método si tiene autenticación, al entrar, generar clave aleatorio y enviarla por correo
					var perfil = new
					{
						Email = User.Identity.Name,
						Nombre = User.Claims.First(x => x.Type == "FullName").Value,
						Rol = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role).Value
					};
					Random rand = new Random(Environment.TickCount);
					string randomChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";
					string nuevaClave = "";
					for (int i = 0; i < 8; i++)
					{
						nuevaClave += randomChars[rand.Next(0, randomChars.Length)];
					}//!Falta hacer el hash a la clave y actualizar el usuario con dicha clave
					var message = new MimeKit.MimeMessage();
					message.To.Add(new MailboxAddress(perfil.Nombre, perfil.Email));
					message.From.Add(new MailboxAddress("Sistema", config["SMTPUser"]));
					message.Subject = "Prueba de Correo desde API";
					message.Body = new TextPart("html")
					{
						Text = @$"<h1>Hola</h1>
						<p>¡Bienvenido, {perfil.Nombre}!</p>",//falta enviar la clave generada (sin hashear)
					};
					message.Headers.Add("Encabezado", "Valor");//solo si hace falta
					MailKit.Net.Smtp.SmtpClient client = new SmtpClient();
					client.ServerCertificateValidationCallback = (object sender,
						System.Security.Cryptography.X509Certificates.X509Certificate certificate,
						System.Security.Cryptography.X509Certificates.X509Chain chain,
						System.Net.Security.SslPolicyErrors sslPolicyErrors) =>
					{ return true; };
					client.Connect("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.Auto);
					client.Authenticate(config["SMTPUser"], config["SMTPPass"]);//estas credenciales deben estar en el user secrets
					await client.SendAsync(message);
					return Ok(perfil);
				}
				catch (Exception ex)
				{
					return BadRequest(ex.Message);
				}
			}

		// GET api/<controller>/email
		[HttpPost("email")]
		[AllowAnonymous]
		public async Task<IActionResult> GetByEmail([FromForm] string email)
		{
			try
			{ //método sin autenticar, busca el propietario x email
				var entidad = await contexto.Propietarios.FirstOrDefaultAsync(x => x.Email == email);
				//para hacer: si el propietario existe, mandarle un email con un enlace con el token
				//ese enlace servirá para resetear la contraseña
				//Dominio sirve para armar el enlace, en local será la ip y en producción será el dominio www...
				var dominio = environment.IsDevelopment() ? HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() : "www.misitio.com";
				return entidad != null ? Ok(entidad) : NotFound();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// GET api/<controller>/GetAll
		[HttpGet("GetAll")]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				return Ok(await contexto.Propietarios.ToListAsync());
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// POST api/<controller>/login
		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<IActionResult> Login([FromForm] LoginView loginView)
		{ 
			try
			{
				string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
					password: loginView.Clave,
					salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
					prf: KeyDerivationPrf.HMACSHA1,
					iterationCount: 1000,
					numBytesRequested: 256 / 8));
					Console.WriteLine( loginView.Usuario);
				var p = await contexto.Propietarios.FirstOrDefaultAsync(x => x.Email == loginView.Usuario);
			
				if (p == null || p.Clave != hashed)
				{
					return BadRequest("Nombre de usuario o clave incorrecta");
				}
				else
				{
					var key = new SymmetricSecurityKey(
						System.Text.Encoding.ASCII.GetBytes(config["TokenAuthentication:SecretKey"]));
					var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, p.Email),
						new Claim("FullName", p.Nombre + " " + p.Apellido),
						new Claim(ClaimTypes.Role, "Propietario"),
						new Claim(ClaimTypes.NameIdentifier, p.Id.ToString())
					
						

						
					};

					var token = new JwtSecurityToken(
						issuer: config["TokenAuthentication:Issuer"],
						audience: config["TokenAuthentication:Audience"],
						claims: claims,
						expires: DateTime.Now.AddMinutes(60),
						signingCredentials: credenciales
					);
					return Ok(new JwtSecurityTokenHandler().WriteToken(token));
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// POST api/<controller>
		[HttpPost]
		public async Task<IActionResult> Post([FromForm] Propietario entidad)
		{
			try
			{
				if (ModelState.IsValid)
				{
					await contexto.Propietarios.AddAsync(entidad);
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
		[HttpPut("put")]
		public async Task<IActionResult> Put( [FromBody] Propietario entidad)
		{   var user = HttpContext.User;

    var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);

  
 if (int.TryParse(userIdClaim, out int id))
   {
    try
			{
				if (ModelState.IsValid)
				{
					entidad.Id = id;
					Propietario original = await contexto.Propietarios.FindAsync(id);
					if (String.IsNullOrEmpty(entidad.Clave))
					{
						entidad.Clave = original.Clave;
					}
					else
					{
						entidad.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
							password: entidad.Clave,
							salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
							prf: KeyDerivationPrf.HMACSHA1,
							iterationCount: 1000,
							numBytesRequested: 256 / 8));
					}
					contexto.Entry(original).State = EntityState.Detached; 
					contexto.Propietarios.Update(entidad);

					await contexto.SaveChangesAsync();
					return Ok(entidad);
				}
				return BadRequest();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
   }
    else
    {
        return BadRequest("Invalid user ID format in token");
    }
			
		}

		// DELETE api/<controller>/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				if (ModelState.IsValid)
				{
					var p = await contexto.Propietarios.FindAsync(id);
					if (p == null)
						return NotFound();
					contexto.Propietarios.Remove(p);
					await contexto.SaveChangesAsync();
					return Ok(p);
				}
				return BadRequest();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// GET: api/Propietarios/test
		[HttpGet("test")]
		[AllowAnonymous]
		public IActionResult Test()
		{
			try
			{
				return Ok("anduvo");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// GET: api/Propietarios/test/5
		[HttpGet("test/{codigo}")]
		[AllowAnonymous]
		public IActionResult Code(int codigo)
		{
			try
			{
				//StatusCodes.Status418ImATeapot //constantes con códigos

				return StatusCode(codigo, new { Mensaje = "Anduvo", Error = false });

			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		// GET api/<controller>/5

[HttpGet("GetInmueblesPropietarios")]
 // Requires valid JWT token for access
public IActionResult GetInmueblesPropietarios()
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

/*
[AllowAnonymous]
[HttpPost("tokenes")]
public async Task<IActionResult> Tokenes([FromForm] string email)
{
    Console.WriteLine("Tokenes");

    try
    {
        Console.WriteLine($"Email recibido: {email}");

        // Generar una nueva clave aleatoria
        Random rand = new Random(Environment.TickCount);
        string randomChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789";
        string nuevaClave = "";
        for (int i = 0; i < 8; i++)
        {
            nuevaClave += randomChars[rand.Next(0, randomChars.Length)];
        }

        // Hashear la nueva clave
        string hashedClave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: nuevaClave,
            salt: Encoding.ASCII.GetBytes(config["Salt"]),
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 1000,
            numBytesRequested: 256 / 8));

        Console.WriteLine("Clave generada y hasheada");

        // Actualizar la clave del usuario en la base de datos
        var usuario = await contexto.Propietarios.FirstOrDefaultAsync(u => u.Email == email);
        if (usuario == null)
        {
            Console.WriteLine("Usuario no encontrado");
            return NotFound("Usuario no encontrado");
        }
        usuario.Clave = hashedClave;
        await contexto.SaveChangesAsync();

        Console.WriteLine("Clave actualizada en la base de datos");

        // Enviar correo electrónico con la nueva clave
        var message = new MimeKit.MimeMessage();
        var apellido = usuario.Apellido ?? "Usuario"; // Valor predeterminado si usuario.Apellido es nulo
        var nombre = usuario.Nombre ?? "Usuario"; // Valor predeterminado si usuario.Nombre es nulo
        Console.WriteLine("1");
        message.To.Add(new MailboxAddress(apellido, email));
        Console.WriteLine("2");
        message.From.Add(new MailboxAddress("Sistema", email));
        Console.WriteLine("3");
        message.Subject = "Nueva Clave de Acceso";
        Console.WriteLine("4");
        message.Body = new TextPart("html")
        {
            Text = @$"<h1>Hola {nombre}</h1>
                     <p>Tu nueva clave de acceso es: <strong>{nuevaClave}</strong></p>"
        };

        Console.WriteLine("Preparando el mensaje de correo electrónico");

        using (var client = new MailKit.Net.Smtp.SmtpClient())
        {
            Console.WriteLine("5");
            client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            await client.ConnectAsync(config["Smtp:Host"], int.Parse(config["Smtp:Port"]), MailKit.Security.SecureSocketOptions.Auto);
            Console.WriteLine("Conectado al servidor SMTP");

            // Autenticarse en el servidor SMTP
            await client.AuthenticateAsync(config["Smtp:User"], config["Smtp:Pass"]);
            Console.WriteLine("Autenticado en el servidor SMTP");

            // Enviar el mensaje
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            Console.WriteLine("Correo electrónico enviado");
        }
        Console.WriteLine("6");

        return Ok(usuario);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        return BadRequest(ex.Message);
    }
}

*/

  
 [HttpPost("emailes")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByEmailes([FromForm] string email)
        {
            try
            {
                // Buscar el propietario por email
                var entidad = await contexto.Propietarios.FirstOrDefaultAsync(x => x.Email == email);
                if (entidad == null)
                {
                    return BadRequest("El email ingresado no existe.");
                }   


				GenerarToken gtoken=new GenerarToken();

                // Generar el token y el enlace
                var dominio = environment.IsDevelopment() ? config["AppSettings:DevelopmentDomain"] : config["AppSettings:ProductionDomain"];
                string token = gtoken.Token(entidad.Clave,entidad);
                string enlace = $"{dominio}Token/Index?access_token={token}";
                Console.WriteLine("Enlace con token: " + enlace);

                // Enviar el correo electrónico
                var message = new MimeKit.MimeMessage();
                message.To.Add(new MailboxAddress(entidad.Nombre, entidad.Email));
                message.From.Add(new MailboxAddress("Sistema", config["Smtp:User"]));
                message.Subject = "Restablecer contraseña";
                message.Body = new TextPart("html")
                {
                    Text = $@"<p>Hola {entidad.Nombre}:</p>
                              <p>Hemos recibido una solicitud de restablecimiento de contraseña de tu cuenta.</p>
                              <p>Haz clic en el botón que aparece a continuación para cambiar tu contraseña.</p>
                              <p>Ten en cuenta que este enlace es válido solo durante 24 horas. Una vez transcurrido el plazo, deberás volver a solicitar el restablecimiento de la contraseña.</p>
                              <a href='{enlace}'>Cambiar contraseña</a>"
                };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                    await client.ConnectAsync(config["Smtp:Host"], int.Parse(config["Smtp:Port"]), MailKit.Security.SecureSocketOptions.Auto);
                    await client.AuthenticateAsync(config["Smtp:User"], config["Smtp:Pass"]);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                    Console.WriteLine("Correo electrónico enviado");
                }
               String perfecto="perfecto";
              return Content(perfecto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}

		





/*
{
    "id": 50,
    "nombre": "luzza@hotmail.com",
    "apellido": "Juaquin",
    "dni": "luzza@hotm",
    "clave": "B1i+YL7S7zFRLyoit6h7qDAkM+WItPrMjqkR+B5YXm4=",
    "telefono": "1111111",
    "email": "luzza@hotmail.com",
    "domicilio": "pucara"
}
*/