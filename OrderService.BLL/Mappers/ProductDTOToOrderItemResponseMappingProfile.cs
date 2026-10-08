using AutoMapper;
using OrderService.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.BLL.Mappers
{
    public class ProductDTOToOrderItemResponseMappingProfile : Profile
    {
        public ProductDTOToOrderItemResponseMappingProfile()
        {
            CreateMap<ProductDto, OrderItemResponse>()
              .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.name));
              
        }
    }
}
