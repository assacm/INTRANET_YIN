using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace YINSA_INTRANET.Models
{
	public class RegistroUser
	{
		[Required(ErrorMessage = "El campo es requerido. Debe agregar un nombre identificador del usuario.")]
		//[Remote(action: "ExisteUsuario", controller:"Usuarios" )]
		public string userName { get; set; }
		[Required(ErrorMessage = "Debe agregar un nombre corto del usuario.")]
		public string name { get; set; }

		[Required(ErrorMessage = "Este campo es requerido")]
		[EmailAddress(ErrorMessage = "Debe ingresar un correo válido")]
		public string email { get; set; }

		[DataType(DataType.Password)]
		[Required(ErrorMessage = "El campo es requerido")]	
		[StringLength(maximumLength: 20, MinimumLength = 8, ErrorMessage = "La contraseña debe tener un mínimo de {2} y un máximo de {1} caracteres.")]
		[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,16}$",
			ErrorMessage = "La contraseña debe contener al menos una letra minúscula, una letra mayúscula y un número.")]
		public string password { get; set; }
		
	

		[Required(ErrorMessage = "Este campo es requerido")]
		public int rolId { get; set; }

	

	}
	public class RegistroUserVM : RegistroUser
	{
		/// <summary>
		/// Cuenta del usuario que realiza el registro.
		/// </summary>
		public string IdCuenta { get; set; } 
		public List<SelectListItem> GrupoProv { get; set; }

		public List<Item> Permisos { get; set; }
		//public RegistroUser registro { get; set; }

		[Required(ErrorMessage = "El campo es requerido")]
		//[StringLength(maximumLength:20,MinimumLength=8, ErrorMessage ="La contraseña debe tener un mínimo de {2} y un máximo de {1} caracteres.")]
		[DataType(DataType.Password)]
		public string confPass { get; set; }
		//public CuentaAsignacion Cuentas { get; set; }
		public List<Cuenta> cuentas { get; set; }
		public List<Permiso> permisos { get; set; }
		public List<Item> filtros { get; set; }

	}

	public class RegistroPost : RegistroUser
	{
		[Required(ErrorMessage = "El campo es requerido")]
		//[StringLength(maximumLength:20,MinimumLength=8, ErrorMessage ="La contraseña debe tener un mínimo de {2} y un máximo de {1} caracteres.")]
		//[DataType(DataType.Password)]
		public string confPass { get; set; }
		//public CuentaAsignacion Cuentas { get; set; }
		public List<Cuenta> cuentas { get; set; }
		public List<Permiso> permisos { get; set; }
		public List<Item> filtros { get; set; }
	}


	public class Permiso
	{
		public int IdPermiso { get; set; }
		public bool Leer { get; set; }
		public bool Crear { get; set; }
		public bool Editar { get; set; }
		public bool Autorizar { get; set; }
		public bool Eliminar { get; set; }

	}
	public class PermisoAsignacion
	{
		public int IdUsuario { get; set; }
		public List<Permiso> Permisos{ get; set; }
	}
	public class FiltrosAsignacion
	{
		public int IdUsuario { get; set; }
		public List<Item> Filtros { get; set; }
	}
}
