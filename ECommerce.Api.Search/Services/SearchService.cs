using ECommerce.Api.Search.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace ECommerce.Api.Search.Services
{
    public class SearchService : ISearchService
    {
        private readonly IOrdersService _orderService;
        private readonly IProductsService _productsService;
        private readonly ICustomersService _customersService;

        public SearchService(IOrdersService orderService, IProductsService productsService, ICustomersService customersService)
        {
            _orderService = orderService;
            _productsService = productsService;
            _customersService = customersService;
        }

        public async Task<(bool IsSuccess, dynamic SearchResults)> SearchAsync(int customerId)
        {
            var ordersResult = await _orderService.GetOrdersAsync(customerId);
            var productsResult = await _productsService.GetProductsAsync();
            var customersResult = await _customersService.GetCustomerAsync(customerId);

            if(ordersResult.IsSuccess)
            {
                foreach(var order in ordersResult.orders)
                {
                    foreach(var item in order.Items)
                    {
                        item.ProductName = productsResult.IsSuccess ?
                                            productsResult.products.Where(p => p.Id == item.ProductId).FirstOrDefault()?.Name:
                                            "Product information is not available";
                    }
                }
                var result = new
                {
                    Orders = ordersResult.orders,
                    CustomerName = customersResult.IsSuccess ? customersResult.customer.Name: "Customer information is not available",
                    CustomerAddress = customersResult.IsSuccess ? customersResult.customer.Address : "Customer information is not available"
                };

                return (true, result);
            }
            return (false, null);
        }
    }
}
