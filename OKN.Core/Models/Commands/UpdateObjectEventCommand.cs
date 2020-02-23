using System;
using System.Collections.Generic;

namespace OKN.Core.Models.Commands
{
    public class UpdateObjectEventCommand : BaseCommandWithInitiator
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
        
        public List<FileInfo> Photos { get; set; }
        
        public List<FileInfo> Files { get; set; }
    }
}