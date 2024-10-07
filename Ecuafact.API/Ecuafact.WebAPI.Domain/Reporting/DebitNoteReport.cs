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
    public class DebitNoteReport : ReportBase<Document>
    {
        public Issuer Issuer { get; set; }
        public DebitNoteReport(Issuer issuer)
        {
            Issuer = issuer;
        }

        protected override string ReportName => "RptNotaDebito";

        protected override List<ReportDataSource> GetDataSources(Document document)
        {
            var dataSet = GetDataSet(document);
            var dataSources = new List<ReportDataSource>
            {
                new ReportDataSource("DsNotaDebito", dataSet.Tables["DsNotaDebito"]),
                new ReportDataSource("DsNotaDebitoMotivo", dataSet.Tables["DsNotaDebitoMotivo"]),
                new ReportDataSource("DsNotaDebitoAdicional", dataSet.Tables["DsNotaDebitoAdicional"]),
                new ReportDataSource("DsFormaPago", dataSet.Tables["DsFormaPago"]),
                new ReportDataSource("DsLogo", dataSet.Tables["DsLogo"]),
                new ReportDataSource("DsInfomacionTributaria", dataSet.Tables["DsInfomacionTributaria"])
            };

            return dataSources;
        }

        private DataSet GetDataSet(Document model)
        {
            var ds = new DataSet();
            var dsNotaDebito = ds.Tables.Add("DsNotaDebito");
            var dsNotaDebitoMotivo = ds.Tables.Add("DsNotaDebitoMotivo");
            var dsNotaDebitoAdicionales = ds.Tables.Add("DsNotaDebitoAdicional");
            var dsFormaPago = ds.Tables.Add("DsFormaPago");
            var dsLogo = ds.Tables.Add("DsLogo");
            var dsInfomacionTributaria = ds.Tables.Add("DsInfomacionTributaria");

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

            dsNotaDebito.Columns.Add("CodDocModificado", typeof(System.String));
            dsNotaDebito.Columns.Add("NumDocModificado", typeof(System.String));
            dsNotaDebito.Columns.Add("FechaEmisionDocSustento", typeof(System.String));

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
            dsNotaDebito.Columns.Add("MOtivo", typeof(System.String));
            dsNotaDebito.Columns.Add("GuiaRemision", typeof(System.String));

            dsNotaDebito.Columns.Add("MicroEmpresa", typeof(System.String));
            dsNotaDebito.Columns.Add("AgenteRetencion", typeof(System.String));
            dsNotaDebito.Columns.Add("ValorIva", typeof(System.String));
            dsNotaDebito.Columns.Add("ValorIva2", typeof(System.String));
            dsNotaDebito.Columns.Add("Iva2", typeof(System.Decimal));
            dsNotaDebito.Columns.Add("Subtotal_Iva2", typeof(System.Decimal));          


            var ValorIva2 = "";
            decimal Iva2 = 0;
            decimal Subtotal_Iva2 = 0;
            var taxRates = model.DebitNoteInfo.TotalTaxes.ToList().Where(s => s.TaxRate != 0).ToList();
            var valorIva = "15";
            if (taxRates.Count > 1)
            {
                model.DebitNoteInfo.SubtotalVat = taxRates[0].TaxableBase;
                valorIva = $"{Convert.ToInt32(taxRates[0].TaxRate)}";
                model.DebitNoteInfo.ValueAddedTax = taxRates[0].TaxValue;
                ValorIva2 = $"{Convert.ToInt32(taxRates[1].TaxRate)}%";
                Iva2 = taxRates[1].TaxValue;
                Subtotal_Iva2 = taxRates[1].TaxableBase;
            }
            else
            {
                valorIva = taxRates.Count > 0 ? $"{Convert.ToInt32(taxRates[0].TaxRate)}": valorIva;
            }


            dsNotaDebito.Rows.Add(model.Id, valorIva, Issuer.EnvironmentType.GetCoreValue(), Issuer.IssueType.GetCoreValue(), Issuer.BussinesName.ToUpper(),
                Issuer.TradeName.ToUpper(), Issuer.RUC, model.AccessKey, model.DocumentTypeCode, model.EstablishmentCode, model.IssuePointCode, 
                model.Sequential, model.MainAddress ?? Issuer.MainAddress, model.ContributorId, model.IssuedOn.ToString("dd/MM/yyyy"), model.AuthorizationDate,
                model.EstablishmentAddress ?? model.MainAddress,  Issuer.IsSpecialContributor ? Issuer.ResolutionNumber : "", Issuer.IsAccountingRequired ? "SI" : "NO",
                model.ContributorIdentificationType, model.ContributorName.ToUpper(), model.ContributorIdentification,
                model.DebitNoteInfo.Subtotal, model.DebitNoteInfo.ReferenceDocumentCode, model.DebitNoteInfo.ReferenceDocumentNumber, 
                model.DebitNoteInfo.ReferenceDocumentDate,0, model.DebitNoteInfo.Total, model.Currency, model.Status, model.DebitNoteInfo.SubtotalVat, 
                model.DebitNoteInfo.SubtotalVatZero, model.DebitNoteInfo.SubtotalNotSubject, model.DebitNoteInfo.SubtotalExempt, model.DebitNoteInfo.Subtotal, 
                model.DebitNoteInfo.TotalDiscount, model.DebitNoteInfo.SpecialConsumTax, model.DebitNoteInfo.ValueAddedTax, model.DebitNoteInfo.Total, 
                model.DebitNoteInfo.ModifiedValue, model.AuthorizationNumber, 0M, model.DebitNoteInfo.Reason, "",
                Issuer.IsRimpe ? (model.IssuedOn >= new DateTime(2022, 1, 1, 0, 0, 0) ? TextoInfoAdicionalEnum.IsRimpe.GetCoreValue() : "") : "",
                Issuer.IsRetentionAgent ? (model.IssuedOn >= new DateTime(2020, 10, 1, 0, 0, 0) ? Issuer.AgentResolutionNumber : "") : "", 
                $"{valorIva}%", ValorIva2, Iva2, Subtotal_Iva2);


            dsNotaDebitoMotivo.Columns.Add("IdNotaDebitoMotivo", typeof(System.Int64));    
            dsNotaDebitoMotivo.Columns.Add("Razon", typeof(System.String));
            dsNotaDebitoMotivo.Columns.Add("Valor", typeof(System.Decimal));
            dsNotaDebitoMotivo.Columns.Add("IdNotaDebito", typeof(System.Int64));

            var i = 0;
            foreach (var detail in model.DebitNoteInfo.DebitNoteDetail)
            {
                dsNotaDebitoMotivo.Rows.Add(detail.Id, detail.Reason, detail.Value, detail.DebitNoteInfoId);
                i++;
            }

            dsFormaPago.Columns.Add("Id", typeof(System.Int64));
            dsFormaPago.Columns.Add("formaPago", typeof(System.String));
            dsFormaPago.Columns.Add("total", typeof(System.Decimal));
            dsFormaPago.Columns.Add("plazo", typeof(System.String));
            dsFormaPago.Columns.Add("unidadTiempo", typeof(System.String));
            var f = 0;
            foreach (var pay in model.DebitNoteInfo.Payments)
            {
                dsFormaPago.Rows.Add(f, pay.Name, pay.Total, pay.Term, pay.TimeUnit);
                f++;
            }


            dsNotaDebitoAdicionales.Columns.Add("IdNotaDebitoAdicional", typeof(System.Int64));
            dsNotaDebitoAdicionales.Columns.Add("Nombre", typeof(System.String));
            dsNotaDebitoAdicionales.Columns.Add("Descripcion", typeof(System.String));
            dsNotaDebitoAdicionales.Columns.Add("IdNotaDebito", typeof(System.Int32));


            var a = 0;
            foreach (var item in model.AdditionalFields)
            {
                if (item.Name != MySystemExtensions.AgenteRetencion)
                {
                    dsNotaDebitoAdicionales.Rows.Add(a, item.Name, item.Value, item.LineNumber);
                    a++;
                }
            }

            dsInfomacionTributaria.Columns.Add("Nombre", typeof(System.String));
            dsInfomacionTributaria.Columns.Add("Valor", typeof(System.String));
            if (Issuer.IsSpecialContributor)
            {
                dsInfomacionTributaria.Rows.Add("Contribuyente Especial Nro.: ", Issuer.ResolutionNumber);
            }
            if (Issuer.IsRetentionAgent && model.IssuedOn >= new DateTime(2020, 10, 1, 0, 0, 0))
            {
                dsInfomacionTributaria.Rows.Add("Agente de Retención Nro. Resolución: ", Issuer.AgentResolutionNumber);
            }
            if (Issuer.IsGeneralRegime)
            {
                dsInfomacionTributaria.Rows.Add(TextoInfoAdicionalEnum.IsGeneralRegime.GetCoreValue(), "");
            }
            if (Issuer.IsPopularBusiness)
            {
                dsInfomacionTributaria.Rows.Add(TextoInfoAdicionalEnum.IsPopularBusiness.GetCoreValue(), "");
            }
            if (Issuer.IsRimpe && model.IssuedOn >= new DateTime(2022, 1, 1, 0, 0, 0))
            {
                dsInfomacionTributaria.Rows.Add(TextoInfoAdicionalEnum.IsRimpe.GetCoreValue(), "");
            }
            if (Issuer.IsSimplifiedCompaniesRegime)
            {
                dsInfomacionTributaria.Rows.Add(TextoInfoAdicionalEnum.IsSimplifiedCompaniesRegime.GetCoreValue(), "");
            }
            if (Issuer.IsSkilledCraftsman)
            {
                dsInfomacionTributaria.Rows.Add(TextoInfoAdicionalEnum.IsSkilledCraftsman.GetCoreValue(), Issuer.SkilledCraftsmanNumber);
            }


            dsLogo.Columns.Add("Ruc", typeof(System.String));
            dsLogo.Columns.Add("Logo", typeof(System.Byte[]));
            dsLogo.Columns.Add("ImgClaveAcceso", typeof(System.Byte[]));

            dsLogo.Rows.Add(model.RUC, Issuer.GetLogo(), model.GeneraCodigoBarra(BarcodeLib.TYPE.CODE128));



            return ds;
        }
    }
}
