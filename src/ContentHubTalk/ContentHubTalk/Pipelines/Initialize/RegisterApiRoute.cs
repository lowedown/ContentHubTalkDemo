using Sitecore.Pipelines;
using System.Web.Http;
using System.Web.Routing;

namespace ContentHubTalk.Pipelines.Initialize
{
    public class RegisterApiRoute
    {
        public virtual void Process(PipelineArgs args)
        {
            RouteTable.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{action}"
            );
        }
    }
}