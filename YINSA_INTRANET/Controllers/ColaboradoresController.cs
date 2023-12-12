using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using YINSA_INTRANET.Models;
using YINSA_INTRANET.Servicios;

namespace YINSA_INTRANET.Controllers
{
	[Authorize]
	public class ColaboradoresController : Controller
	{
        private readonly IUsuariosService usuariosService;
		private readonly IArchivosService archivosService;
		private readonly IReportesService reportesService;
		private readonly IEmpleadosService empleadosService;
		private readonly IDocumentosService documentosService;

		public ColaboradoresController(IUsuariosService usuariosService, 
			IArchivosService archivosService, IReportesService reportesService,
			IEmpleadosService empleadosService, IDocumentosService documentosService )
        {
            this.usuariosService = usuariosService;
			this.archivosService = archivosService;
			this.reportesService = reportesService;
			this.empleadosService = empleadosService;
			this.documentosService = documentosService;
		}
        public async Task<IActionResult> Index()
		{
			var id = usuariosService.GetSocioId();
			//if (!usuariosService.VerificacionCuenta(id)) return RedirectToAction("NoEncontrado", "Home");
			var e = await empleadosService.Empleado(id);
			var facts = await archivosService.AutorizacionProveedores(0);

			var model = new ColaboradorViewModel
			{
				Empleado = e,
				FacturasPendientes = facts.Item2

			};

            return View(model);
		}
		[HttpGet]
		[Authorize(Roles = "admin")]
        public async Task<IActionResult> Editar(string id) 
        {

			var empleado = await empleadosService.Empleado(id);
			
			var puestos = await reportesService.GetPuestos();
			var depar = await reportesService.GetDepartamentos();

			var model = new EmpleadoViewModel()
			{  Empleado = empleado,
				Puestos = puestos,
				Departamentos = depar
			};
            return View(model); 
        }
        [HttpPost]
		[Authorize(Roles = "admin")]
		public async Task<IActionResult> Editar(EmpleadoViewModel e)
		{
			

			var puestos = await reportesService.GetPuestos();
			var depar = await reportesService.GetDepartamentos();

			e.Puestos = puestos;
			e.Departamentos = depar;


			if (!ModelState.IsValid) 
			{
				ViewBag.Message = "Debe completar los campos requeridos.";
				return View(e);
			}

			var res = await empleadosService.Editar(e.Empleado);

			if (res.StatusCode == 200)
				ViewBag.Message = "Registro exitoso.";
			else
				ViewBag.Message = "Ha ocurrido un problema.";
		
			return View(e);
		}

		public async Task<IActionResult> Registro()
		{
			var puestos = await reportesService.GetPuestos();
			var depar = await reportesService.GetDepartamentos();

			var model = new EmpleadoViewModel()
			{
				Puestos = puestos,
				Departamentos = depar
			};
			return View(model);
		}
		[HttpPost]
		public async Task<IActionResult> Registro(EmpleadoViewModel e)
		{
			if (!ModelState.IsValid)
			{
				return View(e);
			}

			var puestos = await reportesService.GetPuestos();
			var depar = await reportesService.GetDepartamentos();

			e.Puestos = puestos;
			e.Departamentos = depar;


			if (!ModelState.IsValid)
			{
				ViewBag.Message = "Debe completar los campos requeridos.";
				return View(e);
			}

			var res = await empleadosService.Registro(e.Empleado);

			if (res.StatusCode == 200)
				ViewBag.Message = "Registro exitoso.";
			else
				ViewBag.Message = "Ha ocurrido un problema.";

			return View(e);
		}


		[HttpGet]
		//[Authorize(Policy = "Contador, Admin")]
		//[Authorize(Policy ="Admin")]
		[Authorize(Roles = "contador, admin")]
		public async Task<IActionResult> Contabilidad()
		{
			var id = usuariosService.GetSocioId();

			if (string.IsNullOrEmpty(id)) { return View("NoEncontrado", "Home"); }
			var user = await empleadosService.Empleado(id);

			if (user == null) { return RedirectToAction("NoEncontrado", "Home"); }

			(DateTime inicio, DateTime fin) = reportesService.RangoFechas(DateTime.Now.Month,DateTime.Now.Year);

			List<ArchivoDoc> archivosProv = new();

			var facts = await archivosService.DocCount(0);

			var arch = await archivosService.Documentos(-1, 0, inicio, fin); //-1 para mandar estado default: null, traer todos
			if (arch.Status)
			{
				archivosProv = arch.Archivos;
			}

			var model = new ControlViewModel()
			{
				//MenuOption = ,
				//SelectTabla = post.SelectTabla,
				SelectMonth = ReportesService.SelectMonth(DateTime.Now.Year),
				ArchivosProv = archivosProv,
				FacturasPendientes = facts,
				Nombre = $"{user.Nombre} {user.ApellidoPa} {user.ApellidoPa} ",
				Puesto =user.Puesto,
				IdCuenta =id 
			};

			return View(model);
		}
		[HttpPost]
		[Authorize(Roles = "contador,admin")]
		public async Task<IActionResult> Contabilidad(ControlViewModel post)
		{
			if (string.IsNullOrEmpty(post.IdCuenta)) { return View("NoEncontrado", "Home"); }
			var user = await empleadosService.Empleado(post.IdCuenta);


			var facts = await archivosService.DocCount(0);

			List<Documento> facturasSap = new();
			List<ArchivoDoc> archivosProv = new();
			List<FacturaJSON> factXML = new();
			(DateTime inicio, DateTime fin) = reportesService.RangoFechas(post.Month, post.Year);

			//Obtener facturas según : Clientes Proveedores <- switch, Fechas, Estado
			if (post.Socio == 0) { facturasSap = await documentosService.FacturasCliente(null, post.Estado, inicio, fin); }

			if (post.Socio == 1) { facturasSap = await documentosService.FacturasProveedor(null, post.Estado, inicio, fin); }
			//Obtener archivos subidos de proveedores

			if (post.SelectTabla == 1)
			{
				var r = await archivosService.FacturasXML(null, 0);
				factXML = r.FacturasXML;
			}

			var arch = await archivosService.Documentos(-1, 0,inicio, fin); //-1 para mandar estado default: null, traer todos
			if (arch.Status)
			{
				archivosProv = arch.Archivos;
			}

			var model = new ControlViewModel()
			{
				MenuOption = post.MenuOption,
				SelectTabla = post.SelectTabla,
				SelectMonth = ReportesService.SelectMonth(DateTime.Now.Year),
				FacturasPendientes = facts,
				FacturasSAP = facturasSap,
				ArchivosProv = archivosProv,
				FacturasXML = factXML,
				IdCuenta = post.IdCuenta
			};
			
			return View(model);
		}
	}
}
