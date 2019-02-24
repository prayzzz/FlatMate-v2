using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FlatMate.Module.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;

namespace FlatMate.Module.Offers.Domain.Raw
{
    public interface IRawOfferDataService
    {
        Task<(Result, RawOfferDataDto)> Save(string data, int companyId);
    }

    [Inject]
    public class RawOfferDataService : IRawOfferDataService
    {
        private readonly OffersDbContext _dbContext;

        private readonly ILogger<RawOfferDataService> _logger;
        private readonly IMapper _mapper;

        public RawOfferDataService(OffersDbContext dbContext, IMapper mapper, ILogger<RawOfferDataService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<(Result, RawOfferDataDto)> Save(string data, int marketId)
        {
            var offerData = new RawOfferData
            {
                Created = DateTime.Now,
                Data = data,
                Hash = ComputeHash(data),
                MarketId = marketId
            };

            var response = await _dbContext.RawOfferData.FirstOrDefaultAsync(d => d.Hash == offerData.Hash);
            if (response != null)
            {
                _logger.LogInformation("OfferData with same hash already saved");
                return (Result.Success, _mapper.Map<RawOfferDataDto>(response));
            }

            _dbContext.RawOfferData.Add(offerData);
            var result = await _dbContext.SaveChangesAsync();
            var savedOfferData = await _dbContext.RawOfferData
                                                 .Include(d => d.Market)
                                                 .FirstOrDefaultAsync(d => d.Id == offerData.Id);

            return (result, _mapper.Map<RawOfferDataDto>(savedOfferData));
        }

        private static string ComputeHash(string data)
        {
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(Encoding.UTF8.GetBytes(data)).ToHexString();
            }
        }
    }
}