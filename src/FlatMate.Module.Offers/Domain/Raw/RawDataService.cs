using prayzzz.Common.Mapping;
using prayzzz.Common.Results;
using System.Threading.Tasks;

namespace FlatMate.Module.Offers.Domain.Raw
{
    public class RawDataService
    {
        private readonly OffersDbContext _dbContext;
        private readonly IMapper _mapper;

        public RawDataService(OffersDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public Task<(Result, RawApiDataDto)> Save(RawApiDataDto dto)
        {
            var data = new RawApiData
            {
                CompanyId = dto.CompanyId,
                Date = dto.Date,
                Data = dto.Data
            };

            return Save(data);
        }

        private async Task<(Result, RawApiDataDto)> Save(RawApiData data)
        {
            var result = await _dbContext.SaveChangesAsync();
            if (result.IsError)
            {
                return (result, null);
            }

            return (result, _mapper.Map<RawApiDataDto>(data));
        }
    }
}
