using AutoMapper;

using GozemApi.Models;

namespace GozemApi
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ApplicationUser, ApplicationUserViewModel>();
            CreateMap<NewApplicationUser, ApplicationUser>()
                .ForMember(dest => dest.UserName,
                    options => { options.MapFrom((newApplicationUser, applicationUser) => newApplicationUser.Email); })
                .ForMember(dest => dest.ProfilePhoto, options => { options.MapFrom((_, __) => (string) null); });
        }
    }
}