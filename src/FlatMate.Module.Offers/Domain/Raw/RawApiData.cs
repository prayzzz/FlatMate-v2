using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Common.Dtos;
using FlatMate.Module.Offers.Domain.Companies;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlatMate.Module.Offers.Domain.Raw
{
    [Table("RawApiData")]
    public class RawApiData : DboBase
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Data { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; }

        [Required]
        public int CompanyId { get; set; }
    }

    public class RawApiDataDto : DtoBase
    {
        public DateTime Date { get; set; }

        public string Data { get; set; }

        public CompanyDto Company { get; set; }

        public int CompanyId { get; set; }
    }
}
