using Lightest.Data.Models;
using Lightest.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Lightest.Tests.IdentityServer.Tests.BatchAccountController
{
    public abstract class BaseTest
    {
        protected readonly Mock<UserManager<ApplicationUser>> _userManager;
        protected readonly Mock<IPasswordGenerator> _passwordGenerator;

        protected Lightest.IdentityServer.Controllers.BatchAccountController _controller
        {
            get
            {
                var controller = new Lightest.IdentityServer.Controllers.BatchAccountController(_passwordGenerator.Object,
                    _userManager.Object);

                return controller;
            }
        }

        protected BaseTest()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            _passwordGenerator = new Mock<IPasswordGenerator>();
        }
    }
}
