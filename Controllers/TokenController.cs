using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.Logging;
using Api.Net.Models;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Laboratorio_3.Models.VO;
using Laboratorio_3.Models;
using Microsoft.EntityFrameworkCore;
// Asegúrate de que este espacio de nombres es correcto y contiene RepositorioContrato

namespace Api.Net.Controllers
{
    public class TokenController : Controller
    {
        private readonly ILogger<TokenController> _logger;
         private readonly IConfiguration config;
         private readonly DataContext contexto;
       

        // Inyección de dependencias a través del constructor
        public TokenController(ILogger<TokenController> logger,IConfiguration config,DataContext contexto)
        {    this.config = config;
            _logger = logger;
          this.contexto = contexto;
        }

        // Acción para el método Index
         [HttpGet("Token/Index")] 
        public IActionResult Index()
        {
          

            return View(Index);
        }

         [HttpPost("Token/Post")]
        public async Task<IActionResult> PostAsync([FromBody] Password request)
        {
            try
            {
                var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader == null || !authHeader.StartsWith("Bearer "))
                {
                    return Unauthorized(new { message = "Token no proporcionado o malformado" });
                }

                var token = authHeader.Substring("Bearer ".Length).Trim();

                var handler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = config["TokenAuthentication:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = config["TokenAuthentication:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenAuthentication:SecretKey"])),
                    ValidateLifetime = true
                };

                var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;

                var claims = jwtToken.Claims;
                var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

                if (nameClaim == null)
                {
                    return Unauthorized(new { message = "Claim 'name' no encontrada en el token" });
                }

                GenerarToken gentoken = new GenerarToken();
                string tokenes = gentoken.DevolverToken(request.ConfirmPassword);

                Console.WriteLine($"Claim 'name': {nameClaim}");

                // Buscar al propietario por su email
                var propietario = await contexto.Propietarios.SingleOrDefaultAsync(p => p.Email == nameClaim);
                if (propietario == null)
                {
                    return NotFound(new { message = "Propietario no encontrado" });
                }

                propietario.Clave = tokenes;
                contexto.Propietarios.Update(propietario);
                await contexto.SaveChangesAsync();

                return Ok(new { message = "Solicitud POST recibida y procesada correctamente." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Unauthorized(new { message = "Token inválido o expirado" });
            }
        }
}
   
    
}






  
