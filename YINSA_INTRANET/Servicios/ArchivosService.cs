using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Net.Mime;
using System.Net.NetworkInformation;
using System.Security.Claims;
using YINSA_INTRANET.Models;

using Microsoft.AspNetCore.Authentication.Cookies;
using System.Reflection;
using System.Xml;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components.Forms;

namespace YINSA_INTRANET.Servicios
{
    public interface IArchivosService
    {	
		/// <summary>
		/// Actualiza el estado de revisión del archivo
		/// </summary>
		/// <param name="upd">Datos del documento a actualizar.</param>
		/// <returns>Cadena de respuesta.</returns>
		Task<Respuesta> Actualizar(UpdDocumento upd);
		Task<Respuesta> ActualizarXML(Factura fac);
		/// <summary>
		/// Agrega una autorización a un documento.
		/// </summary>
		/// <param name="aut">Información de la autorización.</param>
		/// <returns>Estado de respuesta.</returns>
		Task<Respuesta> AsignarAutorizacion(Autorizar aut);

		/// <summary>
		/// Lista de autorizaciones de determinado documento.
		/// </summary>
		/// <param name="idDoc">Identificador del documento.</param>
		/// <returns></returns>
		Task<List<AutorizacionesDoc>> AutorizacionesDocumento(int idDoc);
		/// <summary>
		/// Obtiene la lista de proveedores por autorizar según su estado y según el usuario que autoriza.
		/// </summary>
		/// <param name="estado"></param>
		/// <returns>Lista de proveedores a los que autoriza el usuario y un valor que contiene el conteo de los archivos de estos proveedores.</returns>
		Task<(List<ProveedorCount>, int)> AutorizacionProveedores(int estado);

		/// <summary>
		/// Obtiene lista de usuarios con permiso de autorización para determinado socio.
		/// </summary>
		/// <param name="idSocio">Identificador del socio.</param>
		/// <returns>Lista de autorizaciones.</returns>
		Task<List<AutorizacionesDoc>> AutorizadoresDocumento(string idSocio);
		/// <summary>
		/// Actualiza la información de una autorización asignada existente.
		/// </summary>
		/// <param name="aut"></param>
		/// <returns></returns>
		Task<Respuesta> AutorizarDoc(Autorizar aut);
		Task<Respuesta> CrearComentario(string com,int idDoc);

		/// <summary>
		/// Obtiene el conteo de archivos pendientes de revisión.
		/// <param name="grupo">Identifica el grupo de proveedores de los cuales se obtendrán sus archivos.</param>
		/// </summary>
		/// <returns>Entero reprentativo del número de archivos pendientes.</returns>
		Task<int> DocCount(int grupo);
		/// <summary>
		/// Obtiene la información detallada del documento y archivos para su revisión.
		/// </summary>
		/// <param name="id">Identificador del documento a revisar.</param>
		/// <returns>Información del documento, comentarios y archivos adjuntos.</returns>
		Task<RespuestaArchivos> DocumentById(int id);
		/// <summary>
		/// Obtiene la lista de documentos con archivos según su estado y fecha.
		/// </summary>
		/// <param name="estado">Clave numérica del Estado del documento.</param>
		/// <param name="f1">Fecha inicial de consulta.</param>
		/// <param name="f2">Fecha final de consulta.</param>
		/// <returns>Lista de documentos según filtros.</returns>
		Task<RespuestaArchivos> Documentos(int estado,int grupo, DateTime f1, DateTime f2);
		/// <summary>
		/// Realiza post de archivos ingresados en formato Base64.
		/// </summary>
		/// <param name="archivo"></param>
		/// <param name="grupo">Identifica el grupo de proveedores de los cuales se obtendrán sus archivos.</param>
		/// <param name="tipo">Identifica al tipo de documento al que se asigna el archivo: Factura, Nota de crédito, etc.</param>
		/// <returns></returns>
		Task<RespuestaArchivos> DocumentosBase64(ArchivoPOSTJs archivo, string tipo);
		/// <summary>
		/// Obtiene la lista de documentos con archivos según el socio, estado y fecha.
		/// </summary>
		/// <param name="idSocio">Identificador del socio.</param>
		/// <param name="estado">Clave numérica del Estado del documento.</param>
		/// <param name="f1">Fecha inicial de consulta.</param>
		/// <param name="f2">Fecha final de consulta.</param>
		/// <returns>Listado de documentos.</returns>
		Task<RespuestaArchivos> DocumentosSocio(string idSocio, int estado, DateTime f1, DateTime f2);
		Task<RespuestaArchivos> FacturasXML(string idSocio, int estado);

