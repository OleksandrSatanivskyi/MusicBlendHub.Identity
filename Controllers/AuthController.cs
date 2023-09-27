using Microsoft.AspNetCore.Mvc;
using MusicBlendHub.Identity.Data.DbContexts;
using MusicBlendHub.Identity.Models;
using MusicBlendHub.Identity.Requests;
using MusicBlendHub.Identity.Services;
using MusicBlendHub.Identity.Validations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MusicBlendHub.Identity.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService AuthService;

        public AuthController(IAuthDbContext dbContext, IConfiguration configuration)
        {
            AuthService = new AuthService(dbContext, configuration);
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request) 
        {
            var token = await AuthService.Login(request);
            return Ok(token);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest request) 
        {
            var validator = new RegisterRequestValidator();
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(error => error.ErrorMessage);
                return BadRequest(errors);
            }

            await AuthService.Register(request);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(AuthToken token)
        {
            await AuthService.VerifyEmail(await JwtToVerifyEmailRequest(token.TokenString));
            return NoContent();
        }

        private async Task<VerifyEmailRequest> JwtToVerifyEmailRequest(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.ReadJwtToken(token);
            var request = new VerifyEmailRequest();

            foreach (Claim claim in jwt.Claims)
            {
                if (claim.Type == "Id")
                    request.Id = Guid.Parse(claim.Value);
                else if (claim.Type == "VerificationCode")
                    request.VerificationCode = claim.Value;
            }

            if (request.Id != Guid.Empty && !string.IsNullOrEmpty(request.VerificationCode)) return request;
            else throw new InvalidOperationException($"Email verification failed: invalid credentials");
        }
    }
}
