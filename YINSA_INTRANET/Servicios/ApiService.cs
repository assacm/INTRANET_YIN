using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Xml.Linq;
using YINSA_INTRANET.Models;
using System.Net;
namespace YINSA_INTRANET.Servicios
{
	public interface IApiService
	{
		Task<string> Get(string endpoint);
		Task<string> Get(string endpoint, string token);
		Task<string> GetParams(string endpoint, IEnumerable<KeyValuePair<string, string>> parametros);
		Task<string> GetRequest(string token, string endpoint, IEnumerable<KeyValuePair<string, string>> parametros);
        Task<string> PostFile(string endpoint, ArchivoPost formData);
        Task<string> PostRequest(string token, string endpoint, string body);
		Task<string> PostRequest(string endpoint, string body);
	}
	public class ApiService : IApiService
	{
		private readonly IConfiguration config;
        private readonly ILogger<ApiService> logger;
        private readonly string _url;

		public ApiService(IConfiguration configuration, ILogger<ApiService> logger)
		{
			this.config = configuration;
            this.logger = logger;
            _url = config["url"];
		}
		public async Task<string> Get(string endpoint)
		{
			try {
				var url = _url + endpoint;
				HttpClient req = new()
				{
					BaseAddress = new Uri(url)
				};

				req.DefaultRequestHeaders.Accept.Clear();
				req.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

				//req.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

				string datos = "[]";
				var response = await req.GetAsync(url);
				// Stream MyStr = await response.Content.ReadAsStreamAsync();


				if (response.IsSuccessStatusCode)
				{
					datos = await response.Content.ReadAsStringAsync();
				}
				else
				{
					string r = await response.Content.ReadAsStringAsync();
					logger.LogError(r);
				}



				return datos;
			}
			catch (Exception ex) {
				logger.LogError(ex.Message);
				return "[]";
			}
		}
		public async Task<string> Get(string endpoint, string token)
		{
			try { 
				var url = _url + endpoint;
				HttpClient req = new();
				// req.BaseAddress = new Uri(url);

				req.DefaultRequestHeaders.Accept.Clear();
				req.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

				req.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

				string datos = "[]";
				var response = await req.GetAsync(url);
				// Stream MyStr = await response.Content.ReadAsStreamAsync();


				if (response.IsSuccessStatusCode)
				{
					datos = await response.Content.ReadAsStringAsync();
					
				}
				else
				{
					string r = await response.Content.ReadAsStringAsync();
					logger.LogError(r);
				}

				return datos;
			}
			catch (Exception ex) {
				logger.LogError(ex.Message);
				return "[]";
			}
		}
		public async Task<string> GetRequest(string token,string endpoint, IEnumerable<KeyValuePair<string, string>> parametros)
		{
			try { 
				var url = _url + endpoint;
				var newUrl = new Uri(QueryHelpers.AddQueryString(url, parametros));
				HttpClient req = new();

				req.DefaultRequestHeaders.Accept.Clear();
				req.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

				req.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
				string datos = "[]";
				var response = await req.GetAsync(newUrl);

				if (response.IsSuccessStatusCode)
				{
					datos = await response.Content.ReadAsStringAsync();
				}
				else
				{
					string r = await response.Content.ReadAsStringAsync();
					logger.LogError(r);
				}

				return datos;
			}
			catch (Exception ex) {
				logger.LogError(ex.Message);
				return "[]";
			}
		}
		public async Task<string> PostRequest(string token, string endpoint, string body)
		{
			try { 
				var url = _url + endpoint;
				var uri = new Uri(url);
				HttpClient client = new();

				var contentType = "application/json"; // Establece el tipo de contenido del cuerpo de la solicitud según tu necesidad
													  //var requestBody = body;
				var content = new StringContent(body, Encoding.UTF8, contentType); 


				client.DefaultRequestHeaders.Accept.Clear();
				//client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

				client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
				string datos = "[]";
				var response = await client.PostAsync(uri, content); //dinamico
				if (response.IsSuccessStatusCode)
				{
					datos = await response.Content.ReadAsStringAsync();
				}
				else
				{
					string r = await response.Content.ReadAsStringAsync();
					logger.LogError(r);
				}

				return datos;
			}
			catch (Exception ex) {
				logger.LogError(ex.Message);
				return "[]";
			}
		}
		public async Task<string> PostRequest(string endpoint, string body)
		{
			try { 
					var url = _url + endpoint;
					var uri = new Uri(url);
					HttpClient client = new();

					var contentType = "application/json"; // Establece el tipo de contenido del cuerpo de la solicitud según tu necesidad
					//var requestBody = body;
					var content = new StringContent(body, Encoding.UTF8, contentType); //dinamico


					client.DefaultRequestHeaders.Accept.Clear();
					//client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

					//client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
					string datos = "[]";
					var response = await client.PostAsync(uri, content); //dinamico
					if (response.IsSuccessStatusCode)
					{
						datos = await response.Content.ReadAsStringAsync();
					}
					else
					{
						string r = await response.Content.ReadAsStringAsync();
						logger.LogError(r);
					}
				
					return datos;
			}
			catch (Exception ex) {
				logger.LogError(ex.Message);
				return "[]";
			}
		}
        public async Task<string> PostFile(string endpoint, ArchivoPost formData)
        {
            try
            {
                var url = _url + endpoint;
                var uri = new Uri(url);
                HttpClient client = new();
                client.DefaultRequestHeaders.Accept.Clear();

                var multipartFormContent = new MultipartFormDataContent();
				multipartFormContent.Add(new StringContent(formData.IdSocio), name: "IdSocio");
				multipartFormContent.Add(new StringContent(formData.IdUser.ToString()), name: "IdUser");
				multipartFormContent.Add(new StringContent(formData.IdUser.ToString()), name: "IdUserUpd");
				multipartFormContent.Add(new StringContent(formData.ValidXML.ToString()), name: "ValidXML");
				multipartFormContent.Add(new StringContent(formData.ValidSAT.ToString()), name: "ValidSAT");
				multipartFormContent.Add(new StringContent(formData.SAP.ToString()), name: "SAP");
				multipartFormContent.Add(new StringContent(formData.Contabilidad.ToString()), name: "Contabilidad");
				multipartFormContent.Add(new StringContent(formData.DocEntry.ToString()), name: "DocEntry");
				multipartFormContent.Add(new StringContent(formData.DocNum.ToString()), name: "DocNum");
				multipartFormContent.Add(new StringContent(formData.Descripcion), name: "Descripcion");
                multipartFormContent.Add(new StringContent(formData.TipoDoc), name: "TipoDoc");
                multipartFormContent.Add(new StringContent(formData.TipoObjSAP.ToString()), name: "TipoObjSAP");
                multipartFormContent.Add(new StringContent(formData.Concepto), name: "Concepto");
				//multipartFormContent.Add(new StringContent(formData.Comentario), name: "Comentario");


				var fileStreamContent = new StreamContent(formData.File.OpenReadStream());
                fileStreamContent.Headers.ContentType = new MediaTypeHeaderValue(formData.File.ContentType);


                multipartFormContent.Add(fileStreamContent, name: "File", fileName: formData.File.FileName);



                string datos = "[]";
                var response = await client.PostAsync(uri, multipartFormContent);
                if (response.IsSuccessStatusCode)
                {
                    datos = await response.Content.ReadAsStringAsync();

                }
				else
				{
					string r = await response.Content.ReadAsStringAsync();
					logger.LogError(r);
				}

				return datos;
            }
            catch (Exception ex)
            {
				logger.LogError(ex.Message);
				return "[]";
			}

        }
       
