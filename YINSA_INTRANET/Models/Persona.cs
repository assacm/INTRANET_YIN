namespace YINSA_INTRANET.Models
{
	public class Persona
	{
		public string Nombre { get; set; }
		public int Telefono { get; set; }
		public int Telefono2 { get; set; }
		public string Email { get; set; }
		public string CURP { get; set; }
		public DateTime FechaNacimiento { get; set; }
		public string TipoSangre { get; set; }
		public List<string> Alergias { get; set; }
	}
}
