using Microsoft.AspNetCore.Mvc.Rendering;
using YINSA_INTRANET.Servicios;

namespace YINSA_INTRANET.Models
{
	public class ControlViewModel
	{
		public string IdCuenta { get; set; }
		/// <summary>
		/// Muestra el número de documentos de factura pendientes de proveedores o empleados por revisar.
		/// </summary>
		public int FacturasPendientes { get; set; }

		/// <summary>
		/// Muestra el número de documentos pendientes de proveedores o empleados por revisar.
		/// </summary>
		public int DocumentosPendientes { get; set; }

		/// <summary>
		/// Muestra el número de notificaciones pendientes de revisar.
		/// </summary>
		public int No_Notificaciones { get; set; }
		/// <summary>
		/// Identifica la opción seleccionada del menú lateral. 
		/// </summary>
		public int MenuOption { get; set; }

		/// <summary>
		/// Identifica la tab seleccionada relacionada con la tabla a mostrar.
		/// </summary>
		public int SelectTabla { get; set;}
		/// <summary>
		/// Identifica el tipo de socios de los que se quiere obtener facturas: proveedor, cliente.
		/// </summary>
		public int Socio { get; set; }
		/// <summary>
		///  Representa el mes en que se presentan la facturas
		/// </summary>
		public int Month { get; set; }
		/// <summary>
		/// Representa el año de las facturas SAP a visualizar.
		/// </summary>
		public int Year { get; set; }
		/// <summary>
		/// Representa el estado de las facturas a través de un identificador numérico.
		/// </summary>
		public int Estado { get; set; }
		public List<SelectListItem> SelectMonth { get; set; }
		public List<SelectListItem> SelectYear { get; set; } = ReportesService.SelectYear();    
		/// <summary>
																	/// Listado de facturas SAP según el tipo de socio: Proveedor, Cliente.
																	/// </summary>
		public List<Documento> FacturasSAP { get; set; }
		/// <summary>
		/// Listado de facturas subidas por proveedores a la Intranet. 
		/// </summary>
		public List <ArchivoDoc> ArchivosProv { get; set; }
		/// <summary>
		/// Lista de usuarios registrados.
		/// </summary>
		public List<Usuario> Usuarios { get; set; }
		/// <summary>
		/// Lista de empleados registrados.
		/// </summary>
		public List<Empleado> Empleados { get; set; }
		public List<FacturaJSON> FacturasXML { get; set; }

		/// <summary>
		/// Nombre del empleado con acceso a la vista.
		/// </summary>
		public string Nombre { get; set; }
		/// <summary>
		/// Nombre del puesto de trabajo del empleado con acceso a la vista.
		/// </summary>
		public string Puesto { get; set; }
	}
}
