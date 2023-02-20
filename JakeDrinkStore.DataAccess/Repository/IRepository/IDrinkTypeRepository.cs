using JakeDrinkStore.Models;

namespace JakeDrinkStore.DataAccess.Repository.IRepository
{
    public interface IDrinkTypeRepository : IRepository<DrinkType>
    {
        void Update(DrinkType drinkType);
    }
}
