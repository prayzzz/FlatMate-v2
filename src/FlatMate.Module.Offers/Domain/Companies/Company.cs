using System;
using System.ComponentModel.DataAnnotations.Schema;
using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Common.Dtos;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Offers.Domain.Companies
{
    public enum Company
    {
        None = 0,
        Rewe = 1,
        Penny = 2,
        NettoDiscount = 3,
        Netto = 4,
        AldiNord = 5,
        AldiSued = 6
    }

    [Table("CompanyData")]
    public class CompanyData : DboBase
    {
        public Company Company => (Company) Id;

        public Guid? ImageGuid { get; set; }

        public string Name { get; set; }
    }

    public class CompanyDto : DtoBase
    {
        public Company Company { get; set; }

        public Guid? ImageGuid { get; set; }

        public string Name { get; set; }
    }

    [Inject]
    public class CompanyMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<CompanyData, CompanyDto>(MapToDto);
        }

        private static CompanyDto MapToDto(CompanyData company, MappingContext mappingContext)
        {
            return new CompanyDto
            {
                Company = company.Company,
                Id = company.Id,
                ImageGuid = company.ImageGuid,
                Name = company.Name
            };
        }
    }
}