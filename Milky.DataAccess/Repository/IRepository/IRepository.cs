using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Milky.DataAccess.Repository.IRepository // Declare a namespace for the IRepository interface
{
	public interface IRepository<T> where T : class // Declare a generic interface IRepository<T> where T is a class
	{
		// Method signature to retrieve all entities of type T
		IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter=null, string? includeProperties = null); //multiple category extraction

		// Method signature to retrieve a single entity based on a filter expression
		T Get(Expression<Func<T, bool>> filter, string? includeProperties = null , bool tracked = false); 
		void Add(T entity); // Method signature to add an entity of type T
		void Remove(T entity); // Method signature to remove an entity of type T
		void RemoveRange(IEnumerable<T> entity); // Method signature to remove a range of entities of type T

	}
}
