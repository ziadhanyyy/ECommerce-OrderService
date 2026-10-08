using MongoDB.Driver;
using OrderService.Data.Entities;
using OrderService.Data.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {

        private readonly IMongoCollection<Order> _orders;
        private readonly string CollectionName = "orders";

        public OrderRepository(IMongoDatabase mongo)
        {
            _orders = mongo.GetCollection<Order>(CollectionName);
        }
        public async Task<Order?> AddOrderAsync(Order order)
        {
            order._id=Guid.NewGuid();
           foreach(OrderItem item in order.Items)
            {
                item._id=Guid.NewGuid();    
            }
           await _orders.InsertOneAsync(order);
            return order;
        }

        public async Task<bool> DeleteOrderAsync(Guid id)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(temp => temp._id, id);

            Order? existingOrder = (await _orders.FindAsync(filter)).FirstOrDefault();

            if (existingOrder == null)
            {
                return false;
            }

            DeleteResult deleteResult = await _orders.DeleteOneAsync(filter);

            return deleteResult.DeletedCount > 0;
        }

        public async Task<Order?> GetOrderByConditionAsync(FilterDefinition<Order> filter)
        {
            return (await _orders.FindAsync(filter)).FirstOrDefault();
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return (await _orders.FindAsync(Builders<Order>.Filter.Empty)).ToList();
        }

        public async Task<IEnumerable<Order?>> GetOrdersByConditionAsync(FilterDefinition<Order> filter)
        {
            return (await _orders.FindAsync(filter)).ToList();
        }

        public  async Task<Order?> UpdateOrderAsync(Order order)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(temp => temp._id, order._id);

            Order? existingOrder = (await _orders.FindAsync(filter)).FirstOrDefault();

            if (existingOrder == null)
            {
                return null;
            }
            order._id = existingOrder._id;

            ReplaceOneResult replaceOneResult = await _orders.ReplaceOneAsync(filter, order);

            return order;
        }
    }
}