		/// <summary>
		/// Cambia un archivo de carpeta
		/// </summary>
		/// <param name="name">Nombre del archivo.</param>
		/// <param name="ext">Extensión del archivo.</param>
		/// <param name="origen">Ruta completa del origen del archivo.</param>
		/// <param name="destino">Ruta a carpeta destino.</param>
		/// <returns></returns>
		Respuesta Mover(string name, string ext, string origen, string destino);
		Task<List<ComentarioModel>> ObtenerComentarios(int idDoc);

		/// <summary>
		/// Realiza el post de un archivo IFormFile.
		/// </summary>
		/// <param name="archivo"></param>
		/// <returns>Cadena con estatus de respuesta.</returns>
		Task<string> PostFromFile(ArchivoPost archivo);
		/// <summary>
		/// Obtiene lista de proveedores con un conteo de sus archivos según su estado.
		/// </summary>
		/// <param name="estado">Identifica el estado de los archivos según la calve numérica.</param>
		/// <param name="grupo">Identifica el grupo de proveedores de los cuales se obtendrán sus archivos.</param>
		/// <returns>Lista de proveedores.</returns>
		Task<RespuestaArchivos> ProveedoresCount(int estado, int grupo);
		Task<Factura> ReadXML(string ruta);
		Task<Factura> ReadXMLFromBase64(string data);
		Task<Respuesta> SubirXML(List<Factura> facturas, int idDoc);
		Task<Factura> ValidXMLSAT(Factura fact);
	}
    public class ArchivosService:IArchivosService
    {
        private readonly IConfiguration _config;
        private readonly IApiService api;
       
        private readonly IUsuariosService usuariosService;
		private readonly IHttpContextAccessor context;
		private readonly ILogger<ArchivosService> logger;

		public ArchivosService(IConfiguration configuration,IApiService apiService,
            IUsuariosService usuariosService, IHttpContextAccessor context, ILogger<ArchivosService> logger)
        {
            this._config = configuration;
            this.api = apiService;
            this.usuariosService = usuariosService;
			this.context = context;
			this.logger = logger;
		}

		public async Task<RespuestaArchivos> ProveedoresCount(int estado, int grupo)
		{
			string e = estado switch
			{
				0 => "Pendiente",
				1 => "Aprobado",
				2 => "Rechazado",
				_ => null
			};

			string endpoint = _config["endpoint:archivos:proveedores"];

			//var resp = await api.Get(endpoint);
			var par = new List<KeyValuePair<string, string>>
					{
						new KeyValuePair<string, string>("estado", e),
						new KeyValuePair<string, string>("grupo", grupo.ToString())
					};
			var resp = await api.GetParams(endpoint, par);

			if (resp == "[]" || string.IsNullOrEmpty(resp)) { return new RespuestaArchivos { StatusCode = 400, StatusMessage = "Hubo un error.", Status = false }; }

			var r = Newtonsoft.Json.JsonConvert.DeserializeObject<RespuestaArchivos>(resp);

			return r;

		}
		public async Task<int> DocCount(int grupo)
        {
			string endpoint = _config["endpoint:archivos:count"];

			var par = new List<KeyValuePair<string, string>>
					{
						new KeyValuePair<string, string>("grupo", grupo.ToString())
					};
			var resp = await api.GetParams(endpoint,par);

            if (resp == "[]" || string.IsNullOrEmpty(resp)) { return 0; }

            var r = Newtonsoft.Json.JsonConvert.DeserializeObject<RespuestaArchivos>(resp);
            
            return Convert.ToInt32(r.Valor);
		}

