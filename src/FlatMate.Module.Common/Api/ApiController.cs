using FlatMate.Module.Common.Api.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Common.Api
{
    [Authorize]
    [EnsurePostBodyFilter]
    public class ApiController : Controller
    {
        private readonly IMapper _mapper;
        protected readonly MappingContext MappingContext;

        public ApiController(IMapper mapper)
        {
            _mapper = mapper;

            MappingContext = new MappingContext();
        }

        protected T Map<T>(object data) where T : class
        {
            return _mapper.Map<T>(data, MappingContext);
        }
    }
}