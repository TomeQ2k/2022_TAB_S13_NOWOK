﻿using System.Threading;
using System.Threading.Tasks;
using Library.Database.Transaction.Abstractions;
using Library.EventBus;
using Library.Shared.Events.Abstractions;
using Library.Shared.Logging;
using MediatR;
using Venue.API.Application.Abstractions;
using Venue.API.Application.Handlers;

namespace Venue.API.Application.Features.DeleteVenue
{
    public class DeleteVenueCommandHandler : IRequestHandler<DeleteVenueCommand, DeleteVenueResponse>
    {
        private readonly IVenueLocationService _venueLocationService;
        private readonly ITransactionManager _transactionManager;
        private readonly IEventSender _eventSender;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger _logger;

        public DeleteVenueCommandHandler(IVenueLocationService venueLocationService,
            ITransactionManager transactionManager,
            IEventSender eventSender,
            IEventAggregator eventAggregator,
            ILogger logger)
        {
            _venueLocationService = venueLocationService;
            _transactionManager = transactionManager;
            _eventSender = eventSender;
            _eventAggregator = eventAggregator;
            _logger = logger;
        }

        public async Task<DeleteVenueResponse> Handle(DeleteVenueCommand request, CancellationToken cancellationToken)
        {
            using (var scope = _transactionManager.CreateScope())
            {
                _logger.Trace("> Database transaction began");

                var venueWithoutLocation = await _venueLocationService.DeleteLocationFromVenueAsync(request.VenueId);

                await _eventSender.SendEventAsync(EventBusTopics.Venue, venueWithoutLocation.FirstStoredEvent,
                    cancellationToken);

                var response = await new DeleteVenueOrchestrator(_eventAggregator, _logger)
                    .OrchestrateTransactionAsync(venueWithoutLocation.FirstStoredEvent.TransactionId, venueWithoutLocation.FirstStoredEvent.EventId);

                scope.Complete();

                _logger.Trace("< Database transaction committed");

                return new DeleteVenueResponse { DeletedVenueId = venueWithoutLocation.VenueId };
            }
        }
    }
}