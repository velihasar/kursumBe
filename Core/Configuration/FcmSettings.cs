namespace Core.Configuration
{
    /// <summary>
    /// Firebase Cloud Messaging (FCM) ayarlarını tutar.
    /// </summary>
    public class FcmSettings
    {
        public string ProjectId { get; set; }
        public string ServiceAccountJsonPath { get; set; }
    }
}
