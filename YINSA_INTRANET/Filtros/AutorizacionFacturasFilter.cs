using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Semantics;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Intrinsics.Arm;
using System;
using YINSA_INTRANET.Servicios;
using System.Security.Claims;
using YINSA_INTRANET.Models;

namespace YINSA_INTRANET.Filtros
{
	public class AutorizacionFacturasFilter :ActionFilterAttribute //IAuthorizationFilter
	{
		
		private readonly IUsuariosService usuariosService;

		public AutorizacionFacturasFilter(IUsuariosService usuariosService)
		{			
			this.usuariosService = usuariosService;
		}


		public override async void OnActionExecuting(ActionExecutingContext context)//OnActionExecuting(ActionExecutingContext context)
		{
			string username;
			ClaimsPrincipal c = context.HttpContext.User;

			if (c.Identity == null)
			{
				context.HttpContext.Response.Redirect("~/Home/NoEncontrado");
				//context.Result = new ForbidResult();
				//return;
			}
			if (!c.Identity.IsAuthenticated)
			{
				context.HttpContext.Response.Redirect("~/Home/NoEncontrado");
				//context.Result = new ForbidResult();
				//return;
			}
			username = c.FindFirstValue("UserName");
			var user = await usuariosService.UserByName(username);
						

			if(user.rol == "socio") {
				context.HttpContext.Response.Redirect("~/Home/NoEncontrado");
				//{ context.Result = new ForbidResult();
				//	return;
			}
			Permiso permiso = new();
			if (user.permisos != null)
			{
				permiso = user.permisos.Find(p => p.IdPermiso == 1);//permiso Facturas de proveedor
			}
			if(!permiso.Autorizar)
			{
				context.HttpContext.Response.Redirect("~/Home/NoEncontrado");

				//context.Result = new ForbidResult();
				//return;
			}
			base.OnActionExecuting(context);
			//if (permiso.Autorizar)
			//{
			//	if (user.rol == "empleado")
			//	{ //context.HttpContext.Items["grupos"] = user.filtros.Select(f => f.Id).ToList(); 
			//		context.ActionArguments["grupoprovs"] = user.filtros.Select(f => f.Id).ToList();
			//	}
			//}
			//else { context.Result = new ForbidResult(); }

		}

	
	}
	


}
