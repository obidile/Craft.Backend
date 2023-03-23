using AutoMapper;
using Craft.Application.Common.Models;
using Craft.Domain.Entities;

namespace Craft.Application.Common.Mappers;

public class IMapProfile : Profile
{
    public IMapProfile()
    {
        CreateMap<User, UserModel>().ReverseMap();


    }
}
