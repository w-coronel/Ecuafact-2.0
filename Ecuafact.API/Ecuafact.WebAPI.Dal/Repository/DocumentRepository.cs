using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ecuafact.WebAPI.Dal.Repository.Extensions;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Repository;

namespace Ecuafact.WebAPI.Dal.Repository
{
    public class DocumentRepository: EntityRepository<Document>, IDocumentRepository
    {

        public DocumentRepository(DbContext entitiesContext)
            : base(entitiesContext) { }

        public void AddDocument(Document document)
        {
            using (DbContextTransaction transaction = DataContext.Database.BeginTransaction())
            {
                try
                {
                    DataContext.Set<Document>().Add(document);
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

        public void UpdateDocument(Document document, bool updateAll)
        {
            using (DbContextTransaction transaction = DataContext.Database.BeginTransaction())
            {
                try
                {
                    this.Edit(document);

                    this.ValidateDependants(document);

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

        private void ValidateDependants(Document document)
        {
            if (document.AdditionalFields != null)
            {
                var fields = document.AdditionalFields.Where(x => x.Id > 0);
                if (fields.Any() && document.AdditionalFields.Any(x => x.Id == 0))
                {
                    DataContext.Set<AdditionalField>()
                        .RemoveRange(fields);
                }
            }

            if (document.InvoiceInfo != null)
            {
                if (document.InvoiceInfo.Details != null)
                {
                    var details = document.InvoiceInfo.Details.Where(x => x.Id > 0);
                    if (details.Any() && document.InvoiceInfo.Details.Any(x => x.Id == 0))
                    {
                        details.Where(x => x.Taxes != null).ToList()
                            .ForEach(detail => DataContext.Set<Tax>()
                                .RemoveRange(detail.Taxes)); 

                        DataContext.Set<DocumentDetail>()
                            .RemoveRange(details); 
                    }
                }

                if (document.InvoiceInfo.Payments != null)
                {
                    var payments = document.InvoiceInfo.Payments.Where(x => x.Id > 0);
                    if (payments.Any() && document.InvoiceInfo.Payments.Any(x => x.Id == 0))
                    {
                        DataContext.Set<Payment>()
                            .RemoveRange(payments);
                    }
                }

                if (document.InvoiceInfo.TotalTaxes != null)
                {
                    var taxes = document.InvoiceInfo.TotalTaxes.Where(x => x.Id > 0);
                    if (taxes.Any() && document.InvoiceInfo.TotalTaxes.Any(x => x.Id == 0))
                    {
                        DataContext.Set<TotalTax>()
                            .RemoveRange(taxes);
                    }
                }
            }
            else if (document.CreditNoteInfo != null)
            {
                if (document.CreditNoteInfo.Details != null)
                {
                    var details = document.CreditNoteInfo.Details.Where(x => x.Id > 0);
                    if (details.Any() && document.CreditNoteInfo.Details.Any(x => x.Id == 0))
                    {
                        details.Where(x => x.Taxes != null).ToList()
                            .ForEach(detail => DataContext.Set<Tax>()
                                .RemoveRange(detail.Taxes));

                        DataContext.Set<DocumentDetail>()
                            .RemoveRange(details);
                    }
                }

                if (document.CreditNoteInfo.Payments != null)
                {
                    var payments = document.CreditNoteInfo.Payments.Where(x => x.Id > 0);
                    if (payments.Any() && document.CreditNoteInfo.Payments.Any(x => x.Id == 0))
                    {
                        DataContext.Set<Payment>()
                            .RemoveRange(payments);
                    }
                }

                if (document.CreditNoteInfo.TotalTaxes != null)
                {
                    var taxes = document.CreditNoteInfo.TotalTaxes.Where(x => x.Id > 0);
                    if (taxes.Any() && document.CreditNoteInfo.TotalTaxes.Any(x => x.Id == 0))
                    {
                        DataContext.Set<TotalTax>()
                            .RemoveRange(taxes);
                    }
                }
            }
            else if (document.RetentionInfo != null)
            {
                if (document.RetentionInfo.Details != null)
                {
                    var details = document.RetentionInfo.Details.Where(x => x.Id > 0);
                    if (details.Any() && document.RetentionInfo.Details.Any(x => x.Id == 0))
                    {
                        DataContext.Set<RetentionDetail>()
                            .RemoveRange(details);
                    }
                }
                if (document.RetentionInfo.Payments != null)
                {
                    var payments = document.RetentionInfo.Payments.Where(x => x.Id > 0);
                    if (payments.Any() && document.RetentionInfo.Payments.Any(x => x.Id == 0))
                    {
                        DataContext.Set<Payment>()
                            .RemoveRange(payments);
                    }
                }

                if (document.RetentionInfo.TotalTaxes != null)
                {
                    var taxes = document.RetentionInfo.TotalTaxes.Where(x => x.Id > 0);
                    if (taxes.Any() && document.RetentionInfo.TotalTaxes.Any(x => x.Id == 0))
                    {
                        DataContext.Set<TotalTax>()
                            .RemoveRange(taxes);
                    }
                }
            }
            else if (document.ReferralGuideInfo != null)
            {
                if (document.ReferralGuideInfo.Details != null)
                {
                    var details = document.ReferralGuideInfo.Details.Where(x => x.Id > 0);
                    if (details.Any() && document.ReferralGuideInfo.Details.Any(x => x.Id == 0))
                    {
                        DataContext.Set<ReferralGuideDetail>()
                            .RemoveRange(details);
                    }
                }
            }
            else if (document.SettlementInfo != null)
            {
                if (document.SettlementInfo.Details != null)
                {
                    var details = document.SettlementInfo.Details.Where(x => x.Id > 0);
                    if (details.Any() && document.SettlementInfo.Details.Any(x => x.Id == 0))
                    {
                        details.Where(x => x.Taxes != null).ToList()
                            .ForEach(detail => DataContext.Set<Tax>()
                                .RemoveRange(detail.Taxes));

                        DataContext.Set<DocumentDetail>()
                            .RemoveRange(details);
                    }
                }

                if (document.SettlementInfo.Payments != null)
                {
                    var payments = document.SettlementInfo.Payments.Where(x => x.Id > 0);
                    if (payments.Any() && document.SettlementInfo.Payments.Any(x => x.Id == 0))
                    {
                        DataContext.Set<Payment>()
                            .RemoveRange(payments);
                    }
                }

                if (document.SettlementInfo.TotalTaxes != null)
                {
                    var taxes = document.SettlementInfo.TotalTaxes.Where(x => x.Id > 0);
                    if (taxes.Any() && document.SettlementInfo.TotalTaxes.Any(x => x.Id == 0))
                    {
                        DataContext.Set<TotalTax>()
                            .RemoveRange(taxes);
                    }
                }
            }
            else if (document.DebitNoteInfo != null)
            {
                if (document.DebitNoteInfo.DebitNoteDetail != null)
                {
                    var details = document.DebitNoteInfo.DebitNoteDetail.Where(x => x.Id > 0);
                    if (details.Any() && document.DebitNoteInfo.DebitNoteDetail.Any(x => x.Id == 0))
                    {
                        DataContext.Set<DebitNoteDetail>()
                            .RemoveRange(details);
                    }
                }

                if (document.DebitNoteInfo.Payments != null)
                {
                    var payments = document.DebitNoteInfo.Payments.Where(x => x.Id > 0);
                    if (payments.Any() && document.DebitNoteInfo.Payments.Any(x => x.Id == 0))
                    {
                        DataContext.Set<Payment>()
                            .RemoveRange(payments);
                    }
                }

                if (document.DebitNoteInfo.TotalTaxes != null)
                {
                    var taxes = document.DebitNoteInfo.TotalTaxes.Where(x => x.Id > 0);
                    if (taxes.Any() && document.DebitNoteInfo.TotalTaxes.Any(x => x.Id == 0))
                    {
                        DataContext.Set<TotalTax>()
                            .RemoveRange(taxes);
                    }
                }
            }
        }

        public Document GetDocumentById(long issuerId, long documentId)
        {
            var document = base.FindBy(o => o.IssuerId == issuerId && o.Id == documentId)
                .LoadDocumentReferences()
                .FirstOrDefault();

            if (document != null)
            {
                this.FillDocumentDetails(document);

                return document;
            }

            throw new KeyNotFoundException($"El Documento # {documentId} no existe!");
        }           

        public Document GetDocument(long issuerId, string documentType,
            string documentNumber, string establishmentCode, string issuePointCode)
        {
            // Es un valor requerido
            if (!string.IsNullOrEmpty(documentNumber))
            {
                var docId = base.FindBy(o => o.IssuerId == issuerId
                           && o.DocumentTypeCode == documentType
                           && o.Sequential.EndsWith(documentNumber)
                           && o.EstablishmentCode.EndsWith(establishmentCode)
                           && o.IssuePointCode.EndsWith(issuePointCode))
                    .Select(x => x.Id)
                    .FirstOrDefault();

                if (docId > 0)
                {
                    return GetDocumentById(issuerId, docId) ;
                }
            }

            return null;
        }

        public IQueryable<Document> GetAllDocumentsByIssuer(long issuerId)
        {
            return base.FindBy(o => o.IssuerId == issuerId);
        }

        public IQueryable<Document> GetDocumentsByIssuer(long issuerId, string documentType, DocumentStatusEnum? status, bool authorizeDate)
        {
            // Si el parametro estado es nulo no filtra por el estado, 
            // de lo contrario se filtra por el id del estado
            IQueryable<Document> list;
            if (status.HasValue && authorizeDate)
            {
                list = base.FindBy(o => o.IssuerId == issuerId && o.DocumentTypeCode == documentType && o.Status == status.Value && o.AuthorizationDate != null);
            }
            else if (status.HasValue)
            {
                list = base.FindBy(o => o.IssuerId == issuerId && o.DocumentTypeCode == documentType && o.Status == status.Value);
            }            
            else
            {
                list = base.FindBy(o => o.IssuerId == issuerId && o.DocumentTypeCode == documentType);
            }
             
            return list.LoadDocumentReferences();
        }         

        private void FillDocumentDetails(Document document)
        {
            if (document.InvoiceInfo != null)
            {
                DataContext.Entry(document.InvoiceInfo).Collection(s => s.Details).Load();
                DataContext.Entry(document.InvoiceInfo).Collection(s => s.Payments).Load();
                DataContext.Entry(document.InvoiceInfo).Collection(s => s.TotalTaxes).Load();
                 
                if (document.InvoiceInfo.Details != null)
                {
                    foreach (var item in document.InvoiceInfo.Details)
                    {
                        DataContext.Entry(item).Collection(s => s.Taxes).Load();
                    }
                }
            }

            if (document.RetentionInfo != null)
            {
                DataContext.Entry(document.RetentionInfo).Collection(s => s.Details).Load();
                DataContext.Entry(document.RetentionInfo).Collection(s => s.TotalTaxes).Load();
                DataContext.Entry(document.RetentionInfo).Collection(s => s.Payments).Load();
            }


            if (document.ReferralGuideInfo != null)
            {
                DataContext.Entry(document.ReferralGuideInfo).Collection(s => s.Details).Load();
            }

            if (document.CreditNoteInfo != null)
            {
                DataContext.Entry(document.CreditNoteInfo).Collection(s => s.Details).Load();
                DataContext.Entry(document.CreditNoteInfo).Collection(s => s.TotalTaxes).Load();

                if (document.CreditNoteInfo.Details != null)
                {
                    foreach (var item in document.CreditNoteInfo.Details)
                    {
                        DataContext.Entry(item).Collection(s => s.Taxes).Load();
                    }
                }
            }

            if (document.SettlementInfo != null)
            {
                DataContext.Entry(document.SettlementInfo).Collection(s => s.Details).Load();
                DataContext.Entry(document.SettlementInfo).Collection(s => s.Payments).Load();
                DataContext.Entry(document.SettlementInfo).Collection(s => s.TotalTaxes).Load();

                if (document.SettlementInfo.Details != null)
                {
                    foreach (var item in document.SettlementInfo.Details)
                    {
                        DataContext.Entry(item).Collection(s => s.Taxes).Load();
                    }
                }
            }

            if (document.DebitNoteInfo != null)
            {
                DataContext.Entry(document.DebitNoteInfo).Collection(s => s.DebitNoteDetail).Load();
                DataContext.Entry(document.DebitNoteInfo).Collection(s => s.Payments).Load();
                DataContext.Entry(document.DebitNoteInfo).Collection(s => s.TotalTaxes).Load();               
            }

        }

        public long GetNextDocumentSequential(long issuerId, string establishment, string issuePoint, short documentTypeId, string documentTypeDescription)
        {
            SqlParameter issuerIdParam = new SqlParameter("@IssuerId", issuerId);
            SqlParameter establishmentParam = new SqlParameter("@EstablishmentCode", establishment);
            SqlParameter issuePointParam = new SqlParameter("@IssuePointCode", issuePoint);
            SqlParameter documentTypeIdParam = new SqlParameter("@DocumentTypeId", documentTypeId);
            SqlParameter documentTypeDescriptionParam = new SqlParameter("@DocumentType", documentTypeDescription);

            var sequentialOutParam = new SqlParameter
            {
                ParameterName = "@Sequential",
                SqlDbType = SqlDbType.BigInt,
                Direction = ParameterDirection.Output
            };

            DataContext.Database.ExecuteSqlCommand("EXEC SpGetNextDocumentSequential @IssuerId, @EstablishmentCode,@IssuePointCode, @DocumentTypeId,@DocumentType,@Sequential out", 
                issuerIdParam,
                establishmentParam,
                issuePointParam,
                documentTypeIdParam, 
                documentTypeDescriptionParam,
                sequentialOutParam);
            return Convert.ToInt64(sequentialOutParam.Value);
        }

        public String GenerateDocumentXml(long documentId)
        {
            try
            {
                SqlParameter iderpParam = new SqlParameter("@iderp", documentId.ToString());
                var xmlDocOutParam = new SqlParameter
                {
                    ParameterName = "@XmlDocument",
                    SqlDbType = SqlDbType.Xml,
                    Direction = ParameterDirection.Output
                };
                DataContext.Database.ExecuteSqlCommand("EXEC get_xmlComprobante @iderp, @XmlDocument out", iderpParam, xmlDocOutParam);
                return xmlDocOutParam.Value.ToString();

            }
            catch (Exception ex)
            {
                var txt = ex;
                return null;
            }
        }

        public long GetCounterDocument(long issuerId)
        {
            try
            {
                SqlParameter issuerIdParam = new SqlParameter("@IssuerId", issuerId);
                var balanceDocumentOutParam = new SqlParameter
                {
                    ParameterName = "@BalanceDocument",
                    SqlDbType = SqlDbType.BigInt,
                    Direction = ParameterDirection.Output
                };

                DataContext.Database.ExecuteSqlCommand("EXEC SpDocumentCounter @IssuerId, @BalanceDocument out", issuerIdParam, balanceDocumentOutParam);
                return Convert.ToInt64(balanceDocumentOutParam.Value);
            }
            catch (Exception ex)
            {
                var txt = ex;
                return 0;
            }
        }

        public void DetailsDependants(Document document)
        {
            using (DbContextTransaction transaction = DataContext.Database.BeginTransaction())
            {
                try
                {
                    if (document.InvoiceInfo != null)
                    {
                        if (document.InvoiceInfo.Details != null)
                        {
                            var details = document.InvoiceInfo.Details.Where(x => x.Id > 0);
                            if (details.Any())
                            {
                                details.Where(x => x.Taxes != null).ToList()
                                    .ForEach(detail => DataContext.Set<Tax>()
                                        .RemoveRange(detail.Taxes));

                                DataContext.Set<DocumentDetail>()
                                    .RemoveRange(details);
                            }
                        }

                        if (document.InvoiceInfo.TotalTaxes != null)
                        {
                            var taxes = document.InvoiceInfo.TotalTaxes.Where(x => x.Id > 0);
                            if (taxes.Any())
                            {
                                DataContext.Set<TotalTax>()
                                    .RemoveRange(taxes);
                            }
                        }
                    }
                    else if (document.CreditNoteInfo != null)
                    {
                        if (document.CreditNoteInfo.Details != null)
                        {
                            var details = document.CreditNoteInfo.Details.Where(x => x.Id > 0);
                            if (details.Any())
                            {
                                details.Where(x => x.Taxes != null).ToList()
                                    .ForEach(detail => DataContext.Set<Tax>()
                                        .RemoveRange(detail.Taxes));

                                DataContext.Set<DocumentDetail>()
                                    .RemoveRange(details);
                            }
                        }

                        if (document.CreditNoteInfo.TotalTaxes != null)
                        {
                            var taxes = document.CreditNoteInfo.TotalTaxes.Where(x => x.Id > 0);
                            if (taxes.Any())
                            {
                                DataContext.Set<TotalTax>()
                                    .RemoveRange(taxes);
                            }
                        }
                    }                   
                    else if (document.SettlementInfo != null)
                    {
                        if (document.SettlementInfo.Details != null)
                        {
                            var details = document.SettlementInfo.Details.Where(x => x.Id > 0);
                            if (details.Any())
                            {
                                details.Where(x => x.Taxes != null).ToList()
                                    .ForEach(detail => DataContext.Set<Tax>()
                                        .RemoveRange(detail.Taxes));

                                DataContext.Set<DocumentDetail>()
                                    .RemoveRange(details);
                            }
                        }

                        if (document.SettlementInfo.TotalTaxes != null)
                        {
                            var taxes = document.SettlementInfo.TotalTaxes.Where(x => x.Id > 0);
                            if (taxes.Any())
                            {
                                DataContext.Set<TotalTax>()
                                    .RemoveRange(taxes);
                            }
                        }
                    }
                    else if (document.DebitNoteInfo != null)
                    {
                        if (document.DebitNoteInfo.DebitNoteDetail != null)
                        {
                            var details = document.DebitNoteInfo.DebitNoteDetail.Where(x => x.Id > 0);
                            if (details.Any())
                            {                              

                                DataContext.Set<DebitNoteDetail>()
                                    .RemoveRange(details);
                            }
                        }

                        if (document.DebitNoteInfo.TotalTaxes != null)
                        {
                            var taxes = document.DebitNoteInfo.TotalTaxes.Where(x => x.Id > 0);
                            if (taxes.Any())
                            {
                                DataContext.Set<TotalTax>()
                                    .RemoveRange(taxes);
                            }
                        }
                    }

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

        public void ReloadEngineDocument(string accessKey)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(accessKey))
                {
                    SqlParameter claveAcceso = new SqlParameter("@accessKey", accessKey);
                    DataContext.Database.ExecuteSqlCommand("EXEC Sp_ReloadDocument @accessKey", claveAcceso);
                }
            }
            catch(Exception){ }
        }
    }
}
