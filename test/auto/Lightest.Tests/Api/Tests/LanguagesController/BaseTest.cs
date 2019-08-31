using System;
using System.Security.Claims;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Lightest.Tests.Api.Tests.LanguagesController
{
    public abstract class BaseTest : Api.BaseTest
    {
        protected Lightest.Api.Controllers.LanguagesController _controller
        {
            get
            {
                var controller = new Lightest.Api.Controllers.LanguagesController(_context, _accessServiceMock.Object, _userManager.Object);

                controller.ControllerContext.HttpContext = new DefaultHttpContext
                {
                    User = _claimsPrincipalMock.Object
                };
                return controller;
            }
        }

        protected readonly Mock<IAccessService<Language>> _accessServiceMock;
        protected readonly Mock<ClaimsPrincipal> _claimsPrincipalMock;
        protected readonly Language _language;

        public BaseTest()
        {
            _accessServiceMock = GenerateAccessService<Language>();
            _claimsPrincipalMock = GenerateClaimsMock();

            _language = new Language
            {
                Id = Guid.NewGuid(),
                Name = "name",
                Extension = "ext"
            };
        }
    }
}
