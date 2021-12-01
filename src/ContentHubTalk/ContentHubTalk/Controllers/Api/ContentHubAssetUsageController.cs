using Sitecore.Abstractions;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.Data.Items;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ContentHubTalk.Controllers.Api
{
	[EnableCors(origins: "*", headers: "*", methods: "*")]
	public class ContentHubAssetUsageController : ApiController
	{
		private readonly BaseSettings _settings;

		public ContentHubAssetUsageController(BaseSettings settings)
		{
			_settings = settings;
		}

		[HttpGet]
		public IHttpActionResult GetUsages(long assetId)
		{
			if (!_settings.GetBoolSetting("ContentHubCustomisations.EnableAssetUsageController", false))
			{
				return Unauthorized();
			}

			return Ok(GetRelevantItems(assetId).Where(i => i != null).Select(MapFields));
		}

		private IEnumerable<Item> GetRelevantItems(long assetId)
		{
			using (IProviderSearchContext context = ContentSearchManager
						.GetIndex("sitecore_master_index").CreateSearchContext())
			{
				var results =
					context.GetQueryable<ContentHubImagesSearchResultItem>()
					.Where(x => x.ContentHubAssetIds.Contains(assetId.ToString()))
					.GetResults();

				if (results.Any())
				{
					return results.Hits.Select(h => h.Document.GetItem());
				}

				return new List<Item>();
			}
		}

		private AssetUsage MapFields(Item item)
		{
			return new AssetUsage()
			{
				ItemId = item.ID.ToString(),
				ItemName = item.Name,
				ItemPath = item.Paths.FullPath,
				Language = item.Language.Name,
				Version = item.Version.Number
			};
		}
	}

	public class AssetUsage
	{
		public string ItemId { get; set; }
		public string ItemName { get; set; }
		public string ItemPath { get; set; }
		public string Language { get; set; }
		public int Version { get; set; }
	}
}