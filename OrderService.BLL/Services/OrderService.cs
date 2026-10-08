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
            if (order== null)
            {
                throw new ArgumentNullException(nameof(order));
            }
            ValidationResult orderAddRequestValidationResult = await _orderAddRequestValidator.ValidateAsync(order);
            if (!orderAddRequestValidationResult.IsValid)
            {
                string errors = string.Join(", ", orderAddRequestValidationResult.Errors.Select(temp => temp.ErrorMessage));
                throw new ArgumentException(errors);
            }

            List<ProductDto?> products = new List<ProductDto?>();
            foreach (OrderItemAddRequest orderItemAddRequest in order.items)
            {
                ValidationResult orderItemAddRequestValidationResult = await _orderItemAddRequestValidator.ValidateAsync(orderItemAddRequest);

                if (!orderItemAddRequestValidationResult.IsValid)
                {
                    string errors = string.Join(", ", orderItemAddRequestValidationResult.Errors.Select(temp => temp.ErrorMessage));
                    throw new ArgumentException(errors);
                }

                ProductDto? product = await _productsMicroserviceClient.GetProductById(orderItemAddRequest.ProductID);
                if (product == null)
                {
                    throw new ArgumentException("Invalid Product ID");
                }

                products.Add(product);
            }
            UserDTO? user = await _usersMicroserviceClient.GetUserById(order.userid);
            if (user == null)
            {
                throw new ArgumentException("Invalid User ID");
            }
            Order orderInput = _mapper.Map<Order>(order); 
            foreach (OrderItem orderItem in orderInput.Items)
            {
                orderItem.TotalPrice = orderItem.Quantity * orderItem.UnitPrice;
            }
            orderInput.Total = orderInput.Items.Sum(temp => temp.TotalPrice);

            Order? addedOrder = await _orderRepository.AddOrderAsync(orderInput);

            if (addedOrder == null)
            {
                return null;
            }

            OrderResponse addedOrderResponse = _mapper.Map<OrderResponse>(addedOrder); 
            if (addedOrderResponse != null)
            {
                foreach (OrderItemResponse orderItemResponse in addedOrderResponse.OrderItems)
                {
                    ProductDto? productDTO = products.Where(temp => temp.id == orderItemResponse.ProductID).FirstOrDefault();

                    if (productDTO == null)
                        continue;

                    _mapper.Map<ProductDto, OrderItemResponse>(productDTO, orderItemResponse);
                }
            }

            if (addedOrderResponse != null)
            {
                if (user != null)
                {
                    _mapper.Map<UserDTO, OrderResponse>(user, addedOrderResponse);
                }
            }

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

        public async  Task<OrderResponse?> UpdateOrder(OrderUpdateRequest order)
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

            List<ProductDto> products = new List<ProductDto>();

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

                products.Add(product);
            }

            UserDTO? user = await _usersMicroserviceClient.GetUserById(order.UserID);
            if (user == null)
            {
                throw new ArgumentException("Invalid User ID");
            }

            Order orderInput = _mapper.Map<Order>(order); 

            foreach (OrderItem orderItem in orderInput.Items)
            {
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
                    ProductDto? productDTO = products.Where(temp => temp.id == orderItemResponse.ProductID).FirstOrDefault();

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
