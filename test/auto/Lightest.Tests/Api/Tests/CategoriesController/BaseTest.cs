using System;
using System.Collections.Generic;
using System.Security.Claims;
using Lightest.AccessService.Interfaces;
using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Lightest.Tests.Api.Tests.CategoriesController
{
    public abstract class BaseTest : Api.BaseTest
    {
        protected Lightest.Api.Controllers.CategoriesController _controller
        {
            get
            {
                var controller = new Lightest.Api.Controllers.CategoriesController(_context, _accessServiceMock.Object, _userManager.Object);

                controller.ControllerContext.HttpContext = new DefaultHttpContext
                {
                    User = _claimsPrincipalMock.Object
                };
                return controller;
            }
        }

        protected readonly Mock<IAccessService<Category>> _accessServiceMock;
        protected readonly Mock<ClaimsPrincipal> _claimsPrincipalMock;

        protected readonly Category _parent;
        protected readonly Category _child1;
        protected readonly Category _child2;
        protected readonly CategoryUser _categoryUser;
        protected readonly Checker _checker;
        protected readonly TaskDefinition _task;

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
                Users = new List<UserTask>()
            };
            _task.Users.Add(new UserTask
            {
                UserId = _user.Id,
                TaskId = _task.Id
            });

            _parent = new Category
            {
                Id = Guid.NewGuid(),
                Name = "parent",
                Public = true
            };
            _child1 = new Category
            {
                Id = Guid.NewGuid(),
                Name = "child1",
                Public = true,
                ParentId = _parent.Id,
                Parent = _parent
            };
            _child2 = new Category
            {
                Id = Guid.NewGuid(),
                Name = "child2",
                Public = false,
                ParentId = _parent.Id,
                Parent = _parent
            };

            _parent.Tasks = new List<TaskDefinition>
            {
                _task
            };

            _parent.SubCategories = new List<Category>
            {
                _child1,
                _child2
            };
            _categoryUser = new CategoryUser
            {
                UserId = _user.Id,
                CategoryId = _child2.Id,
                User = _user,
                Category = _child2
            };
            _child2.Users = new List<CategoryUser>
            {
                _categoryUser
            };
        }

        protected virtual void AddDataToDb()
        {
            _context.Tasks.Add(_task);
            _context.Categories.Add(_parent);
            _context.Categories.Add(_child1);
            _context.Categories.Add(_child2);
        }
    }
}
