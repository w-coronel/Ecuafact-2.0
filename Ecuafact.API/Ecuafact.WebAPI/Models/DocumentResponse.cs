using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ecuafact.WebAPI.Domain.Entities;
using Newtonsoft.Json;

namespace Ecuafact.WebAPI.Models
{

    public class RecibidosQueryModel : ObjectQueryModel<DocumentResponse>
    {
        public string DeductibleType { get; set; } = "-1";

        public RecibidosQueryModel()
        {
            Data = new DocumentResponse();
        }
    }

    public class DocumentResponseDetail
    {
        public Result result { get; set; }
        public DocumentReceived document { get; set; }
    }

    public class DocumentResponse
    {
        public Result result { get; set; }
        public List<DocumentReceived> documents { get; set; }
        public Meta meta { get; set; }
    }

    public class Result
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class DocumentReceived
    {
        public object MSG { get; set; }
        public object PDF { get; set; }
        public object XML { get; set; }
        public string accessKey { get; set; }
        public string authorizationDate { get; set; }
        public object authorizationDocumentDocumentSupport { get; set; }
        public string authorizationNumber { get; set; }
        public string codTypeDoc { get; set; }
        public object customsDocument { get; set; }
        public string date { get; set; }
        public object dateDocumentSupport { get; set; }
        public int deductibleId { get; set; }
        //public object details { get; set; }
        public object discount { get; set; }
        public object documentSupport { get; set; }
        public object endDate { get; set; }
        public object endPoint { get; set; }
        public string identificationNumber { get; set; }
        public string iva { get; set; }
        public object ivaType { get; set; }
        public string name { get; set; }
        public string pk { get; set; }
        public object plate { get; set; }
        public object reason { get; set; }
        public object receiverEstablishmentCode { get; set; }
        public object receiverIdentificationNumber { get; set; }
        public object receiverName { get; set; }
        public object route { get; set; }
        public string sequence { get; set; }
        public object sequenceDocumentSupport { get; set; }
        public object startDate { get; set; }
        public object startPoint { get; set; }
        public string state { get; set; }
        public string subTotal { get; set; }
        public string subTotal0 { get; set; }
        public string subTotal12 { get; set; }
        public string subTotal15 { get; set; }        
        public string total { get; set; }
        public string typeDoc { get; set; }
        public string SupportTypeCode { get; set; }
        public List<details> details { get; set; }
        public List<PaymentType> PaymentTypes { get; set; }
        public List<totalTax> totalTaxs { get; set; }
        

    }

    public class details
    {
        public string code { get; set; }
        public object code2 { get; set; }
        public object date { get; set; }
        public int deductible { get; set; }
        public string description { get; set; }
        public string discount { get; set; }
        public object fiscalYear { get; set; }
        public object percentage { get; set; }
        public string pk { get; set; }
        public string price { get; set; }
        public string quantity { get; set; }
        public object reason { get; set; }
        public object sequence { get; set; }
        public string subTotal { get; set; }
        public object taxCode { get; set; }
        public object taxType { get; set; }
        public object taxable { get; set; }
        public object typeDoc { get; set; }
        public object value { get; set; }
    }

    public class totalTax
    {
        public long id { get; set; }        
        public string taxCode { get; set; }
        public string percentageTaxCode { get; set; }
        public decimal taxableBase { get; set; }
        public decimal taxValue { get; set; }
        public decimal taxRate { get; set; }
    }

    public class PaymentType
    {
        public string code { get; set; }
        public int term { get; set; }
        public string total { get; set; }
        public string unitTime { get; set; }

    }

    public class Meta
    {
        public int count { get; set; }
        public int countPage { get; set; }
        public int currentPage { get; set; }
        public int maxDocs { get; set; }
    }

    public class SendXMLInvoiceResponse
    {
        [JsonProperty("result")]
        public Result Result { get; set; }
        [JsonProperty("document")]
        public DocumentReceived Document { get; set; }
    }

    public class UploadFileResponseModel
    {
        [JsonProperty("result")]
        public Result Result { get; set; }
        [JsonProperty("document")]
        public DocumentReceived Document { get; set; }
    }

}