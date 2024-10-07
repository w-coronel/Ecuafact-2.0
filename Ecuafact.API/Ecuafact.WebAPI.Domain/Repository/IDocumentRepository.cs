using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ecuafact.WebAPI.Domain.Entities;

namespace Ecuafact.WebAPI.Domain.Repository
{
    public interface IDocumentRepository
    {
        long GetNextDocumentSequential(long issuerId, string establishment, string issuePoint, short documentTypeId,string documentTypeDescription);

        long GetCounterDocument(long issuerId);        

        void AddDocument(Document invoice);

        void UpdateDocument(Document invoice, bool updateAll);

        IQueryable<Document> GetDocumentsByIssuer(long issuerId, string documentType, DocumentStatusEnum? status, bool authorizeDate);

        IQueryable<Document> GetAllDocumentsByIssuer(long issuerId );

        Document GetDocumentById(long issuerId, long documentId);
          
        Document GetDocument(long issuerId, string documentType,
            string documentNumber, string establishmentCode, string issuePointCode);

        void DetailsDependants(Document document);

        String GenerateDocumentXml(long documentId);

        void  ReloadEngineDocument(string accessKey);
    }
}
