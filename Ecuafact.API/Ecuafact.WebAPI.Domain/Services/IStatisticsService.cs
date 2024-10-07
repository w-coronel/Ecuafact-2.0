using Ecuafact.WebAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Services
{
    public interface IStatisticsService
    {

        #region ### Issuer Statistics ###

        DashboardInfo GetDashboard(long issuerId);
       
        #endregion

    }
}
