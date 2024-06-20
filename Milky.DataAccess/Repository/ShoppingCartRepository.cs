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
	public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
	{
		private readonly ApplicationDbContext _db;

		public ShoppingCartRepository(ApplicationDbContext db) : base(db) 
		{
            _db= db;
        }

		public void Update(ShoppingCart obj) // Method to update a Category entity in the database
		{
			_db.ShoppingCarts.Update(obj); // Use the Category DbSet from ApplicationDbContext to update the specified Category entity
		}
	}
}
