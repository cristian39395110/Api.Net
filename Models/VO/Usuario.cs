

using System.ComponentModel.DataAnnotations.Schema;

namespace Laboratorio_3.Models.VO;


	public class Usuario
	{
		
		public int Id { get; set; }
		
		public string Nombre { get; set; }
		public string Apellido { get; set; }
		public string Email { get; set; }
		public string Pass { get; set; }
		
		public string Avatar { get; set; }
		 

         [NotMapped]
    	public IFormFile AvatarFile { get; set; }
		public int Rol { get; set; }		
		public Boolean EliminarFoto{ get; set; }
        public string RolNombre => Rol > 0 ? ((enRoles)Rol).ToString() : "";

		public static IDictionary<int, string> ObtenerRoles()
		{
			SortedDictionary<int, string> roles = new SortedDictionary<int, string>();
			Type tipoEnumRol = typeof(enRoles);
			foreach (var valor in Enum.GetValues(tipoEnumRol))
			{
				roles.Add((int)valor, Enum.GetName(tipoEnumRol, valor));
			}
			return roles;
		}
	}


	public enum enRoles
	{
		
		Administrador = 1,
		Empleado = 2
	}
    