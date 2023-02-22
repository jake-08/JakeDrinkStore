using JakeDrinkStore.Models;

namespace JakeDrinkStore.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        int IncrementCount(ShoppingCart shoppingCart, int count);
        int DecrementCount(ShoppingCart shoppingCart, int count);
        int IncrementCaseCount(ShoppingCart shoppingCart, int caseCount);
        int DecrementCaseCount(ShoppingCart shoppingCart, int caseCount);
    }
}
