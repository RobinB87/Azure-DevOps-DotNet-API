using System;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AzureDevOpsAPI
{
    /// <summary>
    /// Service collection extensions for the Azure DevOps class library.
    /// </summary>
    public static class AzureDevOpsExtensions
    {
        /// <summary>
        /// The name of the HttpClient to be used for communication with the Azure DevOps API.
        /// </summary>
        public const string HttpClientName = "AzureDevOps";

        /// <summary>
        /// Adds services for communicating with the Azure DevOps API.
        /// </summary>
        public static void AddAzureDevOps(this IServiceCollection services, IConfiguration configuration)
        {
            var azureDevOpsConfiguration = configuration.Get<AzureDevOpsConfiguration>();

            // Register configuration
            services.Configure<AzureDevOpsConfiguration>(configuration);

            // Register services
            services.AddTransient<IGitService, GitService>();

            // Register named HttpClient
            services.AddHttpClient(HttpClientName, config =>
            {
                config.DefaultRequestHeaders.Add("Accept", $"application/json;api-version={azureDevOpsConfiguration.ApiVersion}");
                config.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.ASCII.GetBytes($":{azureDevOpsConfiguration.PersonalAccessToken}")));
            });
        }
    }
}