		public async Task<RespuestaArchivos> DocumentById(int id)
		{
			string endpoint = _config["endpoint:archivos:id"];
            endpoint += id;
		
            var resp = await api.Get(endpoint);

			if (resp == "[]" || string.IsNullOrEmpty(resp)) { return new RespuestaArchivos { StatusCode=400,StatusMessage="Hubo un error."} ; }

			var r = Newtonsoft.Json.JsonConvert.DeserializeObject<RespuestaArchivos>(resp);

            return r;
		}
		public async Task<RespuestaArchivos> DocumentosSocio(string idSocio, int estado, DateTime f1, DateTime f2)
		{
			string endpoint = _config["endpoint:archivos:socio"];

			string e = estado switch
			{
				0 => "Pendiente",
				1 => "Aprobado",
				2 => "Rechazado",
				_ => null
			};

			//var resp = await api.Get(endpoint);
			var par = new List<KeyValuePair<string, string>>
					{
						new KeyValuePair<string, string>("idSocio", idSocio ),
						new KeyValuePair<string, string>("estado", e),
						new KeyValuePair<string, string>("f1", f1.ToString("yyyy-MM-dd")),
						new KeyValuePair<string, string>("f2", f2.ToString("yyyy-MM-dd"))
					};
			var resp = await api.GetParams(endpoint, par);

			if (resp == "[]" || string.IsNullOrEmpty(resp)) { return new RespuestaArchivos { StatusCode = 400, StatusMessage = "Hubo un error.", Status = false }; }

			var r = Newtonsoft.Json.JsonConvert.DeserializeObject<RespuestaArchivos>(resp);

			return r;
		}
		public async Task<RespuestaArchivos> Documentos(int estado, int grupo,DateTime f1, DateTime f2)
		{
			string e = estado switch
			{
				0 => "Pendiente",
				1 => "Aprobado",
				2 => "Rechazado",
				_ => null
			};
			string endpoint = _config["endpoint:archivos:estado"];

            //var resp = await api.Get(endpoint);
            var par = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("estado", e),
						 new KeyValuePair<string, string>("grupo", grupo.ToString()),
						new KeyValuePair<string, string>("f1", f1.ToString("yyyy-MM-dd")),
						new KeyValuePair<string, string>("f2", f2.ToString("yyyy-MM-dd"))
					};
			var resp = await api.GetParams(endpoint,par);

			if (resp == "[]" || string.IsNullOrEmpty(resp)) { return new RespuestaArchivos { StatusCode = 400, StatusMessage = "Hubo un error.", Status = false }; }

			var r = Newtonsoft.Json.JsonConvert.DeserializeObject<RespuestaArchivos>(resp);

			return r;
		}
		public async Task<Respuesta> CrearComentario(string com, int idDoc)
		{
			var id = usuariosService.GetSocioId();
			int idUser = await usuariosService.GetUserId();

			var model = new ComentarioModel()
			{
				IdDoc = idDoc,
				IdSocio = id,
				IdUsuario = idUser,
				Comentario = com
			};

			var body = Newtonsoft.Json.JsonConvert.SerializeObject(model);

			string endpoint = _config["endpoint:archivos:comentar"];
			var resp = await api.PostRequest(endpoint, body);
			if (resp == "[]" || string.IsNullOrEmpty(resp)) { return new Respuesta { StatusCode = 400, StatusMessage = "Hubo un error.", Status = false }; }

			var r = Newtonsoft.Json.JsonConvert.DeserializeObject<Respuesta>(resp);
			return r;
		}
		public async Task<List<ComentarioModel>> ObtenerComentarios(int idDoc)
		{
			string endpoint = _config["endpoint:archivos:comentarios"] + idDoc;

			var resp = await api.Get(endpoint);

			List<ComentarioModel> com = new();

			if (resp == "[]" || string.IsNullOrEmpty(resp)) { return com; }

			var r = Newtonsoft.Json.JsonConvert.DeserializeObject<RespuestaComentarios>(resp);

			com = r.Comentarios;

			return com;
		}
		public async Task<Respuesta> Actualizar(UpdDocumento upd)
        {
		
			//Bloque para actualizar la ruta del archivo según el estado
			var body = Newtonsoft.Json.JsonConvert.SerializeObject(upd);



			string endpoint = _config["endpoint:archivos:actualizar"];
			var resp = await api.PostRequest(endpoint, body);
			if (resp == "[]" || string.IsNullOrEmpty(resp)) { return new Respuesta { StatusCode = 400, StatusMessage = "Hubo un error.", Status = false }; }

			var r = Newtonsoft.Json.JsonConvert.DeserializeObject<Respuesta>(resp);

			//mover a la ruta que indicamos
			//if (upd.Estatus)
			//{
			//	ArchivoAprobado(upd.FileName, upd.Ruta);
			//}


			return r;
		}
		public async Task<string> Subir(ArchivoPost archivo) {
           
			archivo.IdSocio = usuariosService.GetSocioId(); ;
            archivo.IdUser = await usuariosService.GetUserId();

			string endpoint = _config["endpoint:archivos:subir"];

			var resp = await api.PostFile(endpoint, archivo);

			return resp;

		}   	
        
