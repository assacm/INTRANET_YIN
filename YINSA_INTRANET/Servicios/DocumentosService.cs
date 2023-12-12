
using Microsoft.Extensions.Logging;
using YINSA_INTRANET.Models;

namespace YINSA_INTRANET.Servicios
{
	public interface IDocumentosService
	{  
		/// <summary>
		/// Obtiene una orden de compra según su DocEntry. (creo, capaz y es de la tabla de documentos de la intranet)
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		Task<string> OrdenCompra(string id);
		/// <summary>
		/// Obtiene las 5 primeras compras pendientes de subida de archivo de factura.
		/// NOTA: Actualmente se basa en estado pendiente, debería basarse en si tiene archivos adjuntos.
		/// </summary>
		/// <param name="id">Identificador del proveedor.</param>
		/// <returns>Lista de modelo Documento con referencia a las compras del proveedor indicado.</returns>
		Task<string> ComprasTop5(string id);
		/// <summary>
		/// Obtiene facturas de clientes.
		/// </summary>
		/// <param name="id">Identificador del cliente, sin es null obtiene todos los clientes.</param>
		/// <param name="estado">Identifica el estado de las facturas: 0 = pendientes, 1:finalizadas, 2:canceladas.</param>
		/// <param name="inicio">Fecha inicial del periodo de obtención de facturas.</param>
		/// <param name="fin">Fecha final del periodo de obtención de facturas.</param>
		/// <returns>Lista de facturas.</returns>
		Task<List<Documento>> FacturasCliente(string id, int estado, DateTime inicio, DateTime fin);
		/// <summary>
		/// Obtiene facturas de proveedores.
		/// </summary>
		/// <param name="id">Identificador del cliente, sin es null obtiene todos los clientes.</param>
		/// <param name="estado">Identifica el estado de las facturas: 0 = pendientes, 1:finalizadas, 2:canceladas.</param>
		/// <param name="inicio">Fecha inicial del periodo de obtención de facturas.</param>
		/// <param name="fin">Fecha final del periodo de obtención de facturas.</param>
		/// <returns>Lista de facturas.</returns>
		Task<List<Documento>> FacturasProveedor(string id, int estado, DateTime inicio, DateTime fin);
		Task<List<Documento>> EdoCuentaCliente(string id, DateTime inicio, DateTime fin);
		Task<List<Documento>> EdoCuentaProveedor(string id, DateTime inicio, DateTime fin);
		Task<List<Documento>> FacturacionCliente(string id, DateTime inicio, DateTime fin);
		Task<List<Documento>> FacturacionProveedor(string id, DateTime inicio, DateTime fin);
		Task<List<Documento>> Pedidos(string id, int estado, DateTime inicio, DateTime fin);
		Task<List<Documento>> Compras(string id, int estado, DateTime inicio, DateTime fin);
		Task<List<Documento>> NotasCreditoCliente(string id, int estado, DateTime inicio, DateTime fin);
		Task<List<Documento>> NotasCreditoProveedor(string id, int estado, DateTime inicio, DateTime fin);
		/// <summary>
		/// Obtiene los compras del socio indicado según el estado y el rango de fechas. Versión 2 utiliza una diferente estructura, agrupando los productos.
		/// </summary>
		/// <param name="id">Identificador del proveedor</param>
		/// <param name="estado">Estado del pedido</param>
		/// <param name="inicio">Fecha inicial</param>
		/// <param name="fin">Fecha final</param>
		/// <returns>Lista de compras</returns>
		Task<List<DocumentoV2>> ComprasV2(string id, int estado, DateTime inicio, DateTime fin);
		/// <summary>
		/// Obtiene las compras del socio indicado según el estado y el rango de fechas. Versión 2 utiliza una diferente estructura, agrupando los productos. Se incluye la recepçión de la compra.
		/// </summary>
		/// <param name="id">Identificador del cliente</param>
		/// <param name="estado">Estado del pedido</param>
		/// <param name="inicio">Fecha inicial</param>
		/// <param name="fin">Fecha final</param>
		/// <returns>Lista de pedidos</returns>
		Task<List<DocumentoV2>> PedidosV2(string id, int estado, DateTime inicio, DateTime fin);
		/*
Task<string> ComprasCanceladas(IEnumerable<KeyValuePair<string, string>> parametros);
Task<string> ComprasFinalizadas(IEnumerable<KeyValuePair<string, string>> parametros);
Task<string> ComprasPendientes(IEnumerable<KeyValuePair<string, string>> parametros);*/
		/*

			Task<string> FactCanClient(IEnumerable<KeyValuePair<string, string>> parametros);
			Task<string> FactCanProveedor(IEnumerable<KeyValuePair<string, string>> parametros);
			Task<string> FactFinClient(IEnumerable<KeyValuePair<string, string>> parametros);
			Task<string> FactFinProveedor(IEnumerable<KeyValuePair<string, string>> parametros);
			Task<string> FactPenClient(IEnumerable<KeyValuePair<string, string>> parametros);
			Task<string> FactPenProveedor(IEnumerable<KeyValuePair<string, string>> parametros);*/
		/*
			Task<string> NTCRCanClient(IEnumerable<KeyValuePair<string, string>> parametros);
			Task<string> NTCRCanProveedor(IEnumerable<KeyValuePair<string, string>> parametros);
			Task<string> NTCRFinClient(IEnumerable<KeyValuePair<string, string>> parametros);
			Task<string> NTCRFinProveedor(IEnumerable<KeyValuePair<string, string>> parametros);
			Task<string> NTCRPenClient(IEnumerable<KeyValuePair<string, string>> parametros);
			Task<string> NTCRPenProveedor(IEnumerable<KeyValuePair<string, string>> parametros);*/
		/*
			Task<string> PedidosCancelados(IEnumerable<KeyValuePair<string, string>> parametros);
			Task<string> PedidosFinalizados(IEnumerable<KeyValuePair<string, string>> parametros);
			Task<string> PedidosPendientes(IEnumerable<KeyValuePair<string, string>> parametros);*/
	}
	public class DocumentosService : IDocumentosService
	{
		private readonly IConfiguration _config;
        private readonly ILogger<DocumentosService> logger;
        private readonly IApiService _api;

