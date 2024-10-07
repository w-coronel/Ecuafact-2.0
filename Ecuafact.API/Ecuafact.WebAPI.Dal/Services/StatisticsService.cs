using Ecuafact.WebAPI.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Ecuafact.WebAPI.Domain.Repository;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Dal.Core;
using System.Data.Entity;

namespace Ecuafact.WebAPI.Dal.Services
{
    public class StatisticsService : IStatisticsService
    {
        readonly EcuafactExpressContext _entitiesContext = new EcuafactExpressContext();       

        public DashboardInfo GetDashboard(long issuerId)
        {            
            SqlParameter issuerIdParam = new SqlParameter("@IssuerId", issuerId);
            var result = _entitiesContext.Database.SqlQuery<DashboardInfo>("EXEC SpGetStatistics @IssuerId", issuerIdParam);
            return result.FirstOrDefault();
        }        
    }
}
