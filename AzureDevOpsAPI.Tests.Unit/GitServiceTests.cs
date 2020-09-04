//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.TeamFoundation.SourceControl.WebApi;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Newtonsoft.Json;
//using Moq;

//namespace AzureDevOpsAPI.Tests.Unit
//{
//    public class GitServiceTests
//    {
//        private const string Pat = "abc123";

//        [TestClass]
//        public class Constructor : GitServiceTestBase
//        {
//            [TestMethod]
//            public void InitializesServiceCorrect()
//            {
//                Assert.IsNotNull(new GitService(Pat));
//            }

//            [TestMethod]
//            [ExpectedException(typeof(ArgumentNullException))]
//            public void ThrowsArgumentNullExceptionWhenPersonalAccessTokenIsNull()
//            {
//                var newGitService = new GitService(null);
//            }
//        }

//        //[TestClass]
//        //public class GetRepositoriesAsync : GitServiceTestBase
//        //{
//        //    [TestMethod]
//        //    public async Task ProcessesRequestCorrect()
//        //    {
//        //        var repos = new GitRepository[]
//        //        {
//        //            new GitRepository
//        //            {
//        //                Name = "Name"
//        //            },
//        //            new GitRepository
//        //            {
//        //                Name = "Name2"
//        //            }
//        //        };

//        //        // Arrange
//        //        GitServiceMock
//        //            .Setup(s => s.GetRepositoriesAsync())
//        //            .ReturnsAsync(repos)
//        //            .Verifiable();

//        //        var sut = CreateService();

//        //        // Act
//        //        var result = sut.GetRepositoriesAsync();

//        //        // Assert
//        //        VerifyMocks();
//        //        Assert.IsNotNull(result);
//        //        Assert.AreEqual(repos, result);
//        //    }
//        //}

//        public class GitServiceTestBase
//        {
//            private const string PersonalAccessToken = "abc123";
//            private const string ApiAddress = "https://dev.azure.com/robinbette/_apis/git/repositories";
//            protected readonly Mock<IGitService> GitServiceMock = new Mock<IGitService>();

//            private readonly HttpClient _client = new HttpClient
//            {
//                DefaultRequestHeaders =
//                {
//                    Authorization = new AuthenticationHeaderValue("Basic",
//                        Convert.ToBase64String(
//                            Encoding.ASCII.GetBytes($"\"\":{PersonalAccessToken}"))),
//                    Accept = { new MediaTypeWithQualityHeaderValue("application/json") }
//                }
//            };

//            protected GitService CreateService()
//            {
//                var httpClient = _client;
//                return new GitService(PersonalAccessToken);
//            }

//            protected void VerifyMocks()
//            {
//                GitServiceMock.Verify();
//            }

//            protected static HttpResponseMessage GetMockResponse<T>(T fakeServerResponse) => new HttpResponseMessage(HttpStatusCode.OK)
//            {
//                Content = new StringContent(
//                    JsonConvert.SerializeObject(fakeServerResponse),
//                    Encoding.UTF8, 
//                    "application/json")
//            };

//            protected static HttpResponseMessage GetNoContentMockResponse()
//            {
//                return new HttpResponseMessage(HttpStatusCode.NoContent);
//            }

//            private static bool IsRequestMatch<TRequest>(HttpRequestMessage message, TRequest request, HttpMethod expectedHttpMethod, string expectedPath)
//            {
//                // Assert that the correct http method is used
//                if (message.Method != expectedHttpMethod)
//                {
//                    return false;
//                }

//                // Assert that the correct uri is called
//                if (message.RequestUri.AbsoluteUri != ApiAddress + expectedPath)
//                {
//                    return false;
//                }

//                // Assert that the correct message body is included
//                return message.Content.ReadAsStringAsync().Result == JsonConvert.SerializeObject(request);
//            }
//        }
//    }
//}
