using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Lightest.IdentityServer.Services
{
    public class PasswordGenerator : IPasswordGenerator
    {
        protected readonly IdentityOptions _identityOptions;

        public PasswordGenerator(IdentityOptions identityOptions)
        {
            _identityOptions = identityOptions;
        }

        public string GeneratePassword()
        {
            var password = GenerateRandomPassword(_identityOptions.Password);
            return password;
        }

        private static string GenerateRandomPassword(PasswordOptions options)
        {
            var charSets = new string[] {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ",
                "abcdefghijkmnopqrstuvwxyz",
                "0123456789",
                "!@$?_-"
            };

            var rand = new Random();
            var chars = new List<char>();

            if (options.RequireUppercase)
            {
                chars.Insert(rand.Next(0, chars.Count),
                    charSets[0][rand.Next(0, charSets[0].Length)]);
            }

            if (options.RequireLowercase)
            {
                chars.Insert(rand.Next(0, chars.Count),
                    charSets[1][rand.Next(0, charSets[1].Length)]);
            }

            if (options.RequireDigit)
            {
                chars.Insert(rand.Next(0, chars.Count),
                    charSets[2][rand.Next(0, charSets[2].Length)]);
            }

            if (options.RequireNonAlphanumeric)
            {
                chars.Insert(rand.Next(0, chars.Count),
                    charSets[3][rand.Next(0, charSets[3].Length)]);
            }

            var count = Math.Max(options.RequiredLength, 10);

            for (var i = chars.Count; i < count
                || chars.Distinct().Count() < options.RequiredUniqueChars; i++)
            {
                var rcs = charSets[rand.Next(0, charSets.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }
    }
}
