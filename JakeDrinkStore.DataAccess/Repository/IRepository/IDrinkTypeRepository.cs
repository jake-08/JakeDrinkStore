using JakeDrinkStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JakeDrinkStore.DataAccess.Repository.IRepository
{
    public interface IDrinkTypeRepository : IRepository<DrinkType>
    {
        void Update(DrinkType drinkType);
    }
}
