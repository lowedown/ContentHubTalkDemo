using Newtonsoft.Json;

namespace ContentHubFacade.Models.MClientData.Rendition
{
	public class Renditions
	{
		[JsonProperty("pdf")]
		public RenditionDetail Pdf { get; set; }
		[JsonProperty("mp4")]
		public RenditionDetail Mp4 { get; set; }
	}
}
