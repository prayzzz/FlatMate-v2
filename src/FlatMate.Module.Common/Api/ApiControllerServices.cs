using prayzzz.Common.Mapping;
using prayzzz.Common.Attributes;
using App.Metrics;

namespace FlatMate.Module.Common.Api
{
    public interface IApiControllerServices
    {
        IMetricsRoot MetricsRoot { get; }

        IMapper Mapper { get; }
    }

    [Inject]
    public class ApiControllerServices : IApiControllerServices
    {
        public ApiControllerServices(IMetricsRoot metricsRoot, IMapper mapper)
        {
            MetricsRoot = metricsRoot;
            Mapper = mapper;
        }

        public IMetricsRoot MetricsRoot { get; }

        public IMapper Mapper { get; }
    }
}
