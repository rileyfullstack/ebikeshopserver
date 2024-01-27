using System;
namespace ebikeshopserver.Services.SecretSettings
{
	public class SecretSettingsService
	{
        private static IConfiguration Configuration { get; set; }

        static SecretSettingsService()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
        }

        public static string GetConnectionString()
        {
            return Configuration.GetConnectionString("SecretHasher");
        }
    }
}

