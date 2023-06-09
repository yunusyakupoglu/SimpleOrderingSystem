﻿
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.DataAccess;
using Entities;
using Entities.Concrete;
using Entities.Dtos;

namespace DataAccess.Abstract
{
    public interface IStoreRepository : IEntityRepository<Store> 
    {
        Task<List<StoreDto>> GetStoreDto();
        Task<Store> GetStoreByProductId(int productId);
        Task<bool> IsExistStore(int productId, int stock);
    }
}