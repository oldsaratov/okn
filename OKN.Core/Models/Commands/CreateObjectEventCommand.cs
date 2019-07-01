﻿using System;
using System.Collections.Generic;

namespace OKN.Core.Models.Commands
{
    public class CreateObjectEventCommand
    {
        public string ObjectId { get; set; }
        public string EventId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public List<OKNObjectEventLink> Links { get; set; }
        public List<OKNObjectEventImage> Images { get; set; }

        public DateTime OccuredAt { get; set; }
	    
	    public long UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
