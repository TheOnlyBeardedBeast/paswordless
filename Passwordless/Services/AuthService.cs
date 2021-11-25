using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Passwordless.Services
{
    public interface IAuthService
    {
        Task<string> Login(string email);
        void Confirm(string confirmationId);
        void Check(string requestId);
        void ConfirmRealTime();
    }

    public class AuthService: IAuthService
    {
        protected readonly IMailService mailService;
        protected readonly IMemoryCache cache;

        public AuthService(IMailService mailService, IMemoryCache cache)
        {
            this.mailService = mailService;
            this.cache = cache;
        }

        public async Task<string> Login(string email)
        {
            var confirmationCode = Guid.NewGuid().ToString();
            var requestCode = Guid.NewGuid().ToString();

            cache.Set($"confirmation:{confirmationCode}", requestCode);
            cache.Set($"request:{requestCode}", false);

            await mailService.Send(email, confirmationCode);

            return requestCode;
        }

        public void Check()
        {
            throw new NotImplementedException();
        }

        public void Confirm(string confirmationId)
        {
            throw new NotImplementedException();
        }

        public void Check(string requestId)
        {
            throw new NotImplementedException();
        }

        public void ConfirmRealTime()
        {
            throw new NotImplementedException();
        }
    }
}
