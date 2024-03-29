﻿using JakeDrinkStore.DataAccess.Repository.IRepository;
using System.Reflection.Emit;

namespace JakeDrinkStore.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public ITagRepository Tag { get; private set; }
        public IDrinkTypeRepository DrinkType { get; private set; }
        public IProductRepository Product { get; private set; }
        public ProductTagRepository ProductTag { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailsRepository OrderDetails { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        { 
            _db = db;
            Tag = new TagRepository(_db);
            DrinkType = new DrinkTypeRepository(_db);
            Product = new ProductRepository(_db);
            ProductTag = new ProductTagRepository(_db);
            Company = new CompanyRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            OrderDetails = new OrderDetailsRepository(_db);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
