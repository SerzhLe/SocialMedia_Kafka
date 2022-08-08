using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CQRS.Core.Events
{
    public class EventModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        //objectId - is type for Mongo Id
        //bsonrepresentation allows to convert Object id to string and let us to work with string
        public string Id { get; set; }

        public DateTime TimeStamp { get; set; }

        //id of post this event associated with
        public Guid AggregateId { get; set; }

        public string AggregateType { get; set; }

        public int Version { get; set; }

        public string EventType { get; set; }

        //our data
        public BaseEvent EventData { get; set; }
    }
}
