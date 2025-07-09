using ItemManagementSystem.Domain.DataModels;
using ItemManagementSystem.Domain.Dto;
using AutoMapper;

namespace ItemManagementSystem.Application.MappingProfiles;

public class ItemManagementProfile : Profile
{
  public ItemManagementProfile()
  {
    CreateMap<ItemType, ItemTypeDto>().ReverseMap();
    CreateMap<ItemModel, ItemModelDto>().ReverseMap();
    CreateMap<PurchaseRequest, PurchaseRequestDto>().ReverseMap();
    CreateMap<PurchaseRequestItem, PurchaseRequestItemDto>().ReverseMap();
    CreateMap<ItemRequest, ItemRequestDto>().ReverseMap();
    CreateMap<RequestItem, RequestItemDto>().ReverseMap(); 
    CreateMap<ItemRequest, ItemRequestDto>()
    .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.RequestItems));
  }
}
