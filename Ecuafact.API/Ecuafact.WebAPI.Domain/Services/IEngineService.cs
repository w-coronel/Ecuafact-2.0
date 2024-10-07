using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Entities.Engine;

namespace Ecuafact.WebAPI.Domain.Services
{
    public interface IEngineService
    {
        OperationResult UpdateEmisor(Issuer issuer);
        DocumentStatusInfo GetDocumentInfo(long id);
        OperationResult UpdateEmisorTextoInfoAdicional(EmisorTextoInfoAdicional issuer);
        OperationResult AddEmisorTextoInfoAdicional(List<EmisorTextoInfoAdicional> models);       
    }
}
