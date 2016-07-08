namespace ServiceConnect
{
    public class RequestObject
    {
        public string   Method { get; set; }
        public string   Content_Type { get; set; }
        public string   Authorization { get; set; }
        public string[] Headers { get; set; }
        public string   Body { get; set; }
    }
}
