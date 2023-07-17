using AutoMapper;
using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Domain.Entities;
using RecordHub.CatalogService.Domain.Models;

namespace RecordHub.CatalogService.Application.Mappers
{
    public class StyleProfile : Profile
    {
        public StyleProfile()
        {
            CreateMap<Style, StyleDTO>();
            CreateMap<StyleModel, Style>();
        }
    }
}
