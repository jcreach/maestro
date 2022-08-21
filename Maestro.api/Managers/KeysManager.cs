using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maestro.api.Managers
{
    public class KeysManager
    {
        /// <summary>
        /// Generate the application key
        /// </summary>
        /// <returns>application key</returns>
        public string GenerateApplicationKey()
            => Guid.NewGuid().ToString(); // TODO

        /// <summary>
        /// Generate the configuration key
        /// </summary>
        /// <returns>configuration key</returns>
        public string GenerateConfigurationKey()
            => Guid.NewGuid().ToString();
    }
}
