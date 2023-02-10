using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration.CommandLine;

namespace Configuration
{
    public static class ConfigurationHostBuilderExtensions
    {
        public static IConfigurationBuilder AddConfiguration(
            this IConfigurationBuilder builder,
            IHostEnvironment env)
        {
            string basePath = env.ContentRootPath;

            CommandLineConfigurationSource commandLineSource = builder
                .Sources
                .OfType<CommandLineConfigurationSource>()
                .FirstOrDefault();

            builder.Sources.Clear();
            
            // 00-01 appsettings
            builder = builder
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", true, true);

            // 03 environment variables
            builder = builder.AddEnvironmentVariables();

            // 04 azure keyvault
            builder = AddAzureKeyVault(builder);

            // 05 commandline args
            if (commandLineSource != null)
            {
                builder = builder.Add(commandLineSource);
            }

            return builder;
        }

        private static IConfigurationBuilder AddAzureKeyVault(IConfigurationBuilder builder)
        {
            IConfiguration config = builder.Build();

            string keyVaultUrl = config["KeyVault:Uri"];
            Console.WriteLine($"keyVaultUri={keyVaultUrl}");
            if (!Uri.TryCreate(keyVaultUrl, UriKind.Absolute, out Uri keyVaultUri))
            {
                return builder;
            }

            var secretClient = new SecretClient(
                keyVaultUri,
                new DefaultAzureCredential(new DefaultAzureCredentialOptions()
                {
                    ExcludeSharedTokenCacheCredential = true,
                }));

            AzureKeyVaultConfigurationOptions options =
                new AzureKeyVaultConfigurationOptions
                {
                    Manager = new HandlingRbacKeyVaultSecretManager(secretClient),
                };
            int refreshIntervalMinutes = 60;

            if (int.TryParse(
                    config["KeyVault:RefreshIntervalMinutes"],
                    out int refreshInterval) &&
                refreshInterval > 0)
            {
                if (refreshInterval < 30)
                {
                    throw new InvalidOperationException($"Invalid refresh interval {refreshInterval}, minimum refresh interval is 30 minutes");
                }

                refreshIntervalMinutes = refreshInterval;
            }

            options.ReloadInterval = TimeSpan.FromMinutes(refreshIntervalMinutes);

            builder = builder.AddAzureKeyVault(secretClient, options);

            return builder;
        }

        static bool IsDevelopment(string environmentName)
        {
            return string.Equals(
                       "Development",
                       environmentName,
                       StringComparison.InvariantCultureIgnoreCase);
        }
    }
}