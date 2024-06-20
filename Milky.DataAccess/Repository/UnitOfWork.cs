using Milky.DataAccess.Data;
using Milky.DataAccess.Repository.IRepository;
using Milky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milky.DataAccess.Repository
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly ApplicationDbContext _db;

		// This public property provides both read and write access to an instance that implements the ICategoryRepository interface.
		public ICategoryRepository Category { get; private set; }
		public IProductRepository Product { get; private set; }

		public IApplicationUserRepository ApplicationUser { get; private set; }

		public IShoppingCartRepository ShoppingCart { get; private set; }

		public IOrderHeaderRepository OrderHeader { get; private set; }

		public IOrderDetailRepository OrderDetail { get; private set; }

		// Constructor that takes an instance of ApplicationDbContext and calls the base class constructor with it
		public UnitOfWork(ApplicationDbContext db)

		{
			_db = db;

			// Initializing the Category property with a new instance of CategoryRepository, passing the ApplicationDbContext instance
			Category = new CategoryRepository(_db);
			Product = new ProductRepository(_db);
			ShoppingCart = new ShoppingCartRepository(_db);
			ApplicationUser =new ApplicationUserRepository(_db);
			OrderHeader = new OrderHeaderRepository(_db);
			OrderDetail = new OrderDetailRepository(_db);

		}


		// Implementation of the Save method declared in the IUnitOfWork interface
		public void Save()
		{
			_db.SaveChanges();
		}
	}
}
