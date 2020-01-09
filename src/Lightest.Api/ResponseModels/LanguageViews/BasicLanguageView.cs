namespace Lightest.Api.ResponseModels.Language
{
    public class BasicLanguageView : BasicNameView
    {
        public int MemoryLimit { get; set; }

        public int TimeLimit { get; set; }
    }
}
