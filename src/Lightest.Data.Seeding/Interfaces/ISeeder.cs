using System.Threading.Tasks;

namespace Lightest.Data.Seeding.Interfaces
{
    public interface ISeeder
    {
        Task Seed();

        Task AddTestData();
    }
}
