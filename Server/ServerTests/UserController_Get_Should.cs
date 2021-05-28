using System.Net;
using System.Net.Http;
using FluentAssertions;
using NUnit.Framework;

namespace ServerTests
{
    public class UserController_Get_Should : UsersApiTestsBase
    {
        [Test]
        public void Test2_Code404_WhenUserIdIsUnknown()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get, RequestUri = BuildUsersByIdUri("77777777-6666-6666-6666-777777777777")
            };
            request.Headers.Add("Accept", "application/json");
            var response = HttpClient.Send(request);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.ShouldNotHaveHeader("Content-Type");
        }

        [Test]
        public void Test3_Code404_WhenUserIdIsTrash()
        {
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            request.RequestUri = BuildUsersByIdUri("trash");
            request.Headers.Add("Accept", "application/json");
            var response = HttpClient.Send(request);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.ShouldNotHaveHeader("Content-Type");
        }
    }
}