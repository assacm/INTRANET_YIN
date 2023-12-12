using Microsoft.AspNetCore.Mvc.Rendering;

namespace YINSA_INTRANET.Models
{
	public class RevisionArchivosVM
	{
		public string IdEmpleado { get; set; }
		/// <summary>
		/// Representa el listado de proveedores a mostrar según la revisión de archivos deseada.
		/// </summary>
		public int ListState { get; set; }
		/// <summary>
		/// Estado de la lista de archivos a mostrar.
		/// </summary>
		public int Estado { get; set; } //tabs
		/// <summary>
		/// Indica el periodo del que se quieren visualizar los archivos.
		/// Nota:Se mostrarán todos los archivos pendientes de revisar sin importar su fecha.
		/// </summary>
		public int Periodo { get; set; } //select

		/// <summary>
		/// Lista de archivos subidos por el proveedor seleccionado.
		/// </summary>
		public List<ArchivoDoc> Archivos{ get; set; }
		/// <summary>
		/// Contiene la información completa del archivo seleccionado de la lista.
		/// </summary>
		public ArchivoDoc Detalle { get; set; }
		/// <summary>
		/// Lista de proveedores con archivos pendientes.
		/// </summary>
		public List<ProveedorCount> Proveedores {  get; set; }
		/// <summary>
		/// Identificador del proveedor seleccionado.
		/// </summary>
		public string IdProveedor { get; set; }

		public List<SelectListItem> SelectPeriodo { get; set; }
	}



}
