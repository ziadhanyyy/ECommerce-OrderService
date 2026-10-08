using AutoMapper;
using OrderService.BLL.DTO;
using OrderService.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OrderService.BLL.Mappers
{
    public class OrderAddRequestToOrderMappingProfile : Profile
    {
        public OrderAddRequestToOrderMappingProfile()
        {
            CreateMap<OrderAddRequest, Order>()
              .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.userid))
              .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.orderdate))
              .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.items))
              .ForMember(dest => dest._id, opt => opt.Ignore())
              .ForMember(dest => dest.Total, opt => opt.Ignore());
        }
    }
}