		public DocumentosService(IConfiguration configuration, ILogger<DocumentosService> logger, IApiService api)
		{
			_config = configuration;
            this.logger = logger;
            _api = api;
		}

		public async Task<List<Documento>> FacturasProveedor(string id, int estado,DateTime inicio, DateTime fin)
		{
			string e = estado switch
			{
				0 => "PEN",
				1 => "FIN",
				2 => "CAN",
				_ => "PEN"
			};

			var param = new List<KeyValuePair<string, string>>
					{	
						new KeyValuePair<string, string>("id",id),
                        new KeyValuePair<string, string>("estatus", e),
                        new KeyValuePair<string, string>("inicio", inicio.ToString("yyyy-MM-dd")),
						new KeyValuePair<string, string>("fin", fin.ToString("yyyy-MM-dd"))
					};

			//string endpoint = _config["endpoint:facturas:proveedor:prov"];
			string endpoint = _config["endpoint:facturas:proveedor"];
			var resp = await _api.GetParams(endpoint, param);
			if (resp == "[]" || string.IsNullOrEmpty(resp)) //return new RespuestaDocumentos() { Status = false, StatusCode = 400, StatusMessage = "Sin facturas." };
			{ return new List<Documento>(); }

			var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Documento>>(resp);

			return list;
				
				/*new RespuestaDocumentos()
			{
				Status = true,
				StatusMessage = "Correcto",
				StatusCode = 200,
				Documentos = list
			};*/
		}
		public async Task<List<Documento>> FacturasCliente(string id, int estado, DateTime inicio, DateTime fin)
		{
			string e = estado switch
			{
				0 => "PEN",
				1 => "FIN",
				2 => "CAN",
				_ => "PEN"
			};
			var param = new List<KeyValuePair<string, string>>
					{
						new KeyValuePair<string, string>("id",id),
                        new KeyValuePair<string, string>("estatus", e),
                        new KeyValuePair<string, string>("inicio", inicio.ToString("yyyy-MM-dd")),
						new KeyValuePair<string, string>("fin", fin.ToString("yyyy-MM-dd"))
					};

			//string endpoint = _config["endpoint:facturas:cliente:client"];
			string endpoint = _config["endpoint:facturas:cliente"];
			var resp = await _api.GetParams(endpoint, param);

			if (resp == "[]" || string.IsNullOrEmpty(resp)) //return new RespuestaDocumentos() { Status = false, StatusCode = 400, StatusMessage = "Sin facturas." };
			{ return new List<Documento>(); }

			var list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Documento>>(resp);

			return list;

			/*new RespuestaDocumentos()
		{
			Status = true,
			StatusMessage = "Correcto",
			StatusCode = 200,
			Documentos = list
		};*/
		}

