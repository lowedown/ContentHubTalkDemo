using System.Collections.Generic;
using System.Threading.Tasks;
using ContentHubFacade.ContentHub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ContentHubFacade.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AssetsController : ControllerBase
	{
		private readonly IContentHubClientService _contentHubClient;

		//Helps / speeds up development
		private static readonly IDictionary<string, long> _pictureParkAssetIdCache = new Dictionary<string, long>();

		public AssetsController(IContentHubClientService contentHubClientService, IConfiguration config)
		{
			_contentHubClient = contentHubClientService;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult> Get(long id)
		{
			var assetData = await _contentHubClient.GetContentHubAssetAsync(id);

			if (assetData == null)
			{
				return NotFound();
			}

			return Ok(assetData);
		}
	}
}
