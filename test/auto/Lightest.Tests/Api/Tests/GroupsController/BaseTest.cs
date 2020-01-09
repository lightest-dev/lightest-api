using System;
using System.Collections.Generic;
using System.Security.Claims;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Http;
using Moq;
using Sieve.Services;

namespace Lightest.Tests.Api.Tests.GroupsController
{
    public abstract class BaseTest : Api.BaseTest
    {
        protected Lightest.Api.Controllers.GroupsController _controller
        {
            get
            {
                var controller = new Lightest.Api.Controllers.GroupsController(_context,
                    _accessServiceMock.Object, _userManager.Object, _sieveProcessorMock.Object);

                controller.ControllerContext.HttpContext = new DefaultHttpContext
                {
                    User = _claimsPrincipalMock.Object
                };
                return controller;
            }
        }

        protected readonly Mock<IAccessService<Group>> _accessServiceMock;
        protected readonly Mock<ClaimsPrincipal> _claimsPrincipalMock;
        protected readonly Mock<ISieveProcessor> _sieveProcessorMock;

        protected readonly Group _parent;
        protected readonly Group _child1;
        protected readonly Group _child2;
        protected readonly UserGroup _categoryUser;

        public BaseTest()
        {
            _sieveProcessorMock = GenerateSieveProcessor<Group>();
            _accessServiceMock = GenerateAccessService<Group>();
            _claimsPrincipalMock = GenerateClaimsMock();

            _parent = new Group
            {
                Id = Guid.NewGuid(),
                Name = "parent"
            };
            _child1 = new Group
            {
                Id = Guid.NewGuid(),
                Name = "child1",
                ParentId = _parent.Id,
                Parent = _parent
            };
            _child2 = new Group
            {
                Id = Guid.NewGuid(),
                Name = "child2",
                ParentId = _parent.Id,
                Parent = _parent
            };

            _parent.SubGroups = new List<Group>
            {
                _child1,
                _child2
            };

            _categoryUser = new UserGroup
            {
                UserId = _user.Id,
                GroupId = _child2.Id,
                User = _user,
                Group = _child2
            };
            _child2.Users = new List<UserGroup>
            {
                _categoryUser
            };
        }

        protected virtual void AddDataToDb()
        {
            _context.Groups.Add(_parent);
            _context.Groups.Add(_child1);
            _context.Groups.Add(_child2);
        }
    }
}
