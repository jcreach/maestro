namespace Maestro.api.Models
{
    public class ApplicationInformationsExtended : ApplicationInformations
    {
        public ApplicationInformationsExtended() : base()
        {
            ConfigurationKey = string.Empty;
        }
        public ApplicationInformationsExtended(string applicationName, string version, string key) : base(applicationName, version, key)
        {
            ConfigurationKey = string.Empty;
        }

        public ApplicationInformationsExtended(string applicationName, string version, string key, string configurationKey) : base(applicationName, version, key)
        {
            ConfigurationKey = configurationKey;
        }

        public string ConfigurationKey { get; set; }
    }
}
