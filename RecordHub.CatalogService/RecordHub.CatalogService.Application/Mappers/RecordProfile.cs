using AutoMapper;
using RecordHub.CatalogService.Application.Converters;
using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Domain.Entities;
using RecordHub.CatalogService.Domain.Models;

namespace RecordHub.CatalogService.Application.Mappers
{
    public class RecordProfile : Profile
    {
        public RecordProfile()
        {
            CreateMap<Artist, ArtistDTO>();
            CreateMap<Label, LabelDTO>();
            CreateMap<Style, StyleDTO>();

            CreateMap<Country, string>()
                .ForMember(dest => dest, opt => opt.MapFrom(src => src.Name));

            CreateMap<RecordModel, Record>().ConvertUsing(new RecordTypeConverter());
        }
    }
}
