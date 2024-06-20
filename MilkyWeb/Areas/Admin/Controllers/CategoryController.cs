using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Milky.DataAccess.Data;
using Milky.DataAccess.Repository.IRepository;
using Milky.Models;
using Milky.Utility;

namespace MilkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class CategoryController : Controller // defined a new class inherits from controller class of asp.net
    {
        private readonly IUnitOfWork _unitOfWork;     
        public CategoryController(IUnitOfWork unitOfWork) 
                                                          
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList(); //to retrieve a list of categories
                                                                                     // This line retrieves a list of categories from the database and assigns it to a local variable objCategoryList
            return View(objCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost] //used to handle form submissions and other data modifications.
        public IActionResult Create(Category obj)
        {
            // Check if the category name already exists
            if (_unitOfWork.Category.GetAll().Any(c => c.name == obj.name)) //to see if a category with the same name already exists in the database.
            {
                ModelState.AddModelError("name", "Category name already exists.");
            }

            // Check if the display order already exists
            if (_unitOfWork.Category.GetAll().Any(c => c.displayOrder == obj.displayOrder))
            {
                ModelState.AddModelError("displayOrder", "Display order already exists.");
            }

            if (ModelState.IsValid)  //check validations
            {
                _unitOfWork.Category.Add(obj); //add object to database catagory
                _unitOfWork.Save();
                TempData["Success"] = "Category Created Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }


            Category CategoryFromDb = _unitOfWork.Category.Get(u => u.id == id); // Attempts to find a category using id

            if (CategoryFromDb == null)
            {
                return NotFound();
            }
            return View(CategoryFromDb); // If the category is found, it returns a view for editing the category,
        }

        [HttpPost] 
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)  
            {
                _unitOfWork.Category.Update(obj); //add object to database catagory
                _unitOfWork.Save();
                TempData["Success"] = "Category Updated Successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
			List<Category> objCategoryList = _unitOfWork.Category.GetAll().ToList();
            return Json(new {data = objCategoryList});
		}

		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var categoryToBeDeleted = _unitOfWork.Category.Get(u => u.id == id);

			if (categoryToBeDeleted == null)
			{
				return Json(new { success = false, message = "Error while Deleting" });
			}
			
			_unitOfWork.Category.Remove(categoryToBeDeleted);
			_unitOfWork.Save();
			

			return Json(new { success = true, message = "File Deleted Successfully" });
		}

		#endregion
	}
}
