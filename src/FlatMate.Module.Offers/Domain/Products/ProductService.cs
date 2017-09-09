using Microsoft.EntityFrameworkCore;
using prayzzz.Common.Attributes;
using prayzzz.Common.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlatMate.Module.Offers.Domain.Products
{
    public interface IProductService
    {
        Task<IEnumerable<ProductCategoryDto>> GetCategories();
    }

    [Inject]
    public class ProductService : IProductService
    {
        private readonly OffersDbContext _dbContext;

        private readonly IMapper _mapper;

        public ProductService(OffersDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductCategoryDto>> GetCategories()
        {
            return (await _dbContext.ProductCategories.ToListAsync()).Select(_mapper.Map<ProductCategoryDto>);
        }
    }
}
