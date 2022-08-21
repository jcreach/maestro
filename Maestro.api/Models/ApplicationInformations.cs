using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maestro.api.Models
{
    public class ApplicationInformations
    {
        public string ApplicationName { get; set; }
        public string Key { get; set; }
        public string Version { get; set; }
        public ApplicationInformations()
        {
            ApplicationName = string.Empty;
            Key = string.Empty;
            Version = string.Empty;
        }

        public ApplicationInformations(string applicationName, string version, string key)
        {
            ApplicationName = applicationName;
            Version = version;
            Key = key;
        }
    }
}