		public Respuesta ArchivoAprobado(string name, string origen) 
		{
			string endpoint = _config["configuracion:rutas:validXML"];			

			return Mover(name, "xml", origen, endpoint);

		}
		/*
		public Respuesta ArchivoRechazado(string name, string origen) 
		{
			string endpoint = _config["configuracion:rutas:rechazados"];
			Respuesta res = new();
			string[] ext = new string[2] { "pdf", "xml" };


			foreach (var e in ext)
			{
				res = Mover(name, e, origen, endpoint);
			}

			return res;

		}*/
		public Respuesta Mover(string name,string ext,string origen, string destino)
		{
			try
			{
				//string inicial = $"{origen}/{name}.{ext}";
				string final = $"{destino}{name}.{ext}";

				File.Move(origen, final);
				return new Respuesta { Status = true, StatusMessage = "Se ha procesado el archivo", StatusCode = 200 };
				//return new Respuesta { Status = false, StatusMessage = "No se ha podido procesar el archivo.", StatusCode = 400 };
			}
			catch(Exception ex) 
			{	logger.LogError(ex.Message);
				return new Respuesta { Status=false, StatusMessage=ex.ToString(), StatusCode=500 };
				
			}
		}//acceso directo a subir archivo único endpoint /documentos/archivo
		public async Task<string> PostFromFile(ArchivoPost archivo)
		{

			var id = usuariosService.GetSocioId();
			archivo.IdSocio = id;
			//  archivo.Tipo = "Factura";
			string endpoint = _config["endpoint:documentos:subir"];

			var resp = await api.PostFile(endpoint, archivo);

			return resp;

		} 
		public async Task<RespuestaArchivos> DocumentosBase64(ArchivoPOSTJs archivo,string tipo) 
		{

            var id = usuariosService.GetSocioId();
            int idUser = await usuariosService.GetUserId();
            string endpoint = _config["endpoint:archivos:subir"];
            ArchivoPost post;
            string resp = "";
            List<IFormFile> files = new();
            List<string> ext = new();

            foreach (var item in archivo.files)
            {
                var file = ConvertFromBase64(item.data, item.name, item.type);
                ext.Add(ExtensionArchivo(file));              
                files.Add(file);             
            }

            if(!(ext.Contains("pdf") && ext.Contains("xml")))
            {
                return new RespuestaArchivos() { Status=false, StatusMessage = "No se agregaron los archivos correspondientes." };
			}

			foreach (var file in files)
			{
				post = new ArchivoPost()
				{
					IdSocio = id,
					IdUser = idUser,
					IdUserUpd = idUser,
					DocEntry = archivo.docEntry,
                    DocNum = archivo.docNum,
					TipoDoc = tipo,
					ValidSAT = true,
					ValidXML =false,
					SAP = false,
					TipoObjSAP = archivo.obj,
					Contabilidad = false,
					Descripcion = archivo.descripcion,	
					Concepto ="FACTPROV",
					File = file
				};

				resp = await api.PostFile(endpoint, post);
			}

			if (resp == "[]")
				return new RespuestaArchivos { Status = false, StatusMessage = "Algo ha salido mal." };

			RespuestaArchivos respuestaArchivos = Newtonsoft.Json.JsonConvert.DeserializeObject<RespuestaArchivos>(resp);

			return respuestaArchivos;           
        }