		public async Task<List<Documento>> Pedidos(string id, int estado, DateTime inicio, DateTime fin)
		{
			string e = estado switch
			{
				0 => "PEN",
				1 => "FIN",
				2 => "CAN",
				_ => "PEN"
			};
			IEnumerable<KeyValuePair<string, string>> param =
				new List<KeyValuePair<string, string>>
					{
						new KeyValuePair<string, string>("id", id),
                        new KeyValuePair<string, string>("estatus", e),
                        new KeyValuePair<string, string>("inicio", inicio.ToString("yyyy-MM-dd")),
						new KeyValuePair<string, string>("fin", fin.ToString("yyyy-MM-dd"))
					};
			string endpoint = _config["endpoint:pedidos:pedidos"];
			var resp = await _api.GetParams(endpoint, param);

			List<Documento> docs = new();

			if (resp == "[]" || string.IsNullOrEmpty(resp)) { return docs; }

			docs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Documento>>(resp);

			return docs;
		}
		public async Task<List<DocumentoV2>> PedidosV2(string id, int estado, DateTime inicio, DateTime fin)
		{
			string e = estado switch
			{
				0 => "PEN",
				1 => "FIN",
				2 => "CAN",
				_ => "PEN"
			};
			IEnumerable<KeyValuePair<string, string>> param =
				new List<KeyValuePair<string, string>>
					{
						new KeyValuePair<string, string>("id", id),
                        new KeyValuePair<string, string>("estatus", e),
                        new KeyValuePair<string, string>("inicio", inicio.ToString("yyyy-MM-dd")),
						new KeyValuePair<string, string>("fin", fin.ToString("yyyy-MM-dd"))
					};
			string endpoint = _config["endpoint:pedidos:pedidos2"];
			var resp = await _api.GetParams(endpoint, param);

			List<DocumentoV2> docs = new();

			if (resp == "[]" || string.IsNullOrEmpty(resp)) { return docs; }

			docs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DocumentoV2>>(resp);

			return docs;
		}
		public async Task<List<Documento>> Compras(string id, int estado, DateTime inicio, DateTime fin)
		{
			string e = estado switch
			{
				0 => "FIN",
				1 => "PEN",
				2 => "CAN",
				_ => "FIN"
			};
			IEnumerable<KeyValuePair<string, string>> param =
				new List<KeyValuePair<string, string>>
					{
						new KeyValuePair<string, string>("id", id),
                        new KeyValuePair<string, string>("estatus", e),
                        new KeyValuePair<string, string>("inicio", inicio.ToString("yyyy-MM-dd")),
						new KeyValuePair<string, string>("fin", fin.ToString("yyyy-MM-dd"))
				};
			string endpoint = _config["endpoint:compras:compras"];
			var resp = await _api.GetParams(endpoint, param);

			List<Documento> docs = new();

			if (resp == "[]" || string.IsNullOrEmpty(resp)) { return docs; }

			docs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Documento>>(resp);

			return docs;
		}
		public async Task<List<DocumentoV2>> ComprasV2(string id, int estado, DateTime inicio, DateTime fin)
		{
			string e = estado switch
			{
				0 => "FIN",
				1 => "PEN",
				2 => "CAN",
				_ => "FIN"
			};
			IEnumerable<KeyValuePair<string, string>> param =
				new List<KeyValuePair<string, string>>
					{
						new KeyValuePair<string, string>("id", id),
                        new KeyValuePair<string, string>("estatus", e),
                        new KeyValuePair<string, string>("inicio", inicio.ToString("yyyy-MM-dd")),
						new KeyValuePair<string, string>("fin", fin.ToString("yyyy-MM-dd"))
						
					};
			string endpoint = _config["endpoint:compras:compras2"];
			var resp = await _api.GetParams(endpoint, param);

			List<DocumentoV2> docs = new();

			if (resp == "[]" || string.IsNullOrEmpty(resp)) { return docs; }

			docs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DocumentoV2>>(resp);

			return docs;
		}
		public async Task<List<Documento>> NotasCreditoCliente(string id, int estado, DateTime inicio, DateTime fin)
		{
			string e = estado switch
			{
				0 => "PEN",
				1 => "FIN",
				2 => "CAN",
				_ => "PEN"
			};
			IEnumerable<KeyValuePair<string, string>> param =
				new List<KeyValuePair<string, string>>
					{
						new KeyValuePair<string, string>("id", id),
                        new KeyValuePair<string, string>("estatus", e),
                        new KeyValuePair<string, string>("inicio", inicio.ToString("yyyy-MM-dd")),
						new KeyValuePair<string, string>("fin", fin.ToString("yyyy-MM-dd"))
					};
			string endpoint = _config["endpoint:notascredito:cliente"];
			var resp = await _api.GetParams(endpoint, param);

			List<Documento> docs = new();

			if (resp == "[]" || string.IsNullOrEmpty(resp)) { return docs; }

			docs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Documento>>(resp);

			return docs;
		}

