using JakeDrinkStore.DataAccess.Repository.IRepository;
using JakeDrinkStore.Models;

namespace JakeDrinkStore.DataAccess.Repository
{
    public class ProductTagRepository : Repository<ProductTag>
    {
        // Repository class requires ApplicationDbContext
        private readonly ApplicationDbContext _db;
        public ProductTagRepository(ApplicationDbContext db) :base(db)
        {
            _db = db;
        }
    }
}
