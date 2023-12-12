using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace YINSA_INTRANET.Models
{
	public class Empleado
	{
		public string IdCuenta { get; set; }
		[Required(ErrorMessage = "Debe ingresar el nombre del empleado.")]
		public string Nombre { get; set; }
		[Required(ErrorMessage ="Debe ingresar el apellido paterno del empleado.")]
		public string ApellidoPa { get; set; }
		[Required(ErrorMessage = "Debe ingresar el apellido materno del empleado.")]
		public string ApellidoMa { get; set; }
		public string Nss { get; set; }
		[Required(ErrorMessage = "Debe ingresar el número de teléfono celular.")]
		[Phone]
		public string Celular { get; set; }
		public string Telefono { get; set; }
		[Required(ErrorMessage = "Debe ingresar el correo electrónico.")]
		[EmailAddress]
		public string Email { get; set; }
		[Display(Name = "Departamento")]
		[Required(ErrorMessage = "Debe seleccionar un departamento.")]
		[Range(1, int.MaxValue, ErrorMessage = "Por favor, seleccione un departamento.")] 
		public int IdDepartamento { get; set; }
		public string Departamento { get; set; }

		[Display(Name = "Puesto")]
		[Required(ErrorMessage = "Debe seleccionar un puesto de trabajo.")]
		[Range(1, int.MaxValue, ErrorMessage = "Por favor, seleccione un puesto de trabajo.")]
		public int IdPuesto { get; set; }
		public string Puesto { get; set; }

		public string TipoSangre { get; set; }
		public int Vacaciones { get; set; }
		//[DisplayFormat(DataFormatString = "{0:dd 'de' MMMM 'del' yyyy}", ApplyFormatInEditMode = true)]
		//[DataType(DataType.Date)]
		//[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		[Required(ErrorMessage = "Debe ingresar la fecha de ingreso del empleado a la empresa.")]

		public DateTime FechaIngreso { get; set; } = DateTime.Parse(new DateTime().ToString("yyyy-MM-dd"));
		//[DisplayFormat(DataFormatString = "{0:dd 'de' MMMM 'del' yyyy}", ApplyFormatInEditMode = true)]
		//[DataType(DataType.Date)]
		//[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
		[Required(ErrorMessage = "Debe ingresar la fecha de nacimiento del empleado.")]
		public DateTime Nacimiento { get; set; } = DateTime.Parse(new DateTime().ToString("yyyy-MM-dd"));
	}

	public class EmpleadoViewModel
	{
		public Empleado Empleado { get; set; }

		public List<SelectListItem> Puestos { get; set; }
		public List<SelectListItem> Departamentos { get; set; }

	}
}
