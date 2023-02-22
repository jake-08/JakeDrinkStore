using JakeDrinkStore.DataAccess.Repository.IRepository;
using JakeDrinkStore.Models;

namespace JakeDrinkStore.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _db;
        public ShoppingCartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public int IncrementCount(ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count += count;
            return shoppingCart.Count;
        }

        public int DecrementCount(ShoppingCart shoppingCart, int count)
        {
            shoppingCart.Count -= count;
            return shoppingCart.Count;
        }

        public int IncrementCaseCount(ShoppingCart shoppingCart, int caseCount)
        {
            shoppingCart.CaseCount += caseCount;
            return shoppingCart.CaseCount;
        }

        public int DecrementCaseCount(ShoppingCart shoppingCart, int caseCount)
        {
            shoppingCart.CaseCount -= caseCount;
            return shoppingCart.CaseCount;
        }
    }
}
