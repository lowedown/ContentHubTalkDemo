using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentHubFacade.ContentHub.Config
{
	public class ContentHubConfig
	{
		public string Endpoint { get; set; }
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public string CDNBaseUrl { get; set; }
	}
}
