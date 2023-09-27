namespace MusicBlendHub.Identity.Models
{
    public class UnconfirmedUserRecord
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string VerificationCode { get; set; }
        public DateTime DeletionDate { get; set; }

        public UnconfirmedUserRecord(Guid id, string name, string email, string password, string verificationCode, DateTime creationDate)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            VerificationCode = verificationCode;
            DeletionDate = creationDate;
        }

        public UnconfirmedUserRecord()
        {
            
        }
    }
}
