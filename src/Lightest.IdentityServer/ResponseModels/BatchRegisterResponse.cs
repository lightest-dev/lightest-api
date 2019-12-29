using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Lightest.IdentityServer.ResponseModels
{
    public class BatchRegisterResponse
    {
        public IEnumerable<PasswordResponse> GeneratedPasswords { get; set; }

        public int GeneratedCount => GeneratedPasswords.Count();

        public IEnumerable<IdentityError> Errors { get; set; }
    }
}
