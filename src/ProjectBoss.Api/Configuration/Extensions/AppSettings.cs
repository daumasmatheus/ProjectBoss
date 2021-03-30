namespace ProjectBoss.Api.Configuration.Extensions
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public int HourToExpire { get; set; }
        public string Emitter { get; set; }
        public string ValidIn { get; set; }
    }
}
