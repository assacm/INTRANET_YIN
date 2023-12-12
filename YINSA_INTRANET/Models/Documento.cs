using System.ComponentModel.DataAnnotations;

namespace YINSA_INTRANET.Models
{
	public class Documento
	{
        public int TipoObjSAP { get; set; }

        public string TipoKey { get; set; }
		public string Tipo { get; set; }
		public int DocEntry { get; set; }
		public int NoDocumento { get; set; }
		public int? NoSerie { get; set; }
		public string TipoDocumento { get; set; }
		public string Cancelado { get; set; }
		[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime Fecha { get; set; }
		[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime FechaVencimiento { get; set; }
		public string CodigoS { get; set; }
		public string NombreS { get; set; }
		public string RFC { get; set; }
		public string Referencia { get; set; }
		public string Moneda { get; set; }
		public string UUID { get; set; }
        public int AsesorCodigo { get; set; }
        public string Asesor { get; set; }
        public string FormaPago { get; set; }
		public string  MetodoPago { get; set; }
		public int NoLinea { get; set; }
		public string EstatusLinea { get; set; }
		public string ProductoKey { get; set; }
		public string Producto { get; set; }
		public DateTime FechaEntrega { get; set; }

		[DisplayFormat(DataFormatString = "{0:N3}", ApplyFormatInEditMode = true)]
		public float Cantidad { get; set; }
		public float Precio { get; set; }
		public string UnidadMedida { get; set; }
		[DisplayFormat(DataFormatString = "${0:#,##0.00}", ApplyFormatInEditMode = true)]
		public float Importe { get; set; }
		public float Descuento { get; set; }
		public string AsesorKey { get; set; }
		public float TasaImpuesto { get; set; }
		public float ImporteImpto { get; set; }
		public string DireccionEntrega { get; set; }
		[DisplayFormat(DataFormatString = "${0:#,##0.00}", ApplyFormatInEditMode = true)]

		public float Total { get; set; }
		[DisplayFormat(DataFormatString = "${0:#,##0.00}", ApplyFormatInEditMode = true)]

		public float Cargo { get; set; }
		[DisplayFormat(DataFormatString = "${0:#,##0.00}", ApplyFormatInEditMode = true)]

		public float Abono { get; set; }
		public float Saldo { get; set; }
		public string Antiguedad { get; set; }
		public string EstatusCte { get; set; }
        public bool Estatus { get; set; }

    }
}
