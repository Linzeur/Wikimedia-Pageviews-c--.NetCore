namespace ViewCounter.Entities
{
    public class Domain
    {
        public string domainCode { get; set; }
        public int viewCount { get; set; }

        public Domain(string domainCode, int viewCount)
        {
            this.domainCode = domainCode;
            this.viewCount = viewCount;
        }
    }
}
