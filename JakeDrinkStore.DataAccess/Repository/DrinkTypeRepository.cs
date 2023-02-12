using JakeDrinkStore.DataAccess.Repository.IRepository;
using JakeDrinkStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
