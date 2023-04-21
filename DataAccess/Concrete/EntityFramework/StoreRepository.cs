
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

namespace DataAccess.Concrete.EntityFramework
{
    public class StoreRepository : EfEntityRepositoryBase<Store, ProjectDbContext>, IStoreRepository
    {
        public StoreRepository(ProjectDbContext context) : base(context)
        {
        }

        public async Task<List<StoreDto>> GetStoreDto()
        {
            var result = await (from p in Context.Products
                          join s in Context.Stores
                          on p.Id equals s.ProductId
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
    }
}
