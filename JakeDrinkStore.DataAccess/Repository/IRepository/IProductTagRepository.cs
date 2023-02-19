using JakeDrinkStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JakeDrinkStore.DataAccess.Repository.IRepository
{
    public interface IProductTagRepository : IRepository<ProductTag>
    {
        void Update(ProductTag productTag);
    }
}
