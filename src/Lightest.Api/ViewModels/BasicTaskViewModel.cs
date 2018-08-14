using Lightest.Data.Models.TaskModels;

namespace Lightest.Api.ViewModels
{
    public class BasicTaskViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public static BasicTaskViewModel FromTask(TaskDefinition t)
        {
            var result = new BasicTaskViewModel
            {
                Id = t.Id,
                Name = t.Name
            };
            return result;
        }
    }
}