		public async Task<List<Documento>> NotasCreditoProveedor(string id, int estado, DateTime inicio, DateTime fin)
		{
			string e = estado switch
			{
				0 => "PEN",
				1 => "FIN",
				2 => "CAN",
				_ => "PEN"
			};
			IEnumerable<KeyValuePair<string, string>> param =
				new List<KeyValuePair<string, string>>
					{
						new KeyValuePair<string, string>("id", id),
                        new KeyValuePair<string, string>("estatus", e),
                        new KeyValuePair<string, string>("inicio", inicio.ToString("yyyy-MM-dd")),
						new KeyValuePair<string, string>("fin", fin.ToString("yyyy-MM-dd"))
					};
			string endpoint = _config["endpoint:notascredito:proveedor"];
			var resp = await _api.GetParams(endpoint, param);

			List<Documento> docs = new();

			if (resp == "[]" || string.IsNullOrEmpty(resp)) { return docs; }

			docs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Documento>>(resp);

			return docs;
		}
		/*
		public async Task<string> PedidosPendientes(IEnumerable<KeyValuePair<string, string>> parametros)
		{
            string endpoint = _config["endpoint:pedidos:pendientes"];
			var resp = await _api.GetParams(endpoint, parametros);
			return resp;
		}
		public async Task<string> PedidosCancelados(IEnumerable<KeyValuePair<string, string>> parametros)
		{
			string endpoint = _config["endpoint:pedidos:cancelados"];
			var resp = await _api.GetParams(endpoint, parametros);
			return resp;
		}
		public async Task<string> PedidosFinalizados(IEnumerable<KeyValuePair<string, string>> parametros)
		{
			string endpoint = _config["endpoint:pedidos:finalizados"];
			var resp = await _api.GetParams(endpoint, parametros);
			return resp;
		}
        public async Task<string> ComprasPendientes(IEnumerable<KeyValuePair<string, string>> parametros)
        {
            string endpoint = _config["endpoint:compras:pendientes"];
            var resp = await _api.GetParams(endpoint, parametros);
            return resp;
        }
        public async Task<string> ComprasCanceladas(IEnumerable<KeyValuePair<string, string>> parametros)
        {
            string endpoint = _config["endpoint:compras:cancelados"];
            var resp = await _api.GetParams(endpoint, parametros);
            return resp;
        }
        public async Task<string> ComprasFinalizadas(IEnumerable<KeyValuePair<string, string>> parametros)
        {
            string endpoint = _config["endpoint:compras:finalizados"];
            var resp = await _api.GetParams(endpoint, parametros);
            return resp;
        }
        public async Task<string> FactPenProveedor(IEnumerable<KeyValuePair<string, string>> parametros)
		{
            logger.LogInformation("Endpoint ");
            string endpoint = _config["endpoint:facturas:proveedor:pendientes"];
			var resp = await _api.GetParams(endpoint, parametros);
			return resp;
		}
		public async Task<string> FactCanProveedor(IEnumerable<KeyValuePair<string, string>> parametros)
		{
            logger.LogInformation("Endpoint ");
            string endpoint = _config["endpoint:facturas:proveedor:cancelados"];
			var resp = await _api.GetParams(endpoint, parametros);
			return resp;
		}
		public async Task<string> FactFinProveedor(IEnumerable<KeyValuePair<string, string>> parametros)
		{
            logger.LogInformation("Endpoint ");
            string endpoint = _config["endpoint:facturas:proveedor:finalizados"];
			var resp = await _api.GetParams(endpoint, parametros);
			return resp;
		}
		public async Task<string> FactPenClient(IEnumerable<KeyValuePair<string, string>> parametros)
		{
            logger.LogInformation("Endpoint ");
            string endpoint = _config["endpoint:facturas:cliente:pendientes"];
			var resp = await _api.GetParams(endpoint, parametros);
			return resp;
		}
		public async Task<string> FactCanClient(IEnumerable<KeyValuePair<string, string>> parametros)
		{
            logger.LogInformation("Endpoint ");
            string endpoint = _config["endpoint:facturas:cliente:cancelados"];
			var resp = await _api.GetParams(endpoint, parametros);
			return resp;
		}
		public async Task<string> FactFinClient(IEnumerable<KeyValuePair<string, string>> parametros)
		{
            logger.LogInformation("Endpoint ");
            string endpoint = _config["endpoint:facturas:cliente:finalizados"];
			var resp = await _api.GetParams(endpoint, parametros);
			return resp;
		}
		public async Task<string> NTCRPenProveedor(IEnumerable<KeyValuePair<string, string>> parametros)
		{
            logger.LogInformation("Endpoint ");
            string endpoint = _config["endpoint:notascredito:proveedor:pendientes"];
			var resp = await _api.GetParams(endpoint, parametros);
			return resp;
		}
		public async Task<string> NTCRCanProveedor(IEnumerable<KeyValuePair<string, string>> parametros)
		{
            logger.LogInformation("Endpoint ");
            string endpoint = _config["endpoint:notascredito:proveedor:cancelados"];
			var resp = await _api.GetParams(endpoint, parametros);
			return resp;
		}
		public async Task<string> NTCRFinProveedor(IEnumerable<KeyValuePair<string, string>> parametros)
		{
            logger.LogInformation("Endpoint ");
            string endpoint = _config["endpoint:notascredito:proveedor:finalizados"];
			var resp = await _api.GetParams(endpoint, parametros);
			return resp;
		}

		public async Task<string> NTCRPenClient(IEnumerable<KeyValuePair<string, string>> parametros)
		{
            logger.LogInformation("Endpoint ");
            string endpoint = _config["endpoint:notascredito:cliente:pendientes"];
			var resp = await _api.GetParams(endpoint, parametros);
			return resp;
		}
		public async Task<string> NTCRCanClient(IEnumerable<KeyValuePair<string, string>> parametros)
		{
            logger.LogInformation("Endpoint ");
            string endpoint = _config["endpoint:notascredito:cliente:cancelados"];
			var resp = await _api.GetParams(endpoint, parametros);
			return resp;
		}
		public async Task<string> NTCRFinClient(IEnumerable<KeyValuePair<string, string>> parametros)
		{
            logger.LogInformation("Endpoint ");
            string endpoint = _config["endpoint:notascredito:cliente:finalizados"];
			var resp = await _api.GetParams(endpoint, parametros);
			return resp;
		}*/

