namespace MusicBlendHub.Identity.Models
{
    public class AuthToken
    {
        public string TokenString { get; set;}

        public AuthToken(string tokenString)
        {
            TokenString = tokenString;
        }
    }
}
