using Microsoft.AspNetCore.Mvc;
using Milky.DataAccess.Repository.IRepository;
using Milky.Models;
using Milky.Models.ViewModels;
using System.Security.Claims;

namespace MilkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderVM orderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork; 
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            // Fetch all order headers for the specific user
            var orderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");

            // Create a list to store OrderVM objects
            var orderVMs = new List<OrderVM>();

            // Loop through each order header to get its associated order details
            foreach (var orderHeader in orderHeaders)
            {
                // Fetch order details for the current order header
                var orderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderHeader.Id, includeProperties: "Product");

                // Create an OrderVM object with the current order header and its associated order details
                var orderVM = new OrderVM
                {
                    OrderHeader = orderHeader,
                    OrderDetail = orderDetails
                };

                // Add the OrderVM object to the list
                orderVMs.Add(orderVM);
            }
            return View(orderVMs);
        }
        // Pass the list of OrderVM objects to the view
    }
}