		public async Task<List<Documento>> EdoCuentaCliente(string id, DateTime inicio, DateTime fin)
		{
			IEnumerable<KeyValuePair<string, string>> param =
			 new List<KeyValuePair<string, string>>
				 {
						new KeyValuePair<string, string>("id", id),
						new KeyValuePair<string, string>("inicio", inicio.ToString("yyyy-MM-dd")),
						new KeyValuePair<string, string>("fin", fin.ToString("yyyy-MM-dd"))
				 };

			string endpoint = _config["endpoint:estadocuenta:cliente:edocuenta"];
			
			var resp = await _api.GetParams(endpoint, param);

			List<Documento> facturas = new();

			if (resp == "[]" || string.IsNullOrEmpty(resp)) { return facturas; }

			facturas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Documento>>(resp);

			return facturas;
		}
		public async Task<List<Documento>> EdoCuentaProveedor(string id, DateTime inicio, DateTime fin)
		{
			IEnumerable<KeyValuePair<string, string>> param =
			 new List<KeyValuePair<string, string>>
				 {
						new KeyValuePair<string, string>("id", id),
						new KeyValuePair<string, string>("inicio", inicio.ToString("yyyy-MM-dd")),
						new KeyValuePair<string, string>("fin", fin.ToString("yyyy-MM-dd"))
				 };
			string endpoint = _config["endpoint:estadocuenta:proveedor:edocuenta"];
			var resp = await _api.GetParams(endpoint, param);

			List<Documento> facturas = new();

			if (resp == "[]" || string.IsNullOrEmpty(resp)) { return facturas; }

			facturas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Documento>>(resp);

			return facturas;

		}
		public async Task<List<Documento>> FacturacionCliente(string id, DateTime inicio, DateTime fin)
		{
			IEnumerable<KeyValuePair<string, string>> param =
			 new List<KeyValuePair<string, string>>
				 {
						new KeyValuePair<string, string>("id", id),
						new KeyValuePair<string, string>("inicio", inicio.ToString("yyyy-MM-dd")),
						new KeyValuePair<string, string>("fin", fin.ToString("yyyy-MM-dd"))
				 };
			string endpoint = _config["endpoint:estadocuenta:cliente:facturas"];
			var resp = await _api.GetParams(endpoint, param);
			List<Documento> facturas = new();

			if (resp == "[]" || string.IsNullOrEmpty(resp)) { return facturas; }

			facturas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Documento>>(resp);

			return facturas;
		}
		public async Task<List<Documento>> FacturacionProveedor(string id, DateTime inicio, DateTime fin)
		{
			IEnumerable<KeyValuePair<string, string>> param =
			 new List<KeyValuePair<string, string>>
				 {
						new KeyValuePair<string, string>("id", id),
						new KeyValuePair<string, string>("inicio", inicio.ToString("yyyy-MM-dd")),
						new KeyValuePair<string, string>("fin", fin.ToString("yyyy-MM-dd"))
				 };
			string endpoint = _config["endpoint:estadocuenta:proveedor:facturas"];
			var resp = await _api.GetParams(endpoint, param);
			List<Documento> facturas = new();

			if (resp == "[]" || string.IsNullOrEmpty(resp)) { return facturas; }

			facturas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Documento>>(resp);

			return facturas;
		}

		public async Task<string> ComprasTop5(string id)
		{
			IEnumerable<KeyValuePair<string, string>> param =
				new List<KeyValuePair<string, string>>
					{
						new KeyValuePair<string, string>("idSocio", id)
					};
			string endpoint = _config["endpoint:compras:top5"];
			var resp = await _api.GetParams(endpoint, param);
			return resp;
		}

		public async Task<string> OrdenCompra(string id)
		{
			//no sé si enviar el endpoint con el id como parámetro o mandar el endpoint con el id concatenado
			IEnumerable<KeyValuePair<string, string>> param =
			new List<KeyValuePair<string, string>>
				{
						new KeyValuePair<string, string>("idSocio", id)
				};
			string endpoint = _config["endpoint:compras:id"];
			var resp = await _api.GetParams(endpoint, param);
			return resp;
		}
	}
}
