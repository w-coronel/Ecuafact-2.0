﻿using System;
using System.Data;
using System.Collections.Generic;
using Ecuafact.WebAPI.Domain.Entities;
using Microsoft.Reporting.WebForms;
using System.Linq;
using Ecuafact.WebAPI.Domain.Entities.Engine;

namespace Ecuafact.WebAPI.Domain.Reporting
{
    public class ReferralGuideReport : ReportBase<Document>
    {
        public Issuer Issuer { get; set; }
        public ReferralGuideReport(Issuer issuer):base()
        {
            Issuer = issuer;
        }

        protected override string ReportName => "RptGuiaRemision";


        protected override List<ReportDataSource> GetDataSources(Document document)
        {

            var dataSet = GetDataSet(document);
             
            return new List<ReportDataSource>
            {
                 new ReportDataSource("DsGuia", dataSet.Tables["DsGuia"]),
                 new ReportDataSource("DsDetalleGuia", dataSet.Tables["DsDetalleGuia"]),
                 new ReportDataSource("DsGuiaAdicionales", dataSet.Tables["DsGuiaAdicionales"]),
                 new ReportDataSource("DsLogo", dataSet.Tables["DsLogo"]),
                 new ReportDataSource("DsInfomacionTributaria", dataSet.Tables["DsInfomacionTributaria"])
            };
        }

