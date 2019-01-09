using EventFlow.Core;

namespace OKN.Core.Identity
{
    public class ObjectId : IIdentity
    {
        public ObjectId(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}
