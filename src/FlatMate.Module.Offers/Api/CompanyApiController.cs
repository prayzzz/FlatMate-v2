using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlatMate.Module.Common;
using FlatMate.Module.Common.Api;
using FlatMate.Module.Infrastructure.Images;
using FlatMate.Module.Offers.Api.Jso;
using FlatMate.Module.Offers.Domain.Companies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

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

            var (saveResult, image) = await _imageService.Save(ByteHelper.ReadFully(file.OpenReadStream()), file.ContentType);
            if (saveResult.IsError)
            {
                return new ErrorResult<CompanyJso>(saveResult);
            }

            company.ImageGuid = image.Guid;

            var (updateResult, updatedCompany) = await _companyService.UpdateCompany(id, company);

            if (updateResult.IsError)
            {
                return new ErrorResult<CompanyJso>(updateResult);
            }

            return new SuccessResult<CompanyJso>(Map<CompanyJso>(updatedCompany));
        }

        [HttpPost]
        public async Task<Result<CompanyJso>> CreateCompany([FromBody] CompanyJso companyJso)
        {
            var (result, company) = await _companyService.CreateCompany(Map<CompanyDto>(companyJso));

            if (result.IsError)
            {
                return new ErrorResult<CompanyJso>(result);
            }

            return new SuccessResult<CompanyJso>(Map<CompanyJso>(company));
        }

        [HttpGet("{id}")]
        public async Task<Result<CompanyJso>> Get(int id)
        {
            var (result, company) = await _companyService.Get(id);

            if (result.IsError)
            {
                return new ErrorResult<CompanyJso>(result);
            }

            return new SuccessResult<CompanyJso>(Map<CompanyJso>(company));
        }

        [HttpGet]
        public async Task<IEnumerable<CompanyJso>> GetList()
        {
            return (await _companyService.GetList()).Select(Map<CompanyJso>);
        }
    }
}