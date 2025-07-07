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
        }
    }
