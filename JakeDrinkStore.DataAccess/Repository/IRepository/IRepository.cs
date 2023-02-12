﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JakeDrinkStore.DataAccess.Repository.IRepository
{
    /// <summary>
    ///     Generic functions for all DbSet
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : class
    {
        T GetFirstOrDefault(Expression<Func<T, bool>> filter);
        IEnumerable<T> GetAll();
        Task<bool> AnyAsync(Expression<Func<T, bool>> filter);
        void Add(T entity);
        void Remove(T entity);
        // Remove more than one entities 
        void RemoveRange(IEnumerable<T> entities);
    }
}
