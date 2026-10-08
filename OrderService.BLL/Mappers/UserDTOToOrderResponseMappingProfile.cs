using AutoMapper;
using OrderService.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.BLL.Mappers
{
    public class UserDTOToOrderResponseMappingProfile : Profile
    {
        public UserDTOToOrderResponseMappingProfile()
        {
            CreateMap<UserDTO, OrderResponse>()
              .ForMember(dest => dest.UserPersonName, opt => opt.MapFrom(src => src.Name))
              .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
        }
    }
}
