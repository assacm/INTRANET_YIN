namespace YINSA_INTRANET.Models
{
	public class Cuenta
	{
		public string codigoS { get; set; }
		public string nombreS { get; set; }
		public string tipo { get; set; }
	}
	public class CuentaAsignacion
	{
		public int userId { get; set; }
		public List<Cuenta> cuentas { get; set; }

	}

	public class CuentasViewModel
	{
		public string id { get; set; }
		public string tipo { get; set; }
		public List<Cuenta> cuentas { get; set; }
	}
}
