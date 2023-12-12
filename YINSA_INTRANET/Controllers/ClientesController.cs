using YINSA_INTRANET.Models;
using YINSA_INTRANET.Servicios;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Http.Extensions;
using Rotativa.AspNetCore.Options;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace YINSA_INTRANET.Controllers
{
	[Authorize]
	
	public class ClientesController : Controller
	{
		private readonly ISociosService _socioService;
		private readonly IDocumentosService _documentosService;
		private readonly IReportesService _reportesService;
		private readonly IUsuariosService _usuariosService;

		public ClientesController(ISociosService sociosService, IDocumentosService documentosService,
			IReportesService reportesService,
			IUsuariosService usuariosService)
		{
			_socioService = sociosService;
			_documentosService = documentosService;
			_reportesService = reportesService;
			_usuariosService = usuariosService;
		}

		public async Task<IActionResult> Index()
		{
			var id = _usuariosService.GetSocioId();
            if (!_usuariosService.VerificacionCuenta(id)) 
				return RedirectToAction("NoEncontrado", "Home");
		
			List<SelectListItem> options = _reportesService.SelectPeriodo();
			ViewBag.Opciones = options;


			var res = await _socioService.Socio(id);

			if (res == "[]" || res == null) { return RedirectToAction("NoEncontrado", "Home"); }
			var socio = Newtonsoft.Json.JsonConvert.DeserializeObject<Socio>(res);
			var modelo = new SocioViewModel()
			{
				Socio = socio,
			};

			return View(modelo);

		}


		[HttpGet]
		public async Task<IActionResult> Pedidos()
		{
			int vista = 0, periodo = 0;

            var id = _usuariosService.GetSocioId();
			List<SelectListItem> options = _reportesService.SelectPeriodo();
			ViewBag.Opciones = options;

			(DateTime inicio, DateTime fin) = _reportesService.RangoPeriodo(periodo);


			//var productos = await _documentosService.Pedidos(id, vista, inicio, fin);
			var productos = await _documentosService.PedidosV2(id, vista, inicio, fin);

			var usuarioRes = await _socioService.Socio(id);

			var usuario = Newtonsoft.Json.JsonConvert.DeserializeObject<Socio>(usuarioRes);

			var modelo = new TabsViewModel()
			{ IdCuenta = id,
				Socio = usuario,
				Periodo = periodo,
				DetalleProducto = new CardsViewModel() { 					
					Vista=vista,
					ProductosV2 = productos
					//Productos=productos
				}
			};

			return View(modelo);	
		}

		[HttpPost]
        public async Task<IActionResult> Pedidos(int vista, int periodo)
        {
            var id = _usuariosService.GetSocioId();
            List<SelectListItem> options = _reportesService.SelectPeriodo();
            ViewBag.Opciones = options;

            (DateTime inicio, DateTime fin) = _reportesService.RangoPeriodo(periodo);


            //var productos = await _documentosService.Pedidos(id, vista, inicio, fin);
            var productos = await _documentosService.PedidosV2(id, vista, inicio, fin);

            var usuarioRes = await _socioService.Socio(id);

            var usuario = Newtonsoft.Json.JsonConvert.DeserializeObject<Socio>(usuarioRes);

            var modelo = new TabsViewModel()
            {
                IdCuenta = id,
                Socio = usuario,
                Periodo = periodo,
                DetalleProducto = new CardsViewModel()
                {
                    Vista = vista,
                    ProductosV2 = productos
                    //Productos=productos
                }
            };

            return View(modelo);
        }

        public async Task<IActionResult> RotativaPDF(int estado)
		{
			//bool ocultarElemento = true;

			//ViewBag.OcultarElemento = ocultarElemento;

			var id = _usuariosService.GetSocioId();
			DateTime fecha,inicio,fin;

			if (string.IsNullOrEmpty(Request.Form["fechaInput"]))
			{
				fecha = DateTime.Today;
				(inicio, fin) = _reportesService.RangoFechas(fecha.Month, fecha.Year);
			}
			else
			{
				fecha = DateTime.Parse(Request.Form["fechaInput"]);
				(inicio, fin) = _reportesService.RangoFechas(fecha.Month, fecha.Year);
			}
			
			/*IEnumerable<KeyValuePair<string, string>> param =
				new List<KeyValuePair<string, string>>
					{
						new KeyValuePair<string, string>("id", id),
						new KeyValuePair<string, string>("inicio", inicio.ToString("yyyy-MM-dd")),
						new KeyValuePair<string, string>("fin", fin.ToString("yyyy-MM-dd"))
					};
			*/
			List<Documento> documentos = new();
			switch (estado)
			{
				case 0: { documentos = await _documentosService.EdoCuentaCliente(id, inicio, fin); break; }
				case 1: { documentos = await _documentosService.FacturacionCliente(id, inicio, fin); break; }
			}

			//var documentos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Documento>>(res);
			var usuarioRes = await _socioService.Socio(id);
			var usuario = Newtonsoft.Json.JsonConvert.DeserializeObject<Socio>(usuarioRes);
			var modelo = new EstadoCuentaViewModel()
			{
				IdCuenta = id,
				Id = id,
				Nombre = usuario.NombreS,
				Inicio = inicio,
				Fin = fin,
				Estado = estado,
				Documentos = documentos
			};
			return new ViewAsPdf("ToPdf", modelo)
			{
				PageSize = Rotativa.AspNetCore.Options.Size.Letter,
				PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
				FileName = "estadodecuenta.pdf"
			};
		}
		[HttpGet]
		public async Task<IActionResult> EstadoCuenta()
		{
			int periodo = 0, estado =0;

			var id = _usuariosService.GetSocioId();

			DateTime inicio, fin;//, fecha;

			//string request = Request.Form["fechaInput"].ToString();

			//if (string.IsNullOrEmpty(request))
			//{
			//	(inicio, fin) = _reportesService.RangoPeriodo(periodo);
			//}
			//else
			//{
			//	fecha = DateTime.Parse(Request.Form["fechaInput"]);
			//	(inicio, fin) = _reportesService.RangoFechas(fecha.Month, fecha.Year);
			//}

			(inicio, fin) = _reportesService.RangoPeriodo(periodo);

			List<Documento> documentos = new();
			switch (estado)
			{
				case 0: { documentos = await _documentosService.EdoCuentaCliente(id, inicio, fin); break; }
				case 1: { documentos = await _documentosService.FacturacionCliente(id, inicio, fin); break; }
				case 2: { documentos = await _documentosService.FacturasCliente(id, estado, inicio, fin); break; }
			}

			var usuarioRes = await _socioService.Socio(id);
			var usuario = Newtonsoft.Json.JsonConvert.DeserializeObject<Socio>(usuarioRes);
			var modelo = new EstadoCuentaViewModel()
			{
				IdCuenta = id,
				Id = id,
				Nombre = usuario.NombreS,
				Inicio = inicio,
				Fin = fin,
				Estado = estado,
				Documentos = documentos
			};

			return View(modelo);
		}

		[HttpPost]
		public async Task<IActionResult> EstadoCuenta(int periodo, int estado)
		{

			var id = _usuariosService.GetSocioId();

			DateTime inicio, fin, fecha;

			if (string.IsNullOrEmpty(Request.Form["fechaInput"]))
			{
				(inicio, fin) = _reportesService.RangoPeriodo(periodo);
			}
			else
			{
				fecha = DateTime.Parse(Request.Form["fechaInput"]);
				(inicio, fin) = _reportesService.RangoFechas(fecha.Month, fecha.Year);
			}
			/*
			IEnumerable<KeyValuePair<string, string>> param =
				new List<KeyValuePair<string, string>>
					{
						new KeyValuePair<string, string>("id", id),
						new KeyValuePair<string, string>("inicio", inicio.ToString("yyyy-MM-dd")),
						new KeyValuePair<string, string>("fin", fin.ToString("yyyy-MM-dd"))
					};
			*/
			//string res = "";
			List<Documento> documentos = new();
			switch (estado)
			{
				case 0: { documentos = await _documentosService.EdoCuentaCliente(id,inicio,fin); break; }
				case 1: { documentos = await _documentosService.FacturacionCliente(id, inicio, fin); break; }
				//case 2: { res = await _documentosService.FactCanClient(param); break; }
				case 2: { documentos = await _documentosService.FacturasCliente(id, estado, inicio, fin); break; }
			}

			//var documentos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Documento>>(res);
			var usuarioRes = await _socioService.Socio(id);
			var usuario = Newtonsoft.Json.JsonConvert.DeserializeObject<Socio>(usuarioRes);
			var modelo = new EstadoCuentaViewModel()
			{IdCuenta = id,
				Id = id,
				Nombre = usuario.NombreS,
				Inicio = inicio,
				Fin = fin,
				Estado = estado,
				Documentos = documentos
			};

			//if (!ModelIsValid) { }
			return View(modelo);
		}

		
	}
}

