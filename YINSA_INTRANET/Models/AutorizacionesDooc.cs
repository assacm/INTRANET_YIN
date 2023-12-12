namespace YINSA_INTRANET.Models
{
	public class AutorizacionesDoc
	{
		public int IdDoc { get; set; }
		public int IdUsuario { get; set; }
		public string Estado { get; set; }
		/// <summary>
		/// Nombre del empleado que autoriza.
		/// </summary>
		public string Nombre { get; set; }
		public int RolId { get; set; }
		public string Rol { get; set; }
		public string Puesto { get; set; }
		public string Departamento { get; set; }
		public bool Contabilidad { get; set; }
		public string UserName { get; set; }

		public DateTime FechaAuth { get; set; }
	}

	public class Autorizar
	{
		public int IdDoc { get; set; }
		public int IdUsuario { get; set; }
		public string IdCuenta { get; set; }
		public string Estado { get; set; }
		public bool Contabilidad { get; set; }

		public string Comentario { get; set; }
	}
}
