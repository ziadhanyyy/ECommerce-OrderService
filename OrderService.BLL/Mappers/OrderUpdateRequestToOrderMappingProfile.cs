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
    public class OrderUpdateRequestToOrderMappingProfile : Profile
    {
        public OrderUpdateRequestToOrderMappingProfile()
        {
            CreateMap<OrderUpdateRequest, Order>()
              .ForMember(dest => dest._id, opt => opt.MapFrom(src => src.OrderID))
              .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserID))
              .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
              .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems))
              .ForMember(dest => dest.Total, opt => opt.Ignore());
        }
    }
}
