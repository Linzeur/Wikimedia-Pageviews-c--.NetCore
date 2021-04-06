namespace ViewCounter.Entities
{
    public class Page
    {
        public string title { get; set; }
        public int viewCount { get; set; }

        public Page(string title, int viewCount)
        {
            this.title = title;
            this.viewCount = viewCount;
        }
    }
}
