namespace YINSA_INTRANET.Models
{
	public class Socio
	{
		public string CodigoS { get; set; }
		public string NombreS { get; set; }
		public string RFC { get; set; }
		public string Phone1 { get; set; }
		public string Phone2 { get; set; }
		public string E_Mail { get; set; }
		public string SitioWeb { get; set; }
		public Direccion Direccion{ get; set; }
		public List<Direccion> Direcciones { get; set; }
		//public string Direccion { get; set; }
		//public string Cuadra { get; set; }
		//public string CP { get; set; }
		//public string Ciudad { get; set; }
		//public string Pais { get; set; }
		public string IdFiscal { get; set; }
		public string Giro { get; set; } //pendiente de agregar
	}

	public class Direccion
	{
		public string Address { get; set; }
		public string CardCode { get; set; }
		public string Calle { get; set; }
		public string NoCalle { get; set; }
		public string Cuadra { get; set; }
		public string CP { get; set; }
		public string Ciudad { get; set; }
		public string Condado { get; set; }
		public string Pais { get; set; }
		public string Estado { get; set; }
		public string NoExterior { get; set; }
		public string NoInterior { get; set; }
	}



}
