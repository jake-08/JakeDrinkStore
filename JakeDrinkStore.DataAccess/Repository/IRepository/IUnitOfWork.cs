using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JakeDrinkStore.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ITagRepository Tag { get; }
        IDrinkTypeRepository DrinkType { get; }
        IProductRepository Product { get; }
        IProductTagRepository ProductTag { get; }
        void Save();
    }
}