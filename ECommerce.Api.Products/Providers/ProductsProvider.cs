using AutoMapper;
using ECommerce.Api.Products.Db;
using ECommerce.Api.Products.Interfaces;
using ECommerce.Api.Products.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Products.Providers
{
    public class ProductsProvider : IProductsProvider
    {
        private readonly ProductsDbContext _dbContext;
        private readonly ILogger<ProductsProvider> _logger;
        private readonly IMapper _mapper;
        public ProductsProvider(ProductsDbContext dbContext, ILogger<ProductsProvider> logger, IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;

            SeedData();
        }

        private void SeedData()
        {
            if(!_dbContext.Products.Any())
            {
                _dbContext.Products.Add(new Db.Product { Id = 1, Name = "Keyboard", Price = 20, Inventory = 100 });
                _dbContext.Products.Add(new Db.Product { Id = 2, Name = "Mouse", Price = 5, Inventory = 200 });
                _dbContext.Products.Add(new Db.Product { Id = 3, Name = "Speakers", Price = 20, Inventory = 100 });
                _dbContext.Products.Add(new Db.Product { Id = 4, Name = "Charger", Price = 50, Inventory = 100 });
                _dbContext.Products.Add(new Db.Product { Id = 5, Name = "Monitor", Price = 200, Inventory = 50 });
                _dbContext.SaveChanges();
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.Product> products, string ErrorMessage)> GetProductsAsync()
        {
            try
            {
                var products = await _dbContext.Products.ToListAsync();

                if(products != null && products.Any())
                {
                    var result = _mapper.Map<IEnumerable<Db.Product>,IEnumerable<Models.Product>>(products);

                    return (true, result, null);
                }

                return (false, null, "Not Found");
            }
            catch(Exception ex)
            {
                _logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, Models.Product product, string ErrorMessage)> GetProductAsync(int id)
        {
            try
            {
                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);

                if (product != null)
                {
                    var result = _mapper.Map<Db.Product, Models.Product>(product);

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
