using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Milky.DataAccess.Data;
using Milky.DataAccess.Repository.IRepository;
using Milky.Models;
using Milky.Models.ViewModels;
using System.Drawing;
using System.Data;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Milky.Utility;


namespace MilkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles =SD.Role_Admin)]

	public class ProductController : Controller // defined a new class inherits from controller class of asp.net
	{
		private readonly IUnitOfWork _unitOfWork;     
		private readonly IWebHostEnvironment _webHostEnvironment;
		public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)  
																								 
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment; //using dependency injection for accessing wwwroot
		}
		public IActionResult Index()
		{
			List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList(); //to retrieve a list of categories
																											   // This line retrieves a list of categories from the database and assigns it to a local variable objProductList
			return View(objProductList);
		}

		public IActionResult Upsert(int? id)
		{
			ProductVM productVM = new()
			{
				CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
				{
					Text = u.name,
					Value = u.id.ToString()
				}),
				IsTaxIncludedOptions = new List<SelectListItem>
				{
					new SelectListItem { Text = "Yes", Value = "Yes" },
					new SelectListItem { Text = "No", Value = "No" }
				},
				biologicalSourceOptions = new List<SelectListItem>
				{
					new SelectListItem { Text = "Cow", Value = "Cow" },
					new SelectListItem { Text = "Goat", Value = "Goat" },
					new SelectListItem { Text = "Sheep", Value = "Sheep" },
					new SelectListItem { Text = "Buffalo", Value = "Buffalo" },
					new SelectListItem { Text = "Camel", Value = "Camel" },
					new SelectListItem { Text = "Horse", Value = "Horse" },
					new SelectListItem { Text = "Reindeer", Value = "Reindeer" },
					new SelectListItem { Text = "Yak", Value = "Yak" },
				},
				flavorOptions = new List<SelectListItem>
				{
					new SelectListItem { Text = "Whole Milk", Value = "Whole Milk" },
			new SelectListItem { Text = "Skimmed Milk", Value = "Skimmed Milk" },
			new SelectListItem { Text = "1% Milk", Value = "1% Milk" },
			new SelectListItem { Text = "2% Milk", Value = "2% Milk" },
			new SelectListItem { Text = "Toned Milk", Value = "Toned Milk" },
			new SelectListItem { Text = "Homogenized Milk", Value = "Homogenized Milk" },
			new SelectListItem { Text = "Raw Milk", Value = "Raw Milk" },
			new SelectListItem { Text = "Organic Milk", Value = "Organic Milk" },
			new SelectListItem { Text = "Flavored Milk", Value = "Flavored Milk" },
			new SelectListItem { Text = "Fortified Milk", Value = "Fortified Milk" },
				},
				UnitOptions = new List<SelectListItem>
		{
			new SelectListItem { Value = "Milliliter", Text = "Milliliter" },
			new SelectListItem { Value = "Liter", Text = "Liter" }
		},
				ItemFormOptions =new List<SelectListItem> 
				{ new SelectListItem { Text = "Liquid", Value="Liquid"},
				new SelectListItem { Text ="Solid",Value="Solid"},
				new SelectListItem { Text ="Semi-Soilid",Value="Semi-Solid"},
				new SelectListItem { Text ="Powdered",Value="Powdered"},
                new SelectListItem { Text ="Frozen",Value="Frozen"}
                },
				DietTypeOptions =new List<SelectListItem> {
				 new SelectListItem { Text = "Vegetarian", Value="Vegetarian"},
				 new SelectListItem { Text = "Non-Vegetarian", Value="Non-Vegetarian"},
				},
				isItemInStockOptions =new List<SelectListItem>
				{
					new SelectListItem{Text= "In Stock", Value="In Stock"},
					new SelectListItem{Text="Out Of Stock",Value="Out Of Stock"}
				},
				Product = new Product()
			};
			if (id == null || id == 0)
			{
				//create
				return View(productVM);
			}
			else
			{
				//update
				productVM.Product = _unitOfWork.Product.Get(u => u.id == id);
				return View(productVM);
			}

		}
		[HttpPost]
		public IActionResult Upsert(ProductVM productVM, IFormFile? file)
		{
			if (ModelState.IsValid)
			{
				string wwwRootPath = _webHostEnvironment.WebRootPath;
				if (file != null)
				{
					string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
					string productPath = Path.Combine(wwwRootPath, @"images\product");

					if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
					{
						//delete the old image
						var oldImagePath =
							Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

						if (System.IO.File.Exists(oldImagePath))
						{
							System.IO.File.Delete(oldImagePath);
						}
					}

					using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
					{
						file.CopyTo(fileStream);
					}
					productVM.Product.ImageUrl = @"\images\product\" + fileName;
				}

				// Check if the condition is met
				if (productVM.Product.MaxNumberOfItemsInStock == 0 && productVM.Product.isItemInStock == "In Stock")
				{
					// Change the value to "Out Of Stock"
					productVM.Product.isItemInStock = "Out Of Stock";
				}

				_unitOfWork.Product.Update(productVM.Product);  // Update the product entity
				_unitOfWork.Save();  // Save changes


				// Concatenate NetQuantity and SelectedUnit
				if (!string.IsNullOrEmpty(productVM.SelectedUnit))
				{
					productVM.Product.NetQuantity = $"{productVM.Product.NetQuantity} {productVM.SelectedUnit}";
				}

				if (productVM.Product.id == 0)
				{
					_unitOfWork.Product.Add(productVM.Product);
				}
				else
				{
					_unitOfWork.Product.Update(productVM.Product);
				}

				_unitOfWork.Save();
				TempData["success"] = "Product created successfully";
				return RedirectToAction("Index");
			}
			else
			{
				productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
				{
					Text = u.name,
					Value = u.id.ToString()
				});
				return View(productVM);
			}
		}

		#region API CALLS 
		[HttpGet]
		public IActionResult GetAll()
		{
			List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
			return Json(new { data = objProductList });

		}


		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var productToBeDeleted = _unitOfWork.Product.Get(u => u.id == id);
			if (productToBeDeleted == null)
			{
				return Json(new { success = false, message = "Error while Deleting" });
			}
			//delete the old image
			var oldImagePath =
				Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));

			if (System.IO.File.Exists(oldImagePath))
			{
				System.IO.File.Delete(oldImagePath);
			}
			_unitOfWork.Product.Remove(productToBeDeleted);
			_unitOfWork.Save();

			return Json(new { success = true, message = "File Deleted Successfully" });

		}

		#endregion
	}
}

