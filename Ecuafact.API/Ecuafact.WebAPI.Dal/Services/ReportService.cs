using Ecuafact.WebAPI.Dal.Core;
using Ecuafact.WebAPI.Dal.Repository;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Repository;
using Ecuafact.WebAPI.Domain.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Dal.Services
{
    public class ReportService : IReportService
    {
        private readonly IEntityRepository<SalesReport> _salesRepository;
        private readonly IEntityRepository<Sales> _sales;
        private readonly IEntityRepository<PurchaseReport> _purchaseRepository;
        private readonly IEntityRepository<Purchases> _purchase;
        private readonly IEntityRepository<VistaRetenciones> _viewRetentionRepository;
        private readonly IEntityRepository<vwFacturas> _viewInvoiceRepository;
        private EcuafactAppContext DB = new EcuafactAppContext();

        public ReportService(IEntityRepository<SalesReport> salesRepository, IEntityRepository<PurchaseReport> purchaseRepository, IEntityRepository<Sales> sales, 
            IEntityRepository<Purchases> purchase )
        {
            salesRepository.Timeout = 300; // Seconds
            purchaseRepository.Timeout = 300; // Seconds
            _salesRepository = salesRepository;
            _purchaseRepository = purchaseRepository;
            _sales = sales;
            _purchase = purchase;
            _viewInvoiceRepository = new EntityRepository<vwFacturas>(DB);
            _viewRetentionRepository = new EntityRepository<VistaRetenciones>(DB);
        }

        #region Reporte de Ventas 
        public IEnumerable<SalesReport> GetSalesReport(string issuer, DateTime? dateStart, DateTime? dateEnd, string search)
        {
            if (Constants.viewsSalesReport)
            {
                return GetViewsSalesReport(issuer, dateStart, dateEnd, search);
            }
            else {
                //return GetUnionViewsSalesRetention(issuer, dateStart, dateEnd, search);
                return GetSalesRetentionReport(issuer, dateStart, dateEnd, search);
            }
        }

        private IEnumerable<SalesReport> GetViewsSalesReport(string issuer, DateTime? dateStart, DateTime? dateEnd, string search)
        {
            var report = _salesRepository.FindBy(model => model.RUC == issuer);

            if (dateStart != null)
            {
                report = report.Where(model => model.IssuedOn >= dateStart);
            }

            if (dateEnd != null)
            {
                report = report.Where(model => model.IssuedOn <= dateEnd);
            }

            if (!string.IsNullOrEmpty(search))
            {
                report = report.Where(model => model.ContributorRUC.Contains(search)
                || model.ContributorName.ToUpper().Contains(search.ToUpper())
                || model.DocumentNumber.Contains(search.ToUpper())
                || model.AuthorizationNumber.Contains(search.ToUpper())
                );
            }

            report.LogQuery("Se realizo la consulta para el reporte de compras:", $"{DateTime.Now}",
                $"Issuer: {issuer}",
                $"DateStart: {dateStart}",
                $"DateEnd: {dateEnd}",
                $"Search: {search}"
            );

            return report.OrderBy(model => model.DocumentNumber).ToList();
        }

        private IEnumerable<SalesReport> GetUnionViewsSalesRetention(string issuer, DateTime? dateStart, DateTime? dateEnd, string search)
        {
            // reporte de las ventas de la vista de Sales del Express           
            var viewSales = _sales.GetAll();
            if (dateStart == null)
            {
                dateStart = DateTime.Now;
            }
            if (dateEnd == null)
            {
                dateEnd = DateTime.Now.AddMonths(1);
            }

            if (!string.IsNullOrEmpty(search))
            {
                viewSales = viewSales.AsNoTracking().Where(model => model.ContributorRUC.Contains(search)
                || model.ContributorName.ToUpper().Contains(search.ToUpper())
                || model.DocumentNumber.Contains(search.ToUpper())
                || model.AuthorizationNumber.Contains(search.ToUpper())
                );
            }

            viewSales.LogQuery("Se realizo la consulta para el reporte de compras:", $"{DateTime.Now}",
                $"Issuer: {issuer}",
                $"DateStart: {dateStart}",
                $"DateEnd: {dateEnd}",
                $"Search: {search}"
            );

            
            var sales = (from s in viewSales.AsNoTracking()
                         where s.RUC == issuer &&
                        (s.IssuedOn >= dateStart && s.IssuedOn <= dateEnd)
                         select s).OrderBy(o => o.DocumentNumber).ToList();           
           
            if (sales.Count > 0)
            {
                
                // reporte de ventas de la vista de VistaRetenciones del EcuafactApp  
                var viewRetention = _viewRetentionRepository.GetAll();
                dateEnd = dateEnd.Value.AddMonths(1);
                viewRetention.LogQuery("Se realizo la consulta para el reporte de retenciones:", $"{DateTime.Now}",
                    $"Issuer: {issuer}",
                    $"DateStart: {dateStart}",
                    $"DateEnd: {dateEnd}"
                );
                var retention = (from r in viewRetention.AsNoTracking()
                                 where r.Retencion_RUC == issuer &&
                                      (r.Retencion_FechaEmision >= dateStart && r.Retencion_FechaEmision <= dateEnd)
                                 select r).OrderBy(r => r.Retencion_NumeroReferencia).ToList();                
                
                var saleSreport = (from s in sales
                                   join r in retention on s.DocumentNumber.Replace("-", "") equals r.Retencion_NumeroReferencia into salesretention
                                   from sr in salesretention.DefaultIfEmpty()
                                   select new SalesReport
                                   {
                                       Id = s.Id,
                                       IssuerId = s.IssuerId,
                                       IssuedOn = s.IssuedOn,
                                       DocumentTypeCode = s.DocumentTypeCode,
                                       DocumentType = s.DocumentType,
                                       EstablishmentCode = s.EstablishmentCode,
                                       IssuePointCode = s.IssuePointCode,
                                       Sequential = s.Sequential,
                                       DocumentNumber = s.DocumentNumber,
                                       AuthorizationNumber = s.AuthorizationNumber,
                                       AuthorizationDate = s.AuthorizationDate,
                                       ContributorRUC = s.ContributorRUC,
                                       ContributorName = s.ContributorName,
                                       Base0 = s.Base0,
                                       Base12 = s.Base12,
                                       IVA = s.IVA,
                                       Total = s.Total,
                                       Description = s.Description,
                                       Notes = s.Notes,
                                       RetentionId = sr?.IdRetencion,
                                       RetentionContributorRUC = sr?.Retencion_RUC,
                                       RetentionContributor = sr?.Retencion_Comprador,
                                       RetentionIssuedOn = sr?.Retencion_FechaEmision,
                                       RetentionNumber = sr?.Retencion_Numero,
                                       RetentionAuthorizationNumber = sr?.Retencion_Autorizacion,
                                       RetentionReferenceType = sr?.Retencion_CodigoReferencia,
                                       RetentionReferenceNumber = sr?.Retencion_NumeroReferencia,
                                       RetentionReferenceDate = sr?.Retencion_FechaReferencia,
                                       Retention104TaxCode = sr?.Retencion_Codigo104,
                                       RetentionTaxBase = sr?.Renta_BaseImponible,
                                       RetentionTaxRate = sr?.Renta_Porcentaje,
                                       RetentionTaxValue = sr?.Renta_Valor,
                                       RetentionVatCode = sr?.Retencion_CodigoIVA,
                                       RetentionVatBase = sr?.Retencion_BaseIVA,
                                       RetentionVatRate = sr?.Retencion_PorcentajeIVA,
                                       RetentionVatValue = sr?.Retencion_IVARetenido,
                                       RetentionISDCode = sr?.Retencion_CodigoISD,
                                       RetentionISDBase = sr?.Retencion_BaseISD,
                                       RetentionISDRate = sr?.Retencion_PorcentajeISD,
                                       RetentionISDValue = sr?.Retencion_ISDRetenido,
                                       RetentionReason = sr?.Retencion_Motivo,
                                       RetentionDescription = sr?.Retencion_Descripcion,
                                       RetentionNotes = sr?.Retencion_Observaciones,
                                       RetentionRUC = sr?.Retencion_RUCProveedor,
                                       RetentionBusinessName = sr?.Retencion_Proveedor,
                                       RetentionDate = sr?.Documento_FechaEmision,
                                       RUC = s.RUC,
                                       BussinesName = s.BussinesName,
                                       Payment = s.Payment
                                   }).ToList();

                return saleSreport;
            }

            return new List<SalesReport>();
        }

        private IEnumerable<SalesReport> GetSalesRetentionReport(string issuer, DateTime? dateStart, DateTime? dateEnd, string search)
        {
            // reporte de las ventas de la vista de Sales del Express
            TimeSpan ts = new TimeSpan(23, 59, 59);
            var sqlSales = $"select *from sales WITH (NOLOCK) where RUC = @ruc and IssuedOn between @dateStart and @dateEnd";
            if (dateStart == null)
            {
                dateStart = DateTime.Now;
            }
            if (dateEnd == null)
            {
                dateEnd = DateTime.Now.AddMonths(1);
            }            
            dateEnd += ts;
            var paramSales = new List<SqlParameter> {
                new SqlParameter("@ruc", issuer),
                new SqlParameter("@dateStart", dateStart),
                new SqlParameter("@dateEnd", dateEnd)
            };
            if (!string.IsNullOrEmpty(search))
            {
                sqlSales += " and (ContributorRUC like @contributorRuc or UPPER(ContributorName) like @contributorName or DocumentNumber like @documentNumber or AuthorizationNumber like @authorizationNumber)";
                paramSales.Add(new SqlParameter("@contributorRuc", $"%{search}%"));
                paramSales.Add(new SqlParameter("@contributorName", $"%{search.ToUpper()}%"));
                paramSales.Add(new SqlParameter("@documentNumber", $"%{search}%"));
                paramSales.Add(new SqlParameter("@authorizationNumber", $"%{search}%"));
            }           
            sqlSales += " order by DocumentNumber";
            var sales = _sales.ExecSearchesWithStoreProcedure(sqlSales, paramSales.ToArray()).ToList();
            if (sales.Count > 0)
            {
                // reporte de ventas de la vista de VistaRetenciones del EcuafactApp
                dateEnd = dateEnd.Value.AddMonths(1);                
                var sqlRetention = $"select *from VistaRetenciones WITH (NOLOCK) where Retencion_RUC = @ruc and Retencion_FechaEmision between @dateStart and @dateEnd order by Retencion_NumeroReferencia";
                var _paramRetention = new[] {
                    new SqlParameter("@ruc", issuer), 
                    new SqlParameter("@dateStart", dateStart), 
                    new SqlParameter("@dateEnd", dateEnd) 
                };
                var retention = _viewRetentionRepository.ExecSearchesWithStoreProcedure(sqlRetention, _paramRetention).ToList();
                var saleSreport = (from s in sales
                                   join r in retention on s.DocumentNumber.Replace("-", "") equals r.Retencion_NumeroReferencia into salesretention
                                   from sr in salesretention.DefaultIfEmpty()
                                   select new SalesReport
                                   {
                                       Id = s.Id,
                                       IssuerId = s.IssuerId,
                                       IssuedOn = s.IssuedOn,
                                       DocumentTypeCode = s.DocumentTypeCode,
                                       DocumentType = s.DocumentType,
                                       EstablishmentCode = s.EstablishmentCode,
                                       IssuePointCode = s.IssuePointCode,
                                       Sequential = s.Sequential,
                                       DocumentNumber = s.DocumentNumber,
                                       AuthorizationNumber = s.AuthorizationNumber,
                                       AuthorizationDate = s.AuthorizationDate,
                                       ContributorRUC = s.ContributorRUC,
                                       ContributorName = s.ContributorName,
                                       Base0 = s.Base0,
                                       Base12 = s.Base12,
                                       IVA = s.IVA,
                                       Total = s.Total,
                                       Description = s.Description,
                                       Notes = s.Notes,
                                       RetentionId = sr?.IdRetencion,
                                       RetentionContributorRUC = sr?.Retencion_RUC,
                                       RetentionContributor = sr?.Retencion_Comprador,
                                       RetentionIssuedOn = sr?.Retencion_FechaEmision,
                                       RetentionNumber = sr?.Retencion_Numero,
                                       RetentionAuthorizationNumber = sr?.Retencion_Autorizacion,
                                       RetentionReferenceType = sr?.Retencion_CodigoReferencia,
                                       RetentionReferenceNumber = sr?.Retencion_NumeroReferencia,
                                       RetentionReferenceDate = sr?.Retencion_FechaReferencia,
                                       Retention104TaxCode = sr?.Retencion_Codigo104,
                                       RetentionTaxBase = sr?.Renta_BaseImponible,
                                       RetentionTaxRate = sr?.Renta_Porcentaje,
                                       RetentionTaxValue = sr?.Renta_Valor,
                                       RetentionVatCode = sr?.Retencion_CodigoIVA,
                                       RetentionVatBase = sr?.Retencion_BaseIVA,
                                       RetentionVatRate = sr?.Retencion_PorcentajeIVA,
                                       RetentionVatValue = sr?.Retencion_IVARetenido,
                                       RetentionISDCode = sr?.Retencion_CodigoISD,
                                       RetentionISDBase = sr?.Retencion_BaseISD,
                                       RetentionISDRate = sr?.Retencion_PorcentajeISD,
                                       RetentionISDValue = sr?.Retencion_ISDRetenido,
                                       RetentionReason = sr?.Retencion_Motivo,
                                       RetentionDescription = sr?.Retencion_Descripcion,
                                       RetentionNotes = sr?.Retencion_Observaciones,
                                       RetentionRUC = sr?.Retencion_RUCProveedor,
                                       RetentionBusinessName = sr?.Retencion_Proveedor,
                                       RetentionDate = sr?.Documento_FechaEmision,
                                       RUC = s.RUC,
                                       BussinesName = s.BussinesName,
                                       Payment = s.Payment
                                   }).ToList();

                return saleSreport;
            }

            return new List<SalesReport>();
        }

        #endregion

        #region Reporte de Compras 
        public IEnumerable<PurchaseReport> GetPurchaseReport(string issuer, DateTime? dateStart, DateTime? dateEnd, string search)
        {
            if (Constants.viewsSalesReport)
            {
                return GetViewsPurchaseReport(issuer, dateStart, dateEnd, search);
            }
            else
            {
                //return GetUnionViewsPurchaseRetention(issuer, dateStart, dateEnd, search);
                return GetPurchaseRetentionReport(issuer, dateStart, dateEnd, search);                
            }
        }

        private IEnumerable<PurchaseReport> GetViewsPurchaseReport(string issuer, DateTime? dateStart, DateTime? dateEnd, string search)
        {
            var report = _purchaseRepository.All.Where(model => model.RUC == issuer || model.RUC + "001" == issuer);

            if (dateStart != null)
            {
                report = report.Where(model => model.IssuedOn >= dateStart);
            }

            if (dateEnd != null)
            {
                report = report.Where(model => model.IssuedOn <= dateEnd);
            }

            if (!string.IsNullOrEmpty(search))
            {
                report = report.Where(model => model.ContributorRUC.Contains(search)
                    || model.ContributorName.ToUpper().Contains(search.ToUpper())
                    || model.DocumentNumber.Contains(search.ToUpper())
                    || model.AuthorizationNumber.Contains(search.ToUpper())
                );
            }

            report.LogQuery("Se realizo la consulta para el reporte de compras:", $"{DateTime.Now}",
                $"Issuer: {issuer}",
                $"DateStart: {dateStart}",
                $"DateEnd: {dateEnd}",
                $"Search: {search}"
            );

            return report.OrderBy(model => new { model.IssuedOn, model.DocumentNumber }).ToList();
        }

        private IEnumerable<PurchaseReport> GetUnionViewsPurchaseRetention(string issuer, DateTime? dateStart, DateTime? dateEnd, string search)
        {
            // reporte de compras de la vista de vwFacturas del EcuafactApp               
            var viewInvoice = _viewInvoiceRepository.FindBy(model => model.RUC == issuer || model.RUC == issuer.Substring(0, 10));
            if (dateStart != null)
            {
                viewInvoice = viewInvoice.AsNoTracking().Where(model => model.fechaEmision >= dateStart);
            }
            if (dateEnd != null)
            {
                viewInvoice = viewInvoice.AsNoTracking().Where(model => model.fechaEmision <= dateEnd);
            }

            viewInvoice.LogQuery("Se realizo la consulta para el reporte de ventas:", $"{DateTime.Now}",
                $"Issuer: {issuer}",
                $"DateStart: {dateStart}",
                $"DateEnd: {dateEnd}"
            );

            var invoices = viewInvoice.OrderBy(model => model.NumeroDocumento).ToList();
            if (invoices.Count > 0)
            {
                // reporte de las ventas de la vista de Purchases del Express
                var viewPurchase = _purchase.FindBy(model => model.RUC == issuer);

                if (dateStart != null)
                {
                    viewPurchase = viewPurchase.AsNoTracking().Where(model => model.RetentionDate >= dateStart);
                }

                if (dateEnd != null)
                {
                    viewPurchase = viewPurchase.AsNoTracking().Where(model => model.RetentionDate <= dateEnd);
                }

                viewPurchase.LogQuery("Se realizo la consulta para el reporte de retenciones:", $"{DateTime.Now}",
                    $"Issuer: {issuer}",
                    $"DateStart: {dateStart}",
                    $"DateEnd: {dateEnd}",
                    $"Search: {search}"
                );

                var purchases = viewPurchase.OrderBy(model => model.RetentionReferenceNumber).ToList();
                var saleSreport = new List<PurchaseReport>();
                invoices.ForEach(i => {
                    var retention = purchases.Where(r => r.RetentionReferenceDocumentAuth == i.numeroautorizacion && r.RetentionReferenceNumber == i.NumeroDocumento).FirstOrDefault();
                    saleSreport.Add(new PurchaseReport {
                        Id = i.Id,
                        IssuedOn = i.fechaEmision,
                        DocumentTypeCode = i.tipoDocumento,
                        DocumentType = i.documentoType,
                        EstablishmentCode = i.Establecimiento,
                        IssuePointCode = i.PuntoEmision,
                        Sequential = i.secuencial,
                        DocumentNumber = i.NumeroDocumento,
                        AuthorizationNumber = i.numeroautorizacion,
                        AuthorizationDate = i.fechaAutorizacion,
                        ContributorRUC = i.RUCProveedor,
                        ContributorName = i.RazonSocialProveedor,
                        Base0 = i.SubTotal0,
                        Base12 = i.SubTotal12,
                        IVA = i.IVA,
                        Total = i.Total,
                        Description = i.Description,
                        Notes = i.Notes,
                        RetentionId = retention?.RetentionId,
                        RetentionContributorRUC = retention?.RetentionContributorRUC,
                        RetentionContributor = retention?.RetentionContributor,
                        RetentionIssuedOn = retention?.RetentionIssuedOn,
                        RetentionNumber = retention?.RetentionNumber,
                        RetentionAuthorizationNumber = retention?.RetentionAuthorizationNumber,
                        RetentionReferenceType = retention?.RetentionReferenceType,
                        RetentionReferenceNumber = retention?.RetentionReferenceNumber,
                        RetentionReferenceDate = retention?.RetentionReferenceDate,
                        RetentionTaxBase = retention?.RetentionTaxBase,
                        RetentionTaxRate = retention?.RetentionTaxRate,
                        RetentionTaxValue = retention?.RetentionTaxValue,
                        RetentionVatCode = retention?.RetentionVatCode,
                        RetentionVatBase = retention?.RetentionVatBase,
                        RetentionVatRate = retention?.RetentionVatRate,
                        RetentionVatValue = retention?.RetentionVatValue,
                        RetentionISDCode = retention?.RetentionISDCode,
                        RetentionISDBase = retention?.RetentionISDBase,
                        RetentionISDRate = retention?.RetentionISDRate,
                        RetentionISDValue = retention?.RetentionISDValue,
                        RetentionReason = retention?.RetentionReason,
                        RetentionDescription = retention?.RetentionDescription,
                        RetentionNotes = retention?.RetentionNotes,
                        RetentionRUC = retention?.RetentionRUC,
                        RetentionBusinessName = retention?.RetentionBusinessName,
                        RetentionDate = retention?.RetentionDate,
                        RUC = i.RUC,
                        BussinesName = i.RazonSocial,
                        Deductible = i.Deducible ?? ""
                    });
                });

                //var purchases = viewPurchase.OrderBy(model => model.RetentionReferenceNumber).ToList();               
                //var saleSreport = (from i in invoices
                //                   join p in purchases on new {a = i.NumeroDocumento, b = i.numeroautorizacion } equals new {a = p.RetentionReferenceNumber, b = p.RetentionAuthorizationNumber } into purchaseretention
                //                   from ip in purchaseretention.DefaultIfEmpty()                                   
                //                   select new PurchaseReport
                //                   {
                //                       Id = i.Id,
                //                       IssuedOn = i.fechaEmision,
                //                       DocumentTypeCode = i.tipoDocumento,
                //                       DocumentType = i.documentoType,
                //                       EstablishmentCode = i.Establecimiento,
                //                       IssuePointCode = i.PuntoEmision,
                //                       Sequential = i.secuencial,
                //                       DocumentNumber = i.NumeroDocumento,
                //                       AuthorizationNumber = i.numeroautorizacion,
                //                       AuthorizationDate = i.fechaAutorizacion,
                //                       ContributorRUC = i.RUCProveedor,
                //                       ContributorName = i.RazonSocialProveedor,
                //                       Base0 = i.SubTotal0,
                //                       Base12 = i.SubTotal12,
                //                       IVA = i.IVA,
                //                       Total = i.Total,
                //                       Description = i.Description,
                //                       Notes = i.Notes,
                //                       RetentionId = retention?.RetentionId,
                //                       RetentionContributorRUC = retention?.RetentionContributorRUC,
                //                       RetentionContributor = retention?.RetentionContributor,
                //                       RetentionIssuedOn = retention?.RetentionIssuedOn,
                //                       RetentionNumber = retention?.RetentionNumber,
                //                       RetentionAuthorizationNumber = retention?.RetentionAuthorizationNumber,
                //                       RetentionReferenceType = retention?.RetentionReferenceType,
                //                       RetentionReferenceNumber = retention?.RetentionReferenceNumber,
                //                       RetentionReferenceDate = retention?.RetentionReferenceDate,
                //                       RetentionTaxBase = retention?.RetentionTaxBase,
                //                       RetentionTaxRate = retention?.RetentionTaxRate,
                //                       RetentionTaxValue = retention?.RetentionTaxValue,
                //                       RetentionVatCode = retention?.RetentionVatCode,
                //                       RetentionVatBase = retention?.RetentionVatBase,
                //                       RetentionVatRate = retention?.RetentionVatRate,
                //                       RetentionVatValue = retention?.RetentionVatValue,
                //                       RetentionISDCode = retention?.RetentionISDCode,
                //                       RetentionISDBase = retention?.RetentionISDBase,
                //                       RetentionISDRate = retention?.RetentionISDRate,
                //                       RetentionISDValue = retention?.RetentionISDValue,
                //                       RetentionReason = retention?.RetentionReason,
                //                       RetentionDescription = retention?.RetentionDescription,
                //                       RetentionNotes = retention?.RetentionNotes,
                //                       RetentionRUC = retention?.RetentionRUC,
                //                       RetentionBusinessName = retention?.RetentionBusinessName,
                //                       RetentionDate = retention?.RetentionDate,
                //                       RUC = i.RUC,
                //                       BussinesName = i.RazonSocial
                //                   }).ToList();

                return saleSreport;
            }

            return new List<PurchaseReport>();
        }

        private IEnumerable<PurchaseReport> GetPurchaseRetentionReport(string issuer, DateTime? dateStart, DateTime? dateEnd, string search)
        {
            // reporte de las ventas de la vista de Sales del Express
            TimeSpan ts = new TimeSpan(23, 59, 59);
            var sqlInvoices = $"select *from vwFacturas WITH (NOLOCK) where (RUC = @ruc or RUC = @ruc2) and fechaEmision between @dateStart and @dateEnd";
            if (dateStart == null)
            {
                dateStart = DateTime.Now;
            }
            if (dateEnd == null)
            {
                dateEnd = DateTime.Now.AddMonths(1);
            }            
            dateEnd += ts;            
            var paramSales = new List<SqlParameter> {
                new SqlParameter("@ruc", issuer),
                new SqlParameter("@ruc2", issuer.Substring(0, 10)),
                new SqlParameter("@dateStart", dateStart),
                new SqlParameter("@dateEnd", dateEnd)
            };
            if (!string.IsNullOrEmpty(search))
            {
                sqlInvoices += " and (RUCProveedor like @proveedorRuc or UPPER(RazonSocialProveedor) like @proveedorName or NumeroDocumento like @documentNumber or numeroautorizacion like @authorizationNumber)";
                paramSales.Add(new SqlParameter("@proveedorRuc", $"%{search}%"));
                paramSales.Add(new SqlParameter("@proveedorName", $"%{search.ToUpper()}%"));
                paramSales.Add(new SqlParameter("@documentNumber", $"%{search}%"));
                paramSales.Add(new SqlParameter("@authorizationNumber", $"%{search}%"));
            }
            sqlInvoices += " order by NumeroDocumento"; 
            var invoices = _viewInvoiceRepository.ExecSearchesWithStoreProcedure(sqlInvoices, paramSales.ToArray()).ToList();
            if (invoices.Count > 0)
            {
                // reporte de ventas de la vista de VistaRetenciones del EcuafactApp
                dateEnd = dateEnd.Value.AddMonths(1);
                var sqlPurchases = $"select *from Purchases WITH (NOLOCK) where RUC = @ruc and RetentionDate between @dateStart and @dateEnd order by RetentionReferenceNumber";
                var _paramPurchases = new[] {
                    new SqlParameter("@ruc", issuer),
                    new SqlParameter("@dateStart", dateStart),
                    new SqlParameter("@dateEnd", dateEnd)
                };
                var purchases = _purchase.ExecSearchesWithStoreProcedure(sqlPurchases, _paramPurchases).ToList();
                var saleSreport = new List<PurchaseReport>();
                invoices.ForEach(i => {
                    var retention = purchases.Where(r => r.RetentionReferenceDocumentAuth == i.numeroautorizacion && r.RetentionReferenceNumber == i.NumeroDocumento).FirstOrDefault();
                    saleSreport.Add(new PurchaseReport
                    {
                        Id = i.Id,
                        IssuedOn = i.fechaEmision,
                        DocumentTypeCode = i.tipoDocumento,
                        DocumentType = i.documentoType,
                        EstablishmentCode = i.Establecimiento,
                        IssuePointCode = i.PuntoEmision,
                        Sequential = i.secuencial,
                        DocumentNumber = i.NumeroDocumento,
                        AuthorizationNumber = i.numeroautorizacion,
                        AuthorizationDate = i.fechaAutorizacion,
                        ContributorRUC = i.RUCProveedor,
                        ContributorName = i.RazonSocialProveedor,
                        Base0 = i.SubTotal0,
                        Base12 = i.SubTotal12,
                        IVA = i.IVA,
                        Total = i.Total,
                        Description = i.Description,
                        Notes = i.Notes,
                        RetentionId = retention?.RetentionId,
                        RetentionContributorRUC = retention?.RetentionContributorRUC,
                        RetentionContributor = retention?.RetentionContributor,
                        RetentionIssuedOn = retention?.RetentionIssuedOn,
                        RetentionNumber = retention?.RetentionNumber,
                        RetentionAuthorizationNumber = retention?.RetentionAuthorizationNumber,
                        RetentionReferenceType = retention?.RetentionReferenceType,
                        RetentionReferenceNumber = retention?.RetentionReferenceNumber,
                        RetentionReferenceDate = retention?.RetentionReferenceDate,
                        RetentionTaxBase = retention?.RetentionTaxBase,
                        RetentionTaxRate = retention?.RetentionTaxRate,
                        RetentionTaxValue = retention?.RetentionTaxValue,
                        RetentionVatCode = retention?.RetentionVatCode,
                        RetentionVatBase = retention?.RetentionVatBase,
                        RetentionVatRate = retention?.RetentionVatRate,
                        RetentionVatValue = retention?.RetentionVatValue,
                        RetentionISDCode = retention?.RetentionISDCode,
                        RetentionISDBase = retention?.RetentionISDBase,
                        RetentionISDRate = retention?.RetentionISDRate,
                        RetentionISDValue = retention?.RetentionISDValue,
                        RetentionReason = retention?.RetentionReason,
                        RetentionDescription = retention?.RetentionDescription,
                        RetentionNotes = retention?.RetentionNotes,
                        RetentionRUC = retention?.RetentionRUC,
                        RetentionBusinessName = retention?.RetentionBusinessName,
                        RetentionDate = retention?.RetentionDate,
                        RUC = i.RUC,
                        BussinesName = i.RazonSocial,
                        Deductible = i.Deducible ?? ""
                    });
                });

                return saleSreport;
            }

            return new List<PurchaseReport>();
        }
        #endregion
    }
}
