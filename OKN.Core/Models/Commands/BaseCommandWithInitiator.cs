namespace OKN.Core.Models.Commands
{
    public class BaseCommandWithInitiator
    {
        internal string UserId { get; private set; }
        
        public string UserName { get; private set; }
        
        public string Email { get; private set; }
        
        public void SetCreator(string userId, string userName, string email)
        {
            UserId = userId;
            UserName = userName;
            Email = email;
        }
    }
}