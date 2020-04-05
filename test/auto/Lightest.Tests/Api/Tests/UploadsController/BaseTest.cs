using System;
using System.Collections.Generic;
using System.Security.Claims;
using Lightest.AccessService.Interfaces;
using Lightest.Api.RequestModels.UploadRequests;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Lightest.Data.Mongo.Models;
using Lightest.Data.Mongo.Services;
using Lightest.TestingService.Interfaces;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Lightest.Tests.Api.Tests.UploadsController
{
    public abstract class BaseTest : Api.BaseTest
    {
        protected Lightest.Api.Controllers.UploadsController _controller
        {
            get
            {
                var controller = new Lightest.Api.Controllers.UploadsController(_testingServiceMock.Object,
                    _context, _accessServiceMock.Object, _uploadDataRepositoryMock.Object, _userManager.Object);

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
        protected readonly Mock<IUploadDataRepository> _uploadDataRepositoryMock;
        private readonly Language _language;
        protected  Upload _upload;
        protected  CodeUpload _codeUpload;
        protected  UploadData _uploadData;

        public BaseTest()
        {
            _accessServiceMock = GenerateAccessService<Upload>();
            _claimsPrincipalMock = GenerateClaimsMock();
            _testingServiceMock = new Mock<ITestingService>();
            _uploadDataRepositoryMock = new Mock<IUploadDataRepository>();

            var checker = new Checker
            {
                Id = Guid.NewGuid(),
                Name = "name"
            };
            _task = new TaskDefinition
            {
                Id = Guid.NewGuid(),
                Points = 100,
                Name = "name",
                Checker = checker,
                CheckerId = checker.Id
            };
            _language = new Language
            {
                Id = Guid.NewGuid(),
                Name = "name",
                Extension = ".ext"
            };
        }

        protected virtual List<Upload> GenerateUploads(int count)
        {
            var result = new List<Upload>();
        
            for (var i = 0; i < count; i++)
            {
                var upload = new Upload
                {
                    Id = Guid.NewGuid(),
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
        
        protected virtual void GenerateCodeUploads()
        {
            List<Upload> upload =  GenerateUploads(1);
            _upload = upload[0];
            
            var uploadData = new UploadData
                {
                    Id = _upload.Id,
                    Code = $"code{0}"
                };
            _uploadData = uploadData;
            
            var codeUpload = new CodeUpload
            {
                Id = Guid.NewGuid(),
                Code = $"code{0}",
                TaskId = _task.Id,
                LanguageId = _language.Id
            };
            _codeUpload = codeUpload;
            
        }
    }
}
