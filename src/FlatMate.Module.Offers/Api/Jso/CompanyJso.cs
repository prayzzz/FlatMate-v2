using System;
using System.ComponentModel;
using FlatMate.Module.Infrastructure.Api;
using FlatMate.Module.Offers.Domain.Companies;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;

namespace FlatMate.Module.Offers.Api.Jso
{
    public class CompanyJso
    {
        [ReadOnly(true)]
        public Company Id { get; set; }

        [ReadOnly(true)]
        public Guid? ImageGuid { get; set; }

        [ReadOnly(true)]
        public string ImagePath { get; set; }

        public string Name { get; set; }
    }

    [Inject]
    public class CompanyMapper : IDboMapper
    {
        public void Configure(IMapperConfiguration mapper)
        {
            mapper.Configure<CompanyDto, CompanyJso>(MapToJso);
            mapper.Configure<CompanyJso, CompanyDto>(MapToDto);
        }

        private static CompanyDto MapToDto(CompanyJso jso, MappingContext ctx)
        {
            return new CompanyDto
            {
                Id = (int) jso.Id,
                ImageGuid = jso.ImageGuid,
                Name = jso.Name
            };
        }

        private static CompanyJso MapToJso(CompanyDto dto, MappingContext ctx)
        {
            var jso = new CompanyJso
            {
                Id = (Company) dto.Id,
                ImageGuid = dto.ImageGuid,
                Name = dto.Name
            };

            if (dto.ImageGuid.HasValue)
            {
                jso.ImagePath = ImageApiController.GetImagePath(dto.ImageGuid.Value);
            }

            return jso;
        }
    }
}