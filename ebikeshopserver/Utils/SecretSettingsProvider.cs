using System;
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
            return Configuration.GetConnectionString("SecretHasher");
        }
    }
}

