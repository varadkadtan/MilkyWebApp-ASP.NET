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
	public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
	{
		private readonly ApplicationDbContext _db;

		
		public ApplicationUserRepository(ApplicationDbContext db) : base(db) 
		{
            _db= db;
        }

	}
}
