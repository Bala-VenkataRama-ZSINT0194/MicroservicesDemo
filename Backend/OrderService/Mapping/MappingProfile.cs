using AutoMapper;
using OrderService.Data.Entities;
using OrderService.Dtos;

namespace OrderService.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrderDto>();
            CreateMap<CreateOrderDto, Order>();
            CreateMap<UpdateOrderDto, Order>();
        }
    }
}
