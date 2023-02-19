using JakeDrinkStore.DataAccess.Repository.IRepository;
using System.Reflection.Emit;

namespace JakeDrinkStore.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public ITagRepository Tag { get; private set; }
        public IDrinkTypeRepository DrinkType { get; private set; }
        public IProductRepository Product { get; private set; }
        public IProductTagRepository ProductTag { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        { 
            _db = db;
            Tag = new TagRepository(_db);
            DrinkType = new DrinkTypeRepository(_db);
            Product = new ProductRepository(_db);
            ProductTag = new ProductTagRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
