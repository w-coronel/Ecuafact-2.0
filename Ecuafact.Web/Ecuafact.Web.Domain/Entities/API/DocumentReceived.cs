using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ecuafact.Web.Domain.Entities
{
    public class UploadFileResponseModel
    {
        /// <summary>
        /// Resultado
        /// </summary>
        [JsonProperty("result")]
        public ResultModel Result { get; set; }

        /// <summary>
        /// Document
        /// </summary>
        [JsonProperty("document")]
        public DocumentRecividModel Document { get; set; }
    }

    /// <summary>
    /// Result
    /// </summary>
    public class ResultModel
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }


    /// <summary>
    /// Document
    /// </summary>
    public class DocumentRecividModel
    {
        [JsonProperty("MSG")]
        public string MSG { get; set; }
        [JsonProperty("PDF")]
        public string PDF { get; set; }
        [JsonProperty("XML")]
        public string XML { get; set; }
        [JsonProperty("accessKey")]
        public string AccessKey { get; set; }
        [JsonProperty("authorizationDate")]
        public string AuthorizationDate { get; set; }
        [JsonProperty("authorizationDocumentDocumentSupport")]
        public string AuthorizationDocumentDocumentSupport { get; set; }
        [JsonProperty("authorizationNumber")]
        public string AuthorizationNumber { get; set; }
        [JsonProperty("codTypeDoc")]
        public string CodTypeDoc { get; set; }
        [JsonProperty("customsDocument")]
        public string CustomsDocument { get; set; }
        [JsonProperty("date")]
        public string Date { get; set; }
        [JsonProperty("dateDocumentSupport")]
        public string DateDocumentSupport { get; set; }
        [JsonProperty("deductibleId")]
        public string DeductibleId { get; set; }
        [JsonProperty("discount")]
        public string Discount { get; set; }
        [JsonProperty("documentSupport")]
        public string DocumentSupport { get; set; }
        [JsonProperty("endDate")]
        public string EndDate { get; set; }
        [JsonProperty("endPoint")]
        public string EndPoint { get; set; }
        [JsonProperty("identificationNumber")]
        public string IdentificationNumber { get; set; }
        [JsonProperty("iva")]
        public string Iva { get; set; }
        [JsonProperty("ivaType")]
        public string IvaType { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("pk")]
        public string Pk { get; set; }
        [JsonProperty("plate")]
        public string Plate { get; set; }
        [JsonProperty("reason")]
        public string Reason { get; set; }
        [JsonProperty("receiverEstablishmentCode")]
        public string ReceiverEstablishmentCode { get; set; }
        [JsonProperty("receiverIdentificationNumber")]
        public string ReceiverIdentificationNumber { get; set; }
        [JsonProperty("receiverName")]
        public string ReceiverName { get; set; }
        [JsonProperty("route")]
        public string Route { get; set; }
        [JsonProperty("sequence")]
        public string Sequence { get; set; }
        [JsonProperty("sequenceDocumentSupport")]
        public string SequenceDocumentSupport { get; set; }
        [JsonProperty("startDate")]
        public string StartDate { get; set; }
        [JsonProperty("startPoint")]
        public string StartPoint { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("subTotal")]
        public string SubTotal { get; set; }
        [JsonProperty("subTotal0")]
        public string SubTotal0 { get; set; }
        [JsonProperty("subTotal12")]
        public string SubTotal12 { get; set; }
        [JsonProperty("subTotal15")]
        public string SubTotal15 { get; set; }        
        [JsonProperty("total")]
        public string Total { get; set; }
        [JsonProperty("typeDoc")]
        public string TypeDoc { get; set; }
        [JsonProperty("details")]
        public List<DetailModel> Detail { get; set; }
        [JsonProperty("paymentTypes")]
        public List<PaymentTypesModel> PaymentTypes { get; set; }
        [JsonProperty("totalTax")]
        public List<TotalTaxModel> TotalTax { get; set; }


    }

    /// <summary>
    /// Payment
    /// </summary>
    public class PaymentTypesModel
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("term")]
        public int Term { get; set; }
        [JsonProperty("total")]
        public string Total { get; set; }
        [JsonProperty("unitTime")]
        public string UnitTime { get; set; }
    }

    

    /// <summary>
    /// Detail
    /// </summary>
    public class DetailModel
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("code2")]
        public string Dode2 { get; set; }
        [JsonProperty("date")]
        public string Date { get; set; }
        [JsonProperty("deductible")]
        public int Deductible { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("discount")]
        public string Discount { get; set; }
        [JsonProperty("fiscalYear")]
        public string FiscalYear { get; set; }
        [JsonProperty("percentage")]
        public string Percentage { get; set; }
        [JsonProperty("pk")]
        public string Pk { get; set; }
        [JsonProperty("price")]
        public string Price { get; set; }
        [JsonProperty("quantity")]
        public string Quantity { get; set; }
        [JsonProperty("reason")]
        public string Reason { get; set; }
        [JsonProperty("sequence")]
        public string Sequence { get; set; }
        [JsonProperty("subTotal")]
        public string SubTotal { get; set; }
        [JsonProperty("taxCode")]
        public string TaxCode { get; set; }
        [JsonProperty("taxType")]
        public string TaxType { get; set; }
        [JsonProperty("taxable")]
        public string Taxable { get; set; }
        [JsonProperty("typeDoc")]
        public string TypeDoc { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }

    [XmlRoot("totalConImpuestos")]
    public class totalConImpuestos
    {
        [XmlElement("totalImpuesto")]
        public List <totalImpuesto> totalImpuesto { get; set; }
    }

    public class totalImpuesto
    {
        [XmlElement("codigo")]
        public string codigo { get; set; }
        [XmlElement("codigoPorcentaje")]
        public string codigoPorcentaje { get; set; }
        [XmlElement("baseImponible")]
        public decimal baseImponible { get; set; }
        [XmlElement("valor")]
        public decimal valor { get; set; }

    }
    
    public class infoTributaria
    {       
        public string codDoc { get; set; }
    }

    public class infoCompRetencion
    {
        public string identificacionSujetoRetenido { get; set; }        
    }

    public class infoFactura : infoData { }
    public class infoLiquidacionCompra : infoData { }
    public class infoNotaCredito : infoData { }
    public class infoNotaDebito : infoData { }
    public class infoGuiaRemision : infoData { }
    public class infoData
    {
        public string identificacionComprador { get; set; }
    }

}
