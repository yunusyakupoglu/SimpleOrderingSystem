
using System;
using Core.DataAccess;
using Entities;
using Entities.Concrete;
namespace DataAccess.Abstract
{
    public interface IProductRepository : IEntityRepository<Product>
    {
    }
}