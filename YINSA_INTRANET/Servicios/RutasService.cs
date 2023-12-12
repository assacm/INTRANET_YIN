namespace YINSA_INTRANET.Servicios
{
	public interface IRutasService
	{
		Task<string> ArchivosIntranet();
		Task<string> ArchivosSAP();
	}
	public class RutasService : IRutasService
	{
		private readonly IConfiguration config;
		private readonly IApiService api;

		public RutasService(IConfiguration config, IApiService api)
        {
			this.config = config;
			this.api = api;
		}
        public async Task<string> ArchivosIntranet()
		{
			string endpoint = config["endpoint:rutas:archivosIntranet"];

			var res = await api.Get(endpoint);

			if (string.IsNullOrEmpty(res)) { res = ""; }
			
			return res;
		
		}

		public Task<string> ArchivosSAP()
		{
			throw new NotImplementedException();
		}
	}
}
