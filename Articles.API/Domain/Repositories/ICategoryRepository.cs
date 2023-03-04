using Articles.API.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Articles.API.Domain.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICategoryRepository
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
        Task AddAsync(Category category);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Category> FindByIdAsync(int id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        void Update(Category category);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        void Remove(Category category);
    }
}
