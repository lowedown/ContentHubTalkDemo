using Newtonsoft.Json;

namespace ContentHubFacade.Models.MClientData
{
	public class FileData
	{
		[JsonProperty("properties")]
		public FileProperties Properties { get; set; }
		public string Extension { get; internal set; }
	}
}
