using System.Linq;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace Lightest.Tests.IdentityServer.ServicesTests
{
    public class PasswordGenerator
    {
        protected readonly PasswordOptions _passwordOptions;

        protected Lightest.IdentityServer.Services.PasswordGenerator Generator
        {
            get
            {
                var identityOptions = new IdentityOptions
                {
                    Password = _passwordOptions
                };
                return new Lightest.IdentityServer.Services.PasswordGenerator(identityOptions);
            }
        }

        protected const int ITERATIONS = 200;

        public PasswordGenerator()
        {
            _passwordOptions = new PasswordOptions
            {
                RequireDigit = false,
                RequiredLength = 12,
                RequiredUniqueChars = 4,
                RequireLowercase = false,
                RequireNonAlphanumeric = false,
                RequireUppercase = false
            };
        }

        [Theory]
        [InlineData(1, 10)]
        [InlineData(5, 10)]
        [InlineData(11, 11)]
        [InlineData(20, 20)]
        public void LengthSatisfied(int settingsLength, int expectedLength)
        {
            _passwordOptions.RequiredLength = settingsLength;

            var password = Generator.GeneratePassword();

            Assert.Equal(expectedLength, password.Length);
        }

        [Theory]
        [InlineData(4)]
        [InlineData(10)]
        [InlineData(20)]
        public void UniqueCountSatisfied(int settingsLength)
        {
            _passwordOptions.RequiredUniqueChars = settingsLength;

            var password = Generator.GeneratePassword();

            Assert.True(settingsLength <= password.Distinct().Count(),
                "Not enought unique characters");
        }

        [Fact]
        public void DigitConstraintSatisfied()
        {
            _passwordOptions.RequireDigit = true;

            for (var i = 0; i < ITERATIONS; i++)
            {
                var password = Generator.GeneratePassword();
                Assert.Contains(password, c => char.IsDigit(c));
            }
        }

        [Fact]
        public void UppercaseConstraintSatisfied()
        {
            _passwordOptions.RequireUppercase = true;

            for (var i = 0; i < ITERATIONS; i++)
            {
                var password = Generator.GeneratePassword();
                Assert.Contains(password, c => char.IsUpper(c));
            }
        }

        [Fact]
        public void LowercaseConstraintSatisfied()
        {
            _passwordOptions.RequireLowercase = true;

            for (var i = 0; i < ITERATIONS; i++)
            {
                var password = Generator.GeneratePassword();
                Assert.Contains(password, c => char.IsLower(c));
            }
        }

        [Fact]
        public void NonAlphanumericConstraintSatisfied()
        {
            _passwordOptions.RequireNonAlphanumeric = true;

            for (var i = 0; i < ITERATIONS; i++)
            {
                var password = Generator.GeneratePassword();
                Assert.Contains(password, c => !char.IsLetterOrDigit(c));
            }
        }
    }
}
