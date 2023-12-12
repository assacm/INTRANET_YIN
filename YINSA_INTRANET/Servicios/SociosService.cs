namespace YINSA_INTRANET.Servicios
{
	public interface ISociosService
	{
		Task<string> Socio(string id);
	}
	public class SociosService:ISociosService
	{
		private readonly IConfiguration _config;
		private readonly IApiService _api;

		public SociosService(IConfiguration configuration, IApiService api) {
			this._config = configuration;
			this._api = api;
		}

		public async Task<string> Socio(string id)
		{
			IEnumerable<KeyValuePair<string, string>> param =
			new List<KeyValuePair<string, string>>
				{
						new KeyValuePair<string, string>("id", id)
				};
			string endpoint = _config["endpoint:socios:socio"];

			var resp = await _api.GetParams(endpoint, param);
			return resp;

		}
	}
}
