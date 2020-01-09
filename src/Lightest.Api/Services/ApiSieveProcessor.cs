using Lightest.Data.Models;
using Lightest.Data.Models.TaskModels;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;

namespace Lightest.Api.Services
{
    public class ApiSieveProcessor : SieveProcessor
    {
        public ApiSieveProcessor(
            IOptions<SieveOptions> options)
            : base(options, null, null)
        {
        }

        protected override SievePropertyMapper MapProperties(SievePropertyMapper mapper)
        {
            mapper = base.MapProperties(mapper);
            MapTaskDefinition(mapper);
            MapCategory(mapper);
            MapGroup(mapper);
            MapUser(mapper);

            return mapper;
        }

        private void MapTaskDefinition(SievePropertyMapper mapper)
        {
            mapper.Property<TaskDefinition>(t => t.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<TaskDefinition>(t => t.Public)
                .CanFilter()
                .CanSort();
        }

        private void MapCategory(SievePropertyMapper mapper)
        {
            mapper.Property<Category>(t => t.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<Category>(t => t.Public)
                .CanFilter()
                .CanSort();
        }

        private void MapGroup(SievePropertyMapper mapper)
        {
            mapper.Property<Group>(t => t.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<Group>(t => t.Public)
                .CanFilter()
                .CanSort();
        }

        private void MapUser(SievePropertyMapper mapper)
        {
            mapper.Property<ApplicationUser>(u => u.Name)
                .CanFilter()
                .CanSort();

            mapper.Property<ApplicationUser>(u => u.Surname)
                .CanFilter()
                .CanSort();

            mapper.Property<ApplicationUser>(u => u.Email)
                .CanFilter()
                .CanSort();

            mapper.Property<ApplicationUser>(u => u.UserName)
                .CanFilter()
                .CanSort();
        }
    }
}
