﻿using System.Collections.Generic;

namespace OKN.Core.Models.Commands
{
    public class UpdateObjectCommand : BaseCommandWithInitiator
    {
		public string ObjectId { get; }
		public string Name { get; set; }
		public string Description { get; set; }
	    public string Latitude { get; set; }
	    public string Longitude { get; set; }
	    public EObjectType Type { get; set; }
	    
        
        public FileInfo MainPhoto { get; set; }
        
        public List<FileInfo> Photos { get; set; }

        public UpdateObjectCommand(string objectId)
        {
            ObjectId = objectId;
        }
    }
}
