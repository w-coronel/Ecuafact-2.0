using Ecuafact.WebAPI.Dal.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Repository
{
    public interface IDocumentReceivedRepository
    {
        void AddDocumentReceived(SupplierDocument documentInfo);
        SupplierDocument GetDocumentReceivedById(long documentId, string claveAcceso);
    }
}
