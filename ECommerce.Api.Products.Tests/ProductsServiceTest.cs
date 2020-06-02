using AutoMapper;
using ECommerce.Api.Products.Db;
using ECommerce.Api.Products.Profiles;
using ECommerce.Api.Products.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ECommerce.Api.Products.Tests
{
    public class ProductsServiceTest
    {       
        private void SeedData(ProductsDbContext dbContext)
        {
            for (int i = 1; i <= 10; i++)
            {
                dbContext.Products.Add(new Product
                {
                    Id = i,
                    Name = Guid.NewGuid().ToString(),
                    Inventory = i + 10,
                    Price = (decimal)(i * 3.14)
                });
            }
            dbContext.SaveChanges();   
        }

        [Fact]
        public async Task GetProductsReturnAllProducts()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ProductsDbContext>()
                  .UseInMemoryDatabase(nameof(GetProductsReturnAllProducts))
                  .Options;
            var dbContext = new ProductsDbContext(options);

            SeedData(dbContext);

            var productProfile = new ProductProfile();
            var config = new MapperConfiguration(cfg => cfg.AddProfile(productProfile));
            var mapper = new Mapper(config);

            var productProvider = new ProductsProvider(dbContext, null, mapper);


            //Act
            var result = await productProvider.GetProductsAsync();

            //Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.products.Any());
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public async Task GetProduct_ReturnProduct_UsingValidId()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ProductsDbContext>()
                              .UseInMemoryDatabase(nameof(GetProduct_ReturnProduct_UsingValidId))
                              .Options;
            var dbContext = new ProductsDbContext(options);

            SeedData(dbContext);

            var productProfile = new ProductProfile();
            var config = new MapperConfiguration(cfg => cfg.AddProfile(productProfile));
            var mapper = new Mapper(config);

            var productProvider = new ProductsProvider(dbContext, null, mapper);


            //Act
            var result = await productProvider.GetProductAsync(1);

            //Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.product);
            Assert.Equal(1, result.product.Id);
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public async Task GetProduct_ReturnProduct_UsingInValidId()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<ProductsDbContext>()
                              .UseInMemoryDatabase(nameof(GetProduct_ReturnProduct_UsingInValidId))
                              .Options;
            var dbContext = new ProductsDbContext(options);

            SeedData(dbContext);

            var productProfile = new ProductProfile();
            var config = new MapperConfiguration(cfg => cfg.AddProfile(productProfile));
            var mapper = new Mapper(config);

            var productProvider = new ProductsProvider(dbContext, null, mapper);


            //Act
            var result = await productProvider.GetProductAsync(-1);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.product);
            Assert.NotNull(result.ErrorMessage);
        }
    }
}
