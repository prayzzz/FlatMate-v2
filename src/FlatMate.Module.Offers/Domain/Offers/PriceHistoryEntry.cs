using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FlatMate.Module.Common.DataAccess;

namespace FlatMate.Module.Offers.Domain.Offers
{
    [Table("PriceHistory")]
    public class PriceHistoryEntry : DboBase
    {
        private DateTime _date;

        public PriceHistoryEntry(decimal price, Product product) : this(price, DateTime.Now, product)
        {
        }

        private PriceHistoryEntry(decimal price, DateTime date, Product product)
        {
            Price = price;
            Date = date;
            Product = product;
        }

        [Required]
        public DateTime Date
        {
            get => _date;
            set => _date = value.Date;
        }

        [Required]
        public decimal Price { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [Required]
        public int ProductId { get; set; }
    }
}