		public static IFormFile ConvertFromBase64(string data, string fileName, string contentType)
		{
            string sinprefijo = data.Split(',')[1];
            string base64 = sinprefijo.Replace("\"", "");

            // Convert Base64 string to byte array
            byte[] fileBytes = Convert.FromBase64String(base64);
            
            // Create a MemoryStream from the byte array
            MemoryStream memoryStream = new(fileBytes);
			
				// Create a new FormFile using the MemoryStream, file name, and content type
				IFormFile formFile = new FormFile(memoryStream, 0, memoryStream.Length, "File", fileName)
				{
					Headers = new HeaderDictionary(),
					ContentType = contentType
				};
            formFile.Headers["Content-Disposition"] = $"form-data; name=\"File\"; filename=\"{fileName}\"";
            return formFile;
            
            
        }

        public static string ExtensionArchivo(IFormFile file)
        {
            string exe = Path.GetExtension(file.FileName).ToLower();
			return exe switch
			{
				".pdf" => "pdf",
				".xml" => "xml",
				".png" => "img",
				".jpg" => "img",
				".jpeg" => "img",
				".gif" => "gif",
				_ => "none",
			};

		}

		/// <summary>
		/// Obtiene el flujo de datos de un archivo base64.
		/// </summary>
		/// <param name="data">Archivo en base64</param>
		/// <returns>MemoryStream el archivo.</returns>
		public static MemoryStream ConvertFromBase64(string data)
		{
			string sinprefijo = data.Split(',')[1];
			string base64 = sinprefijo.Replace("\"", "");

			// Convert Base64 string to byte array
			byte[] fileBytes = Convert.FromBase64String(base64);

			// Create a MemoryStream from the byte array
			 MemoryStream memoryStream = new(fileBytes);
			return memoryStream;
		}
		/// <summary>
		/// Valida archivo XML de la ruta indicada.
		/// </summary>
		/// <param name="ruta">Ruta física del archivo XML.</param>
		/// <returns>Modelo de la factura obtenida del XML.</returns>
		public async Task<Factura> ReadXML(string ruta)
		{
			try {
			
				XmlDocument xmlDoc = new XmlDocument();
				string folio = "";
			//	var btsF = File.ReadAllBytes(ruta);
				xmlDoc.Load(ruta);
				var nodo = xmlDoc["cfdi:Comprobante"];
				var receptor = nodo["cfdi:Receptor"];
				var emisor = nodo["cfdi:Emisor"];
				var impuestos = nodo["cfdi:Impuestos"];
				var complemento = nodo["cfdi:Complemento"];
				var timbreFD = complemento["tfd:TimbreFiscalDigital"];

				if (nodo.Attributes["Folio"] != null) 
					folio = nodo.Attributes["Folio"].Value;
				

				Factura factura = new()
				{
					Folio = folio,
					Version = nodo.Attributes["Version"].Value,
					UUID = timbreFD.Attributes["UUID"].Value,
					RFCEmisor = emisor.Attributes["Rfc"].Value,
					Emisor = emisor.Attributes["Nombre"].Value,
					RFCReceptor =receptor.Attributes["Rfc"].Value,
					Receptor = receptor.Attributes["Nombre"].Value,
					UsoCFDI = receptor.Attributes["UsoCFDI"].Value,
					Tipo = nodo.Attributes["TipoDeComprobante"].Value,
					Estatus = nodo.Attributes["Version"].Value,
					Metodo = nodo.Attributes["MetodoPago"].Value,
					FormaPago = nodo.Attributes["FormaPago"].Value,
					Moneda = nodo.Attributes["Moneda"].Value,
					IVA = decimal.Parse(impuestos.Attributes["TotalImpuestosTrasladados"].Value),
					SubTotal = decimal.Parse(nodo.Attributes["SubTotal"].Value),
					Total = decimal.Parse(nodo.Attributes["Total"].Value),
					FechaTimbre = DateTime.Parse(timbreFD.Attributes["FechaTimbrado"].Value),
					Status = true
				};

				if (factura.RFCReceptor != "YIN7808284D0")
				{
					factura.Status = false; factura.Message = "YINSA no es el receptor de esta factura.";
					return factura;
				}

				return await ValidXMLSAT(factura);	

			}
			catch (Exception ex)
			{ 
				logger.LogError(ex.Message);
				return new Factura() { Status = false, Message = ex.Message }; }
		}
		/// <summary>
		/// Valida archivo XML desde Base64.
		/// </summary>
		/// <param name="data">Archivo XML en base64.</param>
		/// <returns>Modelo de la factura obtenida del XML.</returns>
		public async Task<Factura> ReadXMLFromBase64(string data)
		{
			try
			{

				XmlDocument xmlDoc = new XmlDocument();
				string folio = "";
				//	var btsF = File.ReadAllBytes(ruta);
				MemoryStream xml = ConvertFromBase64(data);
				xmlDoc.Load(xml);
				var nodo = xmlDoc["cfdi:Comprobante"];
				var receptor = nodo["cfdi:Receptor"];
				var emisor = nodo["cfdi:Emisor"];
				var impuestos = nodo["cfdi:Impuestos"];
				var complemento = nodo["cfdi:Complemento"];
				var timbreFD = complemento["tfd:TimbreFiscalDigital"];

				if (nodo.Attributes["Folio"] != null) //tronar aquí
					folio = nodo.Attributes["Folio"].Value;


				Factura factura = new()
				{
					Folio = folio,
					Version = nodo.Attributes["Version"].Value,
					UUID = timbreFD.Attributes["UUID"].Value,
					RFCEmisor = emisor.Attributes["Rfc"].Value,
					Emisor = emisor.Attributes["Nombre"].Value,
					RFCReceptor = receptor.Attributes["Rfc"].Value,
					Receptor = receptor.Attributes["Nombre"].Value,
					UsoCFDI = receptor.Attributes["UsoCFDI"].Value,
					Tipo = nodo.Attributes["TipoDeComprobante"].Value,
					Estatus = nodo.Attributes["Version"].Value,
					Metodo = nodo.Attributes["MetodoPago"].Value,
					FormaPago = nodo.Attributes["FormaPago"].Value,
					Moneda = nodo.Attributes["Moneda"].Value,
					IVA = decimal.Parse(impuestos.Attributes["TotalImpuestosTrasladados"].Value),
					SubTotal = decimal.Parse(nodo.Attributes["SubTotal"].Value),
					Total = decimal.Parse(nodo.Attributes["Total"].Value),
					FechaTimbre = DateTime.Parse(timbreFD.Attributes["FechaTimbrado"].Value),
					Status = true
				};

				//if (factura.RFCReceptor != "YIN7808284D0")
				//{
				//	factura.Status = false; factura.Message = "YINSA no es el receptor de esta factura.";
				//	return factura;
				//}


				return await ValidXMLSAT(factura);

			}
			catch (Exception ex) {
				logger.LogError(ex.Message);
				return new Factura() { Status = false, Message = ex.Message }; 
			}
		}
		public  async Task<Factura> ValidXMLSAT(Factura fact)
		{
			try {
				string sTotal = String.Format("{0:0000000000.000000}", fact.Total);

			string datoCBDD = "?id=" + fact.UUID + "&re=" + fact.RFCEmisor + "&rr="
									 + fact.RFCReceptor + "&tt=" + sTotal;

			//string datoCBDD = "?id=" + fact.UUID + "&re=" + fact.RFCEmisor + "&rr="  //prueba de incorrecto
			//						 + fact.RFCEmisor+ "&tt=" + sTotal;
			var consCFDI = new SatFacturaElectronica.ConsultaCFDIServiceClient();

				var acuseCFDI = await consCFDI.ConsultaAsync(datoCBDD); 

			fact.Estatus = acuseCFDI.Estado;
			fact.Message = $"{acuseCFDI.CodigoEstatus} - {acuseCFDI.Estado} - {acuseCFDI.EsCancelable} - Estatus de cancelación: {acuseCFDI.EstatusCancelacion}";
		//	acuseCFDI.EsCancelable; acuseCFDI.EstatusCancelacion; <--- agregar a mensaje
			return fact;
			}
			catch (Exception ex) {
				logger.LogError(ex.Message);
				fact.Message = ex.Message;
				return fact;
			}
		}

