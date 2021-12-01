using Newtonsoft.Json;

namespace ContentHubFacade.Models.MClientData.Rendition
{
	public class RenditionDetail
	{
		[JsonProperty("status")]
		public string Status { get; set; }
		[JsonProperty("properties")]
		public Properties Properties { get; set; }
	}
}
