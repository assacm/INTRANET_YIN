using YINSA_INTRANET.Models;
using YINSA_INTRANET.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Rotativa.AspNetCore;
using Microsoft.AspNetCore.Authorization;

namespace YINSA_INTRANET.Controllers
{
	[Authorize]
	
	public class ProveedoresController:Controller
	{
        private readonly ILogger<ProveedoresController> logger;
        private readonly ISociosService _socioService;
		private readonly IDocumentosService _documentosService;
		private readonly IReportesService _reportesService;
		private readonly IUsuariosService _usuariosService;

		public ProveedoresController(ILogger<ProveedoresController> logger,ISociosService socioService, IUsuariosService usuariosService,
			IDocumentosService documentoService, IReportesService reportesService)
        {
            this.logger = logger;
            _socioService = socioService;
			_documentosService = documentoService;
			_reportesService= reportesService;
			_usuariosService = usuariosService;
        }
		//[Authorize(Policy = "Socio")]
		//[Authorize(Policy = "Admin")]
		public async Task<IActionResult> Index()
		{
			var id = _usuariosService.GetSocioId();

			if (!_usuariosService.VerificacionCuenta(id)) 
				return RedirectToAction("NoEncontrado", "Home");

			List<SelectListItem> options = _reportesService.SelectPeriodo();
			ViewBag.Opciones = options;

		

			var res = await _socioService.Socio(id);

			if(res == "[]" || res==null) { return RedirectToAction("NoEncontrado", "Home"); }
			var socio = Newtonsoft.Json.JsonConvert.DeserializeObject<Socio>(res);
			
			var compras = await _documentosService.ComprasTop5(id);
			var c = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Documento>>(compras);

			if (c == null)
				c = new List<Documento>();

			var modelo = new SocioViewModel()
			{
				Socio = socio,
				Pedidos = c
			};

			return View(modelo);
        }
		public async Task<IActionResult> Facturacion(int estado, int mes, int year)
		{

			var id = _usuariosService.GetSocioId();
			//var res = "";
			DateTime fecha,inicio, fin; ;

			//var algo = Request.Form["fechaInput"].ToString();
			if (string.IsNullOrEmpty(Request.Form["fechaInput"]))
			{
				(inicio, fin) = _reportesService.RangoFechas(mes, year);
			}
			else
			{
				fecha = DateTime.Parse(Request.Form["fechaInput"]);
				(inicio, fin) = _reportesService.RangoFechas(fecha.Month, fecha.Year);
			}
			/*IEnumerable<KeyValuePair<string, string>> param =
				new List<KeyValuePair<string, string>>
					{
						new KeyValuePair<string, string>("id",id),
						new KeyValuePair<string, string>("inicio", inicio.ToString("yyyy-MM-dd")),
						new KeyValuePair<string, string>("fin", fin.ToString("yyyy-MM-dd"))
					};
			switch (estado)
			{
				case 0: {  res = await _documentosService.FactPenProveedor(param); break; }
				case 1: {  res = await _documentosService.FactFinProveedor(param); break; }
				case 2: { res = await _documentosService.FactCanProveedor(param); break; }
			}*/

			var facturas = await _documentosService.FacturasProveedor(id,estado,inicio,fin);
			//var facturas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Documento>>(res);
            
            var usuarioRes = await _socioService.Socio(id);
			var usuario = Newtonsoft.Json.JsonConvert.DeserializeObject<Socio>(usuarioRes);

			var modelo = new FacturacionViewModel()
			{
				Id = id,
				Nombre = usuario.NombreS,
				Estado = estado,
				Inicio = inicio,
				Fin = fin,
				Facturas = facturas
			};

			//if (!ModelIsValid) { }
			return View(modelo);
		}

		public IActionResult Factura()
		{
			return View();
		}
		public IActionResult OrdenCompra()
		{
			return View();
		}
		
