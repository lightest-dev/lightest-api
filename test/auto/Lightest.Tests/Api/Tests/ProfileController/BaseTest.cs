using System.Security.Claims;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Http;
using Moq;
using Sieve.Services;

namespace Lightest.Tests.Api.Tests.ProfileController
{
    public abstract class BaseTest : Api.BaseTest
    {
        protected Lightest.Api.Controllers.ProfileController _controller
        {
            get
            {
                var controller = new Lightest.Api.Controllers.ProfileController(_context,
                    _accessServiceMock.Object, _roleHelperMock.Object,
                    _userManager.Object, _sieveProcessorMock.Object);

                controller.ControllerContext.HttpContext = new DefaultHttpContext
                {
                    User = _claimsPrincipalMock.Object
                };
                return controller;
            }
        }

        protected readonly Mock<IAccessService<ApplicationUser>> _accessServiceMock;
        protected readonly Mock<ClaimsPrincipal> _claimsPrincipalMock;
        protected readonly Mock<ISieveProcessor> _sieveProcessorMock;
        protected readonly Mock<IRoleHelper> _roleHelperMock;

        public BaseTest()
        {
            _sieveProcessorMock = GenerateSieveProcessor<ApplicationUser>();
            _accessServiceMock = GenerateAccessService<ApplicationUser>();
            _claimsPrincipalMock = GenerateClaimsMock();
            _roleHelperMock = GenerateRoleHelper();
        }
    }
}
