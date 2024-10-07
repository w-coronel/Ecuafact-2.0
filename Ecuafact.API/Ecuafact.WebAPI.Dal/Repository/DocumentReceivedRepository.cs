using Ecuafact.WebAPI.Dal.Core;
using Ecuafact.WebAPI.Dal.Repository.Extensions;
using Ecuafact.WebAPI.Domain.Dal.Core;
using Ecuafact.WebAPI.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Dal.Repository
{
    public  class DocumentReceivedRepository : EntityRepository<SupplierDocument>, IDocumentReceivedRepository
    {
        public DocumentReceivedRepository(DbContext entitiesContext)
           : base(entitiesContext) { }

        public void AddDocumentReceived(SupplierDocument documentInfo)
        {
            using (DbContextTransaction transaction = DataContext.Database.BeginTransaction())
            {
                try
                {
                    DataContext.Set<SupplierDocument>().Add(documentInfo);
                    DataContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public SupplierDocument GetDocumentReceivedById(long documentId, string claveAcceso)
        {

            var documentInfo = !string.IsNullOrWhiteSpace(claveAcceso) ?
                                base.FindBy(o => o.AccessKey == claveAcceso)
                                    .LoadDocumentReceivedReferences()
                                    .FirstOrDefault() :
                                base.FindBy(o => o.DocumentPk == documentId)
                                    .LoadDocumentReceivedReferences()
                                    .FirstOrDefault();

            return documentInfo;

            throw new KeyNotFoundException($"El documento # {documentId} no existe!");
        }


       
    }
}
