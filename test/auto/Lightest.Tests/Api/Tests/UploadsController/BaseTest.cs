using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models.TaskModels;
using Lightest.TestingService.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Lightest.Tests.Api.Tests.UploadsController
{
    public class BaseTest: Api.BaseTest
    {
        protected Lightest.Api.Controllers.UploadsController _controller
        {
            get
            {
                var controller = new Lightest.Api.Controllers.UploadsController(_testingServiceMock.Object,
                    _context, _accessServiceMock.Object, _userManager.Object);

                controller.ControllerContext.HttpContext = new DefaultHttpContext
                {
                    User = _claimsPrincipalMock.Object
                };
                return controller;
            }
        }

        protected readonly Mock<IAccessService<Upload>> _accessServiceMock;
        protected readonly Mock<ClaimsPrincipal> _claimsPrincipalMock;
        protected readonly Mock<ITestingService> _testingServiceMock;
        protected readonly TaskDefinition _task;

        public BaseTest()
        {
            _accessServiceMock = GenerateAccessService<Upload>();
            _claimsPrincipalMock = GenerateClaimsMock();

            _task = new TaskDefinition
            {
                Id = Guid.NewGuid(),
                Points = 100,
                Name = "name"
            };
        }

        protected virtual List<Upload> GenerateUploads(int count)
        {
            var result = new List<Upload>();

            for (var i = 0; i < count; i++)
            {
                var upload = new Upload
                {
                    Code = $"code{i}",
                    Task = _task,
                    TaskId = _task.Id,
                    UserId = _user.Id,
                    UploadDate = DateTime.Now.AddMinutes(i)
                };
                result.Add(upload);
            }

            return result;
        }
    }
}
