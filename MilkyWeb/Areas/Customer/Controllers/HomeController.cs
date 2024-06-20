using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Milky.DataAccess.Repository.IRepository;
using Milky.Models;
using Milky.Models.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Milky.Utility;
using Microsoft.AspNetCore.Http;

namespace MilkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(IEnumerable<int> selectedCategories)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if(claim != null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u=>u.ApplicationUserId == claim.Value).Count());
            }

            var allCategories = _unitOfWork.Category.GetAll().ToList();
            var allProducts = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

            // Apply category filter if selectedCategories is not null
            if (selectedCategories != null && selectedCategories.Any())
            {
                allProducts = allProducts.Where(p => selectedCategories.Contains(p.CategoryID)).ToList();
            }

            var productVM = new ProductVM
            {
                ProductList = allProducts,
                CategoryList = allCategories.Select(c => new SelectListItem { Value = c.id.ToString(), Text = c.name }),
                SelectedCategoryIds = selectedCategories?.ToList() ?? new List<int>()
            };

            ViewBag.Categories = allCategories;

            return View(productVM);
        }

        //public IActionResult Buy(int id)
        public IActionResult Buy(int id)
        {
            var product = _unitOfWork.Product.Get(u => u.id == id,includeProperties: "Category");

            if (product == null)
            {
                // Handle the case where the product with the specified id is not found.
                return NotFound();
            }

            ShoppingCart cart = new ShoppingCart()
            {
                Product = product,
                Count = 2,
                ProductId = id,
            };

            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Buy(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 0)
            {
                // Handle the case where Count is invalid or Product is not set.
                return BadRequest("Invalid shopping cart data.");
            }

            // Set ShoppingCart.Id to null or 0 before saving
            shoppingCart.Id = 0; 

            var claimsIdentity = (ClaimsIdentity)User.Identity; //for getting user id convert the data from User.Identity to ClaimsIdentity
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCart.ApplicationUserId = userId;


            //checks if there is already a shopping cart in the database for the given userId and ProductId.
            //if cart exist retrieves the information and stores in var cartFromDb
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(u=>u.ApplicationUserId == userId && 
            u.ProductId ==shoppingCart.ProductId);

            if (cartFromDb != null) {
                //update cart
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
                _unitOfWork.Save();
            }
            else
            {
                //add cart
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart, 
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId).Count());
            }
            TempData["success"] = "Cart Updated Successfully";
            //_unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Privacy() //handle requests to privacy ur; Home/privacy
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //used to explicitly disable caching for the specific action method to which it is applied.
        public IActionResult Error() //an action to handle error responses
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
