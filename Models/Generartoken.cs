using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Laboratorio_3.Models.VO;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
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

namespace Api.Net.Models
{
    public class GenerarToken
    {
        private readonly IConfiguration config;

        public GenerarToken(IConfiguration config)
        {
            this.config = config;
        }
        public GenerarToken( )
        {
          this.config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();
        }

        public string Token(string clave, Propietario propietario)
        {  string cal="ñsdfs";
            try
            {  
                // Hashear la clave
              string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
					password:propietario.Clave,
					salt: Encoding.ASCII.GetBytes(config["Salt"]),
					prf: KeyDerivationPrf.HMACSHA1,
					iterationCount: 1000,
					numBytesRequested: 256 / 8)
                    );
                
                   
             
                	var key = new SymmetricSecurityKey(
						System.Text.Encoding.ASCII.GetBytes(config["TokenAuthentication:SecretKey"]));
					var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
					var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, propietario.Email),
                    new Claim("FullName", propietario.Nombre + " " + propietario.Apellido),
                    new Claim(ClaimTypes.Role, "Propietario"),
                    new Claim(ClaimTypes.NameIdentifier, propietario.Id.ToString())
                };
                 Console.WriteLine("3");
                var token = new JwtSecurityToken(
                    issuer: config["TokenAuthentication:Issuer"],
                    audience: config["TokenAuthentication:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(5),
                    signingCredentials: credenciales
                );
                  Console.WriteLine("4");

                return new JwtSecurityTokenHandler().WriteToken(token);
                
            }
            catch (Exception ex)
            {
                throw new Exception("Error al generar el token", ex);
            }
        }


        public string DevolverToken(string clave)
        {  
            try
            {  
                // Hashear la clave
              string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
					password:clave,
					salt: Encoding.ASCII.GetBytes(config["Salt"]),
					prf: KeyDerivationPrf.HMACSHA1,
					iterationCount: 1000,
					numBytesRequested: 256 / 8)
                    );
                
                   
             
                return (hashed);
                
            }
            catch (Exception ex)
            {
                throw new Exception("Error al generar el token", ex);
            }
        }





    }
}
