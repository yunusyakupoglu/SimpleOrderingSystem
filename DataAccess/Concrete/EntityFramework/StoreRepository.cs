
using System;
using System.Linq;
using Core.DataAccess.EntityFramework;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using DataAccess.Abstract;
using Entities;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Entities.Dtos;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using ServiceStack;
using System.Collections;

namespace DataAccess.Concrete.EntityFramework
{
    public class StoreRepository : EfEntityRepositoryBase<Store, ProjectDbContext>, IStoreRepository
    {
        public StoreRepository(ProjectDbContext context) : base(context)
        {
        }

        public async Task<Store> GetStoreByProductId(int productId)
        {
            return await Context.Stores.Where(x=>x.ProductId == productId && x.isDeleted == false).SingleOrDefaultAsync();
        }

        public async Task<List<StoreDto>> GetStoreDto()
        {
            var result = await (from p in Context.Products
                          join s in Context.Stores
                          on p.Id equals s.ProductId
                          where s.isDeleted == false
                          select new StoreDto()
                          {
                              ProductName = p.Name,
                              Stock = s.Stock,
                              isReady = s.isReady,
                              ProductId = p.Id,
                              Id = s.Id
                          }).ToListAsync();
            return result;
        }

        public async Task<bool> IsExistStore(int productId, int stock)
        {
            return await Context.Stores.AnyAsync(x=> x.ProductId == productId && x.Stock >= stock && x.isReady == true && x.isDeleted == false);
        }
    }
}