		[HttpGet]
		public async Task<IActionResult> EstadoCuenta()
		{
			int periodo=0, estado = 0;
			var id = _usuariosService.GetSocioId();

			DateTime inicio, fin;//, fecha;
			(inicio, fin) = _reportesService.RangoPeriodo(periodo);
			//if (string.IsNullOrEmpty(Request.Form["fechaInput"]))
			//{
			//	(inicio, fin) = _reportesService.RangoPeriodo(periodo);
			//}
			//else
			//{
			//	fecha = DateTime.Parse(Request.Form["fechaInput"]);
			//	(inicio, fin) = _reportesService.RangoFechas(fecha.Month, fecha.Year);
			//}

		

			List<Documento> documentos = new();
			switch (estado)
			{
				case 0: { documentos = await _documentosService.EdoCuentaProveedor(id, inicio, fin); break; }
				case 1: { documentos = await _documentosService.FacturacionProveedor(id, inicio, fin); break; }
				case 2: { documentos = await _documentosService.FacturasProveedor(id, estado, inicio, fin); break; }
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

			//if (!ModelIsValid) { }
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
			
			/*IEnumerable<KeyValuePair<string, string>> param =
				new List<KeyValuePair<string, string>>
					{
						new KeyValuePair<string, string>("id", id),
						new KeyValuePair<string, string>("inicio", inicio.ToString("yyyy-MM-dd")),
						new KeyValuePair<string, string>("fin", fin.ToString("yyyy-MM-dd"))
					};*/

			List<Documento> documentos = new();
			switch (estado)
			{
				case 0: { documentos = await _documentosService.EdoCuentaProveedor(id, inicio, fin); break; }
				case 1: { documentos = await _documentosService.FacturacionProveedor(id, inicio, fin); break; }
				case 2: { documentos = await _documentosService.FacturasProveedor(id, estado, inicio, fin); break; }
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

		public async Task<IActionResult> Compras()
		{
			int vista =0 , periodo = 0;
			 var id = _usuariosService.GetSocioId();

			List<SelectListItem> options = _reportesService.SelectPeriodo();
            ViewBag.Opciones = options;

            (DateTime inicio, DateTime fin) = _reportesService.RangoPeriodo(periodo);

		
			//var productos = await _documentosService.Compras(id,vista, inicio,fin);
			var productos = await _documentosService.ComprasV2(id, vista, inicio, fin);
		

			var usuarioRes = await _socioService.Socio(id);
            var usuario = Newtonsoft.Json.JsonConvert.DeserializeObject<Socio>(usuarioRes);
			var modelo = new TabsViewModel()
			{IdCuenta = id,
				Socio = usuario,
				Vista = vista,
				DetalleProducto = new CardsViewModel()
				{
                    Vista = vista,
					ProductosV2 = productos
                    //Productos = productos
                }
            };

            return View(modelo);
        }
		[HttpPost]
		public async Task<IActionResult> Compras(int vista, int periodo)
		{

			var id = _usuariosService.GetSocioId();

			List<SelectListItem> options = _reportesService.SelectPeriodo();
			ViewBag.Opciones = options;

			(DateTime inicio, DateTime fin) = _reportesService.RangoPeriodo(periodo);


			//var productos = await _documentosService.Compras(id,vista, inicio,fin);
			var productos = await _documentosService.ComprasV2(id, vista, inicio, fin);


			var usuarioRes = await _socioService.Socio(id);
			var usuario = Newtonsoft.Json.JsonConvert.DeserializeObject<Socio>(usuarioRes);
			var modelo = new TabsViewModel()
			{
				IdCuenta = id,
				Periodo = periodo,
				Socio = usuario,
				Vista = vista,
				DetalleProducto = new CardsViewModel()
				{
					Vista = vista,
					ProductosV2 = productos
					//Productos = productos
				}
			};

			return View(modelo);
		}

		public async Task<IActionResult> RotativaPDF(int estado)
		{

			var id = _usuariosService.GetSocioId();
			DateTime fecha, inicio ,fin;

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
					};*/

			List<Documento> documentos = new();
			switch (estado)
			{
				case 0: { documentos = await _documentosService.EdoCuentaProveedor(id, inicio, fin); break; }
				case 1: { documentos = await _documentosService.FacturacionProveedor(id, inicio, fin); break; }
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
			return new ViewAsPdf("ToPdf", modelo)
			{
				PageSize = Rotativa.AspNetCore.Options.Size.Letter,
				PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
				FileName = "estadodecuenta.pdf"
			};
		}
	}
}
