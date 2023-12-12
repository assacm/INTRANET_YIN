namespace YINSA_INTRANET.Models
{

	/// <summary>
	/// Modelo de la vista de la tarjeta con el detalle del Pedido o Compra.
	/// </summary>
	public class CardsViewModel
	{
		/// <summary>
		/// Valor numérico que identifica la vista seleccionada en los Tabs.
		/// Ejemplo: Pedidos,Compras, Pendientes o Cancelados
		/// </summary>
		public int Vista { get; set; }
		public List<Documento> Productos { get; set; }

		public List<DocumentoV2> ProductosV2 { get; set; }
	}

}
