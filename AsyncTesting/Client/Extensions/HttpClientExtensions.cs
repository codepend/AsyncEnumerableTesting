using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Text;
using System.Text.Json;

namespace AsyncTesting.Client.Extensions
{
    public static class HttpClientExtensions
    {
        public static async IAsyncEnumerable<T> GetStreamedIEnumerableAsync<T>(this HttpClient httpClient, string url )
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.SetBrowserResponseStreamingEnabled(true); // Enable response streaming
            //if (method == HttpMethod.Post)
            //{
            //    request.Content = new StringContent(JsonSerializer.Serialize<U>(postPayload), Encoding.UTF8, "application/json");
            //}

            // Be sure to use HttpCompletionOption.ResponseHeadersRead
            using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            using var stream = await response.Content.ReadAsStreamAsync();

            response.EnsureSuccessStatusCode();

            await foreach (T item in JsonSerializer.DeserializeAsyncEnumerable<T>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, DefaultBufferSize = 128 }))
            {
                yield return item;
            }
        }
    }
}
