using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using YINSA_INTRANET.Models;
using YINSA_INTRANET.Servicios;

namespace YINSA_INTRANET.Filtros
{
	public class CrearFacturasProveedorFilter : IAuthorizationFilter
	{

		private readonly IUsuariosService usuariosService;

		public CrearFacturasProveedorFilter(IUsuariosService usuariosService)
		{
			this.usuariosService = usuariosService;
		}

		public async void OnAuthorization(AuthorizationFilterContext context)
		{
			string username;
			ClaimsPrincipal c = context.HttpContext.User;

			if (c.Identity == null)
			{
				context.Result = new ForbidResult();
			}
			if (!c.Identity.IsAuthenticated)
			{
				//context.HttpContext.Response.Redirect("~/Usuarios/Login");
				context.Result = new ForbidResult();
			}
			username = c.FindFirstValue("UserName");
			var user = await usuariosService.UserByName(username);
			Permiso permiso = new();
			if (user.permisos != null)
				permiso = user.permisos.Find(p => p.IdPermiso == 1);//permiso Facturas de proveedor

			if (!permiso.Crear)
			{ context.Result = new ForbidResult(); }
			//context.ActionArguments["customInfo"] = _customInfo; probablemente para IActionFilter



		}
	}
}
