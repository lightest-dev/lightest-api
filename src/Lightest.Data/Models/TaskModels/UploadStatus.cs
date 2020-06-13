namespace Lightest.Data.Models.TaskModels
{
    public static class UploadStatus
    {
        public static readonly string New = "NEW";

        public static readonly string Queue = "QUEUE";

        public static readonly string EnvironmentSetup = "ENVIRONMENT_SETUP";

        public static readonly string Testing = "TESTING";

        public static readonly string Failed = "FAILED";
    }
}
