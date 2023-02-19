using JakeDrinkStore.DataAccess.Repository.IRepository;
using JakeDrinkStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JakeDrinkStore.DataAccess.Repository
{
    public class ProductTagRepository : Repository<ProductTag>, IProductTagRepository
    {
        // Repository class requires ApplicationDbContext
        private readonly ApplicationDbContext _db;
        public ProductTagRepository(ApplicationDbContext db) :base(db)
        {
            _db = db;
        }

        public void Update(ProductTag productTag)
        {
            _db.ProductTags.Update(productTag);
        }
    }
}
