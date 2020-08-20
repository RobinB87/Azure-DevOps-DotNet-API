using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace AzureDevOpsAPI
{
    public static class Utils
    {
        /// <summary>
        /// Constructs a message body based on a json file and replaces given placeholders.
        /// </summary>
        public static StringContent CreateMessageBody(string filePath, string mediaType, Dictionary<string, object> placeholders = null)
        {
            if (!File.Exists(filePath)) return null;
            var body = File.ReadAllText(filePath);
            placeholders?.ForEach(x => body = body.Replace($"{{{{{x.Key}}}}}", x.Value.ToString()));
            return new StringContent(body, Encoding.UTF8, mediaType);
        }
    }
}
