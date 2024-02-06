using System;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ebikeshopserver.Utils
{
    public class SecretSettingsProvider
    {
        private static IConfiguration Configuration { get; set; }

        static SecretSettingsProvider()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
        }

        public static string GetPasswordHasher()
        {
            return Configuration["ConnectionString:SecretHasher"];
        }

        public static string GetTokenHasher()
        {
            return Configuration["ConnectionString:TokenHasher"];
        }
    }
}
