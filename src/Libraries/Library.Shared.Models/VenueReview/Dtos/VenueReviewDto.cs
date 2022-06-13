﻿using System;

namespace Library.Shared.Models.VenueReview.Dtos
{
    public record VenueReviewDto
    { 
        public string VenueReviewId { get; init; }
        public long VenueId { get; init; }
        public string Content { get; init; }
        public long CreatorId { get; init; }
        public double Rating { get; init; }
        public DateTime CreatedOn { get; init; }
        public DateTime? ModifiedOn { get; init; }
    }
}