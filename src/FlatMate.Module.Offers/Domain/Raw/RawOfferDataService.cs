using FlatMate.Module.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FlatMate.Module.Offers.Domain
{
    public interface IRawOfferDataService
    {
        Task<(Result, RawOfferDataDto)> Save(string data, int companyId);
    }

    [Inject]
    public class RawOfferDataService : IRawOfferDataService
    {
        private readonly OffersDbContext _dbContext;

        private readonly IMapper _mapper;
        private readonly ILogger<RawOfferDataService> _logger;

        public RawOfferDataService(OffersDbContext dbContext, IMapper mapper, ILogger<RawOfferDataService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<(Result, RawOfferDataDto)> Save(string data, int companyId)
        {
            var offerData = new RawOfferData
            {
                CompanyId = companyId,
                Created = DateTime.Now,
                Data = data,
                Hash = ComputeHash(data)
            };

            var response = await _dbContext.RawOfferData.FirstOrDefaultAsync(d => d.Hash == offerData.Hash);
            if (response != null)
            {
                _logger.LogInformation("OfferData with same hash already saved");
                return (SuccessResult.Default, _mapper.Map<RawOfferDataDto>(response));
            }

            _dbContext.Add(data);
            var result = await _dbContext.SaveChangesAsync();
            var savedOfferData = await _dbContext.RawOfferData.Include(d => d.Company).FirstOrDefaultAsync(d => d.Id == offerData.Id);

            return (result, _mapper.Map<RawOfferDataDto>(offerData));
        }

        private string ComputeHash(string data)
        {
            using (var md5 = MD5.Create())
            {
                return ByteHelper.ToHexString(md5.ComputeHash(Encoding.UTF8.GetBytes(data)));
            }
        }
    }
}