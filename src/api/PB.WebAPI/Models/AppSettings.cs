namespace PB.WebAPI.Models
{
    public class AppSettings
    {
        public string TokenSecret { get; set; }

        public string DbUsername { get; set; }
        public string DbPassword { get; set; }
        public string DbURL { get; set; }
    }
}
