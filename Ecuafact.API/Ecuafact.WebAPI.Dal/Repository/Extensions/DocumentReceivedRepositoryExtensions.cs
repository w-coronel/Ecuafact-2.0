using Ecuafact.WebAPI.Dal.Core;
using System;
using System.Data.Entity;
using System.Linq;

namespace Ecuafact.WebAPI.Dal.Repository.Extensions
{
    public static class DocumentReceivedRepositoryExtensions
    {
        public static IQueryable<SupplierDocument> LoadDocumentReceivedReferences(this IQueryable<SupplierDocument> documentInfo)
        {
            return documentInfo
                .Include(doc => doc.DocumentInfo.Select(tax=> tax.TotalTaxes))
                .Include(doc => doc.AdditionalFields)
                .Include(doc => doc.Details)
                .Include(doc => doc.Payments)
                .Include(doc => doc.Reason);
        }
    }
}
