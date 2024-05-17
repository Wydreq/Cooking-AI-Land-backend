using AutoMapper;
using CookingAILand.Core.DAL.Entities;
using CookingAILand.Core.Models;

namespace CookingAILand.Core;

public class CookingMappingProfile : Profile
{
    public CookingMappingProfile()
    {
        CreateMap<CookbookDto, Cookbook>();
        CreateMap<CreateRecipeDto, Recipe>();
        CreateMap<Cookbook, CookbookDto>();
        CreateMap<Cookbook, GetCookbookDto>();
        CreateMap<Recipe, CreateRecipeDto>();
        CreateMap<GetRecipeDto, Recipe>();
        CreateMap<Recipe, GetRecipeDto>();
        CreateMap<GetUserDto, User>();
        CreateMap<User, GetUserDto>();
    }
}