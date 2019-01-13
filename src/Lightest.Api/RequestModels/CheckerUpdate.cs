using System.ComponentModel.DataAnnotations;

namespace Lightest.Api.RequestModels
{
    public class CheckerUpdate : CheckerAdd
    {
        [Required]
        public int Id { get; set; }
    }
}
