using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace AzureDevOpsAPI
{
    /// <summary>
    /// The service with methods to communicate with git
    /// </summary>
    public interface IGitService
    {
        /// <summary>
        /// Get all repositories
        /// </summary>
        Task<GitRepository[]> GetRepositoriesAsync();

        /// <summary>
        /// Get a repository based on id or name
        /// </summary>
        Task<GitRepository> GetRepositoryAsync(string repositoryIdOrName);

        /// <summary>
        /// Create a file in a repository
        /// </summary>
        Task<HttpResponseMessage> CreateFileAsync(string gitRefName, string gitReferenceOldObjectId,
            string fileName, string repository, string base64EncodedFileContent, string repositoryId);

        /// <summary>
        /// Get the old object id, required for CreateFileAsync
        /// </summary>
        Task<string> GetOldObjectIdAsync(string repositoryId);

        /// <summary>
        /// Redeploy a release
        /// </summary>
        /// <param name="releaseId">The release id</param>
        /// <param name="environmentId">The environment id</param>
        Task<HttpResponseMessage> ReDeployRelease(int releaseId, int environmentId);

        /// <summary>
        /// Approve a release
        /// </summary>
        /// <param name="approvalId"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> ApproveRelease(int approvalId);
    }
}
