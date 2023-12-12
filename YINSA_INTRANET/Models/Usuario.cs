using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace YINSA_INTRANET.Models
{
	public class Usuario
	{ //Recibir el usuario al iniciar sesión con la api -- quizás agregar campo de token
		public int userId { get; set; }
		public string userName1 { get; set; }


		[Required(ErrorMessage ="Este campo es requerido")]
		public string userName { get; set; }
		[Required(ErrorMessage = "Debe ingresar un nombre corto representativo del usuario.")]

		public string name { get; set; }

		[Required(ErrorMessage = "Este campo es requerido")]
		[EmailAddress(ErrorMessage = "Debe ingresar un correo válido")]
		public string email { get; set; }
		public DateTime creacion { get; set; }
		public string estatus { get; set; }

		//[StringLength(maximumLength: 20, MinimumLength = 8, ErrorMessage = "La contraseña debe tener un mínimo de {2} y un máximo de {1} caracteres.")]
		//[DataType(DataType.Password)]
		public string password { get; set; }
		public int rolId { get; set; }
		public string rol { get; set; }
		public List<Cuenta> cuentas { get; set; }
		//public string jsonCuentas { get; set; }
		public List<Cuenta> cuentasSocio { get; set; }
		public List<Cuenta> cuentasEmpleado { get; set; }
		public List<Item> filtros { get; set; }
		public List<Permiso> permisos { get; set; }


	}

	public class EditarUsuarioVM : Usuario
	{
		public List<SelectListItem> GrupoProv{ get; set; }
		public List<Item> Permisos { get; set; }
		public string FiltrosJSON { get; set; }
		public string PermisosJSON { get; set; }
		public string CuentasJSON { get; set; }
	}
	//public class CredencialesUsuario
	//{
	//	[Required]
	//	[EmailAddress(ErrorMessage = "Debe ingresar un correo válido")]
	//	public string Email { get; set; }
	//	[Required]
	//	[MaxLength(40, ErrorMessage = "La contraseña no debe exceder 40 caracteres")]
	//	public string Password { get; set; }
	//	[Required]
	//	public int RolId { get; set; }

	//}
	public class LoginUser
	{
		[Required]
		public string userName { get; set; }
		[Required]
		//[StringLength(maximumLength: 20, MinimumLength = 8, ErrorMessage = "La contraseña debe tener un mínimo de {2} y un máximo de {1} caracteres.")]
		[DataType(DataType.Password)]
		public string password { get; set; }
	}
	public class ApiUserResponse
	{
		public Usuario Usuario { get; set; }
		public bool Status { get; set; }
		public string StatusMessage { get; set; }
	}
}
