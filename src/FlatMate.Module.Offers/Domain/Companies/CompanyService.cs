using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Module.Offers.Domain.Companies
{
    public interface ICompanyService
    {
        Task<(Result, CompanyDto)> CreateCompany(CompanyDto dto);

        Task<(Result, CompanyDto)> Get(int id);

        Task<(Result, CompanyDto)> UpdateCompany(int id, CompanyDto dto);

        Task<IEnumerable<CompanyDto>> GetList();
    }

    [Inject]
    public class CompanyService : ICompanyService
    {
        private readonly OffersDbContext _dbContext;
        private readonly ILogger<CompanyService> _logger;
        private readonly IMapper _mapper;

        public CompanyService(OffersDbContext dbContext, IMapper mapper, ILogger<CompanyService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<(Result, CompanyDto)> CreateCompany(CompanyDto dto)
        {
            var company = new Company { Name = dto.Name, ImageGuid = dto.ImageGuid };

            _dbContext.Companies.Add(company);

            return await Save(company);
        }

        public async Task<(Result, CompanyDto)> Get(int id)
        {
            var company = await _dbContext.Companies.FindAsync(id);

            if (company == null)
            {
                return (new ErrorResult(ErrorType.NotFound, "Company not found"), null);
            }

            return (SuccessResult.Default, _mapper.Map<CompanyDto>(company));
        }

        public async Task<(Result, CompanyDto)> UpdateCompany(int id, CompanyDto dto)
        {
            var company = await _dbContext.Companies.FindAsync(id);

            company.ImageGuid = dto.ImageGuid;
            company.Name = dto.Name;

            return await Save(company);
        }

        public async Task<IEnumerable<CompanyDto>> GetList()
        {
            return (await _dbContext.Companies.ToListAsync()).Select(_mapper.Map<CompanyDto>);
        }

        private async Task<(Result, CompanyDto)> Save(Company company)
        {
            var result = await _dbContext.SaveChangesAsync();
            if (result.IsError)
            {
                return (result, null);
            }

            return (result, _mapper.Map<CompanyDto>(company));
        }
    }
}