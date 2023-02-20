using JakeDrinkStore.DataAccess.Repository.IRepository;
using JakeDrinkStore.Models;

namespace JakeDrinkStore.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        // Repository class requires ApplicationDbContext
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) :base(db)
        {
            _db = db;
        }

        public void Update(Product product)
        {
            Product? productFromDb = _db.Products.FirstOrDefault(p => p.Id == product.Id);
            if (productFromDb != null)
            {
                productFromDb.Name = product.Name;
                productFromDb.Description = product.Description;
                productFromDb.Brand = product.Brand;
                productFromDb.ListPrice = product.ListPrice;
                productFromDb.CasePrize = product.CasePrize;
                productFromDb.QuantityPerCase = product.QuantityPerCase;
                productFromDb.BulkCasePrice = product.BulkCasePrice;
                productFromDb.MinBulkCase = product.MinBulkCase;
                productFromDb.DrinkTypeId = product.DrinkTypeId;
                productFromDb.ProductTags = product.ProductTags;

                // Only update image when there is image uploaded, otherwise it will be overwritten with null value. 
                if (productFromDb.ImageUrl != null)
                {
                    productFromDb.ImageUrl = product.ImageUrl;
                }
            }
        }
    }
}
