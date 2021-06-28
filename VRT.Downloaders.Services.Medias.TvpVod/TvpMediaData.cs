namespace VRT.Downloaders.Services.Medias.TvpVod
{
    internal class TvpMediaData
    {
        public string url { get; set; }
        public string status { get; set; }
        public int videoId { get; set; }
        public string platform { get; set; }
        public string userIp { get; set; }
        public bool adaptive { get; set; }
        public bool live { get; set; }
        public string title { get; set; }
        public int duration { get; set; }
        public bool countryIsDefault { get; set; }
        public string mimeType { get; set; }
        public bool ads_enabled { get; set; }
        public int payment_type { get; set; }
        public string distribution_model { get; set; }
        public Format[] formats { get; set; }
    }

    internal class Format
    {
        public string mimeType { get; set; }
        public int totalBitrate { get; set; }
        public int videoBitrate { get; set; }
        public int audioBitrate { get; set; }
        public bool adaptive { get; set; }
        public string url { get; set; }
        public bool downloadable { get; set; }
    }    
}
