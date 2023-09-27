using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MusicBlendHub.Identity.Data.DbContexts;
using MusicBlendHub.Identity.Models;
using MusicBlendHub.Identity.Requests;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MusicBlendHub.Identity.Services
{
    public class AuthService
    {
        public IAuthDbContext DbContext { get; set; }
        public EmailService EmailService { get; set; }
        private readonly IConfiguration Configuration;

        public AuthService(IAuthDbContext dbContext,  IConfiguration configuration)
        {
            DbContext = dbContext;
            EmailService = new EmailService();
            Configuration = configuration;
        }

        public async Task<AuthToken> Login(LoginRequest request)
        {
            var user = await DbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);
            if (user == null) throw new InvalidOperationException($"Login for {request.Email} failed: invalid credentials");

            return await Authenticate(user);
        }

        public async Task Register(RegisterRequest request)
        {
            var u = await DbContext.Users.FirstOrDefaultAsync(user => user.Email == request.Email);
            if (u != null) throw new Exception("User with this email is registered.");
            if (request.Password != request.ConfirmPassword) throw new Exception("Passwords do not match.");

            string pincode = await GeneratePinCode();
            var id = Guid.NewGuid();
            var user = new UnconfirmedUserRecord
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                Password = request.Password,
                VerificationCode = pincode,
                DeletionDate = DateTime.UtcNow.AddDays(1)
            };

            string link = $"http://localhost:4200/auth/verify-email/token={await GenerateEmailConfirmationToken(user)}";
            var existingUnconfirmedUserRecord = await DbContext.UnconfirmedUserRecords.FirstOrDefaultAsync(ur => ur.Email == request.Email);
            if (existingUnconfirmedUserRecord != null) DbContext.UnconfirmedUserRecords.Remove(existingUnconfirmedUserRecord);

            await DbContext.UnconfirmedUserRecords.AddAsync(user);
            await EmailService.SendEmailAsync(request.Email, link);
            await DbContext.SaveChangesAsync();
        }

        public async Task VerifyEmail(VerifyEmailRequest request)
        {
            var unconfirmedUserRecord = await DbContext.UnconfirmedUserRecords.FirstOrDefaultAsync(u => u.Id == request.Id && u.VerificationCode == request.VerificationCode);
            if(unconfirmedUserRecord == null) throw new InvalidOperationException($"Email verification failed: invalid credentials");

            var user = new User(Guid.NewGuid(), unconfirmedUserRecord.Name, unconfirmedUserRecord.Email, unconfirmedUserRecord.Password);
            await DbContext.Users.AddAsync(user);
            DbContext.UnconfirmedUserRecords.Remove(unconfirmedUserRecord);
            await DbContext.SaveChangesAsync();
        }

        private async Task<AuthToken> Authenticate(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(Configuration["Auth:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = Configuration["Auth:Issuer"],
                Audience = Configuration["Auth:Audience"],
                Expires = DateTime.UtcNow.AddHours(int.Parse(Configuration["Auth:TokenLifetime"])),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim("Name", user.Name),
                    new Claim("Email", user.Email)
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthToken(tokenHandler.WriteToken(token));
        }

        private async Task<string> GenerateEmailConfirmationToken(UnconfirmedUserRecord user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(Configuration["Auth:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = Configuration["Auth:Issuer"],
                Audience = Configuration["Auth:Audience"],
                Expires = DateTime.UtcNow.AddHours(int.Parse(Configuration["Auth:TokenLifetime"])),
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim("VerificationCode", user.VerificationCode)
                }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthToken(tokenHandler.WriteToken(token)).TokenString;
        }

        private async Task<string> GeneratePinCode()
        {
            int pincode = 0;
            var random = new Random();
            while (pincode.ToString().Length < 5)
            {
                pincode *= 10;
                pincode += random.Next(0, 10);
            }

            return pincode.ToString();
        }
    }
}
