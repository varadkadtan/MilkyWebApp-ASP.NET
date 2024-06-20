using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milky.DataAccess.Repository.IRepository
{
	public interface IUnitOfWork
	{
		//This property provides a way to access an instance that implements the ICategoryRepository interface.
		ICategoryRepository Category { get; }
		IProductRepository Product { get; }

		IShoppingCartRepository ShoppingCart { get; }

		IApplicationUserRepository ApplicationUser { get; }

		IOrderDetailRepository OrderDetail { get; }

		IOrderHeaderRepository OrderHeader { get; }

		void Save();
	}
}
