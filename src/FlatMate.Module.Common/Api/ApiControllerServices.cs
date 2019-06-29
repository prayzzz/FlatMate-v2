using App.Metrics;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Common.Api
{
    public interface IApiControllerServices
    {
        IMapper Mapper { get; }

        IMetricsRoot MetricsRoot { get; }
    }

    [Inject]
    public class ApiControllerServices : IApiControllerServices
    {
        public ApiControllerServices(IMetricsRoot metricsRoot, IMapper mapper)
        {
            MetricsRoot = metricsRoot;
            Mapper = mapper;
        }

        public IMapper Mapper { get; }

        public IMetricsRoot MetricsRoot { get; }
    }
}