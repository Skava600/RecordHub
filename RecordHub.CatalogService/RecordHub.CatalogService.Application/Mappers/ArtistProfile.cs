using AutoMapper;
using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Domain.Entities;
using RecordHub.CatalogService.Domain.Models;

namespace RecordHub.CatalogService.Application.Mappers
{
    public class ArtistProfile : Profile
    {
        public ArtistProfile()
        {
            CreateMap<Record, RecordDTO>().ForMember(r => r.Country, opt => opt.MapFrom(r => r.Country.Name));
            CreateMap<Artist, ArtistDTO>();
            CreateMap<ArtistModel, Artist>();
        }
    }
}
