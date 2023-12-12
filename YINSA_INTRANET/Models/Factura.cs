using System.ComponentModel.DataAnnotations;

namespace YINSA_INTRANET.Models
{
	public class Factura
	{
		public int IdDoc { get; set; }
		public string Folio { get; set; }
		public string Version { get; set; }
		public string UUID { get; set; } //32 cara
		public string RFCEmisor { get; set; }	//13 carac
		public string Emisor { get; set; }
		public string RFCReceptor { get; set; }
		public string Receptor { get; set; }
		public string UsoCFDI { get; set; }
		public string Tipo { get; set; } //1 char
		public string Estatus { get; set; }
		public string Metodo { get; set; } //3 car
		public string FormaPago { get; set; } 
		public string Moneda {	get; set;}
		[DisplayFormat(DataFormatString = "${0:#,##0.00}", ApplyFormatInEditMode = true)]

		public decimal IVA { get; set; }
		[DisplayFormat(DataFormatString = "${0:#,##0.00}", ApplyFormatInEditMode = true)]

		public decimal SubTotal { get; set; }
		[DisplayFormat(DataFormatString = "${0:#,##0.00}", ApplyFormatInEditMode = true)]

		public decimal Total { get; set; }
		[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime FechaTimbre { get; set; }

		/// <value>Regresa el status de la lectura del xml. </value>
		public bool Status { get; set; }
		/// <value>Regresa un mensaje de confirmación o fallo en la lectura del xml. </value>
		public string Message { get; set; }

	}

	public class FacturaJSON
	{ //AGREGAR CAMPOS DEL RECEPTOR
		public int IdDoc { get; set; }
		public string Folio { get; set; }
		public string Version { get; set; }
		public string UUID { get; set; } //32 cara
		public string RFC { get; set; } //13 carac
		public string RazonSocial { get; set; }
		public string RFCReceptor { get; set; }
		public string Receptor { get; set; }
		public string UsoCFDI { get; set; }
		public string Tipo { get; set; } //1 char
		public string Estatus { get; set; }
		public string Metodo { get; set; } //3 car
		public string FormaPago { get; set; }
		public string Moneda { get; set; }
		[DisplayFormat(DataFormatString = "${0:#,##0.00}", ApplyFormatInEditMode = true)]

		public decimal IVA { get; set; }
		[DisplayFormat(DataFormatString = "${0:#,##0.00}", ApplyFormatInEditMode = true)]

		public decimal SubTotal { get; set; }
		[DisplayFormat(DataFormatString = "${0:#,##0.00}", ApplyFormatInEditMode = true)]
		public decimal Total { get; set; }
		[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime FechaTimbre { get; set; }
	}
}
