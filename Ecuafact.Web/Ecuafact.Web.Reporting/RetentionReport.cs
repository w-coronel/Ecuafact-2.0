using Ecuafact.Web.Domain.Entities;
using Microsoft.Reporting.WebForms;
using System.Collections.Generic;
using System.Data;

namespace Ecuafact.Web.Reporting
{
    public class RetentionReport : ReportBase<RetentionModel>
    {
        public RetentionReport(IssuerDto issuer, string path)
            : base(issuer, path)
        {
        }
        protected override string ReportName => "RptRetencion";

        protected override List<ReportDataSource> GetDataSources(RetentionModel document)
        {
            var dataSet = GetDataSet(document);
              
            return new List<ReportDataSource>
            {
                 new ReportDataSource("DsRetencion", dataSet.Tables["DsRetencion"]),
                 new ReportDataSource("DsDetalleRetencion", dataSet.Tables["DsDetalleRetencion"]),
                 new ReportDataSource("DsRetencionAdicionales", dataSet.Tables["DsRetencionAdicionales"]),
                 new ReportDataSource("DsLogo", dataSet.Tables["DsLogo"])
            };
        }




        public DataSet GetDataSet(RetentionModel model)
        {
            var ds = new DataSet();
            var dsRetencion = ds.Tables.Add("DsRetencion");
            var dsDetalleRetencion = ds.Tables.Add("DsDetalleRetencion");
            var dsRetencionAdicionales = ds.Tables.Add("DsRetencionAdicionales");
            var dsLogo = ds.Tables.Add("DsLogo");


            dsRetencion.Columns.Add("IdProveedor", typeof(System.Int64)); // % IVA --- 
            dsRetencion.Columns.Add("Ambiente", typeof(System.String));
            dsRetencion.Columns.Add("TipoEmision", typeof(System.String));
            dsRetencion.Columns.Add("RazonSocial", typeof(System.String));
            dsRetencion.Columns.Add("NombreComercial", typeof(System.String));
            dsRetencion.Columns.Add("Ruc", typeof(System.String));
            dsRetencion.Columns.Add("ClaveAcceso", typeof(System.String));
            dsRetencion.Columns.Add("CodDoc", typeof(System.String));
            dsRetencion.Columns.Add("Estab", typeof(System.String));
            dsRetencion.Columns.Add("PtoEmi", typeof(System.String));
            dsRetencion.Columns.Add("Secuencial", typeof(System.String));
            dsRetencion.Columns.Add("DirMatriz", typeof(System.String));
            dsRetencion.Columns.Add("FechaEmision", typeof(System.String));
            dsRetencion.Columns.Add("DirEstablecimiento", typeof(System.String));
            dsRetencion.Columns.Add("ContribuyenteEspecial", typeof(System.String));
            dsRetencion.Columns.Add("ObligadoContabilidad", typeof(System.String));
            dsRetencion.Columns.Add("TipoIdentificacionSujetoRetenido", typeof(System.String));
            dsRetencion.Columns.Add("RazonSocialSujetoRetenido", typeof(System.String));
            dsRetencion.Columns.Add("IdentificacionSujetoRetenido", typeof(System.String));
            dsRetencion.Columns.Add("PeriodoFiscal", typeof(System.String));
            dsRetencion.Columns.Add("estado", typeof(System.Int32));
            dsRetencion.Columns.Add("Autorizacion", typeof(System.String));
            dsRetencion.Columns.Add("FechaAutorizacion", typeof(System.String));

            dsRetencion.Rows.Add(12, Issuer.EnvironmentType.GetValorCore(), Issuer.IssueType.GetValorCore(),
                Issuer.BussinesName.ToUpper(), Issuer.TradeName.ToUpper(), Issuer.RUC, model.AccessKey, model.DocumentTypeCode,
                model.EstablishmentCode, model.IssuePointCode, model.Sequential, Issuer.MainAddress,
                model.IssuedOn.ToString("dd/MM/yyyy"), Issuer.MainAddress, Issuer.IsSpecialContributor ? Issuer.ResolutionNumber : "", Issuer.IsAccountingRequired ? "SI" : "NO",
                model.RetentionInfo.IdentificationType, model.RetentionInfo.BusinessName, model.RetentionInfo.Identification, model.RetentionInfo.FiscalPeriod,
                model.Status, model.AuthorizationNumber, model.AuthorizationDate);


            dsDetalleRetencion.Columns.Add("Codigo", typeof(System.String));
            dsDetalleRetencion.Columns.Add("CodigoRetencion", typeof(System.String));
            dsDetalleRetencion.Columns.Add("BaseImponible", typeof(System.String));
            dsDetalleRetencion.Columns.Add("PorcentajeRetener", typeof(System.String));
            dsDetalleRetencion.Columns.Add("ValorRetenido", typeof(System.Decimal));

            dsDetalleRetencion.Columns.Add("CodDocSustento", typeof(System.Int32));
            dsDetalleRetencion.Columns.Add("NumDocSustento", typeof(System.String));
            dsDetalleRetencion.Columns.Add("FechaEmisionDocSustento", typeof(System.String));


            foreach (var item in model.RetentionInfo.Details)
            {
                dsDetalleRetencion.Rows.Add(item.TaxTypeCode, item.RetentionTaxCode, item.TaxBase, item.TaxRate, item.TaxValue,
                    model.RetentionInfo.ReferenceDocumentCode ?? item.ReferenceDocumentCode,
                    model.RetentionInfo.ReferenceDocumentNumber ?? item.ReferenceDocumentNumber,
                    model.RetentionInfo.ReferenceDocumentDate ?? item.ReferenceDocumentDate);
            }


            dsRetencionAdicionales.Columns.Add("IdRetencionAdicional", typeof(System.Int64));
            dsRetencionAdicionales.Columns.Add("Nombre", typeof(System.String));
            dsRetencionAdicionales.Columns.Add("Valor", typeof(System.String));
            dsRetencionAdicionales.Columns.Add("NumLinea", typeof(System.Int32));

              

            var a = 0;
            foreach (var item in model.AdditionalFields)
            {
                dsRetencionAdicionales.Rows.Add(a, item.Name, item.Value, item.LineNumber);
                a++;
            }


            dsLogo.Columns.Add("Ruc", typeof(System.String));
            dsLogo.Columns.Add("Logo", typeof(System.Byte[]));

            dsLogo.Rows.Add(model.RUC, GetIssuerLogo());



            return ds;
        }
    }
}
