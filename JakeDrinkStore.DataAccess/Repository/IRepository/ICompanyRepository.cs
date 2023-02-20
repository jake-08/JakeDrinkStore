using JakeDrinkStore.Models;

namespace JakeDrinkStore.DataAccess.Repository.IRepository
{
    public interface ICompanyRepository : IRepository<Company>
    {
        void Update(Company company);
    }
}