        public async Task<string> GetParams(string endpoint,IEnumerable<KeyValuePair<string,string>> parametros)
		{
			try {
				var url = _url + endpoint;

				//var param = QueryString.Create(parametros);

				//var param = new Dictionary<string, string>() { { "CIKey", "123456789" } };

				var newUrl = new Uri(QueryHelpers.AddQueryString(url, parametros));

				HttpClient req = new();
				// req.BaseAddress = new Uri(url);

				req.DefaultRequestHeaders.Accept.Clear();
				req.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

				//req.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

				string datos = "[]";
				var response = await req.GetAsync(newUrl);
				// Stream MyStr = await response.Content.ReadAsStreamAsync();


				if (response.IsSuccessStatusCode)
				{
					datos = await response.Content.ReadAsStringAsync();

				}
				else
				{
					string r = await response.Content.ReadAsStringAsync();
					logger.LogError(r);
				}

				return datos;
			}
			catch (Exception ex) {
				logger.LogError(ex.Message);
				return "[]";
			}
		}

		//public async Task<string> PostLogin(string endpoint,LoginViewModel body)
		//{
		//	var url = _url + endpoint;
		//	HttpClient client = new();

		//	client.DefaultRequestHeaders.Accept.Clear();
		//	client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

		//	string datos = "[]";
		//	var response = await client.PostAsJsonAsync<LoginViewModel>(url,body);
		//	if (response.IsSuccessStatusCode)
		//	{
		//		datos = await response.Content.ReadAsStringAsync();

		//	}

		//	return datos;
		//}
	}
}