		public async Task<Respuesta> SubirXML(List<Factura> facturas,int idDoc)
		{
			string endpoint = _config["endpoint:archivos:subirXML"];
			string resp = ""; 
			foreach (var fac in facturas)
			{
				FacturaJSON factura = new()
				{
					IdDoc = idDoc,
					Folio = fac.Folio,
					Version = fac.Version,
					UUID = fac.UUID,
					RazonSocial = fac.Emisor,
					RFC = fac.RFCEmisor,
					Tipo = fac.Tipo,
					Metodo = fac.Metodo,
					Estatus = fac.Estatus,
					FormaPago = fac.FormaPago,
					IVA = fac.IVA,
					Moneda = fac.Moneda,
					SubTotal = fac.SubTotal,
					Total = fac.Total,
					FechaTimbre = fac.FechaTimbre,
					RFCReceptor = fac.RFCReceptor,
					Receptor = fac.Receptor,
					UsoCFDI = fac.UsoCFDI

				};

				var body = Newtonsoft.Json.JsonConvert.SerializeObject(factura);

				 resp = await api.PostRequest(endpoint, body);
			}
			
			if (resp == "[]" || string.IsNullOrEmpty(resp))
				return new Respuesta{ Status = false, StatusMessage = "Algo ha salido mal." };

			Respuesta respuesta = Newtonsoft.Json.JsonConvert.DeserializeObject<RespuestaArchivos>(resp);

			return respuesta;
			
		}

