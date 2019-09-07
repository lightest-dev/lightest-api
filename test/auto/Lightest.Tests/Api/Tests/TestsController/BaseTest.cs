using System;
using System.Security.Claims;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Lightest.Tests.Api.Tests.TestsController
{
    public abstract class BaseTest : Api.BaseTest
    {
        protected Lightest.Api.Controllers.TestsController _controller
        {
            get
            {
                var controller = new Lightest.Api.Controllers.TestsController(_context, _accessServiceMock.Object, _userManager.Object);

                controller.ControllerContext.HttpContext = new DefaultHttpContext
                {
                    User = _claimsPrincipalMock.Object
                };
                return controller;
            }
        }

        protected readonly Mock<IAccessService<TaskDefinition>> _accessServiceMock;
        protected readonly Mock<ClaimsPrincipal> _claimsPrincipalMock;
        protected readonly Test _test;

        public BaseTest()
        {
            _accessServiceMock = GenerateAccessService<TaskDefinition>();
            _claimsPrincipalMock = GenerateClaimsMock();

            var task = new TaskDefinition
            {
                Id = Guid.NewGuid(),
                Points = 100,
                Name = "name"
            };

            _test = new Test
            {
                Id = Guid.NewGuid(),
                Input = "input",
                Output = "output",
                TaskId = task.Id,
                Task = task
            };
        }
    }
}
