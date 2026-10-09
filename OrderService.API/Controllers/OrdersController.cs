using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using OrderService.BLL.DTO;
using OrderService.BLL.Interfaces;
using OrderService.BLL.Services;
using OrderService.Data.Entities;

namespace OrderService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersController(IOrderService orderService) {
         _orderService = orderService;
        }
        //GET:/api/Orders
        [HttpGet]
        public async Task<IEnumerable<OrderResponse?>> GetOrdersAsync()
        { 
            return await _orderService.GetOrders();
        }


        //GET:/api/Orders/search/orderid/{orderID}
        [HttpGet("search/orderid/{orderID}")]
        public async Task<OrderResponse?> GetOrderByOrderID(Guid orderID)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(temp => temp._id, orderID);

            OrderResponse? order = await _orderService.GetOrderByCondition(filter);
            return order;
        }


        //GET: /api/Orders/search/productid/{productID}
        [HttpGet("search/productid/{productID}")]
        public async Task<IEnumerable<OrderResponse?>> GetOrdersByProductID(Guid productID)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.ElemMatch(temp => temp.Items,
              Builders<OrderItem>.Filter.Eq(tempProduct => tempProduct.ProductId, productID)
              );

            List<OrderResponse?> orders = await _orderService.GetOrdersByCondition(filter);
            return orders;
        }

        //GET: /api/Orders/search/userid/{userID}
        [HttpGet("search/userid/{userID}")]
        public async Task<IEnumerable<OrderResponse?>> GetOrdersByUserID(Guid userID)
        {
            FilterDefinition<Order> filter = Builders<Order>.Filter.Eq(temp => temp.UserId, userID);

            List<OrderResponse?> orders = await _orderService.GetOrdersByCondition(filter);
            return orders;
        }
        //POST api/Orders
        [HttpPost]
        public async Task<IActionResult> Post(OrderAddRequest orderAddRequest)
        {
            if (orderAddRequest == null)
            {
                return BadRequest("Invalid order data");
            }

            OrderResponse? orderResponse = await _orderService.AddOrder(orderAddRequest);

            if (orderResponse == null)
            {
                return Problem("Error in adding order");
            }


            return Created($"api/Orders/search/orderid/{orderResponse?.OrderID}", orderResponse);
        }


        //PUT api/Orders/{orderID}
        [HttpPut("{orderID}")]
        public async Task<IActionResult> Update(Guid orderID, OrderUpdateRequest orderUpdateRequest)
        {
            if (orderUpdateRequest == null)
            {
                return BadRequest("Invalid order data");
            }

            if (orderID != orderUpdateRequest.OrderID)
            {
                return BadRequest("OrderID in the URL doesn't match with the OrderID in the Request body");
            }

            OrderResponse? orderResponse = await _orderService.UpdateOrder(orderUpdateRequest);

            if (orderResponse == null)
            {
                return Problem("Error in updating order");
            }


            return Ok(orderResponse);
        }


        //DELETE api/Orders/{orderID}
        [HttpDelete("{orderID}")]
        public async Task<IActionResult> Delete(Guid orderID)
        {
            if (orderID == Guid.Empty)
            {
                return BadRequest("Invalid order ID");
            }

            bool isDeleted = await _orderService.DeleteOrder(orderID);

            if (!isDeleted)
            {
                return Problem("Error in deleting order");
            }

            return Ok(isDeleted);
        }
    }
}