		public async Task<Respuesta> ActualizarXML(Factura fac)
		{
			FacturaJSON factura = new()
			{
				IdDoc = fac.IdDoc,
				Folio = fac.Folio,
				Version = fac.Version,
				UUID = fac.UUID,
				RazonSocial = fac.Emisor,
				RFC = fac.RFCEmisor,
				Tipo = fac.Tipo,
				Metodo = fac.Metodo,
				Estatus = fac.Estatus,
				FormaPago = fac.FormaPago,
				IVA = fac.IVA,
				Moneda = fac.Moneda,
				SubTotal = fac.SubTotal,
				Total = fac.Total,
				FechaTimbre = fac.FechaTimbre,
				Receptor = fac.Receptor,
				RFCReceptor = fac.RFCReceptor,
				UsoCFDI = fac.UsoCFDI

			};

			string endpoint = _config["endpoint:archivos:updXML"];

			var body = Newtonsoft.Json.JsonConvert.SerializeObject(factura);

			var resp = await api.PostRequest(endpoint, body);


			if (resp == "[]" || string.IsNullOrEmpty(resp))
				return new Respuesta { Status = false, StatusMessage = "Algo ha salido mal." };

			Respuesta respuesta = Newtonsoft.Json.JsonConvert.DeserializeObject<RespuestaArchivos>(resp);

			return respuesta;

		}
		public async Task<RespuestaArchivos> FacturasXML(string idSocio, int estado)
		{
			string e = estado switch
			{
				0 => "Vigente",
				1 => "Cancelado",
				2 => "No Encontrado",
				_ => null
			};
			string endpoint = _config["endpoint:archivos:getXML"];

			var par = new List<KeyValuePair<string, string>>
					{
						//new KeyValuePair<string, string>("cve", "GTXML"),
						new KeyValuePair<string, string>("idSocio", idSocio),
						new KeyValuePair<string, string>("estatus",e)
					};

			var resp = await api.GetParams(endpoint,par);

			RespuestaArchivos r = new();

			if (string.IsNullOrEmpty(resp) || resp == "[]")
			{
				r.Status = false; r.StatusMessage = "Ha ocurrido un problema.";
				return r; }

			r = Newtonsoft.Json.JsonConvert.DeserializeObject<RespuestaArchivos>(resp);
	
			return r;
		}
		public async Task<Respuesta> AutorizarDoc(Autorizar aut)
		{
			

			string endpoint = _config["endpoint:archivos:autorizaciones:autorizar"];

			var body = Newtonsoft.Json.JsonConvert.SerializeObject(aut);

			var resp = await api.PostRequest(endpoint, body);


			if (resp == "[]" || string.IsNullOrEmpty(resp))
				return new Respuesta { Status = false, StatusMessage = "Algo ha salido mal." };

			Respuesta respuesta = Newtonsoft.Json.JsonConvert.DeserializeObject<Respuesta>(resp);

			return respuesta;

		}
		public async Task<Respuesta> AsignarAutorizacion(Autorizar aut)
		{
			string endpoint = _config["endpoint:archivos:autorizaciones:asignar"];

			var body = Newtonsoft.Json.JsonConvert.SerializeObject(aut);

			var resp = await api.PostRequest(endpoint, body);


			if (resp == "[]" || string.IsNullOrEmpty(resp))
				return new Respuesta { Status = false, StatusMessage = "Algo ha salido mal." };

			Respuesta respuesta = Newtonsoft.Json.JsonConvert.DeserializeObject<Respuesta>(resp);

			return respuesta;
		}
		public async Task<List<AutorizacionesDoc>> AutorizacionesDocumento(int idDoc)
		{
			string endpoint = _config["endpoint:archivos:autorizaciones:doc"] + idDoc.ToString();

			var resp = await api.Get(endpoint);

			List<AutorizacionesDoc> lst = new();

			if(resp == "[]" || string.IsNullOrEmpty(resp)) { return lst; }

			lst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AutorizacionesDoc>>(resp);

			return lst;

		}

