﻿using Library.Shared.Models.Venue.Enums;

namespace Library.Shared.Models.Venue.Dtos
{
    public record VenueDto
    {
        public long VenueId { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public long LocationId { get; init; }
        public string CategoryId { get; init; }
        public long? CreatorId { get; init; }
        public VenuePersistState PersistState { get; init; }
    }
}