using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AzureDevOpsAPI.Models;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AzureDevOpsAPI
{
    public class GitService : IGitService
    {
        public HttpClient Client { get; set; }
        public string ApiVersion { get; set; }
        public string BaseUrl { get; set; }
        public string Organization { get; set; }
        public string Project { get; set; }

        public void SetDefaultHeaders(string personalAccessToken)
        {
            Client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", "", personalAccessToken)))),
                    Accept = {new MediaTypeWithQualityHeaderValue("application/json")}
                }
            };
        }

        public async Task<GitRepository[]> GetRepositoriesAsync()
        {
            try
            {
                var responseBody = await Client.GetStringAsync($"{BaseUrl}/{Organization}/_apis/git/repositories");
                return JsonConvert.DeserializeObject<GitRepository[]>(JObject.Parse(responseBody)["value"].ToString());
            }
            catch (Exception ex)
            {
                throw new Exception($"Repositories could not be found for {Organization}" + ex);
            }
        }

        public async Task<GitRepository> GetRepositoryAsync(string repositoryIdOrName)
        {
            try
            {
                var response =
                    (await Client.GetAsync(
                        $"{BaseUrl}/{Organization}/{Project}/_apis/git/repositories/{repositoryIdOrName}"))
                    .EnsureSuccessStatusCode();
                return JsonConvert.DeserializeObject<GitRepository>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Something went wrong while getting repository with name or id: '{repositoryIdOrName}'" + ex);
            }
        }

        public async Task<HttpResponseMessage> CreateFileAsync(string jsonBodyFilePath, string gitRefName, string gitReferenceOldObjectId,
            string fileName, string repository, string base64EncodedFileContent, string repositoryId)
        {
            try
            {
                var body = Utils.CreateMessageBody(jsonBodyFilePath, "application/json",
                    new Dictionary<string, object>
                    {
                        {"gitRefName", gitRefName},
                        {"gitReferenceOldObjectId", gitReferenceOldObjectId},
                        {"gitCommitMessage", $"Added {fileName} to repository {repository}."},
                        {"gitFilePath", $"/{fileName}"},
                        {"base64EncodedFileContent", base64EncodedFileContent}
                    });

                var uri =
                    $"{BaseUrl}/{Organization}/{Project}/_apis/git/repositories/{repositoryId}/pushes?api-version={ApiVersion}";

                // Content type header
                var request = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = body
                };

                return await Client.SendAsync(request);
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Something went wrong while creation the file '{fileName}'." + ex);
            }
        }

        public async Task<string> GetOldObjectIdAsync(string repositoryId)
        {
            try
            {
                // Api version for getting references is still 5.1
                var responseBody = await Client.GetStringAsync(
                    $"{BaseUrl}/{Organization}/{Project}/_apis/git/repositories/{repositoryId}/refs?api-version=5.1");
                var gitReferences =
                    JsonConvert.DeserializeObject<GitReference[]>(JObject.Parse(responseBody)["value"].ToString());
                return gitReferences[0].objectId;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Something went wrong while getting old object id for repository with id: '{repositoryId}'" + ex);
            }
        }
    }
}
