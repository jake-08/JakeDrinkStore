using JakeDrinkStore.DataAccess.Repository.IRepository;

namespace JakeDrinkStore.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public ICategoryRepository Category { get; private set; }
        public IDrinkTypeRepository DrinkType { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        { 
            _db = db;
            Category = new CategoryRepository(_db);
            DrinkType = new DrinkTypeRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
