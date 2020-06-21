using Lightest.Data;
using Microsoft.EntityFrameworkCore;

namespace Lightest.TransferTest
{
    public class MockDatabase
    {
        private readonly DbContextOptions<RelationalDbContext>
            _options = new DbContextOptionsBuilder<RelationalDbContext>()
            .UseInMemoryDatabase("Add_writes_to_database")
            .Options;

        public RelationalDbContext Context
        {
            get
            {
                var context = new RelationalDbContext(_options);
                return context;
            }
        }
    }
}
