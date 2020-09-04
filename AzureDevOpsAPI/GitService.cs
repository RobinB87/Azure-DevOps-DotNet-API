using AzureDevOpsAPI.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AzureDevOpsAPI
{
    /// <inheritdoc />
    public class GitService : IGitService
    {
        private readonly HttpClient _client;
        private readonly AzureDevOpsConfiguration _config;
        private readonly ILogger<GitService> _logger;

        /// <summary>
        /// Constructor for the git service
        /// </summary>
        /// <param name="clientFactory"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public GitService(IHttpClientFactory clientFactory, IOptions<AzureDevOpsConfiguration> configuration, ILogger<GitService> logger)
        {
            _client = clientFactory.CreateClient(AzureDevOpsExtensions.HttpClientName);
            _config = configuration.Value;
            _logger = logger;
            Init.ConfigureServices();
        }

        /// <inheritdoc />
        public async Task<GitRepository[]> GetRepositoriesAsync()
        {
            try
            {
                var responseBody = await _client.GetStringAsync($"{_config.BaseUrl}/{_config.Organization}/_apis/git/repositories");
                return JsonConvert.DeserializeObject<GitRepository[]>(JObject.Parse(responseBody)["value"].ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Repositories could not be found for {_config.Organization}");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<GitRepository> GetRepositoryAsync(string repositoryIdOrName)
        {
            try
            {
                var response =
                    (await _client.GetAsync(
                        $"{_config.BaseUrl}/{_config.Organization}/{_config.Project}/_apis/git/repositories/{repositoryIdOrName}"))
                    .EnsureSuccessStatusCode();
                return JsonConvert.DeserializeObject<GitRepository>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong while getting repository with name or id: '{repositoryIdOrName}'");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> CreateFileAsync(string gitRefName, string gitReferenceOldObjectId,
            string fileName, string repository, string base64EncodedFileContent, string repositoryId)
        {
            try
            {
                var body = Utils.CreateMessageBody($"{_config.JsonFolder}/create-file.json", "application/json",
                    new Dictionary<string, object>
                    {
                        {"gitRefName", gitRefName},
                        {"gitReferenceOldObjectId", gitReferenceOldObjectId},
                        {"gitCommitMessage", $"Added {fileName} to repository {repository}."},
                        {"gitFilePath", $"/{fileName}"},
                        {"base64EncodedFileContent", base64EncodedFileContent}
                    });

                var uri =
                    $"{_config.BaseUrl}/{_config.Organization}/{_config.Project}/_apis/git/repositories/{repositoryId}/pushes?api-version={_config.ApiVersion}";

                // Content type header
                var request = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = body
                };

                return await _client.SendAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong while creation the file '{fileName}'.");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<string> GetOldObjectIdAsync(string repositoryId)
        {
            try
            {
                // Api version for getting references is still 5.1
                var responseBody = await _client.GetStringAsync(
                    $"{_config.BaseUrl}/{_config.Organization}/{_config.Project}/_apis/git/repositories/{repositoryId}/refs?api-version=5.1");
                var gitReferences =
                    JsonConvert.DeserializeObject<GitReference[]>(JObject.Parse(responseBody)["value"].ToString());
                return gitReferences[0].objectId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"Something went wrong while getting old object id for repository with id: '{repositoryId}'");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> ReDeployRelease(int releaseId, int environmentId)
        {
            try
            {
                var body = Utils.CreateMessageBody($"{_config.JsonFolder}/patch-re-deploy.json", "application/json");

                _config.BaseUrl = "vsrm.dev.azure.com";
                var uri = $"{_config.BaseUrl}/{_config.Organization}/{_config.Project}/_apis/Release/releases/{releaseId}/environments/{environmentId}?api-version={_config.ApiVersion}-preview.1";

                // Content type header
                // TODO: Put must become patch
                var request = new HttpRequestMessage(HttpMethod.Put, uri)
                {
                    Content = body
                };

                return await _client.SendAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong while re-deploying the release with release id: '{releaseId}'");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<HttpResponseMessage> ApproveRelease(int approvalId)
        {
            try
            {
                var body = Utils.CreateMessageBody($"{_config.JsonFolder}/create-approval.json", "application/json");

                _config.BaseUrl = "vsrm.dev.azure.com";
                var uri = $"{_config.BaseUrl}/{_config.Organization}/{_config.Project}/_apis/release/approvals/{approvalId}?api-version={_config.ApiVersion}";

                // Content type header
                var request = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = body
                };

                return await _client.SendAsync(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"Something went wrong while approving the release with with approval id: '{approvalId}'");
                throw;
            }
        }
    }
}
