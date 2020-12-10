using DataFactory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FieldDataTest
{
    public class RestService : IDisposable
    {
        #region Constants

        private const string CONTENT_TYPE = "application/json";

        #endregion Constants

        #region Fields

        private HttpClient _Client = new HttpClient();

        #endregion Fields

        #region Properties

        public bool EchoOn { get; set; } = false;

        #endregion Properties

        #region Operations

        public async Task<TResult> PostAsync<TResult, TPayload>(string address, TPayload payload)
        {
            var s = Serializer.Serialize(payload);
            if (EchoOn) Debug.WriteLine($"Post - {address}\r\n{s}");

            if (_Client == null) _Client = new HttpClient();

            _Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "YWN0aXZpdHktc2VydmljZTpwYXNzd29yZA==");
            HttpResponseMessage response = await _Client.PostAsync(address, new StringContent(s, Encoding.UTF8, CONTENT_TYPE));

            if (!response.IsSuccessStatusCode) throw new Exception(response.ReasonPhrase);

            var stream = await response.Content.ReadAsStringAsync();
            if (stream == null) throw new Exception("No response from server.");
            if (EchoOn) Debug.WriteLine($"Post Response - {address}\r\n{stream}");
            var result = Serializer.Deserialize<TResult>(stream);
            return result;
        }

        public async Task<TResult> GetAsync<TResult>(string address)
        {
            if (EchoOn) Debug.WriteLine($"Get - {address}");
            if (_Client == null) _Client = new HttpClient();
            _Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "YWN0aXZpdHktc2VydmljZTpwYXNzd29yZA==");
            HttpResponseMessage response = await _Client.GetAsync(address);

            if (!response.IsSuccessStatusCode) throw new Exception(response.ReasonPhrase);

            var stream = await response.Content.ReadAsStringAsync();
            if (stream == null) throw new Exception("No response from server.");
            var result = Serializer.Deserialize<TResult>(stream);
            return result;
        }

        public async Task DeleteAsync(string address)
        {
            if (EchoOn) Debug.WriteLine($"Delete - {address}");
            if (_Client == null)
            {
                _Client = new HttpClient();
                _Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "YWN0aXZpdHktc2VydmljZTpwYXNzd29yZA==");
            }
            HttpResponseMessage response = await _Client.DeleteAsync(address);
            if (!response.IsSuccessStatusCode) throw new Exception(response.ReasonPhrase);
        }

        public async Task<TReturn> PostFileDataAsync<TReturn>(string address, string key, byte[] data, string filename)
        {
            //  create a boundary if necessary
            var boundary = ((int)(new Random().NextDouble() * 100000)).ToString();
            boundary = boundary.PadLeft(15, '-');

            //  create content
            var content = new MultipartFormDataContent(boundary);
            var cc = new ByteArrayContent(data);
            cc.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            content.Add(cc, $"\"{key}\"", $"\"{filename}\"");

            _Client = _Client ?? new HttpClient();
            _Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "bWVkaWEtc2VydmljZTpwYXNzd29yZA==");
            //  make the call
            var response = await _Client.PostAsync(address, content);

            var stream = await response.Content.ReadAsStringAsync();
            if (stream == null) throw new Exception("No response from server.");
            if (EchoOn) Debug.WriteLine($"Post Response - {address}\r\n{stream}");
            var result = Serializer.Deserialize<TReturn>(stream);
            return result;
        }

        public void Dispose()
        {
            if (_Client == null) return;
            _Client.Dispose();
            _Client = null;
        }

        #endregion Operations
    }
}
