using System;
using System.Configuration;

namespace LogArchiveTool
{
    /// <summary>
    /// Class for reading the App.config file.
    /// </summary>
    class ConfigHelper
    {
        /// <summary>
        /// This function fetches the values from App.config based on the key passed
        /// </summary>
        /// <param name="lsKey"></param>
        /// <returns></returns>
        public static string GetValue(string lsKey)
        {
            string lsValue = null;

            lsValue = ConfigurationManager.AppSettings[lsKey];

            return lsValue;
        }
    }
}