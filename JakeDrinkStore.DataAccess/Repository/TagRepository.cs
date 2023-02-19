using JakeDrinkStore.DataAccess.Repository.IRepository;
using JakeDrinkStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JakeDrinkStore.DataAccess.Repository
{
    public class TagRepository : Repository<Tag>, ITagRepository
    {
        // Repository class requires ApplicationDbContext
        private readonly ApplicationDbContext _db;
        public TagRepository(ApplicationDbContext db) :base(db)
        {
            _db = db;
        }

        public void Update(Tag category)
        {
            _db.Tags.Update(category);
        }
    }
}
