using AutoMapper;
using BurgerApi.Domain;
using BurgerApi.DTOs;

namespace BurgerApi.Mappings;

/// <summary>
/// Perfil do AutoMapper para mapear Entidades ? DTOs.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Topping, ToppingDto>().ReverseMap();
        CreateMap<CreateToppingDto, Topping>();
        CreateMap<UpdateToppingDto, Topping>();

        CreateMap<Burger, BurgerDto>()
            .ForMember(dest => dest.Toppings,
                opt => opt.MapFrom(src => src.BurgerToppings.Select(bt => bt.Topping)));
        CreateMap<CreateBurgerDto, Burger>();
        CreateMap<UpdateBurgerDto, Burger>();
    }
}