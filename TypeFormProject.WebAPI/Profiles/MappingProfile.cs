using AutoMapper;
using TypeFormProject.DataAccess.Entities;
using TypeFormProject.Models.DTOs;

namespace TypeFormProject.WebAPI.Profiles
{
        public class AppUserProfile : Profile
        {
            public AppUserProfile()
            {
                CreateMap<Register, AppUser>()
                    .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.Email))
                    .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                    .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(s => s.Phone))
                    .ForMember(d => d.FullName, opt => opt.MapFrom(s => $"{s.FirstName} {s.LastName}"));
            }
        }
    }


