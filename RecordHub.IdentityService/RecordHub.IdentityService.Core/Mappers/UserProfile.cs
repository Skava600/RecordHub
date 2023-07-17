using AutoMapper;
using RecordHub.IdentityService.Core.DTO;
using RecordHub.IdentityService.Domain.Data.Entities;

namespace RecordHub.IdentityService.Core.Mappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDTO>();
        }
    }
}
