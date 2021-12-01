using System;

namespace ContentHubFacade.Models
{
    public class PublicLink
	{
		public long Id { get; set; }
		public string Url { get; set; }
		public string ContentType { get; set; }
		public string FileSizeBytes { get; set; }
		public string Width { get; set; }
		public string Height { get; set; }
		public DateTime ExpirationDate { get; set; }
		public string SourceRendition { get; set; }
		public string RelativeUrl { get; set; }
		public string VersionHash { get; set; }
	}
}
