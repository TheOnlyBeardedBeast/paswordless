using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Passwordless.Services;

namespace Passwordless.Controllers
{
    // TODO: add cache value TTLs
    // TODO: add user check and creation
    public class AuthController: Controller 
    {
        protected readonly IMailService mailService;
        protected readonly IMemoryCache cache;

        public AuthController(IMailService mailService, IMemoryCache cache)
        {
            this.mailService = mailService;
            this.cache = cache;
        }

        // Initially called by frontend
        [HttpPost("login")]
        public async Task<string> Login([FromBody]string email)
        {
            var confirmationCode = Guid.NewGuid().ToString();
            var requestCode = Guid.NewGuid().ToString();

            cache.Set($"confirmation:{confirmationCode}", requestCode);
            cache.Set($"request:{requestCode}", false);

            await mailService.Send(email, confirmationCode);

            return requestCode;

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
        public string Check([FromBody] string requestCode)
        {
            bool requestState;

            if (!cache.TryGetValue<bool>($"request:{requestCode}",out requestState))
            {
                return "Error";
            }

            if (!requestState)
            {
                // TODO: return empty response
                return "check again later";
            }

            // TODO: return access and refresh token
            return "logged in";
        }

        public async Task Refresh()
        {
            // TODO
            // Create new accesstoken (refreshtoken?, invalidate?)
            // return new tokens
        }
    }
}
