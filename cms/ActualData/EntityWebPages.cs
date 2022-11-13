namespace ActualData
{
    public class EntityWebPages
    {
        public EntityWebPages(string mainPage, string detailedPage, bool detailedPageRequired = true)
        {
            MainPage = mainPage;
            DetailedPage = detailedPage;
            DetailedPageRequired = detailedPageRequired;
        }

        public string MainPage
        {
            get; private set;
        }

        public string DetailedPage
        {
            get; private set;
        }

        public bool DetailedPageRequired
        {
            get; private set;
        }
    }
}