using netzon_assetment.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace netzon_api_test.API
{
    public class NetzonApi
    {
        private HttpClient _client;
        public NetzonApi SetClient(HttpClient client)
        {
            _client = client;
            return this;
        }
        public async Task<HttpResponseMessage> Login(string email, string password)
        {
            var loginRequest = JsonConvert.SerializeObject(new
            {
                Email = email,
                Password = password
            });
            var loginContent = new StringContent(loginRequest, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1/authentication/login", loginContent);
            return response;
        }

        public async Task<HttpResponseMessage> Register(string firstname, string lastname, string email, string password)
        {
            var json = JsonConvert.SerializeObject(new UserRegistrationViewModel
            {
                Firstname = firstname,
                Lastname = lastname,
                Email = email,
                Password = password
            });
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/v1/users", stringContent);
            return response;
        }

        public async Task<HttpResponseMessage> UpgradeUser(string token, int userToUpgradeId)
        {
            var request = JsonConvert.SerializeObject(new UserUpgradeViewModel
            {
                UserID = userToUpgradeId
            });
            var requestContent = new StringContent(request, Encoding.UTF8, "application/json");

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsync("/api/v1/authentication/admin", requestContent);
            return response;
        }

        public async Task<HttpResponseMessage> GetById(string token, int userId)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync($"/api/v1/users/{userId}");
            return response;
        }

        public async Task<HttpResponseMessage> Search(string token, string q, int limit, int offset)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync($"/api/v1/users?q={Uri.EscapeDataString(q)}&limit={limit}&offset={offset}");
            return response;
        }

        public async Task<HttpResponseMessage> Update(string token, int userToUpdateId, string firstname, string lastname)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var request = JsonConvert.SerializeObject(new UserUpdateViewModel
            {
                UserID = userToUpdateId,
                FirstName = firstname,
                LastName = lastname
            });
            var requestContent = new StringContent(request, Encoding.UTF8, "application/json");
            var response = await _client.PatchAsync($"/api/v1/users", requestContent);
            return response;
        }

        public async Task<HttpResponseMessage> Delete(string token, int userId)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.DeleteAsync($"/api/v1/users/{userId}");
            return response;
        }
    }
}
