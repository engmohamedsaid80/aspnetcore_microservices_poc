using AutoMapper;
using ECommerce.Api.Orders.Db;
using ECommerce.Api.Orders.Interfaces;
using ECommerce.Api.Orders.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Orders.Providers
{
    public class OrderProvider : IOrderProvider
    {
        private readonly OrdersDbContext _dbContext;
        private readonly ILogger<OrderProvider> _logger;
        private readonly IMapper _mapper;
        public OrderProvider(OrdersDbContext dbContext, ILogger<OrderProvider> logger, IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;

            SeedData();
        }

        private void SeedData()
        {
            if(!_dbContext.Orders.Any())
            {
                _dbContext.Orders.Add(new Db.Order
                {
                    Id = 1,
                    CustomerId = 1,
                    OrderDate = DateTime.Parse("2020-05-01"),
                    Total = 200,
                    Items = new List<Db.OrderItem>
                    {
                        new Db.OrderItem{ Id=1, OrderId=1, ProductId=1, Quantity=1, UnitPrice=50},
                        new Db.OrderItem{ Id=2, OrderId=1, ProductId=2, Quantity=2, UnitPrice=50},
                        new Db.OrderItem{ Id=3, OrderId=1, ProductId=3, Quantity=1, UnitPrice=50}
                    }
                });

                _dbContext.Orders.Add(new Db.Order
                {
                    Id = 2,
                    CustomerId = 2,
                    OrderDate = DateTime.Parse("2020-05-08"),
                    Total = 350,
                    Items = new List<Db.OrderItem>
                    {
                        new Db.OrderItem{ Id=4, OrderId=2, ProductId=2, Quantity=2, UnitPrice=50},
                        new Db.OrderItem{ Id=5, OrderId=2, ProductId=1, Quantity=2, UnitPrice=50},
                        new Db.OrderItem{ Id=6, OrderId=2, ProductId=3, Quantity=1, UnitPrice=50}
                    }
                });

                _dbContext.SaveChanges();
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.Order> orders, string ErrorMessage)> GetCustomerOrdersAsync(int customerId)
        {
            try
            {
                var orders = await _dbContext.Orders.Where(o => o.CustomerId == customerId).ToListAsync();

                if(orders != null && orders.Any())
                {
                    var result = _mapper.Map<IEnumerable<Db.Order>, IEnumerable<Models.Order>>(orders);
                    return (true, result, null);
                }

                return (false, null, "Not Found");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }
    }
}
