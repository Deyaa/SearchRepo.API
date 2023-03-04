using Articles.API.Persistence.Contexts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Articles.API.Persistence.Repositories
{
    public abstract class BaseRepository
    {
        protected readonly AppDbContext _context;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
        }

        public void ExecuteStoredProcedure(string storedProcedureName, IDictionary<string, object> paramDic)
        {
            var parameters = GenerateStoredProcedureParams(paramDic);
            string commandText = GenerateCommandText(storedProcedureName, parameters);
            var result = _context.Database.ExecuteSqlRaw(commandText, parameters);
        }

        protected string GenerateCommandText(string storedProcedureName, SqlParameter[] parameters)
        {
            string CommandText = "EXEC {0} {1}";
            string[] ParameterNames = new string[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterNames[i] = parameters[i].ParameterName;
            }
            return string.Format(CommandText, storedProcedureName, string.Join(",", ParameterNames));
        }
        public SqlParameter[] GenerateStoredProcedureParams(IDictionary<string, object> paramsDic)
        {
            int i = 0;
            SqlParameter[] parameters = new SqlParameter[paramsDic.Count];
            foreach (KeyValuePair<string, object> entry in paramsDic)
            {
                parameters[i] = new SqlParameter("@" + entry.Key, entry.Value);
                i++;
            }
            return parameters;
        }
    }
}
