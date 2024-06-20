using Milky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milky.DataAccess.Repository.IRepository // Declare an interface ICategoryRepository that inherits from IRepository<Category>
{
	public interface IOrderHeaderRepository : IRepository<OrderHeader>
	{
		void Update(OrderHeader obj);
		void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
		void UpdateStripePaymentID(int id, string sessionId, string paymentIntentId);
        string GenerateUniqueCode();

        void UpdateUniqueCode(int id, string uniqueCode);
    }
}
