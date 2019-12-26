using System.Collections.Generic;
using OKN.Core.Identity;

namespace OKN.Core.Models.Commands
{
    public class UpdateObjectCommand : BaseCommandWithInitiator
    {
		public string ObjectId { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
	    public decimal Latitude { get; set; }
	    public decimal Longitude { get; set; }
	    public EObjectType Type { get; set; }
	    
        
        public FileInfo MainPhoto { get; set; }
        
        public List<FileInfo> Photos { get; set; }

        public UpdateObjectCommand(ObjectId objectId)
        {
            ObjectId = objectId.Value;
        }
    }
}
