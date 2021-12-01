using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContentHubTalk.Services.Models
{
	public class ContentHubAsset
	{
		public long Id { get; set; }

		public Dictionary<string, string> Metadata { get; set; }

		public IList<string> PreviewLinks { get; set; }

		public IList<PublicLink> PublicLinks { get; set; }

		public ContentHubAsset()
		{
			Metadata = new Dictionary<string, string>();
		}
	}
}