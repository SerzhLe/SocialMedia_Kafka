using CQRS.Core.Domain;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Cmd.Infrastructure.Configurations;

namespace Post.Cmd.Infrastructure.Repositories
{
    public class EventStoreRepository : IEventStoreRepository
    {
        //collection of documents in Mongo, each document = one event
        private readonly IMongoCollection<EventModel> _eventStoreCollection;

        public EventStoreRepository(IOptions<MongoDbConfig> mongoConfig)
        {
            var mongoClient = new MongoClient(mongoConfig.Value.ConnectionString);

            var database = mongoClient.GetDatabase(mongoConfig.Value.Database);

            _eventStoreCollection = database.GetCollection<EventModel>(mongoConfig.Value.Collection);
        }

        public async Task<List<EventModel>> FindAllByAggregateIdAsync(Guid aggregateId)
        {
            return await _eventStoreCollection.Find(e => e.AggregateId == aggregateId)
                .ToListAsync().ConfigureAwait(false);
            //configureawait - increase performance and prevent deadlocks
        }

        public async Task SaveAsync(EventModel @event)
        {
            await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false);
        }
    }
}
