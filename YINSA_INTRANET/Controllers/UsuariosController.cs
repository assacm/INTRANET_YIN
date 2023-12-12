using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Security.Claims;
using YINSA_INTRANET.Models;
using YINSA_INTRANET.Servicios;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services.Description;
using System.Net.Security;
using YINSA_INTRANET.Filtros;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace YINSA_INTRANET.Controllers
{
	[Authorize]
	public class UsuariosController : Controller
	{
		private readonly IUsuariosService usuariosService;
        private readonly ISociosService socioService;
		private readonly IArchivosService archivosService;
		private readonly IReportesService _reportesService;
		private readonly IDocumentosService documentosService;
		private readonly IWebHostEnvironment hostEnvironment;
		private readonly IEmpleadosService empleadosService;

		public UsuariosController(IUsuariosService usuariosService, ISociosService socioService,
			IArchivosService archivosService, IReportesService reportesService, IDocumentosService documentosService,
            IWebHostEnvironment hostEnvironment, IEmpleadosService empleadosService)
		{
			this.usuariosService = usuariosService;
            this.socioService = socioService;
			this.archivosService = archivosService;
			this._reportesService = reportesService;
			this.documentosService = documentosService;
			this.hostEnvironment = hostEnvironment;
			this.empleadosService = empleadosService;
		}

		private List<Cuenta> Cuentas(List<Cuenta> lst1, List<Cuenta> lst2)
		{
			List<Cuenta> cuentas= new();

			if(lst1 != null )
				 cuentas = lst1;
			if ( lst2 != null)
				return cuentas.Concat(lst2).ToList();

			//if (lst1.Count > 0  && lst2.Count > 0)
			//	cuentas = lst1.Concat(lst2).ToList();
			

			return cuentas;
		}
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Login()		
		{

			ClaimsPrincipal c = HttpContext.User;

			if (c.Identity != null)
			{
				if (c.Identity.IsAuthenticated)
				{
				

					string user = c.FindFirstValue("UserName"); //HttpContext.Request.Cookies["UserName"];
					var resp = await usuariosService.UserByName(user);

			
						if (string.IsNullOrEmpty(resp.userName)) 
						return RedirectToAction("NoEncontrado", "Home");

					var cuentas = Cuentas(resp.cuentasSocio, resp.cuentasEmpleado);
					if(cuentas.Count <= 0)
						return RedirectToAction("NoEncontrado", "Home");

					var str = Newtonsoft.Json.JsonConvert.SerializeObject(cuentas);

					HttpContext.Session.SetString("lstCuentas",str);
                    return RedirectToAction("VerificacionCuentas","Usuarios");
                  
                }
			}

			return View(); 
		}
		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login(LoginUser modelo)
		{
			ViewBag.Error = "";
			if(string.IsNullOrEmpty(modelo.userName) || string.IsNullOrEmpty(modelo.password))
			{
				ViewBag.Error = "*  Debe llenar los campos.";
				return View();
			}


			var passwordHash = HashService.ConvertirSha256(modelo.password);
			LoginViewModel login = new()
			{
				userName = modelo.userName,
				password = passwordHash
			};

			var resp = await usuariosService.Login(login);

			if (resp == "[]")
			{
				ViewBag.Error = "*  Credenciales incorrectas.";
				return View();
			}

			var log = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthResponse>(resp);
			if (log.Status != true)
			{
				ViewBag.Error = "Credenciales incorrectas.";
				return View();
			}

			// Crear los claims de identidad con el token
			var claims = new List<Claim>
			{
				new Claim("Token", log.Token),
				new Claim("UserName", log.Usuario.userName),
				new Claim(ClaimTypes.Role, log.Usuario.rol)
			};

			//crear claims para los permisos de autorizacion y creacion de facturas- crear funcion en el futuro
			Permiso permiso = new();
			if (log.Usuario.permisos != null)
			{
				permiso = log.Usuario.permisos.Find(p => p.IdPermiso == 1);//permiso Facturas de proveedor
			}
			if (permiso.Autorizar) { claims.Add(new Claim("authFct", "true")); }
			if(permiso.Crear) { claims.Add(new Claim("subirFct", "true")); }

			// Crear el principal de identidad
			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

			// Crear el claim principal de autenticación
			var principal = new ClaimsPrincipal(identity);

			// Guardar la cookie de autenticación con el token
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

			var cuentas = Cuentas(log.Usuario.cuentasSocio, log.Usuario.cuentasEmpleado);
			if (cuentas.Count <= 0)
				return RedirectToAction("NoEncontrado", "Home");

			var str = Newtonsoft.Json.JsonConvert.SerializeObject(cuentas);

            //TempData["lstCuentas"] = str;
            HttpContext.Session.SetString("lstCuentas", str);

            return RedirectToAction("VerificacionCuentas"); 
		}
		[AllowAnonymous]
		public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //TempData.Remove("lstCuentas");
            HttpContext.Session.Remove("lstCuentas");
            HttpContext.Session.Remove("codigoS");
            return RedirectToAction("Login", "Usuarios");
        }

        [HttpGet]
		[Authorize(Policy = "Admin")]
		public async Task<IActionResult> Registro() {
			var idCuenta = usuariosService.GetSocioId();
			var grupoprov = await _reportesService.GetGrupoProv();
			var permisos = await usuariosService.GetPermisos();


			var model = new RegistroUserVM()
			{
				GrupoProv = grupoprov,
				Permisos = permisos,
				IdCuenta = idCuenta
			};
			return View(model); 
		}
		[HttpPost]
		[Authorize(Policy = "Admin")]
		public async Task<IActionResult> Registro([FromBody] RegistroPost model)//RegistroUserVM model)
		{
			//validación de campos y confirmación de contraseña
			if (!ModelState.IsValid)
			{
				//ModelState.AddModelError(string.Empty, "Ha ocurrido un error de validación.");
				return BadRequest(new { message = "Ha ocurrido un error. Verifique los campos."});
			}
			
			var existe = await usuariosService.UserByName(model.userName);
			if (!string.IsNullOrEmpty(existe.userName))
				return BadRequest(new { message = "El usuario ingresado ya existe." }); //BadRequest? Invalid or algo


			if(model.password!= model.confPass)
				return BadRequest(new { message = "Las contraseñas no coinciden." });


			//registro de usuario
			RegistroUser registro = model;
			registro.userName = registro.userName.ToLower();
			registro.email = registro.email.ToLower();
			registro.password = HashService.ConvertirSha256(registro.password);
			
		
			var resReg= await usuariosService.Registro(registro); //PROBAR MANDANDO BODY EN MINUSCULAS

			var respR = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthResponse>(resReg);

			if (!respR.Status) 
				return BadRequest(new { message = "No se ha podido realizar el registro. Inténtelo de nuevo." });

			//endpoint registro cuentas
			var asignarCuentas = new CuentaAsignacion()
			{
				userId = respR.Usuario.userId,
				cuentas = model.cuentas
			};

			//Validar respuestas

			var resAsig = await usuariosService.AsignarCuenta(asignarCuentas);
		
			if(!resAsig.Status)
				return BadRequest(new { message = "Hubo un problema al asignar las cuentas. Inténtelo de nuevo." });

			Respuesta res = new();

			if(model.permisos != null || model.permisos.Count > 0) { 
				var asignarPermisos = new PermisoAsignacion()
				{
					IdUsuario = respR.Usuario.userId,
					Permisos = model.permisos
				};

				res = await usuariosService.AsignarPermisos(asignarPermisos);
			}
			if (!res.Status)
				return BadRequest(new { message = "Hubo un problema al asignar los permisos. Inténtelo de nuevo." });

			if (model.filtros != null || model.filtros.Count > 0)
			{
				var asignarFiltros = new FiltrosAsignacion()
				{
					IdUsuario = respR.Usuario.userId,
					Filtros = model.filtros
				};

				res = await usuariosService.FiltrosAutorizacion(asignarFiltros);
			}
			if (!res.Status)
				return BadRequest(new { message = "Hubo un problema al asignar los grupos a autorizar. Inténtelo de nuevo." });
			
			return Ok();
		}

		[HttpGet]
		[Authorize(Policy = "Admin")]
		public async Task<IActionResult> Editar(string id) 
		{
			//if (!int.TryParse(id, out int r))
			//{ return Json(new { status = 400, message = "Inválido" }); }
			//if (!int.TryParse(id, out int r))
			//{ return View("NoEncontrado","Home"); }
			if (string.IsNullOrEmpty(id))
			{ return View("NoEncontrado", "Home"); }

			//var resp = await usuariosService.UserById(int.Parse(id));
			var resp = await usuariosService.UserByName(id);

			var cuentas = Cuentas(resp.cuentasSocio, resp.cuentasEmpleado);

			string cuentasJSON = Newtonsoft.Json.JsonConvert.SerializeObject(cuentas);
			string permisosJSON = Newtonsoft.Json.JsonConvert.SerializeObject(resp.permisos);
			string filtrosJSON = Newtonsoft.Json.JsonConvert.SerializeObject(resp.filtros);
			//ViewBag.id = id;
			//ViewBag.cuentas = JsonConvert.SerializeObject(resp.cuentas);
			var filtros = await _reportesService.GetGrupoProv();
			var permisos = await usuariosService.GetPermisos();


			var model = new EditarUsuarioVM()
			{
				userId  = resp.userId,
				userName = resp.userName,
				name = resp.name,
				rolId =resp.rolId,
				rol = resp.rol,
				email = resp.email,
				estatus = resp.estatus,
				creacion = resp.creacion,
				filtros = resp.filtros,
				cuentas = resp.cuentas,
				permisos = resp.permisos,
				Permisos = permisos,
				GrupoProv = filtros,
				PermisosJSON = permisosJSON,
				FiltrosJSON = filtrosJSON,
				CuentasJSON =cuentasJSON
			};

			return View(model);
		}

		[HttpPost]
		[Authorize(Policy = "Admin")]
		public async Task<IActionResult> Editar([FromBody] Usuario usuario)//(Usuario model) 
		{
			if (!ModelState.IsValid)
			{
				//ModelState.AddModelError(string.Empty, "Ha ocurrido un error de validación.");
				//return View(usuario);
				return BadRequest();
			}

			var userO = await usuariosService.UserByName(usuario.userName1);
			usuario.userId = userO.userId;
			//Validar contraseña

			if (!string.IsNullOrEmpty(usuario.password))
			{

				//(bool valid, string e) = UsuariosService.ValidPassword(usuario.password);
				//if (!valid) { return BadRequest(); }

				if (!UsuariosService.ValidPass(usuario.password))
				{

					return BadRequest();
				}
			
				usuario.password = HashService.ConvertirSha256(usuario.password); 			
			}

			usuario.userName = usuario.userName.ToLower();
			usuario.email = usuario.email.ToLower();

			var resp = await usuariosService.Actualizar(usuario);

			if (!resp.Status) { return BadRequest(); }

			return Ok(); 
			
		}
		[HttpGet]
		[Authorize(Policy = "Admin")]
		public async Task<IActionResult> Panel()
		{
			var id = usuariosService.GetSocioId();

			var user = await empleadosService.Empleado(id);

			if(user == null) { return RedirectToAction("NoEncontrado", "Home"); }

			var users = await usuariosService.GetUsers();

			var emp = await empleadosService.Empleados(0);

			var facts = await archivosService.DocCount(0);

			//if (res == null) RedirectToAction("NoEncontrado", "Home");//MEJOR RETORNAR VISTA Y NO MOSTRAR USUARIOS

			var model = new ControlViewModel()
			{
				SelectTabla = 0,
				//SelectMonth = ReportesService.SelectMonth(DateTime.Now.Year),
				Usuarios = users,
				Empleados = emp,
				FacturasPendientes = facts,
				Nombre = $"{user.Nombre} {user.ApellidoPa} {user.ApellidoPa} ",
				Puesto = user.Puesto
			};

			return View(model);

		}

		//[PermisoDeAutorizacion] 
		[HttpPost]
		[Authorize(Policy = "Admin")]
		public async Task<IActionResult> Panel(ControlViewModel post)
		{
			var users = await usuariosService.GetUsers();

			var emp = await empleadosService.Empleados(0);

			var facts = await archivosService.DocCount(0);

			//if (res == null) RedirectToAction("NoEncontrado", "Home");//MEJOR RETORNAR VISTA Y NO MOSTRAR USUARIOS

			var model = new ControlViewModel()
			{
				MenuOption = post.MenuOption,
				SelectTabla = post.SelectTabla,
				//SelectMonth = ReportesService.SelectMonth(DateTime.Now.Year),
				Usuarios = users,
				Empleados = emp,
				FacturasPendientes = facts
			};

			return View(model);

		}

		[HttpGet]
		[Authorize(Policy = "Admin")]
		public async Task<IActionResult> Control() 
		{
			var res = await usuariosService.GetUsers();

			var docs = await archivosService.DocCount(0);

			if (res == null) RedirectToAction("NoEncontrado","Home");//MEJOR RETORNAR VISTA Y NO MOSTRAR USUARIOS

			var model = new ControlViewModel()
			{	SelectTabla = 0,
				SelectMonth = ReportesService.SelectMonth(DateTime.Now.Year),
				Usuarios = res,
				DocumentosPendientes = docs
			};

			return View(model); 
		}
		[HttpPost]
		[Authorize(Policy = "Admin")]
		public async Task<IActionResult> Control(ControlViewModel post)
		{
			var users = await usuariosService.GetUsers();

			var docs = await archivosService.DocCount(0);

			List<Documento> facturasSap = new();
			List<ArchivoDoc> archivosProv = new();
			List<FacturaJSON> factXML = new();
			(DateTime inicio, DateTime fin) = _reportesService.RangoFechas(post.Month, post.Year);

			//Obtener facturas según : Clientes Proveedores <- switch, Fechas, Estado
			if(post.Socio == 0) { facturasSap = await documentosService.FacturasCliente(null, post.Estado, inicio, fin); }

			if(post.Socio == 1) { facturasSap = await documentosService.FacturasProveedor(null, post.Estado, inicio, fin); }
			//Obtener archivos subidos de proveedores

			if(post.SelectTabla == 3)
			{
				var r = await archivosService.FacturasXML(null,0);
				factXML = r.FacturasXML;
			}

			var arch = await archivosService.Documentos(-1,0,inicio,fin); //-1 para mandar estado default: null, traer todos
			if (arch.Status)
			{
				archivosProv = arch.Archivos;
			}
			//if (res == null) RedirectToAction("NoEncontrado", "Home");//MEJOR RETORNAR VISTA Y NO MOSTRAR USUARIOS

			var model = new ControlViewModel()
			{
				SelectTabla = post.SelectTabla,
				SelectMonth = ReportesService.SelectMonth(DateTime.Now.Year),
				Usuarios = users,
				DocumentosPendientes = docs,
				FacturasSAP = facturasSap,
				ArchivosProv = archivosProv,
				FacturasXML = factXML
			};

			return View(model);
		}

		/// <summary>
		/// Método para cargar información sobre archivos de facturas subidas por proveedores.
		/// </summary>
		/// <param name="liststate">Identificador del estado de los archivos para obtener lista de proveedores.</param>
		/// <param name="idDoc">Identificador para mostrar el detalle de un Documento de archivo en específico.</param>
		/// <returns></returns>
		[HttpGet]
		//[ServiceFilter(typeof(AutorizacionFacturasFilter))]
		[Authorize(Policy = "AutorizarFctProv")]
		//[Authorize(Roles ="contador")]
		public async Task<IActionResult> RevisionArchivos(int liststate, int idDoc) 
		{
			
			 List<ArchivoDoc> docs = new();
			string idProv = null; int periodo = 0;

			List<SelectListItem> options = _reportesService.SelectPeriodo();
		//	ViewBag.Opciones = options;
			ViewBag.Detalle = "disable";

			(DateTime inicio, DateTime fin) = _reportesService.RangoPeriodo(periodo);

			//var prov = await archivosService.ProveedoresCount(liststate, grupoprov); 
			var autProvs = await archivosService.AutorizacionProveedores(liststate);

			List<ProveedorCount>  lstProv = autProvs.Item1;

			if (lstProv.Count > 0)
			{   //tenemos que concatenar las listas por si el usuario tiene más de un grupo de proveedores asignado
				//lstProv = prov.ProveedoresArchivos;
				idProv = lstProv.FirstOrDefault().CardCode;
			}

			if (!string.IsNullOrEmpty(idProv)) 
			{
			   	var res = await archivosService.DocumentosSocio(idProv,0, inicio, fin); //("Pendiente",idProv);
				docs = res.Archivos;
			}

			RespuestaArchivos doc = new();

			if (idDoc > 0)
			{ doc = await archivosService.DocumentById(idDoc); ViewBag.Detalle = ""; }
			if (!doc.Status)
			{ doc.Archivo = new();  }



			var model = new RevisionArchivosVM()
			{	IdProveedor = idProv,
				Proveedores = lstProv,
				Archivos = docs,
				Estado = 0,
				Detalle = doc.Archivo,
				Periodo = periodo,
				SelectPeriodo= options,
				IdEmpleado = usuariosService.GetSocioId()
			};

			return View(model);
		}

		/// <summary>
		/// Método post para obtener información de la vista Revisión Archivos según filtros indicados.
		/// </summary>
		/// <param name="id">Identificador para mostrar el detalle de un Documento de archivo en específico.</param>
		/// <param name="liststate">Identificador del estado de los archivos para obtener lista de proveedores.</param>
		/// <param name="estado">Identificador del estado en los filtros de los archivos de factura a mostrar en la sección principal.</param>
		/// <param name="idProveedor">Identificador del proveedor del cual se cargarán los archivos en la sección principal.</param>
		/// <param name="periodo">Identificador del periodo en el que se mostrarán los archivos de facturas.</param>
		/// <returns>Vista Revisión Archivos.</returns>
		[HttpPost]
		[Authorize(Policy = "AutorizarFctProv")]
		public async Task<IActionResult> RevisionArchivos(int id, int liststate,int estado, string idProveedor,int periodo) //(int id, int estado, string idProv, int periodo)  (RevisionArchivosVM post,int id)
		{
			
			List<ProveedorCount> lstProv = new(); List<ArchivoDoc> docs = new();
			string idProv = null;
			

			
			List<SelectListItem> options = _reportesService.SelectPeriodo();
			//ViewBag.Opciones = options;
			(DateTime inicio, DateTime fin) = _reportesService.RangoPeriodo(periodo);

			//var prov = await archivosService.ProveedoresCount(liststate, grupoprov); 
			var autProvs = await archivosService.AutorizacionProveedores(liststate);

			lstProv = autProvs.Item1;

			if (lstProv.Count > 0)
			{   //tenemos que concatenar las listas por si el usuario tiene más de un grupo de proveedores asignado
				//lstProv = prov.ProveedoresArchivos;
				idProv = lstProv.FirstOrDefault().CardCode;
			} //si no se manda un idProveedor, lo obtenemos de la lista de proveedores


			if (!string.IsNullOrEmpty(idProveedor)) { idProv = idProveedor; }//si se manda un idProveedor, ese valor va para el modelo

			if (!string.IsNullOrEmpty(idProv))
			{
				var res = await archivosService.DocumentosSocio(idProv, estado, inicio, fin);
				docs = res.Archivos;
			}


			RespuestaArchivos doc = new();
			List<ComentarioModel> com = new();
			if (id > 0)
			{ 
				doc = await archivosService.DocumentById(id); 
			 	com = await archivosService.ObtenerComentarios(id);
				ViewBag.Detalle = ""; }
			if (!doc.Status)
			{ doc.Archivo = new(); ViewBag.Detalle = "disable"; }
			else
			{
				if (doc.Archivo.Autorizaciones == null || doc.Archivo.Autorizaciones.Count == 0) //si el doc no cuenta con autorizaciones, ponemos la lista de usuarios que lo pueden autorizar
				{ doc.Archivo.Autorizaciones = doc.Archivo.Autorizadores; }
			}

			doc.Archivo.Comentarios = com;



			var model = new RevisionArchivosVM()
			{	IdProveedor =idProv,
				ListState=liststate,
				Proveedores = lstProv,
				Archivos = docs,
				Estado = estado,
				Detalle = doc.Archivo,
				Periodo = periodo,
				SelectPeriodo = options,
				IdEmpleado = usuariosService.GetSocioId()
			};

			return View(model);

			//	ViewBag.Error = "Realizando actualización..."; //modal respuesta actualización msg
			//	ViewBag.ModalUpd = "disable";//display
		}

		[HttpPost]
		[Authorize(Policy = "AutorizarFctProv")]
		public async Task<IActionResult> ValidacionArchivo([FromBody] UpdDocumentoJS doc )//(RevisionArchivosVM doc)
		{
			//ModelState.AddModelError("", "Mensaje de error de prueba");
			//if (!ModelState.IsValid) 
			//{

			//}

			var idUser = await usuariosService.GetUserId();
			var idSocio= usuariosService.GetSocioId();

			if(idUser == 0)
			{
				return BadRequest(new { message = "Algo ha salido mal, inténtelo más tarde." });                //return RedirectToAction("RevisionArchivos", "Usuarios", 
																												//	new { id = doc.IdDoc, estado = 0, message = "Algo ha salido mal, inténtelo más tarde." });
			}

			string estado = (bool.Parse(doc.estatus)) ? "Aprobado" : "Rechazado";
	
			Autorizar aut = new()
			{
				IdDoc = int.Parse(doc.idDoc),
				IdUsuario = idUser,
				IdCuenta = idSocio,
				Estado = estado,
				Comentario = doc.comentario

			};

			var res = await archivosService.AsignarAutorizacion(aut);

			

			if (!res.Status)
				return BadRequest(new { message = "Algo ha salido mal, inténtelo más tarde." });

			if (!string.IsNullOrEmpty(aut.Comentario))
			{ await archivosService.CrearComentario(aut.Comentario, aut.IdDoc); }
			await AutorizarDocumento(aut.IdDoc,aut.IdUsuario);

			return Ok();
			/*
			 * 		var post = new UpdDocumento()
			{                                                                                                                         
				IdDoc = int.Parse(doc.idDoc),
				IdSocio = idSocio,
				IdUsuario = idUser,
				Estado = estado,
				Comentario = doc.comentario

			};
			var res = await archivosService.Actualizar(post);

			if (!res.Status)
			{
				return BadRequest(new { message = "Algo ha salido mal, inténtelo más tarde." });
			}

			//var newRoot = archivosService.Mover();

			return Ok();*/
		}
		private async Task<bool> AutorizarDocumento(int doc, int idUser)
		 { //uno solo que aautorice, contador autoriza
			//obtener autorizaciones del documento
			List<AutorizacionesDoc> autorizaciones = await archivosService.AutorizacionesDocumento(doc);

			bool valido = false; string estado = "Pendiente";

			if (autorizaciones == null || autorizaciones.Count ==0)  
			{ return valido; } //si no tenemos las autorizaciones, no actualizamos el estado general del doc.
			foreach (var aut in autorizaciones)
			{
				
				if (aut.Estado == "Aprobado")
				{ valido = true; estado = "Aprobado"; }
				if (aut.Estado == "Rechazado")
				{ valido = false; estado = "Rechazado"; }
				if(aut.Estado == "Pendiente") 
				{ valido=false; estado = "Pendiente"; }
			}

			if (estado == "Pendiente")
				return false;


			//Respuesta res = new() { Status = false };
			//if (autorizaciones.Count == 2 || estado == "Rechazado") { } //suponiendo que se necesitan mínimo 2 autorizaciones)

			var post = new UpdDocumento()
			{
				IdDoc = doc,
				Estado = valido ? "Aprobado" : "Rechazado",
				IdUser = idUser
			};

			var res = await archivosService.Actualizar(post);
			//var newRoot = archivosService.Mover();

			return res.Status; 

		}
	
		[HttpGet]
        public IActionResult Cuentas()
        {
            var strcuentas = HttpContext.Session.GetString("lstCuentas");

            if (string.IsNullOrEmpty(strcuentas))
				return RedirectToAction("NoEncontrado", "Home");

            var cuentas = JsonConvert.DeserializeObject<List<Cuenta>>(strcuentas);

			//var c = cuentas.FirstOrDefault();

            //return View(new CuentasViewModel() { cuentas=cuentas, id=c.codigoS,tipo=c.tipo});
            return View(new CuentasViewModel() { cuentas = cuentas, id = "", tipo = "" });
        }

        [HttpGet]
		public async Task<IActionResult> BuscarCuentas(string cuenta) {
			
			var resp = await usuariosService.BuscarCuenta(cuenta);

			return Ok(resp);
		
		}
		[HttpGet]
		public async Task<IActionResult> ObtenerCuentas(string id)
		{
			
			if (!int.TryParse(id,out int r))
			{ return Json(new { status=400, message="Inválido" }); }

			var resp = await usuariosService.UserById(int.Parse(id));

			if (resp == null) { return Json(new { status = 400, message = "Inválido" }); }
			
			var cuentas = Cuentas(resp.cuentasSocio, resp.cuentasEmpleado);
		
			return Json(cuentas);

		}

		[HttpPost]
		public IActionResult RedirigirCuenta(string id, string tipo)
        {
            //model.isvalid redirectToAction("NoEncontrado","Home")
			if(string.IsNullOrEmpty(id)) 
			{
                var strcuentas = HttpContext.Session.GetString("lstCuentas");
			
                var cuentas = JsonConvert.DeserializeObject<List<Cuenta>>(strcuentas);
                ViewBag.Validacion = "Debe seleccionar una cuenta";
				return View("Cuentas", new CuentasViewModel() { cuentas = cuentas, id="",tipo="" });
			}

            string[] partes = id.Split(',');
			if(partes.Length > 1) { id = partes[0]; tipo = partes[1]; }

			return TipoCuenta(id, tipo);

        }

		public IActionResult VerificacionCuentas()
		{
			//var strcuentas = TempData["lstCuentas"] as string;
			//  TempData.Remove("lstCuentas");

			string strcuentas = HttpContext.Session.GetString("lstCuentas");

			if (string.IsNullOrEmpty(strcuentas))
			{ return RedirectToAction("NoEncontrado", "Home"); } //redirigir a pantalla que le diga que no tiene cuentas registradas
            
			var cuentas = JsonConvert.DeserializeObject<List<Cuenta>>(strcuentas);

            //si no tiene cuentas, crear otra vista saludando al usuario sugiriéndole comunicarse a yinsa para su asignación de cuentas, supongo.
            var c = cuentas.FirstOrDefault();
            if (cuentas.Count <= 0) { return RedirectToAction("NoEncontrado", "Home"); }

            if (cuentas.Count == 1) //Verificar numero de cuentas. Si es una, ver el tipo de usuario y redirigir
			{
				//return RedirectToAction("RedirigirCuenta", "Usuarios",new {id=c.codigoS, tipo=c.tipo});
				return TipoCuenta(c.codigoS, c.tipo); //RedirectToAction("RedirigirCuenta", "Usuarios", new{ id=c.codigoS, tipo=c.tipo});
            }


		    return RedirectToAction("Cuentas","Usuarios");
		}
        public IActionResult TipoCuenta(string id, string tipo)
        {
            HttpContext.Session.SetString("codigoS", id);
			HttpContext.Session.SetString("tipoCuenta",tipo);
			switch (tipo)
            {
                case "C":
                    {

                        //mandar id de la cuenta
                        return RedirectToAction("Index", "Clientes", new { id = id });
                    }
                case "S":
                    {

                        //mandar id de la cuenta
                        return RedirectToAction("Index", "Proveedores", new { id = id });
                    }
                case "E":
                    {
                        //mandar id de la cuenta
                        return RedirectToAction("Index", "Colaboradores", new { id = id });
                    }

            }

            return RedirectToAction("NoEncontrado", "Home");

        }
		[HttpPost]
		[Authorize(Policy = "Contador")]
		public async Task<IActionResult> ValidarXML([FromBody]int idDoc)
		{
			var doc = await archivosService.DocumentById(idDoc); //int.Parse(idDoc)

			if (!doc.Status)
				return BadRequest(new { message ="Algo ha salido mal"});

			var fact = await archivosService.ReadXML(doc.Archivo.Archivos.FirstOrDefault(f => f.Extension == "xml").Ruta);

			if(!fact.Status)
				return BadRequest(new { message = "Algo ha salido mal" });

			fact.IdDoc = idDoc;

			var res = await archivosService.ActualizarXML(fact);


			if (!res.Status)
				return BadRequest(new { message = "Algo ha salido mal" });
			//return BadRequest(res);

			return Ok(new { message = fact.Message});
		}

		[HttpGet]
		public IActionResult MenuOptions() 
		{
            var tipoCuenta = HttpContext.Session.GetString("tipoCuenta");
            var rol = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;
            string user = "unknown";

			switch (tipoCuenta)
			{
				case "E": { user = "empleado"; break; }
				case "C":{ user = "cliente";break; }
				case "S": { user = "proveedor";break; }
				default: { user = "unknown"; break; }
			}

			if ( rol == "contador" && tipoCuenta =="E")
			{
				user = "contador";
			}
            if (rol == "admin" && tipoCuenta == "E")
            {
                user = "administrador";
            }


			return Json(new { user = user });
		}

    }
}
