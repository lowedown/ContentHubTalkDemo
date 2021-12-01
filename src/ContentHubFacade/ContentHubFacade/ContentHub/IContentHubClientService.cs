using System.Threading.Tasks;
using ContentHubFacade.Models;

namespace ContentHubFacade.ContentHub
{
	public interface IContentHubClientService
	{
		Task<ContentHubAsset> GetContentHubAssetAsync(long id);
		Task<string> TestConnectionAsync();
	}
}
