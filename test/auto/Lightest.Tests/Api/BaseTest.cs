using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Lightest.AccessService.Interfaces;
using Lightest.Data;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Sieve.Services;

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

            mock.Setup(m => m.CanRead(It.IsAny<Guid>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(true);
            mock.Setup(m => m.CanEdit(It.IsAny<Guid>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(true);
            mock.Setup(m => m.CanAdd(It.IsAny<T>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(true);

            return mock;
        }

        protected virtual Mock<IRoleHelper> GenerateRoleHelper()
        {
            var mock = new Mock<IRoleHelper>();
            mock.Setup(m => m.IsAdmin(It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(true);
            mock.Setup(m => m.IsTeacher(It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(true);

            return mock;
        }

        protected virtual Mock<ISieveProcessor> GenerateSieveProcessor<T>()
        {
            var sieveProcessorMock = new Mock<ISieveProcessor>();
            sieveProcessorMock.Setup(p => p.Apply(It.IsAny<Sieve.Models.SieveModel>(),
                It.IsAny<IQueryable<T>>(), It.IsAny<object[]>(), It.IsAny<bool>(),
                It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns((Sieve.Models.SieveModel i1, IQueryable<T> input,
                object[] i2, bool i3, bool i4, bool i5) => input);
            return sieveProcessorMock;
        }

        public void Dispose() => _context.Dispose();
    }
}
