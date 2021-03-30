using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectBoss.Api;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebMotions.Fake.Authentication.JwtBearer;

namespace ProjectBoss.Tests.IntegrationTests.Base
{
    public class HttpRequestIntegrationTestsBase : IntegrationTestsBase
    {
        private readonly TestServer server;
        private readonly HttpClient client;
        private readonly IHost host;

        public HttpRequestIntegrationTestsBase()
        {
            server = new TestServer(new WebHostBuilder().UseEnvironment("IntegrationTest").UseStartup<Startup>());
            client = server.CreateClient();
        }

        protected async Task<HttpResponseMessage> DoPostRequest(string endpoint, object body)
        {
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint);
            httpRequest.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            return await client.SendAsync(httpRequest);
        }

        protected async Task<HttpResponseMessage> DoPostRequest(string endpoint, object body, HttpMethod method)
        {
            HttpRequestMessage httpRequest = new HttpRequestMessage(method, endpoint);
            httpRequest.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            return await client.SendAsync(httpRequest);
        }

        protected async Task<HttpResponseMessage> DoGetRequest(string endpoint, string parameters)
            => await client.GetAsync($"{endpoint}/?{parameters}");        

        protected List<T> GetMultipleResults<T>(string responseString) where T: class
            => JsonSerializer.Deserialize<List<T>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IgnoreNullValues = true });

        protected T GetSingleResult<T>(string responseString) where T : class
            => JsonSerializer.Deserialize<T>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IgnoreNullValues = true });

        protected T GetStructResult<T>(string responseString) where T : struct
            => JsonSerializer.Deserialize<T>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IgnoreNullValues = true });
    }
}
