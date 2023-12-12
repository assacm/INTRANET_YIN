using YINSA_INTRANET.Servicios;

namespace YINSA_INTRANET.Models
{
	public class SocioViewModel
	{
		public Socio Socio { get; set; }

		//public DateTime Fecha { get; set; }
		//public int Mes { get; set; } /*= DateTime.Today.Month;*/
		//public int Year { get; set; } = DateTime.Today.Year;

		public int Periodo { get; set; }
		public List<Documento> Facturas { get; set; }
		public List<Documento> Documentos { get; set; }
		public List<Documento> Pedidos { get; set; }

	}
}
