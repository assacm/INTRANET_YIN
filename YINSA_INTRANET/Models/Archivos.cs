using System.ComponentModel.DataAnnotations;

namespace YINSA_INTRANET.Models
{
	
	/// <summary>
	///  Modelos de archivo para creación e inserción de documento y archivo a través de API.
	/// </summary>
	public class Archivo
	{
		public int IdDoc { get; set; }

		public int Id { get; set; }
		public string IdSocio { get; set; }
		public int IdUser { get; set; }
		public int DocEntry { get; set; }
		public int DocNum { get; set; }
		public string Estatus { get; set; }
		public string Nombre { get; set; }
		public string Descripcion { get; set; }
		/// <value>Representa el tipo de documento al que hacen referencia los archivos de la factura.</value>
		public string TipoDoc { get; set; }
        public int TipoObjSAP { get; set; }

        public string Concepto { get; set; }

		public string Ruta { get; set; }
		public string Extension { get; set; }
		public string Comentario { get; set; }
		public IFormFile File { get; set; }
	}
	/// <summary>
	///  Modelo de archivo requerido por parte del cliente para crear/ realizar post.
	/// </summary>
	public class ArchivoPost
	{
		public int IdDoc { get; set; }

		public string IdSocio { get; set; }
		public int IdUser { get; set; }
		public int IdUserUpd { get; set; }
		public int DocEntry { get; set; }
		public int DocNum { get; set; }
		//public string Estatus { get; set; }
		//public string Nombre { get; set; }
		public string Descripcion { get; set; }
		public string TipoDoc { get; set; }
        public int TipoObjSAP { get; set; }

        public string Concepto { get; set; }
		public bool ValidXML { get; set; }
		public bool ValidSAT { get; set; }
		public bool SAP { get; set; }
		public bool Contabilidad { get; set; }

		//public string Ruta { get; set; }
		//public string Extension { get; set; }
		public string Comentario { get; set; }
		public IFormFile File { get; set; }
		/// <summary>
		/// Datos del archivo XML de la factura
		/// </summary>
		public string JsonXML { get; set; }
	}
	/// <summary>
	///  Modelo para recibir la información del documento, la información de archivos y comentarios relacionados desde la base de datos.
	/// </summary>
	public class ArchivoDoc
	{
		public string Socio { get; set; }
		public int Id { get; set; }
		public int IdUser { get; set; }
		public int IdUserUpd { get; set; }
		public string Descripcion { get; set; }
		public string IdSocio { get; set; }
		public int DocEntry { get; set; }
		public string TipoDoc { get; set; }
        public int TipoObjSAP { get; set; }

        public int DocNum { get; set; }
		public string Estatus { get; set; }
		public bool Estado { get; set; }	
		public DateTime FechaUpd { get; set; }
		public bool ValidXML { get; set; }
		public bool ValidSAT { get; set; }
		public bool SAP { get; set; }
		public bool Contabilidad { get; set; }
		/// <summary>
		/// Para agregar comentario, no obtiene comentario.
		/// </summary>
		public string Comentario { get; set; }
		public List<ArchivoRoot> Archivos { get; set; }
		public List<ComentarioModel> Comentarios { get; set; }
		public List<AutorizacionesDoc> Autorizaciones { get; set; }
		public List<AutorizacionesDoc> AutorizacionesConta { get; set; }
		public List<AutorizacionesDoc> Autorizadores { get; set; }

	}
	/// <summary>
	///  Información proporcionada del archivo.
	/// </summary>
	public class ArchivoRoot
	{
		public string Nombre { get; set; }
		///<value>Ubicación del archivo en el servidor.</value>
		public string Ruta { get; set; }
		public string Extension { get; set; }
	}
		/// <summary>
		///  Modelo para comentarios realizados sobre archivos de documentos.
		/// </summary>
		public class ComentarioModel
		{
			public int Id { get; set; }
			public int IdDoc { get; set; }
			public int IdUsuario { get; set; }
			public string IdSocio { get; set; }
			public string Comentario { get; set; }
			public DateTime FechaUpd { get; set; }
			public string Socio { get; set; }
			public string Usuario { get; set; }
		}

		/// <summary>
		///  Modelo para actualizar información de archivos.
		/// </summary>
		public class UpdDocumento
		{
			/// <value>Identificador del documento a actualizar</value>
			public int IdDoc { get; set; }
		/// <value>Identificador del usuario que actualiza el documento.</value>
			public int IdUser { get; set; }
		/// <value>Identificador de la cuenta desde la que se actualiza el documento.</value>
		//public string IdSocio { get; set; }

		/// <value>Actualiza el estado del documento.</value>
		public string Estado { get; set; }
			///<value>Actualización de ubicación del archivo en el servidor.</value>
			//public string Ruta { get; set; }
			/// <value> Nombre del archivo.</value>
			//public string FileName { get; set; }
			/// <value> Extensión del archivo.</value>
			//public string Extension { get; set; }

			/// <value>Representa información proporcionada por el usuario.</value>
			//public string? Comentario { get; set; }
			
			public bool Estatus { get; set; }
		
		}
		public class UpdDocumentoJS
		{
			public string idDoc { get; set; }
	
			public string comentario { get; set; }

			public string estatus { get; set; }
		}
		/// <summary>
		///  Modelo para realizar el post del archivo e información desde el cliente con Javascript.
		/// </summary>
		public class ArchivoPOSTJs
		{
			public int docEntry { get; set; }
			public int docNum { get; set; }
			/// <summary>
			/// Tipo de objeto de SAP
			/// </summary>
			public int obj { get; set; }

        [Required]
			[MaxLength(50)]
			public string descripcion { get; set; }
			public string comentario { get; set; }
			public List<ArchivoJS> files { get; set; }

		}
	/// <summary>
	///  Modelo del archivo desde el cliente.
	/// </summary>
	public class ArchivoJS
	{
		public string id { get; set; }
		public string name { get; set; }
		public string data { get; set; }
		public string type { get; set; }
	}

	/// <summary>
	/// Lista de proveedores y conteo de archivos según estado. 
	/// </summary>
	public class ProveedorCount
	{
		public string CardCode { get; set; }
		public string CardName { get; set; }
		/// <summary>
		/// Número representativo de los archivos pendientes de revisar.
		/// </summary>
		public int Count { get; set; }
		/// <summary>
		/// Lista de archivos por revisar del proveedor.
		/// </summary>
		public List<ArchivoDoc> Archivos { get; set; }
	}

}
