using Milky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milky.DataAccess.Repository.IRepository // Declare an interface ICategoryRepository that inherits from IRepository<Category>
{
	public interface IApplicationUserRepository : IRepository<ApplicationUser>
	{
	}
}
