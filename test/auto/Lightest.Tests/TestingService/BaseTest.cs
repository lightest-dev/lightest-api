using System;
using Lightest.Data;
using Microsoft.EntityFrameworkCore;

namespace Lightest.Tests.TestingService
{
    public class BaseTest : IDisposable
    {
        protected readonly RelationalDbContext _context;

        public BaseTest()
        {
            var options = new DbContextOptionsBuilder<RelationalDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _context = new RelationalDbContext(options);
        }

        public void Dispose() => _context.Dispose();
    }
}
