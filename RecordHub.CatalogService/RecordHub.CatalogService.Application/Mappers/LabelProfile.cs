using AutoMapper;
using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Domain.Entities;
using RecordHub.CatalogService.Domain.Models;

namespace RecordHub.CatalogService.Application.Mappers
{
    public class LabelProfile : Profile
    {
        public LabelProfile()
        {
            CreateMap<Label, LabelDTO>();
            CreateMap<LabelModel, Label>();
        }

    }
}
