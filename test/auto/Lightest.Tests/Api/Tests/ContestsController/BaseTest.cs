using System;
using System.Collections.Generic;
using System.Security.Claims;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Lightest.Tests.Api.Tests.ContestsController
{
    public abstract class BaseTest : Api.BaseTest
    {
        protected Lightest.Api.Controllers.ContestsController _controller
        {
            get
            {
                var controller = new Lightest.Api.Controllers.ContestsController(_context,
                    _userManager.Object);

                controller.ControllerContext.HttpContext = new DefaultHttpContext
                {
                    User = _claimsPrincipalMock.Object
                };
                return controller;
            }
        }

        protected readonly Mock<IAccessService<Category>> _accessServiceMock;
        protected readonly Mock<ClaimsPrincipal> _claimsPrincipalMock;

        protected readonly Category _category;
        protected readonly Checker _checker;
        protected readonly TaskDefinition _task;
        protected readonly ContestSettings _contest;

        public BaseTest()
        {
            _accessServiceMock = GenerateAccessService<Category>();
            _claimsPrincipalMock = GenerateClaimsMock();

            _checker = new Checker
            {
                Id = Guid.NewGuid(),
                Name = "test",
                Code = "code"
            };

            _task = new TaskDefinition
            {
                Id = Guid.NewGuid(),
                Public = true,
                Points = 100,
                Checker = _checker,
                CheckerId = _checker.Id,
                Name = "name",
                Tests = new List<Test>(),
                Languages = new List<TaskLanguage>(),
                Users = new List<Assignment>()
            };

            _category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "parent",
                Public = true,
                Contest = true,
                Users = new List<CategoryUser>()
            };

            _category.Tasks = new List<TaskDefinition>
            {
                _task
            };

            _contest = new ContestSettings
            {
                CategoryId = _category.Id
            };
        }

        protected virtual void AddDataToDb()
        {
            _context.Tasks.Add(_task);
            _context.Categories.Add(_category);
        }

        protected virtual void AddContestToDb() => _context.Contests.Add(_contest);
    }
}
