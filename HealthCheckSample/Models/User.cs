namespace HealthCheckSample.Models
{
    /// <summary>
    /// User
    /// </summary>
    public class User
    {
        public int id { get; set; }
        public string account { get; set; }
        public string password { get; set; }
        public string enable { get; set; }
        public string create_id { get; set; }
        public DateTime create_time { get; set; }
        public string update_id { get; set; }
        public DateTime update_time { get; set; }
    }
}
