using AutoMapper;
using Ds.Application.Dto;
using Ds.Core;

namespace Ds.Application
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            AllowNullCollections = true;
            CreateMap<PurchaseOrderDto, PurchaseOrder>().ReverseMap();
            CreateMap<OrderDetailDto, OrderDetail>().ReverseMap();
            CreateMap<Supplier, DropdownDto>()
                .ForMember(s=>s.Text, opt => opt.MapFrom(d => d.Name))
                .ReverseMap()
                .ForMember(s=>s.Name, opt=>opt.MapFrom(d=>d.Text));

            CreateMap<Item, DropdownDto>()
                .ForMember(s => s.Text, opt => opt.MapFrom(d => d.Name))
                .ReverseMap()
                .ForMember(s => s.Name, opt => opt.MapFrom(d => d.Text));
        }
    }
}
