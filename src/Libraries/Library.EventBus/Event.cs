﻿using System;

namespace Library.EventBus
{
    public abstract record Event<TData>
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime CreatedOn { get; init; } = DateTime.UtcNow;

        public abstract string EventType { get; }

        public TData Data { get; init; }
    }
}