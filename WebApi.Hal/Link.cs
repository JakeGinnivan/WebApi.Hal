namespace WebApi.Hal
{
    public class Link
    {
        public Link()
        { }

        public Link(string rel, string href, bool isTemplated = false)
        {
            Rel = rel;
            Href = href;
            IsTemplated = isTemplated;
        }

        public string Rel { get; set; }

        public string Href { get; set; }

        public bool IsTemplated { get; set; }
    }
}