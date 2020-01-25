using System;
using System.Linq;
using System.Threading.Tasks;
using Lightest.Api.RequestModels.UserRequests;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Lightest.Tests.Api.Tests.ProfileController
{
    public class PutUser : BaseTest
    {
        private readonly PersonalDataRequest _personalData;

        public PutUser()
        {
            _personalData = new PersonalDataRequest
            {
                Name = "new_name",
                Surname = "new_surename",
                UserId = _user.Id
            };
        }

        [Fact]
        public async Task Forbidden()
        {
            _context.Users.Add(_user);
            await _context.SaveChangesAsync();

            _accessServiceMock.Setup(m => m.HasWriteAccess(It.IsAny<Guid>(),
                It.Is<ApplicationUser>(u => u.Id == _user.Id)))
                .ReturnsAsync(false);

            var result = await _controller.PutUser(_user.Id, _personalData);

            Assert.IsAssignableFrom<ForbidResult>(result);
        }

        [Fact]
        public async Task UserUpdated()
        {
            _context.Users.Add(_user);
            await _context.SaveChangesAsync();

            var result = await _controller.PutUser(_user.Id, _personalData);
            Assert.IsAssignableFrom<OkResult>(result);

            Assert.Single(_context.Users);
            var user = _context.Users.First();
            Assert.Equal(_personalData.Name, user.Name);
            Assert.Equal(_personalData.Surname, user.Surname);
        }

        [Fact]
        public async Task NotFound()
        {
            _personalData.UserId = Guid.NewGuid().ToString();
            _context.Users.Add(_user);
            await _context.SaveChangesAsync();

            var result = await _controller.PutUser(_personalData.UserId, _personalData);
            Assert.IsAssignableFrom<NotFoundResult>(result);
        }

        [Fact]
        public async Task BadRequest()
        {
            _context.Users.Add(_user);
            await _context.SaveChangesAsync();

            var result = await _controller.PutUser(Guid.NewGuid().ToString(), _personalData);
            Assert.IsAssignableFrom<BadRequestResult>(result);
        }
    }
}
