using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Globalization;
using YINSA_INTRANET.Models;

namespace YINSA_INTRANET.Servicios
{
	public interface IReportesService
	{
		Task<List<SelectListItem>> GetDepartamentos();
		Task<List<SelectListItem>> GetGrupoCli();
		Task<List<SelectListItem>> GetGrupoProv();
		Task<List<SelectListItem>> GetPermisos();
		Task<List<SelectListItem>> GetPuestos();
		(DateTime fechaInicio, DateTime fechaFin) RangoFechas(int mes, int año);
        (DateTime fechaInicio, DateTime fechaFin) RangoPeriodo(int periodo);
        List<SelectListItem> SelectPeriodo();
    }
	public class ReportesService: IReportesService
	{
		private readonly IConfiguration _config;
		private readonly IApiService _api;
		private readonly IUsuariosService usuariosService;

		public ReportesService(IConfiguration configuration,
			IApiService apiService, IUsuariosService usuariosService)
        {
			this._config = configuration;
			this._api = apiService;
			this.usuariosService = usuariosService;
		}

		public (DateTime fechaInicio, DateTime fechaFin) RangoFechas(int mes, int year)
		{
			DateTime fechaInicio, fechaFin;

			if (mes <= 0 || mes > 12 || year <= 1990)
			{
				//var hoy = DateTime.Today;
				fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
			}
			else
			{
				fechaInicio = new DateTime(year, mes, 1);
			}

			fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

			return (fechaInicio, fechaFin);
		}
		public (DateTime fechaInicio,DateTime fechaFin) RangoPeriodo(int periodo)
		{
            DateTime fechaInicio, fechaFin;

			switch (periodo)
			{
				default: {
                        fechaInicio = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                        fechaFin = fechaInicio.AddMonths(1).AddDays(-1);
                        break; 
					}
				case 3: { 
						fechaInicio= new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
						fechaFin= fechaInicio.AddMonths(-3).AddDays(-1);
                        break; }
				case > 2000: { 
							fechaInicio= new DateTime(periodo,1, 1);
							fechaFin= fechaInicio.AddYears(1).AddDays(-1);
						break; }
			}
            return (fechaInicio, fechaFin);

        }
        public List<SelectListItem> SelectPeriodo()
        {
			var list = new List<SelectListItem>
			{
				new SelectListItem { Value = "1", Text = "Último mes" },
				new SelectListItem { Value = "3", Text = "Últimos 3 meses" }
			};

			// Agregar opciones para los últimos años
			int currentYear = DateTime.Now.Year;
            for (int i = currentYear; i >= currentYear - 5; i--)
            {
                list.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
            }

            return list;
        }

		public static List<SelectListItem> SelectYear()
		{
			var list = new List<SelectListItem>();
			int currentYear = DateTime.Now.Year;
			for (int i = currentYear; i >= currentYear - 5; i--)
			{
				list.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
			}
			return list;
		}
		public static List<SelectListItem> SelectMonth(int year)
		{
			var list = new List<SelectListItem>();

			if (year == DateTime.Now.Year)
			{
			
				int currentMonth = DateTime.Now.Month;
				for (int i = 1; i <= currentMonth; i++)
				{
					string mes= DateTimeFormatInfo.CurrentInfo.GetMonthName(i);
					list.Add(new SelectListItem { Value = i.ToString(), Text = mes });

				}
				return list;
			}

			for (int i = 1; i >= 12; i++)
			{
				list.Add(new SelectListItem { Value = i.ToString(), Text = i.ToString() });
			}
			return list;
		}

		public async Task<List<SelectListItem>> GetPermisos()
		{
			List<Item> lst = await usuariosService.GetPermisos();

			List<SelectListItem> select = new();

			if ( lst == null || lst.Count < 1) { return select; }

			foreach(var li in lst)
			{
				select.Add(new SelectListItem { Value = li.Id.ToString(), Text = li.Nombre });
			}

			return select;
		}

		public async Task<List<SelectListItem>> GetDepartamentos()
		{
			string endpoint = _config["endpoint:empleados:departamentos"];
			string resp = await _api.Get(endpoint);

			List<Item> lst = new();
			List<SelectListItem> select = new();

			if (string.IsNullOrEmpty(resp) || resp == "[]") { return select; }

			var r = JsonConvert.DeserializeObject<RespuestaItem>(resp);

			if (r.StatusCode == 200)
				lst = r.Response;

			foreach (var li in lst)
			{
				select.Add(new SelectListItem { Value = li.Id.ToString(), Text = li.Nombre });
			}

			return select;
		}

		public async Task<List<SelectListItem>> GetPuestos()
		{
			string endpoint = _config["endpoint:empleados:puestos"];
			string resp = await _api.Get(endpoint);

			List<Item> lst = new();
			List<SelectListItem> select = new();

			if (string.IsNullOrEmpty(resp) || resp == "[]") { return select; }

			var r = JsonConvert.DeserializeObject<RespuestaItem>(resp);

			if (r.StatusCode == 200)
				lst = r.Response;

			foreach (var li in lst)
			{
				select.Add(new SelectListItem { Value = li.Id.ToString(), Text = li.Nombre });
			}

			return select;
		}

		public async Task<List<SelectListItem>> GetGrupoProv()
		{
			string endpoint = _config["endpoint:usuario:grupoprov"];
			string resp = await _api.Get(endpoint);

			List<Item> lst = new();
			List<SelectListItem> select = new();

			if (string.IsNullOrEmpty(resp) || resp == "[]") { return select; }

			var r = JsonConvert.DeserializeObject<RespuestaItem>(resp);

			if (r.StatusCode == 200)
				lst = r.Response;

			foreach (var li in lst)
			{
				select.Add(new SelectListItem { Value = li.Id.ToString(), Text = li.Nombre });
			}

			return select;
		}

		public async Task<List<SelectListItem>> GetGrupoCli()
		{
			string endpoint = _config["endpoint:usuario:grupoprov"];
			string resp = await _api.Get(endpoint);

			List<Item> lst = new();
			List<SelectListItem> select = new();

			if (string.IsNullOrEmpty(resp) || resp == "[]") { return select; }

			var r = JsonConvert.DeserializeObject<RespuestaItem>(resp);

			if (r.StatusCode == 200)
				lst = r.Response;

			foreach (var li in lst)
			{
				select.Add(new SelectListItem { Value = li.Id.ToString(), Text = li.Nombre });
			}

			return select;
		}
	}
}
