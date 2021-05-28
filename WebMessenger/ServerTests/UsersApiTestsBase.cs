using System;
using System.Net;
using System.Net.Http;
using System.Web;
using FluentAssertions;
using FluentAssertions.Common;

namespace ServerTests
{
    public abstract class UsersApiTestsBase
    {
        protected readonly HttpClient HttpClient = new HttpClient();

        protected Uri BuildUsersByIdUri(string userId)
        {
            var uriBuilder = new UriBuilder(Configuration.BaseUrl)
            {
                Path = $"/api/users/{HttpUtility.UrlEncode(userId)}"
            };
            return uriBuilder.Uri;
        }

        protected Uri BuildUsersUri()
        {
            var uriBuilder = new UriBuilder(Configuration.BaseUrl);
            uriBuilder.Path = $"/api/users";
            return uriBuilder.Uri;
        }

        protected Uri BuildUsersWithPagesUri(int? pageNumber, int? pageSize)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);
            if (pageNumber.HasValue)
                query.Add("pageNumber", pageNumber.Value.ToString());
            if (pageSize.HasValue)
                query.Add("pageSize", pageSize.Value.ToString());

            var uriBuilder = new UriBuilder(Configuration.BaseUrl)
            {
                Path = "/api/users", Query = query.ToString() ?? string.Empty
            };
            return uriBuilder.Uri;
        }

        protected void CheckUserCreated(string createdUserId, string createdUserUri, object expectedUser)
        {
            // Проверка, что идентификатор созданного пользователя возвращается в теле ответа
            CheckUser(createdUserId, expectedUser);

            // Проверка, что ссылка на созданного пользователя возвращается в заголовке Location
            var request = new HttpRequestMessage {Method = HttpMethod.Get, RequestUri = new Uri(createdUserUri)};
            request.Headers.Add("Accept", "application/json");
            var response = HttpClient.Send(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ShouldHaveHeader("Content-Type", "application/json; charset=utf-8");
            response.ShouldHaveJsonContentEquivalentTo(expectedUser);
            HttpClient.Send(new HttpRequestMessage {Method = HttpMethod.Delete, RequestUri = new Uri(createdUserUri)});
        }

        protected void CheckUser(string userId, object expectedUser)
        {
            var request = new HttpRequestMessage {Method = HttpMethod.Get, RequestUri = BuildUsersByIdUri(userId)};
            request.Headers.Add("Accept", "application/json");
            var response = HttpClient.Send(request);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ShouldHaveHeader("Content-Type", "application/json; charset=utf-8");
            response.ShouldHaveJsonContentEquivalentTo(expectedUser);
        }

        protected string CreateUser(object user)
        {
            var request = new HttpRequestMessage {Method = HttpMethod.Post, RequestUri = BuildUsersUri()};
            request.Headers.Add("Accept", "*/*");
            request.Content = user.SerializeToJsonContent();
            var response = HttpClient.Send(request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.ShouldHaveHeader("Content-Type", "application/json; charset=utf-8");

            var createdUserId = response.ReadContentAsJson().ToString();
            createdUserId.Should().NotBeNullOrEmpty();
            return createdUserId;
        }

        protected void DeleteUser(string userId)
        {
            var request = new HttpRequestMessage {Method = HttpMethod.Delete, RequestUri = BuildUsersByIdUri(userId)};
            request.Headers.Add("Accept", "*/*");
            var response = HttpClient.Send(request);

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            response.ShouldNotHaveHeader("Content-Type");
        }
    }
}