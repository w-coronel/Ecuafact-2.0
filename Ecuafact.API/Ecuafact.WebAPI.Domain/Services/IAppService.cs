using Ecuafact.WebAPI.Dal.Core;
using Ecuafact.WebAPI.Domain.Entities.App;
using Ecuafact.WebAPI.Domain.Entities.SRI;
using Ecuafact.WebAPI.Domain.Pagination;
using System;
using System.Collections.Generic;

namespace Ecuafact.WebAPI.Domain.Services
{
    public interface IAppService
    {
        OperationResult SRIPassword(string identificacion, string pass);        
        OperationResult<SRIContrib> SearchByRUC(string ruc);
        OperationResult<INFOREGIMEN> IsRimpe(string ruc);
        OperationResult<List<TipoSustento>> GetSustenanceType();
        long SyncUpReceived(string ruc, string yearMont, DateTime dateRequest, string status);
        bool AssignSupportType(long documentoId, string supportTypeCode, int emissionType = 1);
        OperationResult<PaginationList<ApiV3_DocumentosData>> GetDocumentosReceivedPagination(FilterDocumentReceived filter);
        OperationResult<List<ApiV3_DocumentosData>> GetDocumentosReceived(FilterDocumentReceived filter);
        OperationResult<ApiV3_DocumentosData> GetDocumentosReceivedById(long id);
        OperationResult<SupplierDocument> AddSaleNote(SupplierDocument document);
        OperationResult<SupplierDocument> GetDocumentReceivedDetailById(long documentId, string claveAcceso);
        OperationResult CancelDocument(long id);
    }
}