		public async Task<List<AutorizacionesDoc>> AutorizadoresDocumento(string idSocio)
		{
			string endpoint = _config["endpoint:archivos:autorizaciones:autorizadores"];

			var par = new List<KeyValuePair<string, string>>
					{
						new KeyValuePair<string, string>("idSocio", idSocio)
						
					};

			var resp = await api.GetParams(endpoint, par);

			List<AutorizacionesDoc> lst = new();

			if (resp == "[]" || string.IsNullOrEmpty(resp)) { return lst; }

			lst = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AutorizacionesDoc>>(resp);

			return lst;

		}

		public async Task<(List<ProveedorCount>, int)> AutorizacionProveedores(int estado) //, DateTime f1, DateTime f2
		{
			List<ProveedorCount> lstProv = new();
			ClaimsPrincipal c = context.HttpContext.User;
			string rol = c.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;//c.FindFirstValue("Rol");
			string username = c.FindFirstValue("UserName");

			if (rol == "empleado")
			{
				Usuario us = await usuariosService.UserByName(username);

				foreach (var f in us.filtros)
				{
					var p = await ProveedoresCount(estado, f.Id);

					if (p.Status)
						lstProv = lstProv.Concat(p.ProveedoresArchivos).ToList();
				}

			}
			else
			{
				var p = await ProveedoresCount(estado, 0);

				if (p.Status)
					lstProv = p.ProveedoresArchivos;
			}

			int countPendientes = lstProv.Sum( p => p.Count);//0;

			//foreach (var prov in lstProv) { countPendientes += prov.Count; }

			return (lstProv, countPendientes);
		}
		/*
        public static IFormFile ConvertB64(string data, string fileName, string contentType)
        {
            string sinprefijo = data.Split(',')[1];
            string base64 = sinprefijo.Replace("\"","");

                const int blockSize = 4096;
                byte[] buffer = new byte[blockSize];
                int bytesRead;

                MemoryStream memoryStream = new MemoryStream();

                var stream = new MemoryStream(Convert.FromBase64String(base64));
                    
                        while ((bytesRead = stream.Read(buffer, 0, blockSize)) > 0)
                        {
                            memoryStream.Write(buffer, 0, bytesRead);
                        }
                    

                    IFormFile formFile = new FormFile(memoryStream, 0, memoryStream.Length,"file", fileName)
                    {
                        Headers = new HeaderDictionary(),
                        ContentType =contentType
                    };
            formFile.Headers["Content-Disposition"] = $"form-data; name=\"file\"; filename=\"{fileName}\"";
            return formFile;
            
        }*/
	}
}
