using System;
using System.Threading.Tasks;
using Lightest.Api.RequestModels;
using Lightest.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Lightest.Tests.Api.Tests.TestingController
{
    public class AddCheckerResult : BaseTest
    {
        private readonly CheckerCompilationResult _result;
        private readonly Checker _checker;

        public AddCheckerResult()
        {
            _checker = new Checker
            {
                Id = Guid.NewGuid(),
                Name = "name",
                Code = "code",
                Compiled = false,
                Message = "non-empty message"
            };

            _result = new CheckerCompilationResult
            {
                Id = _checker.Id,
                Compiled = true,
                Message = "compilation message"
            };
        }

        [Fact]
        public async Task CheckerNotFound()
        {
            var result = await _controller.AddCheckerResult(_result);
            var badRequest = result as BadRequestObjectResult;
            Assert.NotNull(badRequest);

            var error = badRequest.Value as string;
            Assert.NotNull(error);
            Assert.Equal(nameof(_result.Id), error);
        }

        [Fact]
        public async Task CheckerUpdated()
        {
            _context.Checkers.Add(_checker);
            await _context.SaveChangesAsync();

            var result = await _controller.AddCheckerResult(_result);
            Assert.IsAssignableFrom<OkResult>(result);

            var checker = _context.Checkers.Find(_checker.Id);
            Assert.Equal(_result.Compiled, checker.Compiled);
            Assert.Equal(_result.Message, checker.Message);
        }
    }
}
