using ContentHubTalk.Services.Models;
using Newtonsoft.Json;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace ContentHubTalk.Services
{
	public class ContentHubService
	{
		private static readonly HttpClientHandler _httpHandler = new HttpClientHandler() { UseCookies = false };
		private static readonly HttpClient _client = InitClient();

		private readonly string _endpoint;

		private static HttpClient InitClient()
		{
			var newClient = new HttpClient(_httpHandler) { Timeout = TimeSpan.FromSeconds(30) };
			newClient.DefaultRequestHeaders.CacheControl = CacheControlHeaderValue.Parse("None");
			newClient.DefaultRequestHeaders.Add("apikey", Settings.GetSetting("ContentHubCustomisations.Facade.ApiKey"));
			return newClient;
		}

		public ContentHubService()
		{
			_endpoint = Settings.GetSetting("ContentHubCustomisations.Facade.Endpoint");
		}

		public Task CheckConnection()
		{
			return _callApi($"{_endpoint}/api/check");
		}

		public Task<ContentHubAsset> GetEntityById(long assetId)
		{
			return _callApi<ContentHubAsset>($"{_endpoint}/api/assets/{assetId}");
		}

		private async Task<T> _callApi<T>(string call, HttpMethod method = null)
		{
			var result = await _callApi(call, method).ConfigureAwait(false);

			try
			{
				var content = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
				if (typeof(T) == typeof(string) && !content.StartsWith("\""))
				{
					content = $"\"{content}\"";
				}
				return JsonConvert.DeserializeObject<T>(content);
			}
			catch (JsonSerializationException ex)
			{
				Log.Error($"ContentHubService: Error reading + deserializing response from call: {call}. {ex.Message}",
					ex, this);
				throw;
			}
		}

		private async Task<HttpResponseMessage> _callApi(string call, HttpMethod method = null)
		{
			try
			{
				method = method ?? HttpMethod.Get;
				Log.Debug($"ContentHubService: ApiCall to: {method.Method} {call}");
				return await _client.SendAsync(new HttpRequestMessage(method, call)).ConfigureAwait(false);
			}
			catch (HttpRequestException ex)
			{
				Log.Error($"ContentHubService: Error calling the ContentHub Facade service at: {call}", ex, this);

				throw;
			}
		}
	}
}