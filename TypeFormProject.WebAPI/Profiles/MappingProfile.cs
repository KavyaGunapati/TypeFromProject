using AutoMapper;
using DTO=TypeFormProject.Models.DTOs;
using Entity = TypeFormProject.DataAccess.Entities;
namespace TypeFormProject.WebAPI.Profiles
{
    public class AppUserProfile : Profile
    {
        public AppUserProfile()
        {
            
            CreateMap<DTO.Register,Entity.AppUser>()
                  .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.Email))
                  .ForMember(d => d.Email, opt => opt.MapFrom(s => s.Email))
                  .ForMember(d => d.PhoneNumber, opt => opt.MapFrom(s => s.Phone))
                 .ForMember(d => d.FullName, opt => opt.MapFrom(s => $"{s.FirstName} {s.LastName}"));

            // Organization Entity -> DTO
            CreateMap<Entity.Organization, DTO.Organization>();

            // CreateOrganization -> Organization entity
            CreateMap<DTO.CreateOrganization, Entity.Organization>()
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // UpdateOrganization -> Organization entity (partial updates)
            CreateMap<DTO.UpdateOrganization, Entity.Organization>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // UserOrgRole entity -> DTO
            CreateMap<Entity.UserOrgRole, DTO.UserOrgRole>()
                .ForMember(d => d.Role, opt => opt.MapFrom(s => (int)s.Role));

        }
    }
    }


