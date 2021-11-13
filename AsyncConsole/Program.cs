// See https://aka.ms/new-console-template for more information
using AsyncTesting.Shared;
using System.Buffers;
using System.Text;
using System.Text.Json;

await NegotiateRawJsonStreamAsync();
await NegotiateJsonStreamAsync();

Console.ReadKey();

static async Task NegotiateRawJsonStreamAsync()
{
	Console.WriteLine($"-- {nameof(NegotiateRawJsonStreamAsync)} --");
	Console.WriteLine($"[{DateTime.UtcNow:hh:mm:ss.fff}] Receving weather forecasts . . .");

	using HttpClient httpClient = new();
	httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

	using HttpResponseMessage response = await httpClient.GetAsync("https://localhost:7181/WeatherForecast", HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

	response.EnsureSuccessStatusCode();
	using Stream responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

	while (true)
	{
		byte[] buffer = ArrayPool<byte>.Shared.Rent(128);
		int bytesRead = await responseStream.ReadAsync(buffer);

		Console.WriteLine($"[{DateTime.UtcNow:hh:mm:ss.fff}] ({bytesRead}/{buffer.Length}) {Encoding.UTF8.GetString(buffer)}");

		ArrayPool<byte>.Shared.Return(buffer);

		if (bytesRead == 0)
		{
			break;
		}
	}

	Console.WriteLine($"[{DateTime.UtcNow:hh:mm:ss.fff}] Weather forecasts has been received.");
	Console.WriteLine();
}

static async Task NegotiateJsonStreamAsync()
{
	Console.WriteLine($"-- {nameof(NegotiateJsonStreamAsync)} --");
	Console.WriteLine($"[{DateTime.UtcNow:hh:mm:ss.fff}] Receving weather forecasts . . .");

	using HttpClient httpClient = new();
	httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

	using HttpResponseMessage response = await httpClient.GetAsync("https://localhost:7181/WeatherForecast", HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

	response.EnsureSuccessStatusCode();
	using Stream responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

	await foreach (WeatherForecast weatherForecast in JsonSerializer.DeserializeAsyncEnumerable<WeatherForecast>(responseStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, DefaultBufferSize = 128 }))
	{
		Console.WriteLine($"[{DateTime.UtcNow:hh:mm:ss.fff}] {weatherForecast.Summary}");
	}

	Console.WriteLine($"[{DateTime.UtcNow:hh:mm:ss.fff}] Weather forecasts has been received.");
	Console.WriteLine();
}