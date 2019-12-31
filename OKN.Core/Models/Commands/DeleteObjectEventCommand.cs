namespace OKN.Core.Models.Commands
{
    public class DeleteObjectEventCommand : BaseCommandWithInitiator
    {
        public DeleteObjectEventCommand(string objectId, string eventId)
        {
            ObjectId = objectId;
            EventId = eventId;
        }

        public string ObjectId { get; }

        public string EventId { get; }
    }
}