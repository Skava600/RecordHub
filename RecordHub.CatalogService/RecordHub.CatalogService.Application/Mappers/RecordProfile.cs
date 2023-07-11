using AutoMapper;
using RecordHub.CatalogService.Application.Converters;
using RecordHub.CatalogService.Application.Data;
using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Domain.Entities;
using RecordHub.CatalogService.Domain.Models;

namespace RecordHub.CatalogService.Application.Mappers
{
    public class RecordProfile : Profile
    {
        public RecordProfile() { }
        public RecordProfile(IUnitOfWork unitOfWork) : base()
        {
            CreateMap<Record, RecordDTO>().AfterMap((_, dto) =>
            {
                dto.Artist.Records = null;
            });
            CreateMap<Record, RecordSummaryDTO>();
            CreateMap<RecordModel, Record>().ConvertUsing(new RecordTypeConverter(unitOfWork));
        }
    }
}