        public DataSet GetDataSet(Document model)
        {
            var dataSet = new DataSet();
            var dsGuia = dataSet.Tables.Add("DsGuia");
            var dsDetalleGuia = dataSet.Tables.Add("DsDetalleGuia");
            var dsGuiaAdicionales = dataSet.Tables.Add("DsGuiaAdicionales");
            var dsLogo = dataSet.Tables.Add("DsLogo");
            var dsInfomacionTributaria = dataSet.Tables.Add("DsInfomacionTributaria"); 

            dsGuia.Columns.Add("IdGuia", typeof(System.Int64));
            dsGuia.Columns.Add("IdProveedor", typeof(System.Int64));
            dsGuia.Columns.Add("Ambiente", typeof(System.String));
            dsGuia.Columns.Add("TipoEmision", typeof(System.String));
            dsGuia.Columns.Add("RazonSocial", typeof(System.String));
            dsGuia.Columns.Add("NombreComercial", typeof(System.String));
            dsGuia.Columns.Add("Ruc", typeof(System.String));
            dsGuia.Columns.Add("ClaveAcceso", typeof(System.String));
            dsGuia.Columns.Add("CodDoc", typeof(System.String));
            dsGuia.Columns.Add("Estab", typeof(System.String));
            dsGuia.Columns.Add("PtoEmi", typeof(System.String));
            dsGuia.Columns.Add("Secuencial", typeof(System.String));
            dsGuia.Columns.Add("DirMatriz", typeof(System.String));
            dsGuia.Columns.Add("IdCliente", typeof(System.Int64));
            dsGuia.Columns.Add("FechaEmision", typeof(System.String));
            dsGuia.Columns.Add("FechaAutorizacion", typeof(System.String));
            dsGuia.Columns.Add("DirEstablecimiento", typeof(System.String));
            dsGuia.Columns.Add("ContribuyenteEspecial", typeof(System.String));
            dsGuia.Columns.Add("ObligadoContabilidad", typeof(System.String));
            dsGuia.Columns.Add("TipoIdentificacionComprador", typeof(System.String));
            dsGuia.Columns.Add("GuiaRemision", typeof(System.String));
            dsGuia.Columns.Add("RazonSocialComprador", typeof(System.String));
            dsGuia.Columns.Add("IdentificacionComprador", typeof(System.String));
            dsGuia.Columns.Add("TotalSinImpuesto", typeof(System.Decimal));
            dsGuia.Columns.Add("Propina", typeof(System.Decimal));
            dsGuia.Columns.Add("ImporteTotal", typeof(System.Decimal));
            dsGuia.Columns.Add("Moneda", typeof(System.String));
            dsGuia.Columns.Add("Estado", typeof(System.Int32));
            dsGuia.Columns.Add("Subtotal_12", typeof(System.Decimal));
            dsGuia.Columns.Add("Subtotal_0", typeof(System.Decimal));
            dsGuia.Columns.Add("Subtotal_No_Iva", typeof(System.Decimal));
            dsGuia.Columns.Add("Subtotal", typeof(System.Decimal));
            dsGuia.Columns.Add("TotalDescuento", typeof(System.Decimal));
            dsGuia.Columns.Add("Ice", typeof(System.Decimal));
            dsGuia.Columns.Add("Iva", typeof(System.Decimal));
            dsGuia.Columns.Add("ValorTotal", typeof(System.Decimal));
            dsGuia.Columns.Add("Autorizacion", typeof(System.String));
            dsGuia.Columns.Add("Motivo", typeof(System.String));
            dsGuia.Columns.Add("Dirpartida", typeof(System.String));
            dsGuia.Columns.Add("FechaIniTransporte", typeof(System.String));
            dsGuia.Columns.Add("FechaFinTransporte", typeof(System.String));
            dsGuia.Columns.Add("Placa", typeof(System.String));
            dsGuia.Columns.Add("IdentificacionDestinatario", typeof(System.String));
            dsGuia.Columns.Add("RazonSocialSestinatario", typeof(System.String));
            dsGuia.Columns.Add("DirDestinatario", typeof(System.String));
            dsGuia.Columns.Add("CodDocSustento", typeof(System.String));
            dsGuia.Columns.Add("NumDocSustento", typeof(System.String));
            dsGuia.Columns.Add("FechaEmisionDocSustento", typeof(System.String));
            dsGuia.Columns.Add("NumAutDocSustento", typeof(System.String));
            dsGuia.Columns.Add("Ruta", typeof(System.String));
            dsGuia.Columns.Add("MicroEmpresa", typeof(System.String));
            dsGuia.Columns.Add("AgenteRetencion", typeof(System.String));            

            dsGuia.Rows.Add(model.Id, 12, Issuer.EnvironmentType.GetCoreValue(), Issuer.IssueType.GetCoreValue(), Issuer.BussinesName, Issuer.TradeName, Issuer.RUC,
                model.AccessKey, model.DocumentTypeCode, model.EstablishmentCode, model.IssuePointCode, model.Sequential, model.MainAddress ?? Issuer.MainAddress, model.ContributorId,
                model.IssuedOn.ToString("dd/MM/yyyy"), model.AuthorizationDate, model.EstablishmentAddress ?? model.MainAddress, Issuer.IsSpecialContributor ? Issuer.ResolutionNumber : "", Issuer.IsAccountingRequired ? "SI" : "NO",
                model.ReferralGuideInfo.DriverIdentificationType, "", model.ReferralGuideInfo.DriverName, model.ReferralGuideInfo.DriverIdentification,
                model.Total, 0M, model.Total, model.Currency, model.Status, 0, 0, 0, model.Total, 0, 0, 0, model.Total, model.AuthorizationNumber, model.Reason,
                model.ReferralGuideInfo.OriginAddress, model.ReferralGuideInfo.ShippingStartDate, model.ReferralGuideInfo.ShippingEndDate, model.ReferralGuideInfo.CarPlate,
                model.ReferralGuideInfo.RecipientIdentification, model.ReferralGuideInfo.RecipientName, model.ReferralGuideInfo.RecipientAddress,model.ReferralGuideInfo.ReferenceDocumentCode, 
                model.ReferralGuideInfo.ReferenceDocumentNumber, model.ReferralGuideInfo.ReferenceDocumentDate, model.ReferralGuideInfo.ReferenceDocumentAuth, model.ReferralGuideInfo.ShipmentRoute,
                Issuer.IsRimpe ? (model.IssuedOn >= new DateTime(2022, 1, 1, 0, 0, 0) ? TextoInfoAdicionalEnum.IsRimpe.GetCoreValue() : "") : "",
                Issuer.IsRetentionAgent ? (model.IssuedOn >= new DateTime(2020, 10, 1, 0, 0, 0) ? $"{Issuer.AgentResolutionNumber}" : "") : "");


            dsDetalleGuia.Columns.Add("IdDetalleGuia", typeof(System.Int64));
            dsDetalleGuia.Columns.Add("IdGuia", typeof(System.Int64));
            dsDetalleGuia.Columns.Add("CodigoPrincipal", typeof(System.String));
            dsDetalleGuia.Columns.Add("CodigoAuxiliar", typeof(System.String));
            dsDetalleGuia.Columns.Add("Descripcion", typeof(System.String));
            dsDetalleGuia.Columns.Add("Cantidad", typeof(System.Decimal));
            dsDetalleGuia.Columns.Add("PrecioUnitario", typeof(System.Decimal));
            dsDetalleGuia.Columns.Add("Descuento", typeof(System.Decimal));
            dsDetalleGuia.Columns.Add("PrecioTotalSinImpuesto", typeof(System.Decimal));
            dsDetalleGuia.Columns.Add("DetAdicionalN1", typeof(System.String));
            dsDetalleGuia.Columns.Add("DetAdicionalV1", typeof(System.String));
            dsDetalleGuia.Columns.Add("DetAdicionalN2", typeof(System.String));
            dsDetalleGuia.Columns.Add("DetAdicionalV2", typeof(System.String));
            dsDetalleGuia.Columns.Add("DetAdicionalN3", typeof(System.String));
            dsDetalleGuia.Columns.Add("DetAdicionalV3", typeof(System.String));

            var i = 0;
            foreach (var item in model.ReferralGuideInfo.Details)
            {
                dsDetalleGuia.Rows.Add(i,  model.Id, item.MainCode, item.AuxCode, item.Description, item.Quantity, 
                    0, 0, 0, item.Name1, item.Value1, item.Name2, item.Value2, item.Name3, item.Value3);
                i++;
            }

            dsGuiaAdicionales.Columns.Add("IdGuiaAdicional", typeof(System.Int64));
            dsGuiaAdicionales.Columns.Add("Nombre", typeof(System.String));
            dsGuiaAdicionales.Columns.Add("Valor", typeof(System.String));
            dsGuiaAdicionales.Columns.Add("NumLinea", typeof(System.Int32));
 
            var a = 0;
            foreach (var item in model.AdditionalFields)
            {
                if (item.Name != MySystemExtensions.AgenteRetencion)
                {
                    dsGuiaAdicionales.Rows.Add(a, item.Name, item.Value, item.LineNumber);
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



            return dataSet;
        }
    }
}
