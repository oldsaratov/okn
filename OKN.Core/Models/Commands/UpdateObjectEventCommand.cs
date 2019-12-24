using System;

namespace OKN.Core.Models.Commands
{
    public class UpdateObjectEventCommand
    {
        public UpdateObjectEventCommand(string objectId, string eventId)
        {
            ObjectId = objectId;
            EventId = eventId;
        }

        public string ObjectId { get; }

        public string EventId { get; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime OccuredAt { get; set; }
    }
}