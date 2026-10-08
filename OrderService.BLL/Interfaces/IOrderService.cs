using MongoDB.Driver;
using OrderService.BLL.DTO;
using OrderService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.BLL.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderResponse?>> GetOrders();
        Task<List<OrderResponse?>> GetOrdersByCondition(FilterDefinition<Order> filter);
        Task<OrderResponse?> GetOrderByCondition(FilterDefinition<Order> filter);
        Task<OrderResponse?> AddOrder(OrderAddRequest order);
        Task<OrderResponse?> UpdateOrder(OrderUpdateRequest order);
        Task<bool> DeleteOrder(Guid orderId);

    }
}
