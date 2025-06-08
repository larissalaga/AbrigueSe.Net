namespace AbrigueSe.Configuration
{
    public class ConfigManager
    {
        private static readonly Lazy<ConfigManager> instance = new Lazy<ConfigManager>(() => new ConfigManager());

        private IConfiguration configuration;

        private ConfigManager() { }

        public static ConfigManager Instance => instance.Value;

        public void Initialize(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GetConnectionString(string name)
        {
            return configuration.GetConnectionString(name);
        }

        public T GetValue<T>(string key)
        {
            return configuration.GetValue<T>(key);
        }
    }
}
