using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using System.Reflection;
using YINSA_INTRANET.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace YINSA_INTRANET.Servicios
{
	public interface IUsuariosService
	{
		Task<Respuesta> Actualizar(Usuario model);
		Task<Respuesta> AsignarCuenta(CuentaAsignacion cuentas);
		/// <summary>
		/// Asigna los permisos del usuario.
		/// </summary>
		/// <param name="permisos">Lista con permisos y acciones permitidas.</param>
		/// <returns>Respuesta de estado.</returns>
		Task<Respuesta> AsignarPermisos(PermisoAsignacion permisos);
		Task<string> BuscarCuenta(string cuenta);
		/// <summary>
		/// Asigna los grupos de cuentas para autorizar a un usuario.
		/// </summary>
		/// <param name="filtros">Contiene las asignaciones correspondientes a un usuario.</param>
		/// <returns>Respuesta con estado.</returns>
		Task<Respuesta> FiltrosAutorizacion(FiltrosAsignacion filtros);
		/// <summary>
		/// Obtiene lista de permisos registrados en la base de datos.
		/// </summary>
		/// <returns>Lista genérica con los permisos.</returns>
		Task<List<Item>> GetPermisos();

		/// <summary>
		/// Obtiene el identificador del socio o colaborador que ha iniciado sesión.
		/// </summary>
		/// <returns>Identificador tipo string</returns>
		string GetSocioId();
		/// <summary>
		///  Obtiene el identificador del usuario que ha iniciado sesión.
		/// </summary>
		/// <returns>Identificador tipo int</returns>
		Task<int> GetUserId();
		/// <summary>
		/// Obitene lista de usuarios.
		/// </summary>
		/// <returns>Lista de usuarios.</returns>
		Task<List<Usuario>> GetUsers();

		Task<string> Login(LoginViewModel model);
		Task<string> Registro(RegistroUser model);
		/// <summary>
		/// Obtiene los datos del usuarios según el identificador.
		/// </summary>
		/// <param name="id">Identificador numérico del usuario.</param>
		/// <returns>Instancia de usuario.</returns>
		Task<Usuario> UserById(int id);
		/// <summary>
		/// Obtiene los datos del usuario según su nombre y su rol.
		/// </summary>
		/// <param name="name">Nombre del usuario.</param>
		/// <returns>Instancia de usuario</returns>
		Task<Usuario> UserByName(string name);
        bool VerificacionCuenta(string codigoS);
        //Task<string> Login(LoginViewModel body);
    }
	public class UsuariosService :IUsuariosService
	{
		private readonly IConfiguration _config;
        private readonly ILogger<UsuariosService> logger;
        private readonly IApiService _api;
        private readonly IHttpContextAccessor context;

        public UsuariosService(IConfiguration configuration,ILogger<UsuariosService> logger, 
			IApiService apiService, IHttpContextAccessor context)
		{
			this._config = configuration;
            this.logger = logger;
            this._api = apiService;
            this.context = context;
        }
		public async Task<string> Registro(RegistroUser model)
		{
			var body = Newtonsoft.Json.JsonConvert.SerializeObject(model);
			string endpoint = _config["endpoint:usuario:registro"];
			var resp = await _api.PostRequest(endpoint, body);
			return resp;
		}
		public async Task<string> Login(LoginViewModel model)
        {
            var body = Newtonsoft.Json.JsonConvert.SerializeObject(model);
			string endpoint = _config["endpoint:usuario:login"];
			var resp = await _api.PostRequest(endpoint, body);

			return resp;
		}
		//public async Task<string> Login(LoginViewModel body)
		//{  
		//	string endpoint = _config["endpoint:usuario:login"];
		//	var resp = await _api.PostLogin(endpoint, body);
		//	return resp;
		//}
		public async Task<Respuesta> Actualizar(Usuario model)
		{
			var body = Newtonsoft.Json.JsonConvert.SerializeObject(model);
			string endpoint = _config["endpoint:usuario:actualizar"];
			var resp = await _api.PostRequest(endpoint, body);

			Respuesta r = new();
			if (string.IsNullOrEmpty(resp) || resp == "[]")
			{
				return r;
			}

			r = Newtonsoft.Json.JsonConvert.DeserializeObject<Respuesta>(resp);

			return r;
		}
		public async Task<Respuesta> AsignarCuenta(CuentaAsignacion cuentas)
		{
			var body = Newtonsoft.Json.JsonConvert.SerializeObject(cuentas);
			string endpoint = _config["endpoint:usuario:cuentas:asignar"];
			var resp = await _api.PostRequest(endpoint, body);

			Respuesta r = new Respuesta();

			if (string.IsNullOrEmpty(resp) || resp == "[]") { return r; }

			r = Newtonsoft.Json.JsonConvert.DeserializeObject<Respuesta>(resp);

			return r;

		}
		public async Task<Respuesta> AsignarPermisos(PermisoAsignacion permisos)
		{
			var body = Newtonsoft.Json.JsonConvert.SerializeObject(permisos);
			string endpoint = _config["endpoint:usuario:permisos:asignar"];
			var resp = await _api.PostRequest(endpoint, body);

			Respuesta r = new Respuesta();

			if (string.IsNullOrEmpty(resp) || resp == "[]") { return r; }

			r = Newtonsoft.Json.JsonConvert.DeserializeObject<Respuesta>(resp);

			return r;
		}
		public async Task<Respuesta> FiltrosAutorizacion(FiltrosAsignacion filtros)
		{	

			var body = Newtonsoft.Json.JsonConvert.SerializeObject(filtros);
			string endpoint = _config["endpoint:usuario:autorizaciones:asignar"];
			var resp = await _api.PostRequest(endpoint, body);
			Respuesta r = new Respuesta();

			if (string.IsNullOrEmpty(resp) || resp == "[]") { return r; }

			r = Newtonsoft.Json.JsonConvert.DeserializeObject<Respuesta>(resp);

			return r;
		}
		public async Task<string> BuscarCuenta(string cuenta)
		{
			string endpoint = _config["endpoint:usuario:cuentas:buscar"];

			IEnumerable<KeyValuePair<string, string>> parametros =
				new List<KeyValuePair<string, string>>
					{
						new KeyValuePair<string, string>("cuenta", cuenta) };

			var resp = await _api.GetParams(endpoint, parametros);

			return resp;
		}
		/// <summary>
		/// Verifica que el usuario no ingrese cuentas que no le pertenecen en la URL.
		/// </summary>
		/// <param name="codigoS">Identificador de la cuenta.</param>
		/// <returns>Bool que valida si la cuenta le pertenece al usuario.</returns>
		public bool VerificacionCuenta(string codigoS)
        {
            
            var session = context.HttpContext.Session;
			string list;
			List<Cuenta> cuentas = new();
           
                list = session.GetString("lstCuentas");
            if (list != null)
                cuentas = JsonConvert.DeserializeObject<List<Cuenta>>(list);
            else
				return false;

			return cuentas.Any(c => c.codigoS == codigoS);
		
		}
		public async Task<Usuario> UserByName(string name)
        {
            string endpoint = _config["endpoint:usuario:nombre"];

			IEnumerable<KeyValuePair<string, string>> parametros =
				new List<KeyValuePair<string, string>>
					{
						new KeyValuePair<string, string>("username", name),
					};

			var resp = await _api.GetParams(endpoint, parametros);

			if (string.IsNullOrEmpty(resp) || resp == "[]") { return new Usuario(); }

				var usuario = Newtonsoft.Json.JsonConvert.DeserializeObject<Usuario>(resp);
			return usuario;
		}
		public async Task<Usuario> UserById(int id)
        {
           
            string endpoint = _config["endpoint:usuario:id"];

			IEnumerable<KeyValuePair<string, string>> parametros =
				new List<KeyValuePair<string, string>>
					{
						new KeyValuePair<string, string>("id", id.ToString()) };

			var resp = await _api.GetParams(endpoint, parametros);

			if (string.IsNullOrEmpty(resp) || resp == "[]") { return new Usuario(); }

				var usuario = Newtonsoft.Json.JsonConvert.DeserializeObject<Usuario>(resp);
            return usuario;
		}
		
		public string GetSocioId()
        {
            string id = context.HttpContext.Session.GetString("codigoS"); 
            return id;
		}

		public async Task<int> GetUserId()
		{		
			int id = 0;
			string name = "";
			ClaimsPrincipal c = context.HttpContext.User;
			if (c.Identity != null) {
				if (!c.Identity.IsAuthenticated)
				{
					return id;
				}
				name = c.FindFirstValue("UserName"); 
			}
				

			string endpoint = _config["endpoint:usuario:nombre"];

			IEnumerable<KeyValuePair<string, string>> parametros =
				new List<KeyValuePair<string, string>>
				{
						new KeyValuePair<string, string>("username", name) };

			
			var resp = await _api.GetParams(endpoint, parametros);

			if (resp == "[]" || string.IsNullOrEmpty(resp))
			{
				return id;
			}

			var user = Newtonsoft.Json.JsonConvert.DeserializeObject<Usuario>(resp);
			id = user.userId;

			return id;
		}

		public async Task<List<Usuario>> GetUsers()
		{
			string endpoint = _config["endpoint:usuario:usuarios"];
			string resp = await _api.Get(endpoint);


			if (string.IsNullOrEmpty(resp) || resp == "[]") { return new List<Usuario>(); }

			var users = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Usuario>>(resp);

			return users;
		}

		public async Task<List<Item>> GetPermisos()
		{
			string endpoint = _config["endpoint:usuario:permisos:all"];
			string resp = await _api.Get(endpoint);

			List<Item> lst = new();
			

			if (string.IsNullOrEmpty(resp) || resp == "[]") { return lst; }

			var r = JsonConvert.DeserializeObject<RespuestaItem>(resp);

			if (r.StatusCode == 200)
				lst = r.Response;

		

			return lst;
		}
		/// <summary>
		/// Validador de contraseña.
		/// </summary>
		/// <param name="password">Contraseña a validar.</param>
		/// <returns>Estado bool.</returns>
		public static bool ValidPass(string password)
		{

			if (string.IsNullOrEmpty(password))
			{
				return false;
			}
			// Longitud mínima de 8 caracteres
			if (password.Length < 6)
			{
				return false;
			}
			// Contiene al menos una letra mayúscula
			if (!password.Any(char.IsUpper))
			{
				return false;
			}
			// Contiene al menos una letra minúscula	
			if (!password.Any(char.IsLower))
			{
				return false;
			}
			// Contiene al menos un dígito (número)
			if (!password.Any(char.IsDigit))
			{
				return false;
			}

			// Contiene al menos un carácter especial (por ejemplo, !@#$%^&*)
			//if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
			//{
			//	return false;
			//}

			return true;
		}

		//public async Task<bool> ExisteUsuario(string username)
		//{
		//	var resp = await UserByName(username);

		//	if (!string.IsNullOrEmpty(resp.userName))
		//	{
		//		return true;
		//	}

		//	return false;
		//}
		//public static (bool,string) ValidPassword(string password)
		//{

		//	if (string.IsNullOrEmpty(password))
		//	{
		//		return (false,"Contraseña requerida");
		//	}
		//	// Longitud mínima de 8 caracteres
		//	if (password.Length < 8)
		//	{
		//		return (false, "Debe contener al menos 8 caracteres");
		//	}
		//	// Contiene al menos una letra mayúscula
		//	if (!password.Any(char.IsUpper))
		//	{
		//		return (false, "Debe contener letras mayúsculas");
		//	}
		//	// Contiene al menos una letra minúscula	
		//	if (!password.Any(char.IsLower))
		//	{
		//		return (false, "Debe contener letras minúsculas");
		//	}
		//	// Contiene al menos un dígito (número)
		//	if (!password.Any(char.IsDigit))
		//	{
		//		return (false, "Debe contener números");
		//	}

		//	// Contiene al menos un carácter especial (por ejemplo, !@#$%^&*)
		//	if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
		//	{
		//		return (false, "Debe contener un caracter especial. Ej. !@#$%^&* ");
		//	}

		//	return (true,"true");
		//}

	}
}
