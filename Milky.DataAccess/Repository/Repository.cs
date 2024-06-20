using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Milky.DataAccess.Data;
using Milky.DataAccess.Repository.IRepository;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Milky.Models.ViewModels;


namespace Milky.DataAccess.Repository
{
	// Define a generic class Repository<T> that implements IRepository<T>
	public class Repository<T> : IRepository<T> where T : class 
	{
		// Private field to hold an instance of the ApplicationDbContext
		private readonly ApplicationDbContext _db; 

		internal DbSet<T> dbSet; // Field named dbSet of type DbSet<T>

        public Repository(ApplicationDbContext db) 
		{
            _db = db; 
			this.dbSet = _db.Set<T>(); //Initialize the dbSet field with the DbSet<T> for the entity type T in the context
			_db.Products.Include(u => u.Category).Include(u=>u.CategoryID);
		}
        public void Add(T entity) // Method to add an entity to the DbSet
		{
			dbSet.Add(entity);
		}

		//Method to retrieve a single entity based on a filter expression
		public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
		{
			IQueryable<T> query;

            if (tracked)
			{ 
			query = dbSet;
            }
			else
			{
				query = dbSet.AsNoTracking();
                
            }
            query = query.Where(filter); // Apply the filter expression to the query
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.FirstOrDefault(); // Return the first or default result of the query
        }

        

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties = null) // Method to retrieve all entities from the DbSet
		{
			IQueryable<T> query = dbSet;
			if (filter != null)
			{
				query = query.Where(filter);
			}
			if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query=query.Include(includeProp);
				}
			}
			return query.ToList(); // Return a list of all entities in the query
		}

		public void Remove(T entity) // Method to remove an entity from the DbSet
		{
			dbSet.Remove(entity);

		}

		// Method to remove a range of entities from the DbSet
		public void RemoveRange(IEnumerable<T> entity)
		{
			dbSet.RemoveRange(entity);
		 
		}
	}
}
