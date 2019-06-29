using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Module.Offers.Domain.Companies
{
    public interface ICompanyService
    {
        Task<(Result, CompanyDto)> Get(int id);

        Task<IEnumerable<CompanyDto>> SearchCompanies();

        Task<(Result, CompanyDto)> UpdateCompany(int id, CompanyDto dto);
    }

    [Inject]
    public class CompanyService : ICompanyService
    {
        private readonly OffersDbContext _dbContext;

        private readonly IMapper _mapper;

        public CompanyService(OffersDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<(Result, CompanyDto)> Get(int id)
        {
            var company = await _dbContext.Companies.FindAsync(id);
            if (company == null)
            {
                return (new Result(ErrorType.NotFound, "Company not found"), null);
            }

            return (Result.Success, _mapper.Map<CompanyDto>(company));
        }

        public async Task<IEnumerable<CompanyDto>> SearchCompanies()
        {
            return (await _dbContext.Companies.AsNoTracking().ToListAsync()).Select(_mapper.Map<CompanyDto>);
        }

        public async Task<(Result, CompanyDto)> UpdateCompany(int id, CompanyDto dto)
        {
            var company = await _dbContext.Companies.FindAsync(id);
            if (company == null)
            {
                return (new Result(ErrorType.NotFound, "Company not found"), null);
            }

            company.ImageGuid = dto.ImageGuid;
            company.Name = dto.Name;

            return await Save(company);
        }

        private async Task<(Result, CompanyDto)> Save(CompanyData company)
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