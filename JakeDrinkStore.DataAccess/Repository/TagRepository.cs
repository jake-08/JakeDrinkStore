using JakeDrinkStore.DataAccess.Repository.IRepository;
using JakeDrinkStore.Models;

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
