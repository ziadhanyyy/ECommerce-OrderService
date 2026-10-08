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
    public class OrderToOrderResponseMappingProfile : Profile
    {
        public OrderToOrderResponseMappingProfile()
        {
            CreateMap<Order, OrderResponse>()
              .ForMember(dest => dest.OrderID, opt => opt.MapFrom(src => src._id))
              .ForMember(dest => dest.UserID, opt => opt.MapFrom(src => src.UserId))
              .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
              .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.Items))
              .ForMember(dest => dest.TotalBill, opt => opt.MapFrom(src => src.Total));
        }
    }
}
