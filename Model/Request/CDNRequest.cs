namespace SouthernApi.Model.Request
{
    public class CDNRequest
    {
        public string ItemNumber { get; set; }
        public UrlRequest FrontFullBottle { get; set; }
        public UrlRequest BackFullBottle { get; set; }
        public UrlRequest FrontLabel { get; set; }
        public UrlRequest BackLabel { get; set; }
    }

    public class UrlRequest
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Path { get; set; }
    }
}
