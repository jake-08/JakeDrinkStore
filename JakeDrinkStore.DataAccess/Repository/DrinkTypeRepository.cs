using JakeDrinkStore.DataAccess.Repository.IRepository;
using JakeDrinkStore.Models;

namespace JakeDrinkStore.DataAccess.Repository
{
    public class DrinkTypeRepository : Repository<DrinkType>, IDrinkTypeRepository
    {
        // Repository class requires ApplicationDbContext
        private readonly ApplicationDbContext _db;
        public DrinkTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(DrinkType drinkType)
        {
            _db.DrinkTypes.Update(drinkType);
        }
    }
}
