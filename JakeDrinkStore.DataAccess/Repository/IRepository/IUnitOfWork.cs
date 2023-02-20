namespace JakeDrinkStore.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ITagRepository Tag { get; }
        IDrinkTypeRepository DrinkType { get; }
        IProductRepository Product { get; }
        ProductTagRepository ProductTag { get; }
        ICompanyRepository Company { get; }
        void Save();
    }
}