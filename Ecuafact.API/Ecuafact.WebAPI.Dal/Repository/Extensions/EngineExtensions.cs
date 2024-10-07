using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class EngineExtensions
    {
        public static string Filename 
        {
            get
            {
                var path = ConfigurationManager.AppSettings["Ecuafact:LogLocation"] ?? "D:\\";
                var filename = Path.Combine(path, $"QUERY.LOG.{DateTime.Now.ToFileTime()}.txt");
                return filename;
            }
        }

        internal static void LogQuery<T>(this IQueryable<T> query, params string[] parameters)
        {
            LogQuery(query as DbQuery<T>, parameters);
        }

        internal static void LogQuery<T>(this DbQuery<T> query, params string[] parameters)
        {
            var lines = new List<string>();

            if (parameters != null && parameters.Length > 0)
            {
                lines.AddRange(parameters);
            }
            
            lines.Add("SQL Query:");
            lines.Add(query?.Sql);

            File.WriteAllLines(Filename, lines);
        }
    }
}
