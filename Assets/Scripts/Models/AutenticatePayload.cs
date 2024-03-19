namespace Verisoul.Models
{
    [System.Serializable]
    public class AuthenticatePayload
    {
        public string session_id;
        public Account account;
    }
}