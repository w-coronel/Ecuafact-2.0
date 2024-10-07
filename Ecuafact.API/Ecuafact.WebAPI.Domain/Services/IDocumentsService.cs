using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Ecuafact.WebAPI.Domain.Entities;

namespace Ecuafact.WebAPI.Domain.Services
{
    public interface IDocumentsService
    {
        OperationResult<Document> AddDocument(long issuerId, Document document);
         
        Document GetIssuerDocumentById(long issuerId, long documentId);

        Document GetIssuerDocument(long issuerId, string documentType, string documentNumber, string establishmentCode, string issuePointCode);

        IQueryable<Document> GetIssuerDocuments(long issuerId, string documentType, DocumentStatusEnum? status, bool authorizeDate);

        IQueryable<Document> GetAllIssuerDocuments(long issuerId);
         
        OperationResult<Document> UpdateDocument(Document document);

        OperationResult DeleteDocument(long issuerId, long id, string reason);       

        OperationResult<Document> IssueDocument(long issuerId, long id, string reason, DateTime? issueDate);

        OperationResult<Document> UpdateDocumentStatus(long issuerId, long documentId, DocumentStatusInfo statusInfo);

        OperationResult<Document> DetailsDependants(Document document);

        string GetXmlDocument(long Id);

        void GetAllIssuerDocuments(string accessKey);
    }
}
