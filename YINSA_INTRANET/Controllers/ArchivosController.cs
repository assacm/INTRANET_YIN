using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YINSA_INTRANET.Models;
using YINSA_INTRANET.Servicios;

namespace YINSA_INTRANET.Controllers
{
	[Authorize]

	public class ArchivosController : Controller
	{
		private readonly IArchivosService archivosService;
		private readonly IRutasService rutas;

		public ArchivosController(IArchivosService archivosService, IRutasService rutas)
		{
			this.archivosService = archivosService;
			this.rutas = rutas;
		}
		[HttpPost]
		public async Task<IActionResult> SubirFactura([FromBody] ArchivoPOSTJs req)
		{
			if (string.IsNullOrEmpty(req.descripcion)) { return BadRequest("Debe agregar una descripción."); }

			if (req.files.Count != 2) { return BadRequest("Debe seleccionar algún archivo."); }

			var res = await archivosService.DocumentosBase64(req, "Factura");

			return Ok(res);
		}
		[HttpPost]
		[Authorize(Policy = "AutorizarFctProv")]

		public async Task<IActionResult> ActualizarDoc([FromBody] UpdDocumento upd)
		{
			if (string.IsNullOrEmpty(upd.Estado)) { return BadRequest("Debe actualizar el estado."); }

			var res = await archivosService.Actualizar(upd);

			if (!res.Status) { return BadRequest(res.StatusMessage); }

			return Ok(res);
		}
		[HttpGet]
		public async Task<IActionResult> Archivo(int id)
		{
			var res = await archivosService.DocumentById(id);
			
			if (!res.Status) { return BadRequest("Ha ocurrido un error."); }

			return Ok(res.Archivo);
		}
		public IActionResult PdfViewer(string filename)
		{

			//string rutaFisica = Path.Combine(hostEnvironment.WebRootPath, "Archivos", "MiArchivo.txt");
			//string ruta = Path.Combine(hostEnvironment.WebRootPath, "Archivos", filename) + ".pdf";
			//string ruta = $"/Archivos/{filename}.pdf";
			//string url = await rutas.ArchivosIntranet(); //$"wwwroot/Archivos/{filename}.{tipo}";

			//string ruta = Path.Combine(url, $"{filename}.pdf").Replace("\\", "/");

			//byte[] contenidoArchivo = System.IO.File.ReadAllBytes(ruta);

			

			return View("PdfViewer", filename);
		}
		public async Task<IActionResult> Pdf(string name)
		{
			try {
				string url = await rutas.ArchivosIntranet();

				url = url.Replace("\"", "");

				string ruta = Path.Combine(url, $"{name}.pdf").Replace("\\", "/");

				byte[] contenidoArchivo = System.IO.File.ReadAllBytes(ruta);

				return new FileContentResult(contenidoArchivo, "application/pdf");

			}
			catch (Exception ex)
			{
				// Logear la excepción si es necesario
				Console.WriteLine($"Error al obtener el archivo: {ex.Message}");

				// Devolver un ContentResult vacío para el iframe
				return Content(string.Empty, "text/html");
			}
			

		}
		public async Task<IActionResult> Descargar(string filename, string tipo)
		{
			// Petición de la url a la api

			//string urlOrigen = Path.Combine(hostEnvironment.ContentRootPath, "Archivos\\archivo.pdf");
			//string urlOrigen = $"/Archivos/{filename}.{tipo}";
			//string urlOrigen = Path.Combine(hostEnvironment.ContentRootPath, $"Archivos/{filename}.{tipo}").Replace("\\", "/");
			try { 
				string url = await rutas.ArchivosIntranet(); //$"wwwroot/Archivos/{filename}.{tipo}";
				url = url.Replace("\"", "");

				string urlOrigen = Path.Combine(url, $"{filename}.{tipo}").Replace("\\","/");

				byte[] contenidoArchivo = System.IO.File.ReadAllBytes(urlOrigen);

				// Establece el tipo de contenido y el nombre del archivo para la descarga
				string tipoContenido = "application/octet-stream";
				string nombreArchivo = $"{Guid.NewGuid()}.{tipo}";

				// Descargar el archivo
				return File(contenidoArchivo,tipoContenido,nombreArchivo);
			}
			catch {
				return NotFound();
			}
		}
	}
}
