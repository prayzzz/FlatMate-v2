using FlatMate.Module.Common;
using FlatMate.Module.Common.Api;
using FlatMate.Module.Infrastructure.Images;
using FlatMate.Module.Offers.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlatMate.Module.Offers.Api
{
    [Route(RouteTemplate)]
    public class CompanyApiController : ApiController
    {
        private const string RouteTemplate = "/api/v1/offers/company";

        private readonly ICompanyService _companyService;

        private readonly IImageService _imageService;

        public CompanyApiController(ICompanyService companyService, IImageService imageService, IMapper mapper) : base(mapper)
        {
            _companyService = companyService;
            _imageService = imageService;
        }

        [HttpPost("{id}/image")]
        public async Task<Result<CompanyJso>> AddCompanyImage(int id, IFormFile file)
        {
            var (getResult, company) = await _companyService.Get(id);
            if (getResult.IsError)
            {
                return new ErrorResult<CompanyJso>(getResult);
            }

            var (saveResult, image) = await _imageService.Save(ByteHelper.ReadToEnd(file.OpenReadStream()), file.ContentType);
            if (saveResult.IsError)
            {
                return new ErrorResult<CompanyJso>(saveResult);
            }

            company.ImageGuid = image.Guid;

            return FromTuple(await _companyService.UpdateCompany(id, company), Map<CompanyJso>);
        }

        [HttpGet("{id}")]
        public async Task<Result<CompanyJso>> Get(int id)
        {
            return FromTuple(await _companyService.Get(id), Map<CompanyJso>);
        }

        [HttpGet]
        public async Task<IEnumerable<CompanyJso>> GetList()
        {
            return (await _companyService.GetList()).Select(Map<CompanyJso>);
        }
    }
}