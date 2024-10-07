using Ecuafact.Web.Domain.Entities;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Reporting
{    
    public class DebitNoteReport : ReportBase<DebitNoteModel>
    {
        public DebitNoteReport(IssuerDto issuer, string path)
            : base(issuer, path)
        {
        }

        protected override string ReportName => "RptNotaDebito";

        protected override List<ReportDataSource> GetDataSources(DebitNoteModel document)
        {

            var dataSet = GetDataSet(document);
            var dataSources = new List<ReportDataSource>
            {
                new ReportDataSource("DsNotaDebito", dataSet.Tables["DsNotaDebito"]),
                new ReportDataSource("DsNotaDebitoDetalle", dataSet.Tables["DsNotaDebitoDetalle"]),
                new ReportDataSource("DsNotaDebitoAdicionales", dataSet.Tables["DsNotaDebitoAdicionales"]),
                new ReportDataSource("DsFormaPago", dataSet.Tables["DsFormaPago"]),
                new ReportDataSource("DsLogo", dataSet.Tables["DsLogo"])
            };

            return dataSources;
        }

        private DataSet GetDataSet(DebitNoteModel model)
        {
            var ds = new DataSet();
            var dsNotaDebito = ds.Tables.Add("DsNotaDebito");
            var dsNotaDebitoDetalle = ds.Tables.Add("DsNotaDebitoDetalle");
            var dsNotaDebitoAdicionales = ds.Tables.Add("DsNotaDebitoAdicionales");
            var dsFormaPago = ds.Tables.Add("DsFormaPago");
            var dsLogo = ds.Tables.Add("DsLogo");

            dsNotaDebito.Columns.Add("IdNotaDebito", typeof(System.Int64));
            dsNotaDebito.Columns.Add("IdProveedor", typeof(System.Int64));
            dsNotaDebito.Columns.Add("Ambiente", typeof(System.String));
            dsNotaDebito.Columns.Add("TipoEmision", typeof(System.String));
            dsNotaDebito.Columns.Add("RazonSocial", typeof(System.String));
            dsNotaDebito.Columns.Add("NombreComercial", typeof(System.String));
            dsNotaDebito.Columns.Add("Ruc", typeof(System.String));
            dsNotaDebito.Columns.Add("ClaveAcceso", typeof(System.String));
            dsNotaDebito.Columns.Add("CodDoc", typeof(System.String));
            dsNotaDebito.Columns.Add("Estab", typeof(System.String));
            dsNotaDebito.Columns.Add("PtoEmi", typeof(System.String));
            dsNotaDebito.Columns.Add("Secuencial", typeof(System.String));
            dsNotaDebito.Columns.Add("DirMatriz", typeof(System.String));
            dsNotaDebito.Columns.Add("IdCliente", typeof(System.Int64));
            dsNotaDebito.Columns.Add("FechaEmision", typeof(System.String));
            dsNotaDebito.Columns.Add("FechaAutorizacion", typeof(System.String));
            dsNotaDebito.Columns.Add("DirEstablecimiento", typeof(System.String));
            dsNotaDebito.Columns.Add("ContribuyenteEspecial", typeof(System.String));
            dsNotaDebito.Columns.Add("ObligadoContabilidad", typeof(System.String));
            dsNotaDebito.Columns.Add("TipoIdentificacionComprador", typeof(System.String));
            dsNotaDebito.Columns.Add("RazonSocialComprador", typeof(System.String));
            dsNotaDebito.Columns.Add("IdentificacionComprador", typeof(System.String));
            dsNotaDebito.Columns.Add("TotalSinImpuestos", typeof(System.Decimal));

            dsNotaDebito.Columns.Add("NumDocModificado", typeof(System.String));
            dsNotaDebito.Columns.Add("FechaEmisionDocSustento", typeof(System.String));
            dsNotaDebito.Columns.Add("CodDocModificado", typeof(System.String));

            dsNotaDebito.Columns.Add("Propina", typeof(System.Decimal));
            dsNotaDebito.Columns.Add("ImporteTotal", typeof(System.Decimal));
            dsNotaDebito.Columns.Add("Moneda", typeof(System.String));
            dsNotaDebito.Columns.Add("Estado", typeof(System.Int32));
            dsNotaDebito.Columns.Add("Subtotal_12", typeof(System.Decimal));
            dsNotaDebito.Columns.Add("Subtotal_0", typeof(System.Decimal));
            dsNotaDebito.Columns.Add("Subtotal_No_Iva", typeof(System.Decimal));
            dsNotaDebito.Columns.Add("Subtotal_Exento_IVA", typeof(System.Decimal));
            dsNotaDebito.Columns.Add("Subtotal", typeof(System.Decimal));
            dsNotaDebito.Columns.Add("TotalDescuento", typeof(System.Decimal));
            dsNotaDebito.Columns.Add("Ice", typeof(System.Decimal));
            dsNotaDebito.Columns.Add("Iva", typeof(System.Decimal));
            dsNotaDebito.Columns.Add("ValorTotal", typeof(System.Decimal));
            dsNotaDebito.Columns.Add("ValorModificacion", typeof(System.Decimal));
            dsNotaDebito.Columns.Add("Autorizacion", typeof(System.String));
            dsNotaDebito.Columns.Add("Compensacion", typeof(System.Decimal));
            dsNotaDebito.Columns.Add("Motivo", typeof(System.String));
            dsNotaDebito.Columns.Add("GuiaRemision", typeof(System.String));



            dsNotaDebito.Rows.Add(model.Id, 12, Issuer.EnvironmentType.GetValorCore(), Issuer.IssueType.GetValorCore(), Issuer.BussinesName.ToUpper(), Issuer.TradeName.ToUpper(), Issuer.RUC,
                model.AccessKey, model.DocumentTypeCode, model.EstablishmentCode, model.IssuePointCode, model.Sequential, Issuer.MainAddress,
                model.ContributorId, model.IssuedOn.ToString("dd/MM/yyyy"), model.AuthorizationDate, Issuer.MainAddress,
                Issuer.IsSpecialContributor ? Issuer.ResolutionNumber : "", Issuer.IsAccountingRequired ? "SI" : "NO",
                model.ContributorIdentificationType, model.ContributorName.ToUpper(), model.ContributorIdentification,
                model.DebitNoteInfo.Subtotal, model.DebitNoteInfo.ReferenceDocumentCode, model.DebitNoteInfo.ReferenceDocumentNumber, model.DebitNoteInfo.ReferenceDocumentDate,

                model.DebitNoteInfo.Tip, model.DebitNoteInfo.Total, model.Currency, model.Status,
                model.DebitNoteInfo.SubtotalVat, model.DebitNoteInfo.SubtotalVatZero, model.DebitNoteInfo.SubtotalNotSubject, model.DebitNoteInfo.SubtotalExempt,
                model.DebitNoteInfo.Subtotal, model.DebitNoteInfo.TotalDiscount, model.DebitNoteInfo.SpecialConsumTax, model.DebitNoteInfo.ValueAddedTax,
                model.DebitNoteInfo.Total, model.DebitNoteInfo.ModifiedValue, model.AuthorizationNumber, 0M, "", "");


            dsNotaDebitoDetalle.Columns.Add("IdNotaDebitoo", typeof(System.Int64));
            dsNotaDebitoDetalle.Columns.Add("IdNotaDebitoDetalle", typeof(System.Int64));
            dsNotaDebitoDetalle.Columns.Add("CodigoInterno", typeof(System.String));
            dsNotaDebitoDetalle.Columns.Add("CodigoAdicional", typeof(System.String));
            dsNotaDebitoDetalle.Columns.Add("Descripcion", typeof(System.String));
            dsNotaDebitoDetalle.Columns.Add("Cantidad", typeof(System.Decimal));
            dsNotaDebitoDetalle.Columns.Add("Descuento", typeof(System.Decimal));
            dsNotaDebitoDetalle.Columns.Add("PrecioUnitario", typeof(System.Decimal));
            dsNotaDebitoDetalle.Columns.Add("PrecioTotalSinImpuesto", typeof(System.Decimal));
            dsNotaDebitoDetalle.Columns.Add("DetAdicionalN1", typeof(System.String));
            dsNotaDebitoDetalle.Columns.Add("DetAdicionalV1", typeof(System.String));
            dsNotaDebitoDetalle.Columns.Add("DetAdicionalN2", typeof(System.String));
            dsNotaDebitoDetalle.Columns.Add("DetAdicionalV2", typeof(System.String));
            dsNotaDebitoDetalle.Columns.Add("DetAdicionalN3", typeof(System.String));
            dsNotaDebitoDetalle.Columns.Add("DetAdicionalV3", typeof(System.String));

            var i = 0;
            //foreach (var detail in model.DebitNoteInfo.Details)
            //{
            //    dsNotaDebitoDetalle.Rows.Add(model.Id, i, detail.MainCode, detail.AuxCode,
            //        detail.Description, detail.Amount, detail.Discount, detail.UnitPrice, detail.SubTotal,
            //        detail.Name1, detail.Value1, detail.Name2, detail.Value2, detail.Name3, detail.Value3);

            //    i++;
            //}

            dsFormaPago.Columns.Add("Id", typeof(System.Int64));
            dsFormaPago.Columns.Add("formaPago", typeof(System.String));
            dsFormaPago.Columns.Add("total", typeof(System.Decimal));
            dsFormaPago.Columns.Add("plazo", typeof(System.String));
            dsFormaPago.Columns.Add("unidadTiempo", typeof(System.String));


            dsNotaDebitoAdicionales.Columns.Add("IdNotaDebitoAdicional", typeof(System.Int64));
            dsNotaDebitoAdicionales.Columns.Add("Nombre", typeof(System.String));
            dsNotaDebitoAdicionales.Columns.Add("Valor", typeof(System.String));
            dsNotaDebitoAdicionales.Columns.Add("NumLinea", typeof(System.Int32));


            var a = 0;
            foreach (var item in model.AdditionalFields)
            {
                dsNotaDebitoAdicionales.Rows.Add(a, item.Name, item.Value, item.LineNumber);
                a++;
            }


            dsLogo.Columns.Add("Ruc", typeof(System.String));
            dsLogo.Columns.Add("Logo", typeof(System.Byte[]));
            dsLogo.Rows.Add(model.RUC, GetIssuerLogo());



            return ds;
        }
    }
}
