using System.ComponentModel;
using FlatMate.Module.Offers.Domain;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Offers.Api
{
    public class MarketJso
    {
        public string City { get; set; }

        [ReadOnly(true)]
        public CompanyJso CompanyData { get; set; }

        [ReadOnly(true)]
        public Company CompanyId { get; set; }

        [ReadOnly(true)]
        public int? Id { get; set; }

        public string Name { get; set; }

        public string PostalCode { get; set; }

        public string Street { get; set; }
    }

    [Inject]
    public class MarketMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<MarketDto, MarketJso>(MapToJso);
            mapper.Configure<MarketJso, MarketDto>(MapToDto);
        }

        private MarketDto MapToDto(MarketJso jso, MappingContext ctx)
        {
            return new MarketDto
            {
                City = jso.City,
                Company = ctx.Mapper.Map<CompanyDto>(jso.CompanyData),
                Id = jso.Id,
                Name = jso.Name,
                PostalCode = jso.PostalCode,
                Street = jso.Street
            };
        }

        private MarketJso MapToJso(MarketDto dto, MappingContext ctx)
        {
            return new MarketJso
            {
                City = dto.City,
                CompanyData = ctx.Mapper.Map<CompanyJso>(dto.Company),
                CompanyId = dto.CompanyId,
                Id = dto.Id,
                Name = dto.Name,
                PostalCode = dto.PostalCode.PadLeft(5, '0'),
                Street = dto.Street
            };
        }
    }
}