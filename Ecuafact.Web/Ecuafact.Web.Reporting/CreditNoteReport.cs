using Ecuafact.Web.Domain.Entities;
using Microsoft.Reporting.WebForms;
using System.Collections.Generic;
using System.Data;

namespace Ecuafact.Web.Reporting
{
    public class CreditNoteReport : ReportBase<CreditNoteModel>
    {
        public CreditNoteReport( IssuerDto issuer, string path)
            : base(issuer, path)
        {
        }

        protected override string ReportName => "RptNotaCredito";

        protected override List<ReportDataSource> GetDataSources(CreditNoteModel document)
        {

            var dataSet = GetDataSet(document);
            var dataSources = new List<ReportDataSource>
            {
                new ReportDataSource("DsNotaCredito", dataSet.Tables["DsNotaCredito"]),
                new ReportDataSource("DsNotaCreditoDetalle", dataSet.Tables["DsNotaCreditoDetalle"]),
                new ReportDataSource("DsNotaCreditoAdicionales", dataSet.Tables["DsNotaCreditoAdicionales"]),
                new ReportDataSource("DsFormaPago", dataSet.Tables["DsFormaPago"]),
                new ReportDataSource("DsLogo", dataSet.Tables["DsLogo"])
            };

            return dataSources;
        } 

