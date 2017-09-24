using FlatMate.Module.Common;
using Microsoft.EntityFrameworkCore;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using prayzzz.Common.Results;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FlatMate.Module.Offers.Domain.Raw
{
    public interface IRawOfferDataService
    {
        Task<(Result, RawOfferDataDto)> Save(RawOfferDataDto dto);

        Task<(Result, RawOfferDataDto)> Save(string data, int companyId);
    }

    [Inject]
    public class RawOfferDataService : IRawOfferDataService
    {
        private readonly OffersDbContext _dbContext;

        private readonly IMapper _mapper;

        public RawOfferDataService(OffersDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<(Result, RawOfferDataDto)> Save(RawOfferDataDto dto)
        {
            var hash = ComputeHash(dto.Data);

            var response = await _dbContext.RawOfferData.FirstOrDefaultAsync(d => d.Hash == hash);
            if (response != null)
            {
                return (SuccessResult.Default, _mapper.Map<RawOfferDataDto>(response));
            }

            var data = new RawOfferData
            {
                CompanyId = dto.CompanyId,
                Created = dto.Created,
                Data = dto.Data,
                Hash = hash
            };

            _dbContext.Add(data);
            var result = await _dbContext.SaveChangesAsync();
            var offerData = await _dbContext.RawOfferData.Include(d => d.Company).FirstOrDefaultAsync(d => d.Id == data.Id);

            return (result, _mapper.Map<RawOfferDataDto>(offerData));
        }

        public Task<(Result, RawOfferDataDto)> Save(string data, int companyId)
        {
            return Save(new RawOfferDataDto
            {
                CompanyId = companyId,
                Data = data,
                Created = DateTime.Now,
            });
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
