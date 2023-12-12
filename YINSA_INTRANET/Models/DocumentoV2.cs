using System.ComponentModel.DataAnnotations;

namespace YINSA_INTRANET.Models
{
	/// <summary>
	/// Documento de SAP para representación de compras y pedidos, con agrupación de Productos e información de la recepción de compra.
	/// </summary>
	public class DocumentoV2
	{
        public int TipoObjSAP { get; set; }

        public int DocEntry { get; set; }
		public int NoDocumento { get; set; }
		public int? NoSerie { get; set; }
		public string TipoDocumento { get; set; }
		public string Cancelado { get; set; }
		[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime Fecha { get; set; }
		[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
		public DateTime FechaVencimiento { get; set; }
		public string Referencia { get; set; }
		public string Moneda { get; set; }
		public string UUID { get; set; }
        public int AsesorCodigo { get; set; }
        public string Asesor { get; set; }
        public string Direccion { get; set; }

		public string CodigoS { get; set; }
		public string NombreS { get; set; }
		public float TasaImpuesto { get; set; }
		[DisplayFormat(DataFormatString = "${0:#,##0.00}", ApplyFormatInEditMode = true)]
		public float ImporteImpto { get; set; }
		[DisplayFormat(DataFormatString = "${0:#,##0.00}", ApplyFormatInEditMode = true)]
		public float Subtotal { get; set; }
		[DisplayFormat(DataFormatString = "${0:#,##0.00}", ApplyFormatInEditMode = true)]
		public float Total { get; set; }
		public string Estatus{ get; set; }

		public List<DetalleProducto> DetalleProductos { get; set; }
		//public string Recepcion { get; set; }
		public List<DetalleProducto> Recepcion { get; set; }
		/// <summary>
		/// Indica si se le puede o no adjuntar archivos de factura al Documento.
		/// </summary>
		public bool EstadoFacturas { get; set; }
	}


	public class DetalleProducto
	{
		public int DocEntry { get; set; }
		public int NoDocumento { get; set; }
		public int TipoObjSAP { get; set; }
        public int NoLinea { get; set; }
		public string EstatusLinea { get; set; }
		public string ProductoKey { get; set; }
		public string Producto { get; set; }
		public DateTime FechaEntrega { get; set; }
		[DisplayFormat(DataFormatString = "{0:N3}", ApplyFormatInEditMode = true)]
		public float Cantidad { get; set; }
        [DisplayFormat(DataFormatString = "{0:N3}", ApplyFormatInEditMode = true)]
        public float CantidadPendiente { get; set; }


        [DisplayFormat(DataFormatString = "${0:#,##0.00}", ApplyFormatInEditMode = true)]

		public float Precio { get; set; }
		public string UnidadMedida { get; set; }
		[DisplayFormat(DataFormatString = "${0:#,##0.00}", ApplyFormatInEditMode = true)]

		public float Importe { get; set; }
		[DisplayFormat(DataFormatString = "${0:#,##0.00}", ApplyFormatInEditMode = true)]

		public float Descuento { get; set; }
		public string AsesorKey { get; set; }
		public float TasaImpuesto { get; set; }
		[DisplayFormat(DataFormatString = "${0:#,##0.00}", ApplyFormatInEditMode = true)]
		public float ImporteImpto { get; set; }
		public string DireccionEntrega { get; set; }
        [DisplayFormat(DataFormatString = "${0:#,##0.00}", ApplyFormatInEditMode = true)]

        /// <summary>
        /// Referencia al campo DocTotal cuando se cargan los datos de una Recepción de compra.
        /// </summary>
        public float ImporteTotal { get; set; }
		/// <summary>
		/// Indica si se le puede o no adjuntar archivos de factura al Documento.
		/// </summary>
		public bool EstadoFacturas { get; set; }
	}

	
}