        private DataSet GetDataSet(CreditNoteModel model)
        {
            var ds = new DataSet();
            var dsNotaCredito = ds.Tables.Add("DsNotaCredito");
            var dsNotaCreditoDetalle = ds.Tables.Add("DsNotaCreditoDetalle");
            var dsNotaCreditoAdicionales = ds.Tables.Add("DsNotaCreditoAdicionales");
            var dsFormaPago = ds.Tables.Add("DsFormaPago");
            var dsLogo = ds.Tables.Add("DsLogo");

            dsNotaCredito.Columns.Add("IdNotaCredito", typeof(System.Int64));
            dsNotaCredito.Columns.Add("IdProveedor", typeof(System.Int64));
            dsNotaCredito.Columns.Add("Ambiente", typeof(System.String));
            dsNotaCredito.Columns.Add("TipoEmision", typeof(System.String));
            dsNotaCredito.Columns.Add("RazonSocial", typeof(System.String));
            dsNotaCredito.Columns.Add("NombreComercial", typeof(System.String));
            dsNotaCredito.Columns.Add("Ruc", typeof(System.String));
            dsNotaCredito.Columns.Add("ClaveAcceso", typeof(System.String));
            dsNotaCredito.Columns.Add("CodDoc", typeof(System.String));
            dsNotaCredito.Columns.Add("Estab", typeof(System.String));
            dsNotaCredito.Columns.Add("PtoEmi", typeof(System.String));
            dsNotaCredito.Columns.Add("Secuencial", typeof(System.String));
            dsNotaCredito.Columns.Add("DirMatriz", typeof(System.String));
            dsNotaCredito.Columns.Add("IdCliente", typeof(System.Int64));
            dsNotaCredito.Columns.Add("FechaEmision", typeof(System.String));
            dsNotaCredito.Columns.Add("FechaAutorizacion", typeof(System.String));
            dsNotaCredito.Columns.Add("DirEstablecimiento", typeof(System.String));
            dsNotaCredito.Columns.Add("ContribuyenteEspecial", typeof(System.String));
            dsNotaCredito.Columns.Add("ObligadoContabilidad", typeof(System.String));
            dsNotaCredito.Columns.Add("TipoIdentificacionComprador", typeof(System.String));
            dsNotaCredito.Columns.Add("RazonSocialComprador", typeof(System.String));
            dsNotaCredito.Columns.Add("IdentificacionComprador", typeof(System.String));
            dsNotaCredito.Columns.Add("TotalSinImpuestos", typeof(System.Decimal));

            dsNotaCredito.Columns.Add("CodDocModificado", typeof(System.String));
            dsNotaCredito.Columns.Add("NumDocModificado", typeof(System.String));
            dsNotaCredito.Columns.Add("FechaEmisionDocSustento", typeof(System.String));

            dsNotaCredito.Columns.Add("Propina", typeof(System.Decimal));
            dsNotaCredito.Columns.Add("ImporteTotal", typeof(System.Decimal));
            dsNotaCredito.Columns.Add("Moneda", typeof(System.String));
            dsNotaCredito.Columns.Add("Estado", typeof(System.Int32));
            dsNotaCredito.Columns.Add("Subtotal_12", typeof(System.Decimal));
            dsNotaCredito.Columns.Add("Subtotal_0", typeof(System.Decimal));
            dsNotaCredito.Columns.Add("Subtotal_No_Iva", typeof(System.Decimal));
            dsNotaCredito.Columns.Add("Subtotal_Exento_IVA", typeof(System.Decimal));
            dsNotaCredito.Columns.Add("Subtotal", typeof(System.Decimal));
            dsNotaCredito.Columns.Add("TotalDescuento", typeof(System.Decimal));
            dsNotaCredito.Columns.Add("Ice", typeof(System.Decimal));
            dsNotaCredito.Columns.Add("Iva", typeof(System.Decimal));
            dsNotaCredito.Columns.Add("ValorTotal", typeof(System.Decimal));
            dsNotaCredito.Columns.Add("ValorModificacion", typeof(System.Decimal));
            dsNotaCredito.Columns.Add("Autorizacion", typeof(System.String));
            dsNotaCredito.Columns.Add("Compensacion", typeof(System.Decimal));
            dsNotaCredito.Columns.Add("MOtivo", typeof(System.String));
            dsNotaCredito.Columns.Add("GuiaRemision", typeof(System.String));



            dsNotaCredito.Rows.Add(model.Id, 12, Issuer.EnvironmentType.GetValorCore(), Issuer.IssueType.GetValorCore(), Issuer.BussinesName.ToUpper(), Issuer.TradeName.ToUpper(), Issuer.RUC,
                model.AccessKey, model.DocumentTypeCode, model.EstablishmentCode, model.IssuePointCode, model.Sequential, Issuer.MainAddress,
                model.ContributorId, model.IssuedOn.ToString("dd/MM/yyyy"), model.AuthorizationDate, Issuer.MainAddress,
                Issuer.IsSpecialContributor ? Issuer.ResolutionNumber : "", Issuer.IsAccountingRequired ? "SI" : "NO",
                model.ContributorIdentificationType, model.ContributorName.ToUpper(), model.ContributorIdentification,
                model.CreditNoteInfo.Subtotal, model.CreditNoteInfo.ReferenceDocumentCode, model.CreditNoteInfo.ReferenceDocumentNumber, model.CreditNoteInfo.ReferenceDocumentDate,
                
                model.CreditNoteInfo.Tip, model.CreditNoteInfo.Total, model.Currency, model.Status,
                model.CreditNoteInfo.SubtotalVat, model.CreditNoteInfo.SubtotalVatZero, model.CreditNoteInfo.SubtotalNotSubject, model.CreditNoteInfo.SubtotalExempt,
                model.CreditNoteInfo.Subtotal, model.CreditNoteInfo.TotalDiscount, model.CreditNoteInfo.SpecialConsumTax, model.CreditNoteInfo.ValueAddedTax,
                model.CreditNoteInfo.Total, model.CreditNoteInfo.ModifiedValue,  model.AuthorizationNumber, 0M, model.CreditNoteInfo.Reason, "");


            dsNotaCreditoDetalle.Columns.Add("IdNotaCredito", typeof(System.Int64));
            dsNotaCreditoDetalle.Columns.Add("IdNotaCreditoDetalle", typeof(System.Int64));
            dsNotaCreditoDetalle.Columns.Add("CodigoInterno", typeof(System.String));
            dsNotaCreditoDetalle.Columns.Add("CodigoAdicional", typeof(System.String));
            dsNotaCreditoDetalle.Columns.Add("Descripcion", typeof(System.String));
            dsNotaCreditoDetalle.Columns.Add("Cantidad", typeof(System.Decimal));
            dsNotaCreditoDetalle.Columns.Add("Descuento", typeof(System.Decimal));
            dsNotaCreditoDetalle.Columns.Add("PrecioUnitario", typeof(System.Decimal));
            dsNotaCreditoDetalle.Columns.Add("PrecioTotalSinImpuesto", typeof(System.Decimal));
            dsNotaCreditoDetalle.Columns.Add("DetAdicionalN1", typeof(System.String));
            dsNotaCreditoDetalle.Columns.Add("DetAdicionalV1", typeof(System.String));
            dsNotaCreditoDetalle.Columns.Add("DetAdicionalN2", typeof(System.String));
            dsNotaCreditoDetalle.Columns.Add("DetAdicionalV2", typeof(System.String));
            dsNotaCreditoDetalle.Columns.Add("DetAdicionalN3", typeof(System.String));
            dsNotaCreditoDetalle.Columns.Add("DetAdicionalV3", typeof(System.String));

            var i = 0;
            foreach (var detail in model.CreditNoteInfo.Details)
            {
                dsNotaCreditoDetalle.Rows.Add(model.Id, i, detail.MainCode, detail.AuxCode,
                    detail.Description, detail.Amount, detail.Discount, detail.UnitPrice, detail.SubTotal,
                    detail.Name1, detail.Value1, detail.Name2, detail.Value2, detail.Name3, detail.Value3);

                i++;
            }

            dsFormaPago.Columns.Add("Id", typeof(System.Int64));
            dsFormaPago.Columns.Add("formaPago", typeof(System.String));
            dsFormaPago.Columns.Add("total", typeof(System.Decimal));
            dsFormaPago.Columns.Add("plazo", typeof(System.String));
            dsFormaPago.Columns.Add("unidadTiempo", typeof(System.String));


            dsNotaCreditoAdicionales.Columns.Add("IdNotaCreditoAdicional", typeof(System.Int64));
            dsNotaCreditoAdicionales.Columns.Add("Nombre", typeof(System.String));
            dsNotaCreditoAdicionales.Columns.Add("Valor", typeof(System.String));
            dsNotaCreditoAdicionales.Columns.Add("NumLinea", typeof(System.Int32));
             

            var a = 0;
            foreach (var item in model.AdditionalFields)
            {
                dsNotaCreditoAdicionales.Rows.Add(a, item.Name, item.Value, item.LineNumber);
                a++;
            }


            dsLogo.Columns.Add("Ruc", typeof(System.String));
            dsLogo.Columns.Add("Logo", typeof(System.Byte[]));
             
            dsLogo.Rows.Add(model.RUC, GetIssuerLogo());



            return ds;
        }
    }
} 