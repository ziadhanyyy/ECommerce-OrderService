using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrderService.BLL.DTO;
using OrderService.BLL.HttpClients;
using OrderService.BLL.Interfaces;
using OrderService.Data.Entities;
using OrderService.Data.Interface;
using OrderService.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IValidator<OrderItemAddRequest> _orderItemAddRequestValidator;
        private readonly IValidator<OrderAddRequest> _orderAddRequestValidator;
        private readonly IValidator<OrderItemUpdateRequest> _orderItemUpdateRequestValidator;
        private readonly IValidator<OrderUpdateRequest> _orderUpdateRequestValidator;
        private readonly IMapper _mapper;
        private readonly ProductsMicroserviceClient _productsMicroserviceClient;
        private readonly UsersMicroserviceClient _usersMicroserviceClient;
        public OrderService(IOrderRepository orderRepository, IValidator<OrderItemAddRequest> orderItemAddRequestValidator, IValidator<OrderAddRequest> orderAddRequestValidator, IValidator<OrderItemUpdateRequest> orderItemUpdateRequestValidator, IValidator<OrderUpdateRequest> orderUpdateRequestValidator, IMapper mapper, ProductsMicroserviceClient productsMicroserviceClient, UsersMicroserviceClient usersMicroserviceClient)
        {
            _orderRepository = orderRepository;
            _orderItemAddRequestValidator = orderItemAddRequestValidator;
            _orderAddRequestValidator = orderAddRequestValidator;
            _orderItemUpdateRequestValidator = orderItemUpdateRequestValidator;
            _orderUpdateRequestValidator = orderUpdateRequestValidator;
            _mapper = mapper;
            _productsMicroserviceClient = productsMicroserviceClient;
            _usersMicroserviceClient= usersMicroserviceClient;

        }

        public async Task<OrderResponse?> AddOrder(OrderAddRequest order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            ValidationResult orderValidation = await _orderAddRequestValidator.ValidateAsync(order);
            if (!orderValidation.IsValid)
            {
                string errors = string.Join(", ", orderValidation.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errors);
            }

            // Dictionary for fast lookup by product id
            var products = new Dictionary<Guid, ProductDto>();

            foreach (OrderItemAddRequest item in order.items)
            {
                ValidationResult itemValidation = await _orderItemAddRequestValidator.ValidateAsync(item);
                if (!itemValidation.IsValid)
                {
                    string errors = string.Join(", ", itemValidation.Errors.Select(e => e.ErrorMessage));
                    throw new ArgumentException(errors);
                }

                // Avoid calling the microservice twice for the same product
                if (products.ContainsKey(item.ProductID))
                    continue;

                ProductDto? product = await _productsMicroserviceClient.GetProductById(item.ProductID);
                if (product == null)
                    throw new ArgumentException($"Invalid Product ID: {item.ProductID}");

                products[item.ProductID] = product;
            }

            UserDTO? user = await _usersMicroserviceClient.GetUserById(order.userid);
            if (user == null)
                throw new ArgumentException("Invalid User ID");

            Order orderInput = _mapper.Map<Order>(order);

            foreach (OrderItem orderItem in orderInput.Items)
            {
                // Take the price from the product service, NOT from the client
                orderItem.UnitPrice = products[orderItem.ProductId].price;
                orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice;
            }

            orderInput.Total = orderInput.Items.Sum(i => i.TotalPrice);

            Order? addedOrder = await _orderRepository.AddOrderAsync(orderInput);
            if (addedOrder == null)
                return null;

            OrderResponse addedOrderResponse = _mapper.Map<OrderResponse>(addedOrder);

            foreach (OrderItemResponse itemResponse in addedOrderResponse.OrderItems)
            {
                if (products.TryGetValue(itemResponse.ProductID, out ProductDto? productDto))
                {
                    _mapper.Map(productDto, itemResponse);
                }
            }

            _mapper.Map(user, addedOrderResponse);

            return addedOrderResponse;
        }

        public async Task<bool> DeleteOrder(Guid orderId)
        {
           return await  _orderRepository.DeleteOrderAsync(orderId);
        }

        public async Task<OrderResponse?> GetOrderByCondition(FilterDefinition<Order> filter)
        {
            Order? order = await _orderRepository.GetOrderByConditionAsync(filter);
            if (order == null)
                return null;

            OrderResponse orderResponse = _mapper.Map<OrderResponse>(order);


            if (orderResponse != null)
            {
                foreach (OrderItemResponse orderItemResponse in orderResponse.OrderItems)
                {
                    ProductDto? productDTO = await _productsMicroserviceClient.GetProductById(orderItemResponse.ProductID);

                    if (productDTO == null)
                        continue;

                    _mapper.Map<ProductDto, OrderItemResponse>(productDTO, orderItemResponse);
                }
            }

            if (orderResponse != null)
            {
                UserDTO? user = await _usersMicroserviceClient.GetUserById(orderResponse.UserID);
                if (user != null)
                {
                    _mapper.Map<UserDTO, OrderResponse>(user, orderResponse);
                }
            }

            return orderResponse;
        }

        public async Task<List<OrderResponse?>> GetOrders()
        {
            IEnumerable<Order?> orders = await _orderRepository.GetOrdersAsync();

            IEnumerable<OrderResponse?> orderResponses = _mapper.Map<IEnumerable<OrderResponse>>(orders);

            foreach (OrderResponse? orderResponse in orderResponses)
            {
                if (orderResponse == null)
                {
                    continue;
                }

                foreach (OrderItemResponse orderItemResponse in orderResponse.OrderItems)
                {
                    ProductDto? productDTO = await _productsMicroserviceClient.GetProductById(orderItemResponse.ProductID);

                    if (productDTO == null)
                        continue;

                    _mapper.Map<ProductDto, OrderItemResponse>(productDTO, orderItemResponse);
                }

                UserDTO? user = await _usersMicroserviceClient.GetUserById(orderResponse.UserID);
                if (user != null)
                {
                    _mapper.Map<UserDTO, OrderResponse>(user, orderResponse);
                }
            }


            return orderResponses.ToList();
        }

        public  async Task<List<OrderResponse?>> GetOrdersByCondition(FilterDefinition<Order> filter)
        {
            IEnumerable<Order?> orders = await _orderRepository.GetOrdersByConditionAsync(filter);


            IEnumerable<OrderResponse?> orderResponses = _mapper.Map<IEnumerable<OrderResponse>>(orders);

            foreach (OrderResponse? orderResponse in orderResponses)
            {
                if (orderResponse == null)
                {
                    continue;
                }

                foreach (OrderItemResponse orderItemResponse in orderResponse.OrderItems)
                {
                    ProductDto? productDTO = await _productsMicroserviceClient.GetProductById(orderItemResponse.ProductID);

                    if (productDTO == null)
                        continue;

                    _mapper.Map<ProductDto, OrderItemResponse>(productDTO, orderItemResponse);
                }

                UserDTO? user = await _usersMicroserviceClient.GetUserById(orderResponse.UserID);
                if (user != null)
                {
                    _mapper.Map<UserDTO, OrderResponse>(user, orderResponse);
                }
            }

            return orderResponses.ToList();
        }

        public async Task<OrderResponse?> UpdateOrder(OrderUpdateRequest order)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            ValidationResult orderUpdateRequestValidationResult = await _orderUpdateRequestValidator.ValidateAsync(order);
            if (!orderUpdateRequestValidationResult.IsValid)
            {
                string errors = string.Join(", ", orderUpdateRequestValidationResult.Errors.Select(temp => temp.ErrorMessage));
                throw new ArgumentException(errors);
            }

            Dictionary<Guid, ProductDto> products = new Dictionary<Guid, ProductDto>();

            foreach (OrderItemUpdateRequest orderItemUpdateRequest in order.OrderItems)
            {
                ValidationResult orderItemUpdateRequestValidationResult = await _orderItemUpdateRequestValidator.ValidateAsync(orderItemUpdateRequest);

                if (!orderItemUpdateRequestValidationResult.IsValid)
                {
                    string errors = string.Join(", ", orderItemUpdateRequestValidationResult.Errors.Select(temp => temp.ErrorMessage));
                    throw new ArgumentException(errors);
                }

                ProductDto? product = await _productsMicroserviceClient.GetProductById(orderItemUpdateRequest.ProductID);
                if (product == null)
                {
                    throw new ArgumentException("Invalid Product ID");
                }

                products[orderItemUpdateRequest.ProductID] = product;
            }

            UserDTO? user = await _usersMicroserviceClient.GetUserById(order.UserID);
            if (user == null)
            {
                throw new ArgumentException("Invalid User ID");
            }

            Order orderInput = _mapper.Map<Order>(order);

            foreach (OrderItem orderItem in orderInput.Items)
            {
                orderItem.UnitPrice = products[orderItem.ProductId].price;
                orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice;
            }
            orderInput.Total = orderInput.Items.Sum(temp => temp.TotalPrice);

            Order? updatedOrder = await _orderRepository.UpdateOrderAsync(orderInput);

            if (updatedOrder == null)
            {
                return null;
            }

            OrderResponse updatedOrderResponse = _mapper.Map<OrderResponse>(updatedOrder);

            if (updatedOrderResponse != null)
            {
                foreach (OrderItemResponse orderItemResponse in updatedOrderResponse.OrderItems)
                {
                    products.TryGetValue(orderItemResponse.ProductID, out ProductDto? productDTO);

                    if (productDTO == null)
                        continue;

                    _mapper.Map<ProductDto, OrderItemResponse>(productDTO, orderItemResponse);
                }
            }

            if (updatedOrderResponse != null)
            {
                if (user != null)
                {
                    _mapper.Map<UserDTO, OrderResponse>(user, updatedOrderResponse);
                }
            }

            return updatedOrderResponse;
        }

    }
}
