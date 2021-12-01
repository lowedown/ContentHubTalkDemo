using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;

namespace ContentHubTalk.ContentSearch
{
    public class ContentHubImagesSearchResultItem : SearchResultItem
	{
		[IndexField("contenthub_assetids")]
		public virtual IEnumerable<string> ContentHubAssetIds { get; set; }
	}
}