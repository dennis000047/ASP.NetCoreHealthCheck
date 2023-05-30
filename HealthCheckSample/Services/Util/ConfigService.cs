namespace HealthCheckSample.Services.Util
{
    public class ConfigService
    {
        private readonly IConfiguration _config;
        public ConfigService(IConfiguration config)
        {
            _config = config;
        }

        #region Sql ConStr
        public string GetUserDBConString()
        {
            var dbConString = _config["ConnectionStrings:User"];
            return dbConString;
        }
        #endregion

        #region Other API

        #endregion
    }

}

