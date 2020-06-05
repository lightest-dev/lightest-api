using System;
using System.Collections.Generic;
using System.Security.Claims;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Http;
using Moq;
using Sieve.Services;

namespace Lightest.Tests.Api.Tests.TasksController
{
    public abstract class BaseTest : Api.BaseTest
    {
        protected Lightest.Api.Controllers.TasksController _controller
        {
            get
            {
                var controller = new Lightest.Api.Controllers.TasksController(_context,
                    _userManager.Object, _accessServiceMock.Object, _roleHelper.Object,
                    _sieveProcessorMock.Object);

                controller.ControllerContext.HttpContext = new DefaultHttpContext
                {
                    User = _claimsPrincipalMock.Object
                };
                return controller;
            }
        }

        protected readonly Mock<IAccessService<TaskDefinition>> _accessServiceMock;
        protected readonly Mock<ClaimsPrincipal> _claimsPrincipalMock;
        protected readonly Mock<ISieveProcessor> _sieveProcessorMock;
        protected readonly Mock<IRoleHelper> _roleHelper;

        protected readonly TaskDefinition _task;
        protected readonly Test _test;
        protected readonly Checker _checker;
        protected readonly Language _language;
        protected readonly Category _category;

        public BaseTest()
        {
            _sieveProcessorMock = GenerateSieveProcessor<TaskDefinition>();
            _accessServiceMock = GenerateAccessService<TaskDefinition>();
            _claimsPrincipalMock = GenerateClaimsMock();
            _roleHelper = GenerateRoleHelper();

            _checker = new Checker
            {
                Id = Guid.NewGuid(),
                Name = "test",
                Code = "code"
            };

            _language = new Language
            {
                Id = Guid.NewGuid(),
                Name = "name",
                Extension = "ext"
            };

            _category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "category",
                Public = true
            };

            _task = new TaskDefinition
            {
                Id = Guid.NewGuid(),
                Public = true,
                Points = 100,
                Checker = _checker,
                CheckerId = _checker.Id,
                Category = _category,
                CategoryId = _category.Id,
                Name = "name",
                Tests = new List<Test>(),
                Languages = new List<TaskLanguage>(),
                Users = new List<Assignment>()
            };

            _task.Languages.Add(new TaskLanguage
            {
                Language = _language,
                LanguageId = _language.Id,
                Task = _task,
                TaskId = _task.Id,
                MemoryLimit = 1000,
                TimeLimit = 1000
            });

            _task.Users.Add(new Assignment
            {
                User = _user,
                UserId = _user.Id,
                TaskId = _task.Id,
                CanWrite = true,
                CanRead = true,
                CanChangeAccess = true,
                IsOwner = true
            });

            _test = new Test
            {
                Id = Guid.NewGuid(),
                Input = "input",
                Output = "output",
                TaskId = _task.Id,
                Task = _task
            };

            _task.Tests.Add(_test);
        }

        protected virtual void AddDataToDb()
        {
            _context.Tasks.Add(_task);
            _context.Categories.Add(_category);
            _context.Tests.Add(_test);
            _context.Languages.Add(_language);
            _context.Checkers.Add(_checker);
        }
    }
}
