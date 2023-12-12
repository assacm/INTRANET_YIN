namespace YINSA_INTRANET.Models
{
	public class ColaboradorViewModel
	{


		public Persona Personal { get; set; }
		public string Foto { get; set; }
		//public int Telefono { get; set; }
		//public string Email { get; set; }
		//public string CURP { get; set; }
		//public DateTime FechaNacimiento { get; set; }
		public string NoNomina { get; set; }
		public List<string> Documentos { get; set; }	

		public string Email { get; set; }
		public int Telefono { get; set; }
		public int NSS { get; set; }
		//public string TipoSangre { get; set; }
		//public List<string> Alergias { get; set; }
		public DateTime Inicio { get; set; }
		public string Estatus { get; set;}
		public string Departamento { get; set; }
		public int Vacaciones { get; set;}
		public Persona ContactoEmergencia {get;set;}
		public List<string> Recibos { get; set; }

		public Empleado Empleado { get; set; }
		/// <summary>
		/// Muestra el número de documentos de factura pendientes de proveedores o empleados por revisar.
		/// </summary>
		public int FacturasPendientes { get; set; }

		/// <summary>
		/// Muestra el número de documentos pendientes de proveedores o empleados por revisar.
		/// </summary>
		public int DocumentosPendientes { get; set; }

		/// <summary>
		/// Muestra el número de notificaciones pendientes de revisar.
		/// </summary>
		public int No_Notificaciones { get; set; }
		/// <summary>
		/// Identifica la opción seleccionada del menú lateral. 
		/// </summary>
		public int MenuOption { get; set; }

	}
}
