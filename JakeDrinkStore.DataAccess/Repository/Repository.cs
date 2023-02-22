using JakeDrinkStore.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace JakeDrinkStore.DataAccess.Repository
{
    /// <summary>
    ///     Implementaion of genertic functions
    ///     This class will interact with database directly 
    /// </summary>
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
            // This code is to check Navigation Property name
            //_db.ShoppingCarts.Include(sc => sc.Product).Include(sc => sc.ProductTag);
        }

        // includeProperites - "Category,CoverType"
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.ToList();
        }

        public void Add(T entity)
        {
            dbSet.Add(entity);
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = true)
        {
            IQueryable<T> query = dbSet;

            query = query.Where(filter);

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return query.FirstOrDefault();
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public Task<bool> AnyAsync(Expression<Func<T, bool>> filter)
        {
            IQueryable<T> query = dbSet;

            query = query.Where(filter);

            return query.AnyAsync();
        }
    }
}
