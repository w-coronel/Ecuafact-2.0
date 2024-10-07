using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Entities.Engine;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Reporting
{
    public class InvoicePosReport : ReportBase<Document>
    {
        public Issuer Issuer { get; set; }
        public string TypeSize { get; set; }
        public InvoicePosReport(Issuer issuer, string reportType)
        {
            Issuer = issuer;
            TypeSize = reportType.Contains("POS72") ? "RptFacturaPos72" : "RptFacturaPos55";
        }

        protected override string ReportName => TypeSize;

        protected override List<ReportDataSource> GetDataSources(Document document)
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

        public DataSet GetDataSet(Document model)
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

            dsFactura.Columns.Add("NegocioPopular", typeof(System.String));
            dsFactura.Columns.Add("RegimenGeneral", typeof(System.String));
            dsFactura.Columns.Add("SociedadesSimplificadas", typeof(System.String));
            dsFactura.Columns.Add("ArtesanoCalificado", typeof(System.String));
            dsFactura.Columns.Add("AgenteRetencion", typeof(System.String));
            dsFactura.Columns.Add("MicroEmpresa", typeof(System.String));

            dsFactura.Columns.Add("ValorIva", typeof(System.String));
            dsFactura.Columns.Add("ValorIva2", typeof(System.String));
            dsFactura.Columns.Add("Iva2", typeof(System.Decimal));
            dsFactura.Columns.Add("Subtotal_Iva2", typeof(System.Decimal));

            var ValorIva2 = "";
            decimal Iva2 = 0;
            decimal Subtotal_Iva2 = 0;
            var taxRates = model.InvoiceInfo.TotalTaxes.ToList().Where(s => s.TaxRate != 0).ToList();
            var valorIva = "15";
            if (taxRates.Count > 1)
            {
                model.InvoiceInfo.SubtotalVat = taxRates[0].TaxableBase;
                valorIva = $"{Convert.ToInt32(taxRates[0].TaxRate)}";
                model.InvoiceInfo.ValueAddedTax = taxRates[0].TaxValue;
                ValorIva2 = $"{Convert.ToInt32(taxRates[1].TaxRate)}%";
                Iva2 = taxRates[1].TaxValue;
                Subtotal_Iva2 = taxRates[1].TaxableBase;
            }
            else
            {
                valorIva = taxRates.Count > 0 ? $"{Convert.ToInt32(taxRates[0].TaxRate)}" : valorIva;
            }

            string NegocioPopular = null;
            string RegimenGeneral = null;
            string SociedadesSimplificadas = null;
            string ArtesanoCalificado = null;
            string AgenteRetencion = Issuer.AgentResolutionNumber;
            string MicroEmpresa = null;



            if (Issuer.IsGeneralRegime)
            {
                RegimenGeneral = TextoInfoAdicionalEnum.IsGeneralRegime.GetCoreValue();
            }
            if (Issuer.IsPopularBusiness)
            {
                NegocioPopular = TextoInfoAdicionalEnum.IsPopularBusiness.GetCoreValue();
            }           
            if (Issuer.IsSimplifiedCompaniesRegime)
            {
                SociedadesSimplificadas = TextoInfoAdicionalEnum.IsSimplifiedCompaniesRegime.GetCoreValue();
            }
            if (Issuer.IsSkilledCraftsman)
            {
                ArtesanoCalificado = Issuer.SkilledCraftsmanNumber;
            }



            dsFactura.Rows.Add(model.Id, 12, Issuer.EnvironmentType.GetCoreValue(), Issuer.IssueType.GetCoreValue(), Issuer.BussinesName.ToUpper(), Issuer.TradeName.ToUpper(),
                Issuer.RUC, model.AccessKey, model.DocumentTypeCode, model.EstablishmentCode, model.IssuePointCode, model.Sequential, model.MainAddress ?? Issuer.MainAddress,
                model.ContributorId, model.IssuedOn.ToString("dd/MM/yyyy"), model.AuthorizationDate, model.EstablishmentAddress ?? model.MainAddress,
                Issuer.IsSpecialContributor ? Issuer.ResolutionNumber : "", Issuer.IsAccountingRequired ? "SI" : "NO", model.ContributorIdentificationType, model.InvoiceInfo.ReferralGuide,
                model.ContributorName, model.ContributorIdentification, model.InvoiceInfo.Subtotal, model.InvoiceInfo.Tip, model.InvoiceInfo.Total, model.Currency,
                model.Status, model.InvoiceInfo.SubtotalVat, model.InvoiceInfo.SubtotalVatZero, model.InvoiceInfo.SubtotalNotSubject, model.InvoiceInfo.SubtotalExempt,
                model.InvoiceInfo.Subtotal, model.InvoiceInfo.TotalDiscount, model.InvoiceInfo.SpecialConsumTax, model.InvoiceInfo.ValueAddedTax,
                model.InvoiceInfo.Total, model.AuthorizationNumber, NegocioPopular, RegimenGeneral, SociedadesSimplificadas, ArtesanoCalificado, AgenteRetencion, MicroEmpresa,
                $"{valorIva}%", ValorIva2, Iva2, Subtotal_Iva2);


           
            dsDetalleFactura.Columns.Add("IdDetalleFactura", typeof(System.Int64));            
            dsDetalleFactura.Columns.Add("Cantidad", typeof(System.Decimal));
            dsDetalleFactura.Columns.Add("Descuento", typeof(System.Decimal));
            dsDetalleFactura.Columns.Add("IdFactura", typeof(System.Int64));
            dsDetalleFactura.Columns.Add("PrecioUnitario", typeof(System.Decimal));
            dsDetalleFactura.Columns.Add("Descripcion", typeof(System.String));
            dsDetalleFactura.Columns.Add("CodigoPrincipal", typeof(System.String));
            dsDetalleFactura.Columns.Add("CodigoAuxiliar", typeof(System.String));            
            dsDetalleFactura.Columns.Add("PrecioTotalSinImpuesto", typeof(System.Decimal));
            dsDetalleFactura.Columns.Add("DetAdicionalN1", typeof(System.String));
            dsDetalleFactura.Columns.Add("DetAdicionalV1", typeof(System.String));
            dsDetalleFactura.Columns.Add("DetAdicionalN2", typeof(System.String));
            dsDetalleFactura.Columns.Add("DetAdicionalV2", typeof(System.String));
            dsDetalleFactura.Columns.Add("DetAdicionalN3", typeof(System.String));
            dsDetalleFactura.Columns.Add("DetAdicionalV3", typeof(System.String));

            var i = 0;
            foreach (var detail in model.InvoiceInfo.Details)
            {
                decimal valdesc = 0;
                if (detail.Discount > 0)
                {
                    valdesc = Math.Round((detail.Amount * detail.UnitPrice) * (detail.Discount / 100), 2);
                }
                dsDetalleFactura.Rows.Add(model.Id, detail.Amount, detail.Discount, i, detail.UnitPrice,
                    detail.Description, detail.MainCode, detail.AuxCode, detail.SubTotal,
                    detail.Name1, detail.Value1, detail.Name2, detail.Value2, detail.Name3, detail.Value3);

                i++;
            }

            dsFacturaAdicionales.Columns.Add("IdFacturaAdicional", typeof(System.Int64));
            dsFacturaAdicionales.Columns.Add("Nombre", typeof(System.String));
            dsFacturaAdicionales.Columns.Add("Valor", typeof(System.String));
            dsFacturaAdicionales.Columns.Add("NumLinea", typeof(System.Int32));
            var a = 0;
            foreach (var item in model.AdditionalFields?.Where(s => s?.IsCarrier != 1))
            {
                if (item.Name != MySystemExtensions.AgenteRetencion)
                {
                    dsFacturaAdicionales.Rows.Add(a, item.Name, item.Value, item.LineNumber);
                    a++;
                }
            }

            dsFormaPago.Columns.Add("Id", typeof(System.Int64));
            dsFormaPago.Columns.Add("formaPago", typeof(System.String));
            dsFormaPago.Columns.Add("total", typeof(System.Decimal));
            dsFormaPago.Columns.Add("plazo", typeof(System.Int32));
            dsFormaPago.Columns.Add("unidadTiempo", typeof(System.String));

            var f = 0;
            foreach (var pay in model.InvoiceInfo.Payments)
            {
                dsFormaPago.Rows.Add(f, pay.Name, pay.Total, pay.Term, pay.TimeUnit);
                f++;
            }

            dsLogo.Columns.Add("Ruc", typeof(System.String));
            dsLogo.Columns.Add("Logo", typeof(System.Byte[]));
            //dsLogo.Columns.Add("ImgClaveAcceso", typeof(System.Byte[]));

            //dsLogo.Rows.Add(model.RUC, Issuer.GetLogo(), model.GeneraCodigoBarra(BarcodeLib.TYPE.CODE128));
            dsLogo.Rows.Add(model.RUC, Issuer.GetLogo());

            return ds;
        }
    }
}
