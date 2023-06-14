using AutoMapper;
using RecordHub.IdentityService.Domain.Data.Entities;
using System.Security.Claims;

namespace RecordHub.IdentityService.Core.Mappers
{
    public class UserProfile : Profile
    {

        public UserProfile()
        {
            CreateMap<List<Claim>, User>()
              .ForMember(user => user.UserName, options =>
                  options.MapFrom(claims => claims.Find(claim =>
                      claim.Type == ClaimTypes.Email).Value))
              .ForMember(user => user.Email, options =>
                  options.MapFrom(claims => claims.Find(claim =>
                      claim.Type == ClaimTypes.Email).Value))

            .ForMember(user => user.Id, options =>
                  options.MapFrom(claims => claims.Find(claim =>
                      claim.Type == ClaimTypes.NameIdentifier).Value));

        }
    }
}
