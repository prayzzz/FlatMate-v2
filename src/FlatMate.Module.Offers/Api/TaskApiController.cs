using System.Threading;
using System.Threading.Tasks;
using FlatMate.Module.Common.Api;
using FlatMate.Module.Offers.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FlatMate.Module.Offers.Api
{
    [Route(RouteTemplate)]
    public class TaskApiController : ApiController
    {
        private const string RouteTemplate = "/api/v1/offers/tasks";

        private readonly DeleteOldProductsTask _deleteOldProductsTask;

        public TaskApiController(DeleteOldProductsTask deleteOldProductsTask,
                                 IApiControllerServices services) : base(services)
        {
            _deleteOldProductsTask = deleteOldProductsTask;
        }

        [HttpGet("deleteOldProducts")]
        public Task ExecuteDeleteOldProducts(CancellationToken cancellationToken)
        {
            return _deleteOldProductsTask.ExecuteAsync(cancellationToken);
        }
    }
}