using MongoDB.Driver;
using OrderService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Data.Interface
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<IEnumerable<Order?>> GetOrdersByConditionAsync(FilterDefinition<Order> filter);
        Task<Order?> GetOrderByConditionAsync(FilterDefinition<Order> filter);
        Task <Order?> AddOrderAsync(Order order);
        Task<Order?> UpdateOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(Guid id);
    }
}
