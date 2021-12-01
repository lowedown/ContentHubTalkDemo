using Newtonsoft.Json;

namespace ContentHubFacade.Models.MClientData
{
	public class FileProperties
	{
		[JsonProperty("filesize")]
		public double FileSize { get; set; }

		[JsonProperty("filesizebytes")]
		public string FileSizeBytes { get; set; }

		[JsonProperty("group")]
		public string Group { get; set; }

		[JsonProperty("extension")]
		public string Extension { get; set; }

		[JsonProperty("content_type")]
		public string ContentType { get; set; }

		[JsonProperty("pages")]
		public string Pages { get; set; }
	}
}
