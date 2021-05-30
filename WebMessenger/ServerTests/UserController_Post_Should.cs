using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Server.Models;

namespace ServerTests
{
    public class Tests : UsersApiTestsBase
    {
        [Test]
        public void Test1_Code201_WhenAllIsFine()
        {
            var request = new HttpRequestMessage {Method = HttpMethod.Post, RequestUri = BuildUsersUri()};
            request.Headers.Add("Accept", "*/*");
            var newName = Guid.NewGuid().ToString().Replace("-", "");
            request.Content = new
            {
                login = newName,
                password = "Jackson"
            }.SerializeToJsonContent();
            var response = HttpClient.Send(request);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.ShouldHaveHeader("Content-Type", "application/json; charset=utf-8");

            var createdUserId = response.ReadContentAsJson().ToString();
            createdUserId.Should().NotBeNullOrEmpty();
            var createdUserUri = response.GetRequiredHeader("Location").SingleOrDefault();
            createdUserUri.Should().NotBeNullOrEmpty();

            CheckUserCreated(createdUserId, createdUserUri, new
            {
                id = createdUserId,
                login = newName,
                password = "Jackson",
                avatarFilePath = "null",
                chats = new List<Chat>()
            });
        }

        [Test]
        public void Test2_Code400_WhenEmptyContent()
        {
            var request = new HttpRequestMessage {Method = HttpMethod.Post, RequestUri = BuildUsersUri()};
            request.Headers.Add("Accept", "*/*");
            request.AddEmptyContent("application/json; charset=utf-8");
            var response = HttpClient.Send(request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.ShouldNotHaveHeader("Content-Type");
        }

        [Test]
        public void Test3_Code422_WhenEmptyLogin()
        {
            var request = new HttpRequestMessage {Method = HttpMethod.Post, RequestUri = BuildUsersUri()};
            request.Headers.Add("Accept", "*/*");
            request.Content = new
            {
                password = "Jackson"
            }.SerializeToJsonContent();
            var response = HttpClient.Send(request);

            response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            response.ShouldHaveHeader("Content-Type", "application/json; charset=utf-8");
            var responseContent = response.ReadContentAsJson() as JObject;
            responseContent.Should().NotBeNull();
            responseContent.GetValue("Login").Should().NotBeNullOrEmpty();
        }

        [Test]
        public void Test4_Code422_WhenLoginWithUnAllowedChars()
        {
            var request = new HttpRequestMessage {Method = HttpMethod.Post, RequestUri = BuildUsersUri()};
            request.Headers.Add("Accept", "*/*");
            request.Content = new
            {
                login = "!jackson!",
                password = "Jackson"
            }.SerializeToJsonContent();
            var response = HttpClient.Send(request);

            response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
            response.ShouldHaveHeader("Content-Type", "application/json; charset=utf-8");
            var responseContent = response.ReadContentAsJson() as JObject;
            responseContent.Should().NotBeNull();
            responseContent.GetValue("Login").Should().NotBeNullOrEmpty();
        }
    }
}