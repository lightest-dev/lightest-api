using Lightest.Data;
using Microsoft.EntityFrameworkCore;

namespace Lightest.Tests.DefaultMocks
{
    public class MockDatabase
    {
        private static readonly DbContextOptions<RelationalDbContext>
            _options = new DbContextOptionsBuilder<RelationalDbContext>()
            .UseInMemoryDatabase("test_db")
            .Options;

        public static RelationalDbContext Context
        {
            get
            {
                var context = new RelationalDbContext(_options);
                return context;
            }
        }
    }
}
