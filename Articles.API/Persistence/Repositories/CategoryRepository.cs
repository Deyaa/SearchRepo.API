using Articles.API.Domain.Enums;
using Articles.API.Domain.Models;
using Articles.API.Domain.Repositories;
using Articles.API.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Articles.API.Persistence.Repositories
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {

        }
        public async Task<IEnumerable<Category>> ListAsync()
        {
            return await _context.Categories
                .FromSqlRaw($"EXEC {eStoredProcedures.splGetCategories.ToString()}")
                .ToListAsync();
        }
        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            IDictionary<string, object> paramsDic = new Dictionary<string, object>();
            paramsDic.Add("id", 1);
            var parameters = GenerateStoredProcedureParams(paramsDic);
            string commandText = GenerateCommandText(eStoredProcedures.splGetCategories.ToString(), parameters);

            return await _context.Categories
            .FromSqlRaw(commandText, parameters)
            .ToListAsync();
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
        }

        public async Task<Category> FindByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        public void Update(Category category)
        {
            _context.Categories.Update(category);
        }

        public void Remove(Category category)
        {
            _context.Categories.Remove(category);
        }
    }
}
