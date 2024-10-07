using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Entities.VPOS2
{
    public class OperationQueryResponse
    {
        public OperationResultEnum result { get; set; }
        public string errorCode { get; set; }
        public string errorMessage { get; set; }
        public string operationNumber { get; set; }
        public string authorizationCode { get; set; }
        public string cardNumber { get; set; } 
        public string authenticationECI { get; set; }
        public string billingCountry { get; set; }
        public string billingState { get; set; }
        public string cardType { get; set; } 
        public string language { get; set; }
        public string purchaseAmount { get; set; }
        public string purchaseCurrencyCode { get; set; }
        public string purchaseIPAddress { get; set; }
        public string reserved1 { get; set; }
        public string reserved11 { get; set; }
        public string reserved12 { get; set; }
        public string reserved2 { get; set; }
        public string reserved3 { get; set; }
        public string reserved4 { get; set; }
        public string reserved5 { get; set; }
        public string reserved6 { get; set; }
        public string reserved9 { get; set; }
        public string reserved23 { get; set; }
        public string shippingAddress { get; set; }
        public string shippingCity { get; set; }
        public string shippingCountry { get; set; }
        public string shippingEMail { get; set; }
        public string shippingFirstName { get; set; }
        public string shippingLastName { get; set; }
        public string shippingState { get; set; }
        public string shippingZIP { get; set; }
        public string terminalCode { get; set; }

    }
}
