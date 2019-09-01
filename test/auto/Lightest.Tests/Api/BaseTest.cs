using System;
using System.Collections.Generic;
using System.Security.Claims;
using Lightest.AccessService.Interfaces;
using Lightest.Data;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Lightest.Tests.Api
{
    public abstract class BaseTest : IDisposable
    {
        protected readonly RelationalDbContext _context;
        protected readonly Mock<UserManager<ApplicationUser>> _userManager;

        protected readonly ApplicationUser _user;

        public BaseTest()
        {
            _user = new ApplicationUser
            {
                Id = Guid.NewGuid().ToString(),
                Name = "name",
                Email = "e@mail.com",
                UserName = "name",
                Surname = "surname"
            };
            _userManager = GenerateUserManager();
            _context = GenerateContext();
        }

        protected virtual Mock<UserManager<ApplicationUser>> GenerateUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var manager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            manager.Setup(m => m.FindByIdAsync(It.Is<string>(s => _user.Id == s)))
                .ReturnsAsync(_user);

            return manager;
        }

        protected virtual RelationalDbContext GenerateContext()
        {
            var options = new DbContextOptionsBuilder<RelationalDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var context = new RelationalDbContext(options);
            return context;
        }

        protected virtual Mock<ClaimsPrincipal> GenerateClaimsMock()
        {
            var principal = new Mock<ClaimsPrincipal>();

            principal.Setup(p => p.Claims)
                .Returns(new List<Claim>{
                    new Claim("sub", _user.Id)
                });

            return principal;
        }

        protected virtual Mock<IAccessService<T>> GenerateAccessService<T>()
        {
            var mock = new Mock<IAccessService<T>>();

            mock.Setup(m => m.CheckAdminAccess(It.IsAny<T>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .Returns(true);
            mock.Setup(m => m.CheckReadAccess(It.IsAny<T>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .Returns(true);
            mock.Setup(m => m.CheckWriteAccess(It.IsAny<T>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .Returns(true);

            return mock;
        }

        public void Dispose() => _context.Dispose();
    }
}
