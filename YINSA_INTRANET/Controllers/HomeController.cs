using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using YINSA_INTRANET.Models;
using YINSA_INTRANET.Servicios;

namespace YINSA_INTRANET.Controllers
{
	[Authorize]

	public class HomeController : Controller
	{
		//private readonly ILogger<HomeController> _logger;
        private readonly IArchivosService archivosService;

        public HomeController(IArchivosService  archivosService)
		{
			//_logger = logger;
            this.archivosService = archivosService;
        }

		public async Task<IActionResult> LeerXML() 
		{
			//string ruta = "C:\\Users\\alma_\\OneDrive\\Documentos\\Bindus3\\0.YINSA DESARROLLOS\\YINSA_INTRANET\\YINSA_INTRANET\\wwwroot\\Archivos\\101016-100890-Factura-P00006.xml";

			//string ruta = "wwwroot/Archivos/101016-100890-Factura-P00006.xml";
			string ruta = "wwwroot/Archivos/C6F1D72A-09CB-4FBD-90F9-073594830B01.xml";

			var res = await archivosService.ReadXML(ruta);

			var fact = new FacturaJSON()
			{
				IdDoc = res.IdDoc,
				Folio = res.Folio,
				Version = res.Version,
				UUID = res.UUID,
				RFC = res.RFCEmisor,
				RazonSocial = res.Emisor,
				Tipo = res.Tipo,
				Estatus = res.Estatus,
				Metodo = res.Metodo,
				FormaPago = res.FormaPago,
				Moneda = res.Moneda,
				IVA = res.IVA,
				SubTotal = res.SubTotal,
				Total = res.Total,
				FechaTimbre = res.FechaTimbre
			};

			var json = Newtonsoft.Json.JsonConvert.SerializeObject(fact);

			return View();
		
		}
		public IActionResult Fuentes()
		{
			return View();
		}
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult NoEncontrado()
		{
			return View();
		}

		[HttpPost]
		[Authorize(Policy = "SubirFctProv")]
		public async Task<IActionResult> SubirFactura([FromBody] ArchivoPOSTJs req)
		{
			if (!ModelState.IsValid) { return BadRequest(new { message="Debe llenar los campos y seleccionar los archivos de la factura." }); }
	
			if (string.IsNullOrEmpty(req.descripcion)) { return BadRequest(new { message = "Debe agregar una descripción." }); }

            if (req.files.Count <= 0 ) { return BadRequest(new { message = "Debe seleccionar algún archivo." }); }

			
			List<string> dataXML = req.files.Where(f => f.type == "text/xml").Select(f =>f.data).ToList();


			(bool xml, List<Factura> xmls) = await ValidacionXML(dataXML);

			if (!xml)
				return BadRequest(new { message = "El archivo XML no es válido. Verifique la factura." });

			var res = await archivosService.DocumentosBase64(req, "Factura");

			if (!res.Status)
				return BadRequest(new { message = "No se ha podido subir la factura." });
		
			if (!string.IsNullOrEmpty(req.comentario))
			{ await archivosService.CrearComentario(req.comentario, int.Parse(res.Valor)); }
			/*
			 * AQUI VAMOS A VALIDAR SI SE INSERTARON LOS ARCHIVOS. NOS REGRESA EL IDDOC. 
			 * SEGUN EL IDDOC, USAMOS LA FUNCION SUBIRXML,LE PASAMOS EL MODELO FACTURA Y A ESTE LE ACTUALIZAMOS EL IDDOC
			 */


			var xmlRes = await archivosService.SubirXML(xmls, int.Parse(res.Valor));

			if (!xmlRes.Status)
				return BadRequest(new { message = "No se ha podido subir la factura." });


			return Ok(res);
		}

		private async Task<(bool,List<Factura>)> ValidacionXML(List<string> data)
		{
			bool v = true; List<Factura> fac = new();

			foreach (var d in data) 
			{

				var factura = await archivosService.ReadXMLFromBase64(d);

				if (!factura.Status)
				{  return (v = false, fac); } 

				fac.Add(factura);
			}
			return (v, fac);
		}

  //      [HttpGet]
  //      public IActionResult Subir()//ya no se usará
		//{
  //          return View();
  //      }
  //      [HttpPost]
  //      public async Task<IActionResult> Subir(ArchivoPost a) //ya no se usará
  //      {

		//	var resp = await archivosService.PostFromFile(a);
  //          return View();
  //      }

        
	}
}