using Milky.DataAccess.Data;
using Milky.DataAccess.Repository.IRepository;
using Milky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Milky.DataAccess.Repository
{
	// Declare a class CategoryRepository that inherits from Repository<Category> and implements ICategoryRepository
	public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
	{
		private readonly ApplicationDbContext _db;

		public OrderHeaderRepository(ApplicationDbContext db) : base(db) 
		{
            _db= db;
        }

        public string GenerateUniqueCode()
        {
            string uniqueCode;
            do
            {
                Random random = new Random();
                uniqueCode = new string(Enumerable.Range(0, 12).Select(_ => (char)('0' + random.Next(10))).ToArray());
            }
            while (_db.OrderHeaders.Any(u => u.UniqueCode == uniqueCode));
            return uniqueCode;
        }

        public void UpdateUniqueCode(int id, string uniqueCode)
		{
			var orderFromDb = _db.OrderHeaders.FirstOrDefault(u=> u.Id == id);
			if (orderFromDb != null)
			{
				orderFromDb.UniqueCode = uniqueCode;
				_db.SaveChanges();
			}
		}


        public void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId)
		{
			var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
			if (orderFromDb != null)
			{
				if (!string.IsNullOrEmpty(sessionId))
				{
					orderFromDb.SessionId = sessionId;
				}
				if (!string.IsNullOrEmpty(paymentIntentId)) // Updated condition to check if not empty
				{
					orderFromDb.PaymentIntentId = paymentIntentId;
					orderFromDb.PaymentDate = DateTime.Now;
				}
			}
		}

		public void Update(OrderHeader obj) // Method to update a Category entity in the database
		{
			_db.OrderHeaders.Update(obj); // Use the Category DbSet from ApplicationDbContext to update the specified Category entity
		}

		public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
		{
			var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
			if(orderFromDb != null)
			{
				orderFromDb.OrderStatus = orderStatus;
			}
			if(!string.IsNullOrEmpty(paymentStatus))
			{
				orderFromDb.PaymentStatus = paymentStatus;
			}
		}
	}
}
