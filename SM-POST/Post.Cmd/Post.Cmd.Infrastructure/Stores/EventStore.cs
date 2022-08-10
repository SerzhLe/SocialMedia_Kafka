using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Stores
{
    public class EventStore : IEventStore
    {
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly IEventProducer _eventProducer;

        public EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer)
        {
            _eventStoreRepository = eventStoreRepository;
            _eventProducer = eventProducer;
        }

        public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
        {
            var eventStream = await _eventStoreRepository.FindAllByAggregateIdAsync(aggregateId);

            if (eventStream == null || !eventStream.Any())
            {
                throw new AggregateNotFoundException("Incorrect post id provided.");
            }

            return eventStream.OrderBy(e => e.Version).Select(e => e.EventData).ToList();
        }

        public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
        {
            var eventStream = await _eventStoreRepository.FindAllByAggregateIdAsync(aggregateId);

            //optimistic concurrency control check
            if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion)
            {
                throw new ConcurrencyException();
            }

            var version = expectedVersion;

            foreach (var @event in events)
            {
                version++;

                @event.Version = version;

                var eventModel = new EventModel()
                {
                    TimeStamp = DateTime.Now,
                    AggregateId = aggregateId,
                    AggregateType = nameof(PostAggregate),
                    Version = version,
                    EventType = @event.GetType().Name,
                    EventData = @event
                };

                //save to Mongo DB
                await _eventStoreRepository.SaveAsync(eventModel);

                //send to Kafka
                var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");

                await _eventProducer.ProduceAsync(topic!, @event);
            }
        }
    }
}
