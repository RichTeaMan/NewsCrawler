using System;
using System.Collections.Generic;
using System.Text;

namespace NewsCrawler
{
    public class Configuration
    {
        public Configuration(string documentServerUrl)
        {
            DocumentServerUrl = documentServerUrl;
        }
        public string DocumentServerUrl { get; }
    }
}
