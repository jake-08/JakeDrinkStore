﻿using JakeDrinkStore.Models;

namespace JakeDrinkStore.DataAccess.Repository.IRepository
{
    public interface ITagRepository : IRepository<Tag>
    {
        void Update(Tag tag);
    }
}
