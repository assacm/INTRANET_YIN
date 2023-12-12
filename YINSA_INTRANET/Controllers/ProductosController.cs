using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace YINSA_INTRANET.Controllers
{
	[Authorize]

	public class ProductosController:Controller
	{
        public ProductosController()
        {
            
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
		[HttpGet]
		public IActionResult FichaTecnica()
		{
			return View();
		}
	}
}
