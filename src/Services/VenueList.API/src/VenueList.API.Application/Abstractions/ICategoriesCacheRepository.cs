using System.Collections.Generic;
using System.Threading.Tasks;
using Library.Shared.Caching.Abstractions;
using Library.Shared.Models.Category.Dtos;

namespace VenueList.API.Application.Abstractions
{
    public interface ICategoriesCacheRepository : IMemoryCacheRepository<IList<CategoryDto>>
    {
        Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync();
        Task<CategoryDto> FindCategoryByNameAsync(string name);
    }
}