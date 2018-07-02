using System;
using System.Configuration;

namespace Puzzle.API
{
    public class Configuration
    {
        private static readonly Lazy<string> _appName = new Lazy<string>(() => ConfigurationManager.AppSettings["AppName"]);
        private static readonly Lazy<string> _servicesURL = new Lazy<string>(() => ConfigurationManager.AppSettings["ServicesURL"]);

        /// <summary>
        /// Application Name » To be taken from Config file
        /// </summary>
        public static String AppName { get { return _appName.Value; } }
        public static String ServicesURL { get { return _servicesURL.Value; } }
    }
}
