using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OKN.Core.Models.Entities
{
    [BsonIgnoreExtraElements]
    public class ObjectEntity
    {
        [BsonElement("objectId")]
        public string ObjectId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }

        [BsonElement("latitude")]
        public string Latitude { get; set; }

        [BsonElement("longitude")]
        public string Longitude { get; set; }

        [BsonElement("federal")]
        public BsonDocument Federal { get; set; }

        [BsonElement("type")]
        public EObjectType Type { get; set; }

        [BsonElement("versionInfo")]
        public VersionInfoEntity Version { get; set; }

        [BsonElement("eventsCount")]
        public int EventsCount { get; set; }

        [BsonElement("events")]
        public List<ObjectEventEntity> Events { get; set; }

        [BsonElement("lastEvent")]
        public ObjectEventEntity LastEvent { get; set; }

        [BsonElement("mainPhoto")]
        public FileEntity MainPhoto { get; set; }

        [BsonElement("photos")]
        public List<FileEntity> Photos { get; set; }

        public ObjectEntity() { }

        internal void AddEvent(ObjectEventEntity eventEntity)
        {
            if (Events == null)
            {
                Events = new List<ObjectEventEntity>();
            }

            Events.Add(eventEntity);
            EventsCount = Events.Count;
        }
    }
}