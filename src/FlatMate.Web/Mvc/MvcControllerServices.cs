using FlatMate.Web.Mvc.Json;
using prayzzz.Common.Attributes;

namespace FlatMate.Web.Mvc
{
    public interface IMvcControllerServices
    {
        IJsonService JsonService { get; }
    }

    [Inject]
    public class MvcControllerServices : IMvcControllerServices
    {
        public MvcControllerServices(IJsonService jsonService)
        {
            JsonService = jsonService;
        }

        public IJsonService JsonService { get; }
    }
}