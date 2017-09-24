using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Common.Dtos;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlatMate.Module.Offers.Domain.Companies
{
    public enum Company
    {
        Rewe = 1,
        Netto = 2,
        Penny = 3,
        Aldi = 4
    }

    [Table("CompanyData")]
    public class CompanyData : DboBase
    {
        public Company Company => (Company)Id;

        public Guid? ImageGuid { get; set; }

        public string Name { get; set; }
    }

    public class CompanyDto : DtoBase
    {
        public Guid? ImageGuid { get; set; }

        public string Name { get; set; }

        public Company Company { get; set; }
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
