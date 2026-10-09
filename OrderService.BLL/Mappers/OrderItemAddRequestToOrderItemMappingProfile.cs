using AutoMapper;
using OrderService.BLL.DTO;
using OrderService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.BLL.Mappers
{
    public class OrderItemAddRequestToOrderItemMappingProfile:Profile
    {
        public OrderItemAddRequestToOrderItemMappingProfile()
        {
            CreateMap<OrderItemAddRequest,OrderItem>()
                .ForMember(dest=>dest.ProductId,opt=>opt.MapFrom(src=>src.ProductID))
                .ForMember(dest => dest.UnitPrice, opt => opt.Ignore())
      .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
      .ForMember(dest => dest.TotalPrice, opt => opt.Ignore())
      .ForMember(dest => dest._id, opt => opt.Ignore());
        }
    }
}
