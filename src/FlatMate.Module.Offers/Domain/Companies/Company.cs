using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Common.Dtos;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlatMate.Module.Offers.Domain.Companies
{
    [Table("Company")]
    public class Company : DboBase
    {
        public Guid? ImageGuid { get; set; }

        public string Name { get; set; }
    }

    public class CompanyDto : DtoBase
    {
        public Guid? ImageGuid { get; set; }

        public string Name { get; set; }
    }

    [Inject]
    public class CompanyMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<Company, CompanyDto>(MapToDto);
        }

        private static CompanyDto MapToDto(Company company, MappingContext mappingContext)
        {
            return new CompanyDto
            {
                Id = company.Id,
                ImageGuid = company.ImageGuid,
                Name = company.Name
            };
        }
    }
}
