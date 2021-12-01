using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;
using System.Collections.Generic;

namespace ContentHubTalk.Controllers.Api
{
    public class ContentHubImagesSearchResultItem : SearchResultItem
	{
		[IndexField("contenthub_assetids")]
		public virtual IEnumerable<string> ContentHubAssetIds { get; set; }
	}
}