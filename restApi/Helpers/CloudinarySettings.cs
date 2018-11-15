namespace API.Helpers
{
    public class CloudinarySettings
    {
        public string CloudName { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public void Deconstruct(out string cloudName, out string apiKey, out string apiSecret)
        {
            cloudName = this.CloudName;
            apiKey = this.ApiKey;
            apiSecret = this.ApiSecret;
        }
    }
}