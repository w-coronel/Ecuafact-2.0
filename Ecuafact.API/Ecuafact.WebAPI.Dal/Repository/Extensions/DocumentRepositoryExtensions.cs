using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Repository;

namespace Ecuafact.WebAPI.Dal
{
    public static class DocumentRepositoryExtensions
    {
        public static IEnumerable<Document> GetDocumentsByIssuer(this IEntityRepository<Document> DocumentRepository, long issuerId)
        {
            return DocumentRepository.GetAll().Where(pr => pr.IssuerId == issuerId);
        }

        public static Document One(this IEntityRepository<Document> entityRepository, long DocumentId, long issuerId)
        {
            return entityRepository.FindBy(pr => pr.Id == DocumentId && pr.IssuerId == issuerId).FirstOrDefault();
        }

        public static Document One(this IEntityRepository<Document> entityRepository, string DocumentId, long issuerId)
        {
            return entityRepository.FindBy(pr => pr.RUC.StartsWith(DocumentId) && pr.IssuerId == issuerId).FirstOrDefault();
        }

        public static IEnumerable<Document> Search(this IEntityRepository<Document> DocumentRepository, string searchTerm, long issuerId)
        {
            SqlParameter issuerIdParam = new SqlParameter("@issuerId", SqlDbType.BigInt) { Value = issuerId };
            SqlParameter searchTermParam = new SqlParameter("@searchTerm", SqlDbType.NVarChar) { Value = searchTerm };
            var searchResult = DocumentRepository.ExecSearchesWithStoreProcedure("SpSearchDocuments@searchTerm, @issuerId", searchTermParam,issuerIdParam);
            return searchResult;
        }
         
        public static IQueryable<Document> LoadDocumentReferences(this IQueryable<Document> document)
        {
            return document
                .Include(doc => doc.AdditionalFields)
                .Include(doc => doc.InvoiceInfo)
                .Include(doc => doc.RetentionInfo)
                .Include(doc => doc.CreditNoteInfo)
                .Include(doc => doc.ReferralGuideInfo)
                .Include(doc => doc.SettlementInfo)
                .Include(doc => doc.DebitNoteInfo);
        }


        public static bool IsNumeric(this string valor) => int.TryParse(valor, out _);

        public static bool IsDate(this string valor) => DateTime.TryParse(valor, out _);

        public static bool IsDecimal(this string valor) => decimal.TryParse(valor, out _);

    }
}
