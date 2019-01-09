using System.Collections.Generic;
using EventFlow.Queries;

namespace OKN.Core.Models.Queries
{
    public class ListObjectsForMapQuery : IQuery<List<OknObject>>
    {
        public EObjectType[] Types { get; set; }

        public int ZoomLevel { get; set; }
    }
}
