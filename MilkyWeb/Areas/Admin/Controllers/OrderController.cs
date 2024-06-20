using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Milky.DataAccess.Repository.IRepository;
using Milky.Models;
using Milky.Models.ViewModels;
using Milky.Utility;
using Stripe;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace MilkyWeb.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;

        [BindProperty]
		public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
			_unitOfWork = unitOfWork;
			_emailSender = emailSender;
        }

        public IActionResult Index()
		{
			return View();
		}

		public IActionResult Details(int orderId)
		{
			OrderVM = new()
			{
				OrderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
				OrderDetail = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
			};

			return View(OrderVM);
		}

		[HttpPost]
		public IActionResult UpdateDetails()
		{
			var orderHeaderFromDB = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
			orderHeaderFromDB.Name = OrderVM.OrderHeader.Name;
			orderHeaderFromDB.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;

			_unitOfWork.OrderHeader.Update(orderHeaderFromDB);
			_unitOfWork.Save();

			TempData["success"] = "Order Details Updated Successfully";

			return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDB.Id });
		}

		[HttpPost]
		public IActionResult StartProcessing()
		{
			var orderHeader = _unitOfWork.OrderHeader.Get(u=>u.Id ==  OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
			orderHeader.StartedProcessingTime = TimeOnly.FromTimeSpan(DateTime.Now.TimeOfDay);
			_unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
            _unitOfWork.Save();

			_emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "Started Processing",
				$"<p>The seller has started processing your order.</p>"+
				$"<p>Started Processing at: {orderHeader.StartedProcessingTime}</p>");

			TempData["success"] = "Order Processed Successfully";
			return RedirectToAction(nameof(Details), new {orderId = OrderVM.OrderHeader.Id});
		}

        [HttpPost]
        public IActionResult ReadyForPickup()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id, includeProperties:"ApplicationUser");
            orderHeader.FinishedProcessingTime = TimeOnly.FromTimeSpan(DateTime.Now.TimeOfDay);
			_unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusReadyforPickup);
            _unitOfWork.Save();

            _emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "Ready For Pickup",
                $"<p>Order processing has been completed. Your product is now ready for pickup.</p>"+
				$"<p>Please collect the order before next closing time</P>"+
                $"<p>Finished Processing at: {orderHeader.FinishedProcessingTime}</p>"+
                $"<p>You can use the link below to locate us:</p>" +
                "https://maps.app.goo.gl/6sbgMjoSuRAa4fEz5");


            TempData["success"] = "Order is Ready For Pickup";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        public IActionResult OrderCompleted()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeader.OrderPickupTime = TimeOnly.FromTimeSpan(DateTime.Now.TimeOfDay);
			_unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusCompleted);
            _unitOfWork.Save();

            TempData["success"] = "Order Completed Successfully";
            return RedirectToAction(nameof(Details), new { orderId = OrderVM.OrderHeader.Id });
        }

		[HttpPost]
		public IActionResult OrderCancelled()
		{
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.OrderHeader.Id);
			
			if(orderHeader.PaymentStatus == SD.PaymentStatusApproved)
			{
				var options = new RefundCreateOptions
				{
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = orderHeader.PaymentIntentId
				};

				var service = new RefundService();
				Refund refund = service.Create(options); // using refund object to create refund with the above 
														 // given options

				_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusRefunded);
			}
			else
			{
				_unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
			}
			_unitOfWork.Save();
			TempData["Success"] = "Order Cancelled Successfully";
			return RedirectToAction(nameof(Details), new {orderId = OrderVM.OrderHeader.Id});
        }

        #region API CALLS 
        [HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<OrderHeader> objOrderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();

			switch (status)
			{
				case "inprocess":
					objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
					break;

				case "pending":
					objOrderHeaders = objOrderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusPending);
					break;

				case "readyforpickup":
					objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusReadyforPickup);
					break;

				case "completed":
					objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusCompleted);
					break;

				case "approved":
					objOrderHeaders = objOrderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
					break;

				default:
					break;
			}

			return Json(new { data = objOrderHeaders });

		}
		#endregion

	}
}
