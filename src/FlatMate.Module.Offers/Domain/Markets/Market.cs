using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Common.Dtos;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlatMate.Module.Offers.Domain
{
    [Table("Market")]
    public class Market : DboBase
    {
        public string City { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public CompanyData Company { get; set; }

        public int CompanyId { get; set; }

        [Required]
        public string ExternalId { get; set; }

        [Required]
        public string Name { get; set; }

        [InverseProperty(nameof(Product.Market))]
        public List<Offer> Offers { get; set; }

        public string PostalCode { get; set; }

        [InverseProperty(nameof(Offer.Market))]
        public List<Product> Products { get; set; }

        public string Street { get; set; }
    }

    public class MarketDto : DtoBase
    {
        public string City { get; set; }

        public CompanyDto Company { get; set; }

        public string Name { get; set; }

        public string PostalCode { get; set; }

        public string Street { get; set; }
    }

    [Inject]
    public class MarketMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<Market, MarketDto>(MapToDto);
        }

        private MarketDto MapToDto(Market market, MappingContext mappingContext)
        {
            return new MarketDto
            {
                City = market.City,
                Company = mappingContext.Mapper.Map<CompanyDto>(market.Company),
                Id = market.Id,
                Name = market.Name,
                PostalCode = market.PostalCode,
                Street = market.Street
            };
        }
    }
}