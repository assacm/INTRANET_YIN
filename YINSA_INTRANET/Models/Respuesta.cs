using System.ComponentModel.DataAnnotations;

namespace YINSA_INTRANET.Models
{
	public class AuthResponse
	{	
			public string Token { get; set; }
			public DateTime Expiracion { get; set; }
			public bool Status { get; set; }
			public string StatusMessage { get; set; }
			public Usuario Usuario { get; set; }
		
	}
	public class LoginViewModel
	{
		[Required]
		//[EmailAddress]
		//public string Email { get; set; }
		public string userName { get; set; }

		[Required]
		public string password { get; set; }
	}

	public class Respuesta
	{
		public bool Status { get; set; }
		public int StatusCode { get; set; }
		public string StatusMessage { get; set; }
		
		//public string Response { get; set; }
		/// <summary>
		/// Valor numérico regresado por la API al actualizar, insertar o eliminar registros.
		/// </summary>
		public int ResponseValue { get; set; }
		
		/// <summary>
		/// Mensaje de error según excepciones u otros problemas.
		/// </summary>
		public string ErrorMessage { get; set; }

	}

	public class RespuestaAutenticacion : Respuesta
	{
		public string Token { get; set; }
		public DateTime Expiracion { get; set; }
		public Usuario Usuario { get; set; }
	}
	public class RespuestaArchivos : Respuesta
	{
		public List<ArchivoDoc> Archivos { get; set; }
		public ArchivoDoc Archivo { get; set; }
		public string Valor { get; set; }
		public List<ProveedorCount> ProveedoresArchivos { get; set; }
		public List<FacturaJSON> FacturasXML {  get; set; }
	}

	public class RespuestaDocumentos : Respuesta
	{
		public List<Documento> Documentos { get; set;}
		public Documento Documento { get; set;}
	}

	public class RespuestaEmpleado : Respuesta 
	{ 
		public Empleado Empleado { get; set; }
		public List<Empleado> Empleados { get; set; }
	}

	public class RespuestaItem :Respuesta 
	{
		public List<Item> Response { get; set; }
	}
	public class RespuestaComentarios : Respuesta
	{
		public List<ComentarioModel> Comentarios{ get; set; }
	}



}
