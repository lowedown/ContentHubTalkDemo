using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace ContentHubTalk.ContentSearch
{
	/// <summary>
	/// Extracts ContentHub AssetIDs from image fields
	/// </summary>
	public class ContentHubImagesComputedField : AbstractComputedIndexField
	{
		public ContentHubImagesComputedField(XmlNode configNode) : base(configNode)
		{
		}

		public override object ComputeFieldValue(IIndexable indexable)
		{
			Item item = indexable as SitecoreIndexableItem;

			if (item == null)
			{
				return null;
			}

			IEnumerable<Field> imageFields = item.Fields.Where(f => f.TypeKey == "image");

			if (!imageFields.Any())
			{
				return null;
			}

			List<string> assetIds = new List<string>();

			foreach (Field imageField in item.Fields.Where(f => f.TypeKey == "image"))
			{
				string assetId = ((Sitecore.Data.Fields.ImageField)imageField)
					.GetAttribute("stylelabs-content-id");

				if (!string.IsNullOrEmpty(assetId))
				{
					assetIds.Add(assetId);
				}
			}

			return assetIds.Any() ? assetIds : null;
		}
	}
}
