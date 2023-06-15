﻿namespace RecordHub.CatalogService.Application.DTO
{
    public class RecordDTO
    {
        public Guid Id { get; set; }
        public short Radius { get; set; }
        public int Year { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public LabelDTO Label { get; set; }
        public IList<StyleDTO> Styles { get; set; }
        public string Country { get; set; }
        public ArtistDTO Artist { get; set; }
    }
}
