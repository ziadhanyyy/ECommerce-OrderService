using MongoDB.Driver;
using OrderService.BLL.DTO;
using OrderService.BLL.Interfaces;
using OrderService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.BLL.Services
{
    public class OrderService : IOrderService
    {
        public Task<OrderResponse?> AddOrder(OrderAddRequest order)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteOrder(Guid orderId)
        {
            throw new NotImplementedException();
        }

        public Task<OrderResponse?> GetOrderByCondition(FilterDefinition<Order> filter)
        {
            throw new NotImplementedException();
        }

        public Task<List<OrderResponse?>> GetOrders()
        {
            throw new NotImplementedException();
        }

        public Task<List<OrderResponse?>> GetOrdersByCondition(FilterDefinition<Order> filter)
        {
            throw new NotImplementedException();
        }

        public Task<OrderResponse?> UpdateOrder(OrderUpdateRequest order)
        {
            throw new NotImplementedException();
        }
    }
}
