using AutoMapper;
using CookingAILand.Core.DAL.Entities;
using CookingAILand.Core.Models;

namespace CookingAILand.Core;

public class CookingMappingProfile : Profile
{
    public CookingMappingProfile()
    {
        CreateMap<CreateCookbookDto, Cookbook>();
    }
}