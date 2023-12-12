using Newtonsoft.Json;
using YINSA_INTRANET.Models;

namespace YINSA_INTRANET.Servicios
{
	public interface IEmpleadosService
	{
		Task<Respuesta> Editar(Empleado model);

		/// <summary>
		/// Obtiene un empleado según el idCuenta. 
		/// </summary>
		/// <param name="id">Identificador de cuenta del empleado.</param>
		/// <returns>Datos de empleado especificado.</returns>
		Task<Empleado> Empleado(string id);
		/// <summary>
		/// Obtiene lista de empleados. 
		/// </summary>
		/// <param name="departamento">Indica el departamento del cual se quieren filtrar sus empleados. Opcional.</param>
		/// <returns>Lista de Empleados.</returns>
		Task<List<Empleado>> Empleados(int departamento);
		Task<Respuesta> Registro(Empleado model);
	}
	public class EmpleadosService: IEmpleadosService
	{
		private readonly IConfiguration _config;
		private readonly IApiService _api;

		public EmpleadosService(IConfiguration configuration, 
			IApiService apiService)
		{
			this._config = configuration;
			this._api = apiService;
		}

		public async Task<Respuesta> Registro(Empleado model)
		{
			var body = Newtonsoft.Json.JsonConvert.SerializeObject(model);
			string endpoint = _config["endpoint:empleados:registro"];
			var resp = await _api.PostRequest(endpoint, body);

			Respuesta emp = new Respuesta();

			if (resp != "[]" || !string.IsNullOrEmpty(resp))
			{
				emp = JsonConvert.DeserializeObject<Respuesta>(resp);
			}

			return emp;
		}
		public async Task<Respuesta> Editar(Empleado model)
		{
			var body = Newtonsoft.Json.JsonConvert.SerializeObject(model);
			string endpoint = _config["endpoint:empleados:editar"];
			var resp = await _api.PostRequest(endpoint, body);

			Respuesta emp = new Respuesta();

			if (resp != "[]" || !string.IsNullOrEmpty(resp))
			{
				emp = JsonConvert.DeserializeObject<Respuesta>(resp);
			}

			return emp;
		}
		public async Task<List<Empleado>> Empleados(int departamento)
		{
			//var body = Newtonsoft.Json.JsonConvert.SerializeObject(model);
			/*
			 *  Usar cuando existan departamentos y se necesite filtrar
			 * string endpoint = _config["endpoint:empleados:all"];
			IEnumerable<KeyValuePair<string, string>> parametros =
				new List<KeyValuePair<string, string>>
					{
						new KeyValuePair<string, string>("idDepartamento", departamento) };

			var resp = await _api.GetParams(endpoint, parametros);
			*/

			string endpoint = _config["endpoint:empleados:all"];

			var resp = await _api.Get(endpoint);

			List<Empleado> empleado = new();

			if (resp == "[]" || string.IsNullOrEmpty(resp))
			{
				return empleado;

			}

			var r = JsonConvert.DeserializeObject<RespuestaEmpleado>(resp);

			if (r.StatusCode == 200)
			{
				empleado = r.Empleados;
			}

			return empleado;
		}

		public async Task<Empleado> Empleado(string id)
		{
			//var body = Newtonsoft.Json.JsonConvert.SerializeObject(model);

			
			 string endpoint = _config["endpoint:empleados:id"] + id;
		
			var resp = await _api.Get(endpoint);

			Empleado empleado = new Empleado();

			if (resp == "[]" || string.IsNullOrEmpty(resp))
			{
				return empleado;

			}

			var r = JsonConvert.DeserializeObject<RespuestaEmpleado>(resp);

			if (r.StatusCode == 200)
			{
				empleado = r.Empleado;//JsonConvert.DeserializeObject<Empleado>(resp.Empleado);
			}

			return empleado;
		}
	}
}
