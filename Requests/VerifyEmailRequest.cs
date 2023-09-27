namespace Pastebin.Identity.Requests
{
    public class VerifyEmailRequest
    {
        public Guid Id { get; set; }
        public string VerificationCode { get; set; }
    }
}
