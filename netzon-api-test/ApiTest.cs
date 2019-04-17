using Microsoft.AspNetCore.Mvc.Testing;
using netzon_api_test.API;
using netzon_api_test.Fixtures;
using netzon_assetment.Models;
using netzon_assetment.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Ioc.Autofac;
using Xunit.Priority;

namespace netzon_api_test
{
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class ApiTest : IClassFixture<DatabaseFixture>,
        IClassFixture<NetzonApi>,
        IClassFixture<WebApplicationFactory<netzon_assetment.Startup>>
    {
        private readonly WebApplicationFactory<netzon_assetment.Startup> _factory;
        private readonly NetzonApi _api;
        private readonly DatabaseFixture _dbFixture;

        public ApiTest(WebApplicationFactory<netzon_assetment.Startup> factory, NetzonApi api, DatabaseFixture dbFixture)
        {
            _factory = factory;
            _api = api;
            _dbFixture = dbFixture;
            _api.SetClient(_factory.CreateClient());
        }

        [Theory, Priority(0)]
        [InlineData("John", "Doe", "john.doe@testemail.com", "n3tz0n@123")]
        [InlineData("Maple", "Sandler", "maple.sandler@testemail.com", "n3tz0n@123")]
        [InlineData("Marlene", "Land", "marlene.land@testemail.com", "n3tz0n@123")]
        public async Task RegisterSuccess(string firstname, string lastname, string email, string password)
        {
            var response = await _api.Register(firstname, lastname, email, password);
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            var user = JObject.Parse(response.Content.ReadAsStringAsync().Result).ToObject<User>();
            Assert.True(string.IsNullOrWhiteSpace(user.Password));
        }

        [Theory, Priority(1)]
        [InlineData("John Test", "Doe Test", "john.doe@testemail.com", "n3tz0n@123")]
        [InlineData("Maple Test", "Sandler Test", "maple.sandler@testemail.com", "n3tz0n@123")]
        [InlineData("Marlene Test", "Land Test", "marlene.land@testemail.com", "n3tz0n@123")]
        public async Task UpdateInfomationSuccess(string firstname, string lastname, string email, string password)
        {
            // Login to get token
            var resp = await _api.Login(email, password);
            resp.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", resp.Content.Headers.ContentType.ToString());
            var user = JObject.Parse(resp.Content.ReadAsStringAsync().Result).ToObject<User>();
            Assert.True(string.IsNullOrWhiteSpace(user.Password));
            Assert.True(!string.IsNullOrWhiteSpace(user.Token));
            var token = user.Token;

            resp = await _api.Update(token, user.ID, firstname, lastname);
            resp.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", resp.Content.Headers.ContentType.ToString());
            user = JObject.Parse(resp.Content.ReadAsStringAsync().Result).ToObject<User>();
            Assert.True(string.IsNullOrWhiteSpace(user.Password));
            Assert.Equal(firstname, user.FirstName);
            Assert.Equal(lastname, user.LastName);

            resp = await _api.GetById(token, user.ID);
            resp.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", resp.Content.Headers.ContentType.ToString());
            user = JObject.Parse(resp.Content.ReadAsStringAsync().Result).ToObject<User>();
            Assert.True(string.IsNullOrWhiteSpace(user.Password));
            Assert.Equal(firstname, user.FirstName);
            Assert.Equal(lastname, user.LastName);
        }

        [Theory, Priority(2)]
        [InlineData("John", "Doe", "john.doe@testemail.com")]
        [InlineData("Maple", "Sandler", "maple.sandler@testemail.com")]
        [InlineData("Marlene", "Land", "marlene.land@testemail.com")]
        public async Task AdminUpdateInfomationSuccess(string firstname, string lastname, string email)
        {
            // Login to get token
            var resp = await _api.Login("admin@netzon.com", "n3tz0n@123");
            resp.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", resp.Content.Headers.ContentType.ToString());
            var user = JObject.Parse(resp.Content.ReadAsStringAsync().Result).ToObject<User>();
            Assert.True(string.IsNullOrWhiteSpace(user.Password));
            Assert.True(!string.IsNullOrWhiteSpace(user.Token));
            var token = user.Token;

            resp = await _api.Search(token, firstname, 1, 0);
            resp.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", resp.Content.Headers.ContentType.ToString());
            var respString = resp.Content.ReadAsStringAsync().Result;
            var users = JArray.Parse(respString).Select(j => j.ToObject<User>());
            Assert.True(users != null && users.Count() > 0);
            Assert.Collection(users, u => string.IsNullOrWhiteSpace(u.Password));

            var targetUser = users.FirstOrDefault(u => u.Email == email);
            Assert.NotNull(targetUser);

            resp = await _api.Update(token, targetUser.ID, firstname, lastname);
            resp.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", resp.Content.Headers.ContentType.ToString());
            user = JObject.Parse(resp.Content.ReadAsStringAsync().Result).ToObject<User>();
            Assert.True(string.IsNullOrWhiteSpace(user.Password));
            Assert.Equal(firstname, user.FirstName);
            Assert.Equal(lastname, user.LastName);

            resp = await _api.GetById(token, user.ID);
            resp.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", resp.Content.Headers.ContentType.ToString());
            user = JObject.Parse(resp.Content.ReadAsStringAsync().Result).ToObject<User>();
            Assert.True(string.IsNullOrWhiteSpace(user.Password));
            Assert.Equal(firstname, user.FirstName);
            Assert.Equal(lastname, user.LastName);
        }

        [Theory, Priority(3)]
        [InlineData("john.doe@testemail.com", "n3tz0n@123")]
        public async Task UpgradeUserSuccess(string email, string password)
        {
            // Login to get token
            var resp = await _api.Login("admin@netzon.com", "n3tz0n@123");
            resp.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", resp.Content.Headers.ContentType.ToString());
            var user = JObject.Parse(resp.Content.ReadAsStringAsync().Result).ToObject<User>();
            Assert.True(string.IsNullOrWhiteSpace(user.Password));
            Assert.True(!string.IsNullOrWhiteSpace(user.Token));
            var token = user.Token;

            // Only admin can search users
            resp = await _api.Search(token, "John", 1, 0);
            resp.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", resp.Content.Headers.ContentType.ToString());
            var respString = resp.Content.ReadAsStringAsync().Result;
            var users = JArray.Parse(respString).Select(j => j.ToObject<User>());
            Assert.True(users != null && users.Count() > 0);
            Assert.Collection(users, u => string.IsNullOrWhiteSpace(u.Password));

            var targetUser = users.FirstOrDefault(u => u.Email == email);
            Assert.NotNull(targetUser);

            // Upgrade target user
            resp = await _api.UpgradeUser(token, targetUser.ID);
            resp.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", resp.Content.Headers.ContentType.ToString());

            // Login with new admin
            resp = await _api.Login(email, password);
            resp.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", resp.Content.Headers.ContentType.ToString());
            user = JObject.Parse(resp.Content.ReadAsStringAsync().Result).ToObject<User>();
            Assert.True(string.IsNullOrWhiteSpace(user.Password));
            Assert.True(!string.IsNullOrWhiteSpace(user.Token));
            token = user.Token;

            resp = await _api.Search(token, "John", 1, 0);
            resp.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", resp.Content.Headers.ContentType.ToString());
            respString = resp.Content.ReadAsStringAsync().Result;
            users = JArray.Parse(respString).Select(j => j.ToObject<User>());
            Assert.True(users != null && users.Count() > 0);
            Assert.Collection(users, u => string.IsNullOrWhiteSpace(u.Password));
        }

        [Theory, Priority(4)]
        [InlineData("John", "john.doe@testemail.com")]
        public async Task AdminDeleteUserSuccess(string firstname, string email)
        {
            // Login to get token
            var resp = await _api.Login("admin@netzon.com", "n3tz0n@123");
            resp.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", resp.Content.Headers.ContentType.ToString());
            var user = JObject.Parse(resp.Content.ReadAsStringAsync().Result).ToObject<User>();
            Assert.True(string.IsNullOrWhiteSpace(user.Password));
            Assert.True(!string.IsNullOrWhiteSpace(user.Token));
            var token = user.Token;

            // Only admin can search users
            resp = await _api.Search(token, firstname, 1, 0);
            resp.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", resp.Content.Headers.ContentType.ToString());
            var respString = resp.Content.ReadAsStringAsync().Result;
            var users = JArray.Parse(respString).Select(j => j.ToObject<User>());
            Assert.True(users != null && users.Count() > 0);
            Assert.Collection(users, u => string.IsNullOrWhiteSpace(u.Password));

            var targetUser = users.FirstOrDefault(u => u.Email == email);
            Assert.NotNull(targetUser);

            // Upgrade target user
            resp = await _api.Delete(token, targetUser.ID);
            resp.EnsureSuccessStatusCode();
        }

        [Theory, Priority(5)]
        [InlineData("maple.sandler@testemail.com", "n3tz0n@123")]
        [InlineData("marlene.land@testemail.com", "n3tz0n@123")]
        public async Task DeleteUserSuccess(string email, string password)
        {
            // Login to get token
            var resp = await _api.Login(email, password);
            resp.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", resp.Content.Headers.ContentType.ToString());
            var user = JObject.Parse(resp.Content.ReadAsStringAsync().Result).ToObject<User>();
            Assert.True(string.IsNullOrWhiteSpace(user.Password));
            Assert.True(!string.IsNullOrWhiteSpace(user.Token));
            var token = user.Token;

            // Upgrade target user
            resp = await _api.Delete(token, user.ID);
            resp.EnsureSuccessStatusCode();
        }
    }
}
