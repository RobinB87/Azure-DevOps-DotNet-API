namespace AzureDevOpsAPI
{
    /// <summary>
    /// Configuration for Azure DevOps API services
    /// </summary>
    public class AzureDevOpsConfiguration
    {
        /// <summary>
        /// Personal Access Token for Azure DevOps
        /// </summary>
        public string PersonalAccessToken { get; set; }

        /// <summary>
        /// Version of the Azure DevOps API
        /// </summary>
        public string ApiVersion { get; set; }

        /// <summary>
        /// Uri of the Azure DevOps project
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Folder with the json files
        /// </summary>
        public string JsonFolder { get; set; }

        /// <summary>
        /// Name of the Azure DevOps organization
        /// </summary>
        public string Organization { get; set; }

        /// <summary>
        /// Name of the Azure DevOps project
        /// </summary>
        public string Project { get; set; }
    }
}