using Ecuafact.Web.Domain.Entities;
using Microsoft.Reporting.WebForms;
using System.Collections.Generic;
using System.Data;

namespace Ecuafact.Web.Reporting
{
    public class SettlementReport : ReportBase<SettlementModel>
    {
        public SettlementReport(IssuerDto issuer, string path)
            : base(issuer, path)
        {
        }
        protected override string ReportName => "RptFactura";
         
        protected override List<ReportDataSource> GetDataSources(SettlementModel document)
        {
            var dataSet = GetDataSet(document);
             
            return new List<ReportDataSource> {
                new ReportDataSource("DsFactura", dataSet.Tables["DsFactura"]),
                new ReportDataSource("DsDetalleFactura", dataSet.Tables["DsDetalleFactura"]),
                new ReportDataSource("DsFacturaAdicionales", dataSet.Tables["DsFacturaAdicionales"]),
                new ReportDataSource("DsFormaPago", dataSet.Tables["DsFormaPago"]),
                new ReportDataSource("DsLogo", dataSet.Tables["DsLogo"])
            };
        }
         
        public DataSet GetDataSet(SettlementModel model)
        {
            var ds = new DataSet();
            var dsFactura = ds.Tables.Add("DsFactura");
            var dsDetalleFactura = ds.Tables.Add("DsDetalleFactura");
            var dsFacturaAdicionales = ds.Tables.Add("DsFacturaAdicionales");
            var dsFormaPago = ds.Tables.Add("DsFormaPago");
            var dsLogo = ds.Tables.Add("DsLogo");

            dsFactura.Columns.Add("IdFactura", typeof(System.Int64));
            dsFactura.Columns.Add("IdProveedor", typeof(System.Int64));
            dsFactura.Columns.Add("Ambiente", typeof(System.String));
            dsFactura.Columns.Add("TipoEmision", typeof(System.String));
            dsFactura.Columns.Add("RazonSocial", typeof(System.String));
            dsFactura.Columns.Add("NombreComercial", typeof(System.String));
            dsFactura.Columns.Add("Ruc", typeof(System.String));

            dsFactura.Columns.Add("ClaveAcceso", typeof(System.String));
            dsFactura.Columns.Add("CodDoc", typeof(System.String));
            dsFactura.Columns.Add("Estab", typeof(System.String));
            dsFactura.Columns.Add("PtoEmi", typeof(System.String));
            dsFactura.Columns.Add("Secuencial", typeof(System.String));
            dsFactura.Columns.Add("DirMatriz", typeof(System.String));

            dsFactura.Columns.Add("IdCliente", typeof(System.Int64));
            dsFactura.Columns.Add("FechaEmision", typeof(System.String));
            dsFactura.Columns.Add("FechaAutorizacion", typeof(System.String));
            dsFactura.Columns.Add("DirEstablecimiento", typeof(System.String));
            dsFactura.Columns.Add("ContribuyenteEspecial", typeof(System.String));
            dsFactura.Columns.Add("ObligadoContabilidad", typeof(System.String));

            dsFactura.Columns.Add("TipoIdentificacionComprador", typeof(System.String));
            dsFactura.Columns.Add("GuiaRemision", typeof(System.String));
            dsFactura.Columns.Add("RazonSocialComprador", typeof(System.String));
            dsFactura.Columns.Add("IdentificacionComprador", typeof(System.String));

            dsFactura.Columns.Add("TotalSinImpuesto", typeof(System.Decimal));
            dsFactura.Columns.Add("Propina", typeof(System.Decimal));
            dsFactura.Columns.Add("ImporteTotal", typeof(System.Decimal));
            dsFactura.Columns.Add("Moneda", typeof(System.String));
            dsFactura.Columns.Add("Estado", typeof(System.Int16));

            dsFactura.Columns.Add("Subtotal_12", typeof(System.Decimal));
            dsFactura.Columns.Add("Subtotal_0", typeof(System.Decimal));
            dsFactura.Columns.Add("Subtotal_No_Iva", typeof(System.Decimal));
            dsFactura.Columns.Add("Subtotal_Exento_IVA", typeof(System.Decimal));

            dsFactura.Columns.Add("Subtotal", typeof(System.Decimal));
            dsFactura.Columns.Add("TotalDescuento", typeof(System.Decimal));
            dsFactura.Columns.Add("Ice", typeof(System.Decimal));
            dsFactura.Columns.Add("Iva", typeof(System.Decimal));

            dsFactura.Columns.Add("ValorTotal", typeof(System.Decimal));
            dsFactura.Columns.Add("Autorizacion", typeof(System.String));
            dsFactura.Columns.Add("Compensacion", typeof(System.Decimal));

            dsFactura.Rows.Add(model.Id, 12, Issuer.EnvironmentType.GetValorCore(), Issuer.IssueType.GetValorCore(), Issuer.BussinesName.ToUpper(), Issuer.TradeName.ToUpper(), Issuer.RUC,
                model.AccessKey, model.DocumentTypeCode, model.EstablishmentCode, model.IssuePointCode, model.Sequential, Issuer.MainAddress,
                model.ContributorId, model.IssuedOn.ToString("dd/MM/yyyy"), model.AuthorizationDate, Issuer.MainAddress,
                Issuer.IsSpecialContributor ? Issuer.ResolutionNumber : "", Issuer.IsAccountingRequired ? "SI" : "NO",
                model.ContributorIdentificationType, model.SettlementInfo.ReferralGuide, model.ContributorName, model.ContributorIdentification,
                model.SettlementInfo.Subtotal, model.SettlementInfo.Tip, model.SettlementInfo.Total, model.Currency, model.Status,
                model.SettlementInfo.SubtotalVat, model.SettlementInfo.SubtotalVatZero, model.SettlementInfo.SubtotalNotSubject, model.SettlementInfo.SubtotalExempt,
                model.SettlementInfo.Subtotal, model.SettlementInfo.TotalDiscount, model.SettlementInfo.SpecialConsumTax, model.SettlementInfo.ValueAddedTax,
                model.SettlementInfo.Total, model.AuthorizationNumber, 0M);


            dsDetalleFactura.Columns.Add("IdFactura", typeof(System.Int64));
            dsDetalleFactura.Columns.Add("IdDetalleFactura", typeof(System.Int64));
            dsDetalleFactura.Columns.Add("CodigoPrincipal", typeof(System.String));
            dsDetalleFactura.Columns.Add("CodigoAuxiliar", typeof(System.String));
            dsDetalleFactura.Columns.Add("Descripcion", typeof(System.String));
            dsDetalleFactura.Columns.Add("Cantidad", typeof(System.Decimal));
            dsDetalleFactura.Columns.Add("Descuento", typeof(System.Decimal));
            dsDetalleFactura.Columns.Add("PrecioUnitario", typeof(System.Decimal));
            dsDetalleFactura.Columns.Add("PrecioTotalSinImpuesto", typeof(System.Decimal));
            dsDetalleFactura.Columns.Add("DetAdicionalN1", typeof(System.String));
            dsDetalleFactura.Columns.Add("DetAdicionalV1", typeof(System.String));
            dsDetalleFactura.Columns.Add("DetAdicionalN2", typeof(System.String));
            dsDetalleFactura.Columns.Add("DetAdicionalV2", typeof(System.String));
            dsDetalleFactura.Columns.Add("DetAdicionalN3", typeof(System.String));
            dsDetalleFactura.Columns.Add("DetAdicionalV3", typeof(System.String));

            var i = 0;
            foreach (var detail in model.SettlementInfo.Details)
            {
                dsDetalleFactura.Rows.Add(model.Id, i, detail.MainCode, detail.AuxCode,
                    detail.Description, detail.Amount, detail.Discount, detail.UnitPrice, detail.SubTotal,
                    detail.Name1, detail.Value1, detail.Name2, detail.Value2, detail.Name3, detail.Value3);

                i++;
            }


            dsFacturaAdicionales.Columns.Add("IdFacturaAdicional", typeof(System.Int64));
            dsFacturaAdicionales.Columns.Add("Nombre", typeof(System.String));
            dsFacturaAdicionales.Columns.Add("Valor", typeof(System.String));
            dsFacturaAdicionales.Columns.Add("NumLinea", typeof(System.Int32));

            var a = 0;
            foreach (var item in model.AdditionalFields)
            {
                dsFacturaAdicionales.Rows.Add(a, item.Name, item.Value, item.LineNumber);
                a++;
            }


            dsFormaPago.Columns.Add("Id", typeof(System.Int64));
            dsFormaPago.Columns.Add("formaPago", typeof(System.String));
            dsFormaPago.Columns.Add("total", typeof(System.Decimal));
            dsFormaPago.Columns.Add("plazo", typeof(System.Int32));
            dsFormaPago.Columns.Add("unidadTiempo", typeof(System.String));

            var f = 0;
            foreach (var pay in model.SettlementInfo.Payments)
            {
                dsFormaPago.Rows.Add(f, pay.Name, pay.Total, pay.Term, pay.TimeUnit);
                f++;
            }


            dsLogo.Columns.Add("Ruc", typeof(System.String));
            dsLogo.Columns.Add("Logo", typeof(System.Byte[]));

            
            dsLogo.Rows.Add(model.RUC, GetIssuerLogo());



            return ds;
        }
    }
}
