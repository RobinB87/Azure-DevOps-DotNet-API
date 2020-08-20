namespace AzureDevOpsAPI.Models
{
    public class GitReference
    {
        public string name { get; set; }
        public string objectId { get; set; }
        public GitReferenceCreator creator { get; set; }
        public string url { get; set; }
    }
}