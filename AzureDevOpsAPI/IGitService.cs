using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace AzureDevOpsAPI
{
    public interface IGitService
    {
        HttpClient Client { get; set; }
        string ApiVersion { get; set; }
        string BaseUrl { get; set; }
        string Organization { get; set; }
        string Project { get; set; }

        Task<GitRepository[]> GetRepositoriesAsync();
        Task<GitRepository> GetRepositoryAsync(string repositoryIdOrName);
        Task<HttpResponseMessage> CreateFileAsync(string jsonBodyFilePath, string gitRefName, string gitReferenceOldObjectId,
            string fileName, string repository, string base64EncodedFileContent, string repositoryId);
        Task<string> GetOldObjectIdAsync(string repositoryId);
    }
}
