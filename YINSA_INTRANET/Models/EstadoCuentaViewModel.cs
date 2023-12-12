using System.ComponentModel.DataAnnotations;

namespace YINSA_INTRANET.Models
{
	public class EstadoCuentaViewModel
	{
		public string IdCuenta { get; set; }
		public string Id { get; set; }
		//[DataType(DataType.Date)]
		public string Nombre { get; set; }

		//[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime Inicio { get; set; } = DateTime.Parse(new DateTime().ToString("yyyy-MM-dd"));
		public DateTime Fin { get; set; }
		public int Estado { get; set; }
		public IEnumerable<Documento> Documentos { get; set; }
	}
}
