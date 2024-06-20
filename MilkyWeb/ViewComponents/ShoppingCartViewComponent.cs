using Microsoft.AspNetCore.Mvc;
using Milky.DataAccess.Repository.IRepository;
using Milky.Utility;
using System.Security.Claims;

namespace MilkyWeb.ViewComponents
{
	public class ShoppingCartViewComponent : ViewComponent
	{
		public readonly IUnitOfWork _unitOfWork;

		public ShoppingCartViewComponent(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;

		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
		
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			if (claim != null)
			{
				if(HttpContext.Session.GetInt32(SD.SessionCart) == null) // if sessioncart is null then get data from database else use the
																		 // extracted data
				{
					HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u=>u.ApplicationUserId == claim.Value).Count());
				}
				return View(HttpContext.Session.GetInt32(SD.SessionCart));
			}
			else
			{
				HttpContext.Session.Clear();
				return View(0);
			}
		}
    }
}
