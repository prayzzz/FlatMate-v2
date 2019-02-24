using System;
using System.Threading;
using System.Threading.Tasks;
using FlatMate.Module.Common.DataAccess;
using FlatMate.Module.Offers.Domain.Companies;
using FlatMate.Module.Offers.Domain.Markets;
using FlatMate.Module.Offers.Domain.Offers;
using FlatMate.Module.Offers.Domain.Products;
using FlatMate.Module.Offers.Domain.Raw;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using prayzzz.Common.Results;

namespace FlatMate.Module.Offers
{
    public class OffersDbContext : FlatMateDbContext
    {
        private readonly ILogger<OffersDbContext> _logger;

        public OffersDbContext(DbContextOptions<OffersDbContext> options, ILogger<OffersDbContext> logger) : base(options)
        {
            _logger = logger;
        }

        public virtual DbSet<CompanyData> Companies { get; set; }

        public virtual DbSet<Market> Markets { get; set; }

        public virtual DbSet<Offer> Offers { get; set; }

        public virtual DbSet<PriceHistory> PriceHistoryEntries { get; set; }

        public virtual DbSet<ProductCategory> ProductCategories { get; set; }

        public virtual DbSet<ProductFavorite> ProductFavorites { get; set; }

        public virtual DbSet<ProductInfoDto> ProductInfoDtos { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<RawOfferData> RawOfferData { get; set; }

        public new Result SaveChanges()
        {
            try
            {
                base.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogError(0, e, "Error while saving changes");
                return new Result(ErrorType.InternalError, "Datenbankfehler");
            }

            return Result.Success;
        }

        public new async Task<Result> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                await base.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(0, e, "Error while saving changes");
                return new Result(ErrorType.InternalError, "Datenbankfehler");
            }

            return Result.Success;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Offers");

            modelBuilder.Entity<Product>(ConfigureProductDbo);
        }

        private void ConfigureProductDbo(EntityTypeBuilder<Product> entityTypeBuilder)
        {
            // https://github.com/aspnet/EntityFrameworkCore/issues/6674
            entityTypeBuilder.Metadata
                             .FindNavigation(nameof(Product.PriceHistoryEntries))
                             .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}