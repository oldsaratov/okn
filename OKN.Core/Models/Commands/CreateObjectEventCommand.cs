using System;
using System.Collections.Generic;

namespace OKN.Core.Models.Commands
{
    public class CreateObjectEventCommand : BaseCommandWithInitiator
    {
        public CreateObjectEventCommand(string objectId, string eventId)
        {
            ObjectId = objectId;
            EventId = eventId;
            
            Files = new List<FileInfo>();
            Photos = new List<FileInfo>();
        }
        
        public string ObjectId { get; }

        public string EventId { get; }

        public EObjectEventType Type { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime OccuredAt { get; set; }

        public List<FileInfo> Photos { get; set; }
        
        public List<FileInfo> Files { get; set; }

    }
}
