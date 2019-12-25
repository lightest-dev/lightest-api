using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Lightest.IdentityServer.RequestModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.IdentityServer.Tests.AccountController
{
    public class Logout : BaseTest
    {
        protected readonly Claim _subjectClaim;
        protected readonly LogOutRequest _logOutRequest;

        public Logout()
        {
            _logOutRequest = new LogOutRequest
            {
                ClientName = "Name"
            };

            _subjectClaim = new Claim(JwtClaimTypes.Subject, "Subject");

            var claimsIdentityMock = new Mock<ClaimsIdentity>();

            _claimsPrincipalMock.Setup(m => m.Identity)
                .Returns(claimsIdentityMock.Object);

            claimsIdentityMock
                .Setup(m => m.FindFirst(JwtClaimTypes.Subject))
                .Returns(_subjectClaim);
        }

        [Fact]
        public async Task LogoutSucceded()
        {
            _signInManager.Setup(sm => sm.SignOutAsync())
                .Returns(Task.CompletedTask);

            var result = await _controller.Logout(_logOutRequest);

            Assert.IsAssignableFrom<OkResult>(result);
            _persistedGrantService.Verify(g => g.RemoveAllGrantsAsync(_subjectClaim.Value, _logOutRequest.ClientName));
        }
    }
}
