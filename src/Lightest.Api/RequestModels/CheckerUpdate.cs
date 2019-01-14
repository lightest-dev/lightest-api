using System;
using System.ComponentModel.DataAnnotations;

namespace Lightest.Api.RequestModels
{
    public class CheckerUpdate : CheckerAdd
    {
        [Required]
        public Guid Id { get; set; }
    }
}
