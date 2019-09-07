using System;
using System.Security.Claims;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Lightest.Tests.Api.Tests.CheckersController
{
    public abstract class BaseTest : Api.BaseTest
    {
        protected Lightest.Api.Controllers.CheckersController _controller
        {
            get
            {
                var controller = new Lightest.Api.Controllers.CheckersController(_context, _userManager.Object, _accessServiceMock.Object);

                controller.ControllerContext.HttpContext = new DefaultHttpContext
                {
                    User = _claimsPrincipalMock.Object
                };
                return controller;
            }
        }

        protected readonly Mock<IAccessService<Checker>> _accessServiceMock;
        protected readonly Mock<ClaimsPrincipal> _claimsPrincipalMock;
        protected readonly Checker _checker;
        protected readonly ServerChecker _cachedChecker;

        public BaseTest()
        {
            _accessServiceMock = GenerateAccessService<Checker>();
            _claimsPrincipalMock = GenerateClaimsMock();

            _checker = new Checker
            {
                Id = Guid.NewGuid(),
                Code = "code",
                Name = "name"
            };

            _cachedChecker = new ServerChecker
            {
                CheckerId = _checker.Id,
                Checker = _checker,
                ServerIp = "1"
            };
        }
    }
}
