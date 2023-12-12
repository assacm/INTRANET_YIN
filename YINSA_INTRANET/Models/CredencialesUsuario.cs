using System.ComponentModel.DataAnnotations;

namespace YINSA_INTRANET.Models
{
	public class CredencialesUsuario
	{
		[Required]
		public string UserName { get; set; }
		[Required]
		[MaxLength(40, ErrorMessage = "La contraseña no debe exceder 40 caracteres")]
		public string Password { get; set; }
	}
}
