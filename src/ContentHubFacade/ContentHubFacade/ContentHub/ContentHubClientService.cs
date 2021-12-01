using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ContentHubFacade.ContentHub.Config;
using ContentHubFacade.Models;
using ContentHubFacade.Models.MClientData;
using ContentHubFacade.Models.MClientData.Rendition;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Stylelabs.M.Framework.Essentials.LoadConfigurations;
using Stylelabs.M.Framework.Essentials.LoadOptions;
using Stylelabs.M.Sdk.Contracts.Base;
using Stylelabs.M.Sdk.WebClient;
using Stylelabs.M.Sdk.WebClient.Authentication;

namespace ContentHubFacade.ContentHub
{
	public class ContentHubClientService : IContentHubClientService
	{
		private readonly ILogger<ContentHubClientService> _logger;
		private readonly IWebMClient _client;
		private readonly IOptions<ContentHubConfig> _config;

		public ContentHubClientService(IOptions<ContentHubConfig> config,
			ILogger<ContentHubClientService> logger)
		{
			_config = config;
			_client = CreateClient();
			_logger = logger;
		}

		private IWebMClient CreateClient()
		{
			return MClientFactory.CreateMClient(
				new Uri(_config.Value.Endpoint),
				new OAuthPasswordGrant
				{
					ClientId = _config.Value.ClientId,
					ClientSecret = _config.Value.ClientSecret,
					UserName = _config.Value.UserName,
					Password = _config.Value.Password
				});
		}

		public async Task<ContentHubAsset> GetContentHubAssetAsync(long id)
		{
			var loadConfig = EntityLoadConfiguration.Default;
			loadConfig.CultureLoadOption = new CultureLoadOption("en-US");

			var entity = await _client.Entities.GetAsync(id, loadConfig).ConfigureAwait(false);

			if (entity == null)
			{
				return null;
			}

			var data = await GetContentHubAssetAsync(entity);

			// Get details about public links of this asset
			var publicLinks = await entity.GetRelationAsync<IParentToManyChildrenRelation>("AssetToPublicLink");

			var renditionDetails = entity.GetPropertyValue<JToken>("Renditions")?.ToObject<Renditions>();

			foreach (var publicLinkId in publicLinks.Children)
			{
				var publicLink = await _client.Entities.GetAsync(publicLinkId);
				if (publicLink == null)
				{
					continue;
				}

				var extensionData = GetExtensionData(publicLink);

				var publicLinkDetails = new PublicLink()
				{
					SourceRendition = publicLink.GetPropertyValue<string>("Resource"),
					ExpirationDate = publicLink.GetPropertyValue<DateTime>("ExpirationDate"),
					RelativeUrl = publicLink.GetPropertyValue<string>("RelativeUrl"),
					VersionHash = publicLink.GetPropertyValue<string>("VersionHash"),
					Id = publicLinkId,
					Url = extensionData["public_link"]?.ToString()
				};

				// Map rendition details for known renditions
				if (renditionDetails != null)
				{
					switch (publicLinkDetails.SourceRendition)
					{
						case "downloadOriginal":
							publicLinkDetails.ContentType = data.Metadata["ContentType"];
							publicLinkDetails.FileSizeBytes = data.Metadata["FileSizeBytes"];
							break;
						case "pdf":
							publicLinkDetails.ContentType = renditionDetails.Pdf.Properties?.ContentType;
							publicLinkDetails.FileSizeBytes = renditionDetails.Pdf.Properties?.FileSizeBytes;
							publicLinkDetails.Height = renditionDetails.Pdf.Properties?.Height;
							publicLinkDetails.Width = renditionDetails.Pdf.Properties?.Width;
							break;
						// Handle any custom renditions here
					}
				}

				data.PublicLinks.Add(publicLinkDetails);
			}

			return data;
		}

		private async Task<ContentHubAsset> GetContentHubAssetAsync(IEntity entity)
		{
			_logger.LogInformation($"GetContentHubAssetAsync for entity {entity}");

			var assetData = new ContentHubAsset { Id = entity.Id.GetValueOrDefault() };

			var mainFile = await entity.GetPropertyAsync<ICultureInsensitiveProperty>("MainFile").ConfigureAwait(false);
			var fileData = ((JObject)mainFile.GetValue()).ToObject<FileData>();

			if (fileData?.Properties != null)
			{
				AddIfNotEmpty("ContentType", fileData.Properties.ContentType, assetData.Metadata);
				AddIfNotEmpty("Extension", fileData.Properties.Extension, assetData.Metadata);
				AddIfNotEmpty("FileSizeBytes", fileData.Properties.FileSizeBytes, assetData.Metadata);
				AddIfNotEmpty("Group", fileData.Properties.Group, assetData.Metadata);
				AddIfNotEmpty("Pages", fileData.Properties.Pages, assetData.Metadata);
			}

			var vimeoProp = await entity.GetPropertyAsync<ICultureInsensitiveProperty>("MyCustomField").ConfigureAwait(false);
			AddIfNotEmpty("MyCustomField", vimeoProp.GetValue<string>(), assetData.Metadata);

			return assetData;
		}

		public async Task<string> TestConnectionAsync()
		{
			try
			{
				await _client.TestConnectionAsync();
				return "OK";
			}
			catch (Exception ex)
			{
				_logger.LogError($"Unable to connect to ContentHub: {ex.Message}");
				return "Error";
			}
		}

		private static void AddIfNotEmpty(string key, string value, IDictionary<string, string> collection)
		{
			if (string.IsNullOrEmpty(value))
			{
				return;
			}

			collection.Add(key, value);
		}

		private Dictionary<string, JToken> GetExtensionData(IEntity entity)
		{
			PropertyInfo extensionDataProperty = entity.GetType()
				.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance).Single(pi => pi.Name == "ExtensionData");
			return (Dictionary<string, JToken>)extensionDataProperty.GetValue(entity, null);
		}
	}
}
