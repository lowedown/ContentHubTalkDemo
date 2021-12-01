using ContentHubTalk.Services;
using Newtonsoft.Json.Linq;
using Sitecore.Configuration;
using Sitecore.Connector.ContentHub.DAM.Link;
using Sitecore.Connector.ContentHub.DAM.Models;
using Sitecore.Diagnostics;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace ContentHubTalk.ImageField
{
    public class MetadataEnabledImageField : MImageControl
	{

		public override void HandleMessage(Message message)
		{
			Assert.ArgumentNotNull(message, "message");
			if (message["id"] != this.ID)
			{
				return;
			}

			string name = message.Name;
			// Update asset metadata on refresh
			if (name == "contentimage:refresh")
			{
				string assetId = this.XmlValue.GetAttribute("stylelabs-content-id");
				if (!string.IsNullOrEmpty(assetId))
				{
					UpdateMetadataForAsset(assetId);
					this.SetModified();
					this.MUpdate();
				}

				return;
			}

			// Open asset in ContentHub when clicking on edit
			if (name == "contentimage:edit")
			{
				string assetId = this.XmlValue.GetAttribute("stylelabs-content-id");
				if (!string.IsNullOrEmpty(assetId))
				{
					var url = $"{Settings.GetSetting("ContentHubCustomisations.InstanceUrl")}/en-us/asset/{assetId}";
					SheerResponse.Eval($"window.open('{url}', '_blank');");
					return;
				}
			}

			base.HandleMessage(message);
		}

		// Used for additional metadata
		protected new void BrowseMImage(ClientPipelineArgs args)
		{
			base.BrowseMImage(args);

			if (args.IsPostBack)
			{
				if (string.IsNullOrWhiteSpace(args.Result) || !(args.Result != "undefined"))
					return;
				MDialogResult mdialogResult = JToken.Parse(args.Result).ToObject<MDialogResult>();

				// Get additional metadata on this item through the Picturepark API
				if (mdialogResult.Id != null)
				{
					UpdateMetadataForAsset(mdialogResult.Id);
				}
			}
		}

		protected override void DoRender(HtmlTextWriter output)
		{
			base.DoRender(output);

			IList<string> metadata = new List<string>();

			_addIfNotEmpty("Asset ID", "stylelabs-content-id", metadata);
			_addIfNotEmpty("Pages", "Pages", metadata);
			_addIfNotEmpty("Extension", "Extension", metadata);
			_addIfNotEmpty("Size (Bytes)", "FileSizeBytes", metadata);
			_addIfNotEmpty("Rendition", "Rendition", metadata);
			_addIfNotEmpty("My Custom Field", "MyCustomField", metadata);

			if (!metadata.Any())
			{
				return;
			}

			output.Write($"<div id=\"\" class=\"scContentControlImagePane\"><div id=\"\" class=\"scContentControlImageDetails\">{string.Join("<br/>", metadata)}</div></div>");
		}

		private void _addIfNotEmpty(string label, string attributeName, IList<string> output)
		{
			var value = this.XmlValue.GetAttribute(attributeName);
			if (string.IsNullOrEmpty(value))
			{
				return;
			}

			output.Add($"{label}: {value}");
		}

		protected void UpdateMetadataForAsset(string assetId)
		{
			var service = new ContentHubService();

			try
			{
				var entity = service.GetEntityById(long.Parse(assetId)).Result;

				if (entity != null)
				{
					foreach (var property in entity.Metadata)
					{
						if (!string.IsNullOrEmpty(property.Value))
						{
							Log.Debug($"MetadataEnabledImageField: Updating Metadata field '{property.Key}' to value '{property.Value}' on item {Sitecore.Context.Item}");

							this.XmlValue.SetAttribute(property.Key, property.Value);
						}
					}

					// Update size and extension based on the actual public link
					var publicLinkRelativeUrl = GetRelativeUrlFromPublicLink(this.XmlValue.GetAttribute("src"));
					var publicLinkData = entity.PublicLinks.FirstOrDefault(p => p.RelativeUrl == publicLinkRelativeUrl);
					if (publicLinkData != null)
					{
						if (!string.IsNullOrEmpty(publicLinkData.ContentType))
						{
							this.XmlValue.SetAttribute("ContentType", publicLinkData.ContentType);
						}

						if (!string.IsNullOrEmpty(publicLinkData.FileSizeBytes))
						{
							this.XmlValue.SetAttribute("FileSizeBytes", publicLinkData.FileSizeBytes);
						}

						if (!string.IsNullOrEmpty(publicLinkData.SourceRendition))
						{
							this.XmlValue.SetAttribute("Rendition", publicLinkData.SourceRendition);
						}

						var extension = MapMimeTypeToExtension(publicLinkData.ContentType);
						if (!string.IsNullOrEmpty(extension))
						{
							this.XmlValue.SetAttribute("Extension", extension);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error($"MetadataEnabledImageField: Unable to update metadata for asset ID {assetId} on item {Sitecore.Context.Item.ID}", ex, this);
				Sitecore.Context.ClientPage.ClientResponse.Alert("Error fetching metadata. Asset will be imported without metadata.");
			}
		}

		private string GetRelativeUrlFromPublicLink(string link)
		{
			var matches = Regex.Match(link, @"api\/public\/content\/(.+)\?v=");
			return matches.Groups[1].Value;
		}

		public static string MapMimeTypeToExtension(string mimeType)
		{
			if (string.IsNullOrEmpty(mimeType))
			{
				return null;
			}

			switch (mimeType.ToLower())
			{
				case "image/jpeg":
					return "jpg";
				case "image/tiff":
					return "tif";
				case "image/png":
					return "png";
				case "image/gif":
					return "gif";
				case "video/mp4":
					return "mp4";
				case "application/pdf":
					return "pdf";
				default:
					return null;
			}
		}
	}
}