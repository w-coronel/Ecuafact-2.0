using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Entities.Engine;
using Ecuafact.WebAPI.Domain.Reporting;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Ecuafact.WebAPI.Domain.Reporting
{
    public class CreditNoteReport : ReportBase<Document>
    {
        public Issuer Issuer { get; set; }
        public CreditNoteReport(Issuer issuer)
        {
            Issuer = issuer;
        }

        protected override string ReportName => "RptNotaCredito";

        protected override List<ReportDataSource> GetDataSources(Document document)
        { 
            var dataSet = GetDataSet(document);
            var dataSources = new List<ReportDataSource>
            {
                new ReportDataSource("DsNotaCredito", dataSet.Tables["DsNotaCredito"]),
                new ReportDataSource("DsNotaCreditoDetalle", dataSet.Tables["DsNotaCreditoDetalle"]),
                new ReportDataSource("DsNotaCreditoAdicionales", dataSet.Tables["DsNotaCreditoAdicionales"]),
                new ReportDataSource("DsFormaPago", dataSet.Tables["DsFormaPago"]),
                new ReportDataSource("DsLogo", dataSet.Tables["DsLogo"]),
                new ReportDataSource("DsInfomacionTributaria", dataSet.Tables["DsInfomacionTributaria"])
            };

            return dataSources;
        } 

        private DataSet GetDataSet(Document model)
        {
            var ds = new DataSet();
            var dsNotaCredito = ds.Tables.Add("DsNotaCredito");
            var dsNotaCreditoDetalle = ds.Tables.Add("DsNotaCreditoDetalle");
            var dsNotaCreditoAdicionales = ds.Tables.Add("DsNotaCreditoAdicionales");
            var dsFormaPago = ds.Tables.Add("DsFormaPago");
            var dsLogo = ds.Tables.Add("DsLogo");
            var dsInfomacionTributaria = ds.Tables.Add("DsInfomacionTributaria");

            dsNotaCredito.Columns.Add("IdNotaCredito", typeof(System.Int64));
            dsNotaCredito.Columns.Add("IdProveedor", typeof(System.Int64)); // % IVA --- 
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

            dsNotaCredito.Columns.Add("MicroEmpresa", typeof(System.String));
            dsNotaCredito.Columns.Add("AgenteRetencion", typeof(System.String));
            dsNotaCredito.Columns.Add("ValorIva", typeof(System.String));

            dsNotaCredito.Columns.Add("ValorIva2", typeof(System.String));
            dsNotaCredito.Columns.Add("Iva2", typeof(System.Decimal));
            dsNotaCredito.Columns.Add("Subtotal_Iva2", typeof(System.Decimal));

            var ValorIva2 = "";
            decimal Iva2 = 0;
            decimal Subtotal_Iva2 = 0;
            var taxRates = model.CreditNoteInfo.TotalTaxes.ToList().Where(s => s.TaxRate != 0).ToList();            
            var valorIva = "15";
            if (taxRates.Count > 1)
            {
                model.CreditNoteInfo.SubtotalVat = taxRates[0].TaxableBase;
                valorIva = $"{Convert.ToInt32(taxRates[0].TaxRate)}";
                model.CreditNoteInfo.ValueAddedTax = taxRates[0].TaxValue;
                ValorIva2 = $"{Convert.ToInt32(taxRates[1].TaxRate)}%";
                Iva2 = taxRates[1].TaxValue;
                Subtotal_Iva2 = taxRates[1].TaxableBase;
            }
            else
            {
                valorIva = taxRates.Count > 0 ? $"{Convert.ToInt32(taxRates[0].TaxRate)}": valorIva;
            }

            dsNotaCredito.Rows.Add(model.Id, 12, Issuer.EnvironmentType.GetCoreValue(), Issuer.IssueType.GetCoreValue(), Issuer.BussinesName.ToUpper(), Issuer.TradeName.ToUpper(), Issuer.RUC,
                model.AccessKey, model.DocumentTypeCode, model.EstablishmentCode, model.IssuePointCode, model.Sequential,model.MainAddress ?? Issuer.MainAddress,
                model.ContributorId, model.IssuedOn.ToString("dd/MM/yyyy"), model.AuthorizationDate, model.EstablishmentAddress ?? model.MainAddress,
                Issuer.IsSpecialContributor ? Issuer.ResolutionNumber : "", Issuer.IsAccountingRequired ? "SI" : "NO",
                model.ContributorIdentificationType, model.ContributorName.ToUpper(), model.ContributorIdentification,
                model.CreditNoteInfo.Subtotal, model.CreditNoteInfo.ReferenceDocumentCode, model.CreditNoteInfo.ReferenceDocumentNumber, model.CreditNoteInfo.ReferenceDocumentDate,                
                model.CreditNoteInfo.Tip, model.CreditNoteInfo.Total, model.Currency, model.Status,model.CreditNoteInfo.SubtotalVat, model.CreditNoteInfo.SubtotalVatZero, 
                model.CreditNoteInfo.SubtotalNotSubject, model.CreditNoteInfo.SubtotalExempt,model.CreditNoteInfo.Subtotal, model.CreditNoteInfo.TotalDiscount, model.CreditNoteInfo.SpecialConsumTax, 
                model.CreditNoteInfo.ValueAddedTax,model.CreditNoteInfo.Total,model.CreditNoteInfo.ModifiedValue,  model.AuthorizationNumber, 0M, model.CreditNoteInfo.Reason, "",
                Issuer.IsRimpe ? (model.IssuedOn >= new DateTime(2022, 1, 1, 0, 0, 0) ? TextoInfoAdicionalEnum.IsRimpe.GetCoreValue() : "") : "",
                Issuer.IsRetentionAgent ? (model.IssuedOn >= new DateTime(2020, 10, 1, 0, 0, 0) ? Issuer.AgentResolutionNumber : "") : "", $"{valorIva}%", ValorIva2, Iva2, Subtotal_Iva2);


            dsNotaCreditoDetalle.Columns.Add("IdNotaCredito", typeof(System.Int64));
            dsNotaCreditoDetalle.Columns.Add("IdNotaCreditoDetalle", typeof(System.Int64));
            dsNotaCreditoDetalle.Columns.Add("CodigoInterno", typeof(System.String));
            dsNotaCreditoDetalle.Columns.Add("CodigoAdicional", typeof(System.String));
            dsNotaCreditoDetalle.Columns.Add("Descripcion", typeof(System.String));
            dsNotaCreditoDetalle.Columns.Add("Cantidad", typeof(System.Decimal));
            dsNotaCreditoDetalle.Columns.Add("Descuento", typeof(System.Decimal));
            dsNotaCreditoDetalle.Columns.Add("PorcDescuento", typeof(System.Decimal));
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
                decimal valdesc = 0;
                if (detail.Discount > 0){
                    valdesc = Math.Round((detail.Amount * detail.UnitPrice) * (detail.Discount / 100), 2);
                }
                dsNotaCreditoDetalle.Rows.Add(model.Id, i, detail.MainCode, detail.AuxCode,
                    detail.Description, detail.Amount, valdesc, detail.Discount, detail.UnitPrice, detail.SubTotal,
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
                if (item.Name != MySystemExtensions.AgenteRetencion)
                {
                    dsNotaCreditoAdicionales.Rows.Add(a, item.Name, item.Value, item.LineNumber);
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