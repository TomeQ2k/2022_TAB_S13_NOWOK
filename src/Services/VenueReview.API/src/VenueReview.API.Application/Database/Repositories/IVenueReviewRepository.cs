﻿using System.Collections.Generic;
using System.Threading.Tasks;
using VenueReview.API.Application.Database.PersistenceModels;

namespace VenueReview.API.Application.Database.Repositories
{
    public interface IVenueReviewRepository
    {
        Task<IReadOnlyList<VenueReviewPersistenceModel>> GetVenueReviewsAsync(long venueId);
        
        Task<VenueReviewPersistenceModel> InsertVenueReviewAsync(Domain.Entities.VenueReview venueReview);
        
        Task<bool> DeleteVenueReviewAsync(string venueReviewId);
    }
}