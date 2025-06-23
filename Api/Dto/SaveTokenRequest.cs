namespace Api.Dto
{
    public class SaveTokenRequest
    {
        public string DeviceToken { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty; 
    }
}
