using ItemManagementSystem.Domain.DataModels;
using ItemManagementSystem.Domain.Dto;
using AutoMapper;
using ItemManagementSystem.Domain.Dto.Request;

namespace ItemManagementSystem.Application.MappingProfiles;

public class ItemManagementProfile : Profile
{
  public ItemManagementProfile()
  {
    CreateMap<ItemType, ItemTypeDto>().ReverseMap();
    CreateMap<ItemType,ItemTypeCreateRequest>().ReverseMap();
    CreateMap<ItemModel, ItemModelDto>().ReverseMap();
    CreateMap<ItemModel, ItemModelCreateDto>().ReverseMap();
    CreateMap<PurchaseRequest, PurchaseRequestDto>().ReverseMap();
    CreateMap<PurchaseRequest, PurchaseRequestCreateDto>().ReverseMap();
    CreateMap<PurchaseRequestItem, PurchaseRequestItemDto>().ReverseMap();
    CreateMap<ItemRequest, ItemRequestDto>().ReverseMap();
    // CreateMap<RequestItem, RequestItemDto>().ReverseMap(); 
    CreateMap<ItemRequest, ItemRequestDto>()
    .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.RequestItems));

    CreateMap<PurchaseRequestItemCreateDto, PurchaseRequestItem>()
    .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
    .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
    .ForMember(dest => dest.PurchaseRequestId, opt => opt.Ignore())
    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore());  

     CreateMap<RequestItemDto, RequestItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.RequestId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

        CreateMap<ItemRequest, ItemRequestDto>()
            .ForMember(dest => dest.Items, opt => opt.Ignore());
  }
}
