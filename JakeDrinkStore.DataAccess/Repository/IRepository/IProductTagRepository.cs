using JakeDrinkStore.Models;

namespace JakeDrinkStore.DataAccess.Repository.IRepository
{
    public interface IProductTagRepository : IRepository<ProductTag>
    {
        void Update(ProductTag productTag);
    }
}
