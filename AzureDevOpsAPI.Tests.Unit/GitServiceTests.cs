using System;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace AzureDevOpsAPI.Tests.Unit
{
    public class GitServiceTests
    {
        [TestClass]
        public class Constructor : GitServiceMock
        {
            [TestMethod]
            public void InitializesServiceCorrect()
            {
                const string pat = "abc123";
                var service = new GitService(pat);
                Assert.IsNotNull(service);
            }
        }

        public class GitServiceMock
        {
            private const string PersonalAccessToken = "abc123";
            private const string ApiAddress = "https://dev.azure.com/robinbette/_apis/git/repositories";

            protected GitService CreateService()
            {
                var httpClient = new HttpClient()
                {
                    BaseAddress = new Uri(ApiAddress)
                };

                return new GitService(PersonalAccessToken);
            }

            protected static HttpResponseMessage GetMockResponse<T>(T fakeServerResponse) => new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    JsonConvert.SerializeObject(fakeServerResponse),
                    Encoding.UTF8, 
                    "application/json")
            };

            protected static HttpResponseMessage GetNoContentMockResponse()
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }

            private static bool IsRequestMatch<TRequest>(HttpRequestMessage message, TRequest request, HttpMethod expectedHttpMethod, string expectedPath)
            {
                // Assert that the correct http method is used
                if (message.Method != expectedHttpMethod)
                {
                    return false;
                }

                // Assert that the correct uri is called
                if (message.RequestUri.AbsoluteUri != ApiAddress + expectedPath)
                {
                    return false;
                }

                // Assert that the correct message body is included
                return message.Content.ReadAsStringAsync().Result == JsonConvert.SerializeObject(request);
            }
        }
    }
}
