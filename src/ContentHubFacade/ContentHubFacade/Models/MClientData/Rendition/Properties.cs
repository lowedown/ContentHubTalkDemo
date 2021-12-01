using Newtonsoft.Json;

namespace ContentHubFacade.Models.MClientData.Rendition
{
	public class Properties
	{
		[JsonProperty("content_type")]
		public string ContentType { get; set; }
		[JsonProperty("filesizebytes")]
		public string FileSizeBytes { get; set; }
		[JsonProperty("width")]
		public string Width { get; set; }
		[JsonProperty("height")]
		public string Height { get; set; }
	}
}
