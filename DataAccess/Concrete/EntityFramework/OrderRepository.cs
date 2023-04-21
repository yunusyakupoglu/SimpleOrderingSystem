
using System;
using System.Linq;
using Core.DataAccess.EntityFramework;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using DataAccess.Abstract;
using Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using Entities.Dtos;
using Castle.Core.Resource;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete.EntityFramework
{
    public class OrderRepository : EfEntityRepositoryBase<Order, ProjectDbContext>, IOrderRepository
    {
        public OrderRepository(ProjectDbContext context) : base(context)
        {
        }


        public async Task<List<OrderDto>> GetOrderDto()
        {
            var result = await (from o in Context.Orders
                                join c in Context.Customers on o.CustomerId equals c.Id
                                join p in Context.Products on o.ProductId equals p.Id
                                select new OrderDto
                                {
                                    Id = o.Id,
                                    CustomerId = c.Id,
                                    CustomerName = c.CustomerName,
                                    ProductId = p.Id,
                                    ProductName = p.Name,
                                    Stock = o.Stock
                                }).ToListAsync();
            return result;
        }
    }
}
