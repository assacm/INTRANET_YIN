namespace YINSA_INTRANET.Models
{
    /// <summary>
    /// Modelo para la vista de Pedidos y Compras.
    /// </summary>
    public class TabsViewModel
    {
        public string IdCuenta { get; set; }
        public int Vista { get; set; }
        public Socio Socio { get; set; } //verificar si se usa para algo
        public int Periodo { get; set; }
        public CardsViewModel DetalleProducto { get; set; }
    }
}
