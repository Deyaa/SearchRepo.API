using Articles.API.Domain.Models;
using Articles.API.Domain.Services.Communication;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Articles.API.Domain.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Category>> ListAsync();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        Task<CategoryResponse> SaveAsync(Category category);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        Task<CategoryResponse> UpdateAsync(int id, Category category);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<CategoryResponse> DeleteAsync(int id);
    }
}
