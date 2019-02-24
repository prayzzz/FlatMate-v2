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
using prayzzz.Common.Results;

.Mvc;
using prayzzz.Common.Results;

namespace FlatMate.Module.Offers.Api
{
    [Route(RouteTemplate)]
    public class CompanyApiController : ApiController
    {
        private const string RouteTemplate = "/api/v1/offers/company";

        private readonly ICompanyService _companyService;

        private readonly IImageService _imageService;

        public CompanyApiController(ICompanyService companyService, IImageService imageService, IApiControllerServices services) : base(services)
        {
            _companyService = companyService;
            _imageService = imageService;
        }

        [HttpPost("{id}/image")]
        public async Task<(Result, CompanyJso)> AddCompanyImage(int id, IFormFile file)
        {
            var (getResult, company) = await _companyService.Get(id);
            if (getResult.IsError)
            {
                return (getResult, null);
            }

            var (saveResult, image) = await _imageService.Save(ByteHelper.ReadToEnd(file.OpenReadStream()), file.ContentType);
            if (saveResult.IsError)
            {
                return (saveResult, null);
            }

            if (company.ImageGuid != null)
            {
                await _imageService.Delete(company.ImageGuid.Value);
            }

            company.ImageGuid = image.Guid;

            return MapResultTuple(await _companyService.UpdateCompany(id, company), Map<CompanyJso>);
        }

        [HttpGet("{id}")]
        public async Task<(Result, CompanyJso)> Get(int id)
        {
            return MapResultTuple(await _companyService.Get(id), Map<CompanyJso>);
        }

        [HttpGet]
        public async Task<IEnumerable<CompanyJso>> GetList()
        {
            return (await _companyService.SearchCompanies(
    lect(Map<CompanyJso>);
        }
    }
}