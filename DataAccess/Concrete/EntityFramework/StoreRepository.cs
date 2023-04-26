
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

namespace DataAccess.Concrete.EntityFramework
{
    public class StoreRepository : EfEntityRepositoryBase<Store, ProjectDbContext>, IStoreRepository
    {
        public StoreRepository(ProjectDbContext context) : base(context)
        {
        }

        public async Task<ProductWithStoreDto> GetDtoAsync(int storeId)
        {
            var result = await (from p in Context.Products
                               join s in Context.Stores
                               on p.Id equals s.ProductId
                               where s.Id == storeId
                               select new ProductWithStoreDto
                               {
                                   ProductId = p.Id,
                                   ProductName = p.Name,
                                   Color = p.Color,
                                   Size = p.Size,
                                   Stock = s.Stock,
                                   IsReady = s.isReady,
                                   Id = s.Id,
                                   CreatedDate = s.CreatedDate,
                                   CreatedUserId = s.CreatedUserId,
                                   isDeleted = s.isDeleted,
                                   Status = s.Status
                               }).SingleOrDefaultAsync();
            return result;
        }

        public async Task<ProductWithStoreDto> GetProductWithStore(int productId)
        {
            var result = await (from p in Context.Products
                                join s in Context.Stores
                                on p.Id equals s.ProductId
                                where p.Id == productId
                                select new ProductWithStoreDto
                                {
                                    ProductId = p.Id,
                                    ProductName = p.Name,
                                    Color = p.Color,
                                    Size = p.Size,
                                    Stock = s.Stock,
                                    IsReady = s.isReady,
                                    Id = s.Id,
                                    CreatedDate = s.CreatedDate,
                                    CreatedUserId = s.CreatedUserId,
                                    isDeleted = s.isDeleted,
                                    Status = s.Status
                                }).SingleOrDefaultAsync();
            return result;
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
