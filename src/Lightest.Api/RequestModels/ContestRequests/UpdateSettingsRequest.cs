using System;

namespace Lightest.Api.RequestModels.ContestRequests
{
    public class UpdateSettingsRequest
    {
        public DateTime? StartTime { get; set; }

        public TimeSpan? Length { get; set; }
    }
}
