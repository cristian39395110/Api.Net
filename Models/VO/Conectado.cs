using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using InmobiliariaGutierrez.Models.VO;
using Laboratorio_3.Models.VO;

namespace dotnet.VO
{
	public class Conectado
	{
		[Key]
		public string Usuario { get; set; }

		public string Nombre { get; set; }

		public Conectado()
		{

		}

		public Conectado(Propietario p)
		{
			Usuario = p.Email;
			Nombre = p.Nombre + " " + p.Apellido;
		}
	}
}