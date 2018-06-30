
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace AspectCache.Configuration
{
    public static class CacheConfigurationReader
    {
        public static CacheConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", optional: true);

            var configuration = builder.Build();
            return configuration.GetSection("CacheConfiguration").Get<CacheConfiguration>();
        }
    }
}
