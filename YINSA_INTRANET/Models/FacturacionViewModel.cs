using System.ComponentModel.DataAnnotations;

namespace YINSA_INTRANET.Models
{
	public class FacturacionViewModel
	{
		public string Id { get; set; }
		public string Nombre { get; set; }
		public int Estado { get;set; }
		[DataType(DataType.Date)]
		public DateTime Inicio { get; set; } = DateTime.Parse(new DateTime().ToString("yyyy-MM-dd"));
		public DateTime Fin { get; set; }
		public IEnumerable<Documento> Facturas { get; set; }
	
		public float Total => Facturas.Sum(x => x.Total);




	}
}
