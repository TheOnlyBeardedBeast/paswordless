using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Passwordless.Services;

namespace Passwordless.Controllers
{
    public class LoginInput
    {
        public string Email { get; set; }
    }

    public class CheckInput
    {
        public string RequestCode { get; set; }
    }

    // TODO: add cache value TTLs
    // TODO: add user check and creation
    public class AuthController: Controller 
    {
        protected readonly IMailService mailService;
        protected readonly IMemoryCache cache;
        protected readonly IAuthService authService;

        public AuthController(IMailService mailService, IMemoryCache cache, IAuthService authService)
        {
            this.mailService = mailService;
            this.cache = cache;
            this.authService = authService;
        }

        // Initially called by frontend
        [HttpPost("login")]
        public async Task<string> Login([FromBody] LoginInput input)
        {
            return await this.authService.Login(input.Email);
        }

        // Called from email message
        [HttpGet("confirm/{confirmationCode}")]
        public string Confirm([FromRoute] string confirmationCode)
        {
            var requestCode = cache.Get<string>($"confirmation:{confirmationCode}");

            if (requestCode is null)
            {
                return "Invalid code";
            }

            cache.Remove($"confirmation:{confirmationCode}");
            cache.Set($"request:{requestCode}",true);

            return "Login confirmed";

        }

        // Intervally checked frontend after initial login call success
        [HttpPost("check")]
        public string Check([FromBody] CheckInput input)
        {
            bool requestState;

            if (!cache.TryGetValue<bool>($"request:{input.RequestCode}",out requestState))
            {
                return "Error";
            }

            if (!requestState)
            {
                // TODO: return empty response
                return "check again later";
            }

            // TODO: return access and refresh token
            cache.Remove($"request:{input.RequestCode}");
            return "logged";
        }

        public async Task Refresh()
        {
            // TODO
            // Create new accesstoken (refreshtoken?, invalidate?)
            // return new tokens
        }
    }
}
