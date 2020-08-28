using DataFactory;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FieldDataTest
{
    public static class RestService
    {
        #region Constants

        private const string CONTENT_TYPE = "application/json";

        #endregion Constants

        #region Operations

        public static async Task<ServiceResult<TResult>> PostAsync<TResult, TPayload>(string address, TPayload payload)
        {
            Debug.WriteLine($"Post - {address}");
            var s = Serializer.Serialize(payload);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "YWN0aXZpdHktc2VydmljZTpwYXNzd29yZA==");
                HttpResponseMessage response = await client.PostAsync(address, new StringContent(s, Encoding.UTF8, CONTENT_TYPE));

                if (!response.IsSuccessStatusCode) return new ServiceResult<TResult> { Status = ServiceResultStatus.Error, Message = response.ReasonPhrase };

                var stream = await response.Content.ReadAsStringAsync();
                if (stream == null) throw new Exception("No response from server.");
                var result = Serializer.Deserialize<ServiceResult<TResult>>(stream);
                return (result.Status == ServiceResultStatus.Success) ? result : new ServiceResult<TResult> { Status = ServiceResultStatus.Error, Message = response.ReasonPhrase };
            }
        }

        public static async Task<ServiceResult<TResult>> GetAsync<TResult>(string address)
        {
            Debug.WriteLine($"Get - {address}");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "YWN0aXZpdHktc2VydmljZTpwYXNzd29yZA==");
                HttpResponseMessage response = await client.GetAsync(address);

                if (!response.IsSuccessStatusCode) return new ServiceResult<TResult> { Status = ServiceResultStatus.Error, Message = response.ReasonPhrase };

                var stream = await response.Content.ReadAsStringAsync();
                if (stream == null) throw new Exception("No response from server.");
                var result = Serializer.Deserialize<ServiceResult<TResult>>(stream);
                return (result.Status == ServiceResultStatus.Success) ? result : new ServiceResult<TResult> { Status = ServiceResultStatus.Error, Message = response.ReasonPhrase };
            }
        }

        #endregion Operations
    }
}
