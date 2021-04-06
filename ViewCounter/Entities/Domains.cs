namespace ViewCounter.Entities
{
    public class Domains
    {
        public string domainCode { get; set; }
        public int viewCount { get; set; }

        public Domains(string domainCode, int viewCount)
        {
            this.domainCode = domainCode;
            this.viewCount = viewCount;
        }
    }
}
