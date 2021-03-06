﻿using System.Security.Claims;
using IdentityServer4.Services;
using Lightest.Data.Models;
using Lightest.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace Lightest.Tests.IdentityServer.Tests.AccountController
{
    public abstract class BaseTest
    {
        protected readonly Mock<UserManager<ApplicationUser>> _userManager;
        protected readonly Mock<SignInManager<ApplicationUser>> _signInManager;
        protected readonly Mock<IPersistedGrantService> _persistedGrantService;
        protected readonly Mock<ClaimsPrincipal> _claimsPrincipalMock;
        protected readonly Mock<IPasswordGenerator> _passwordGenerator;

        protected Lightest.IdentityServer.Controllers.AccountController _controller
        {
            get
            {
                var controller = new Lightest.IdentityServer.Controllers.AccountController(_userManager.Object,
                    _persistedGrantService.Object, _signInManager.Object, _passwordGenerator.Object);

                controller.ControllerContext.HttpContext = new DefaultHttpContext
                {
                    User = _claimsPrincipalMock.Object
                };

                return controller;
            }
        }

        protected BaseTest()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            _signInManager = new Mock<SignInManager<ApplicationUser>>(_userManager.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<ILogger<SignInManager<ApplicationUser>>>().Object,
                new Mock<IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<ApplicationUser>>().Object);
            _persistedGrantService = new Mock<IPersistedGrantService>();
            _claimsPrincipalMock = new Mock<ClaimsPrincipal>();
            _passwordGenerator = new Mock<IPasswordGenerator>();
        }
    }
}
