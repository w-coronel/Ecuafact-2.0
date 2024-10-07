using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using VPOS20_PLUGIN;
 
namespace Ecuafact.Web.Payment
{
    public class PaymentResponse
    {
        public string IDACQUIRER { get; set; }
        public string IDCOMMERCE { get; set; }
        public string SESSIONKEY { get; set; }
        public string XMLREQ { get; set; }
        public string DIGITALSIGN { get; set; }

        public string PAYMENTURL { get; set; }
         
        public PaymentData VPOSDATA { get; set; }

        public PaymentRequest PURCHASEORDER { get; set; }
    }

    public class PaymentRequest
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        public string PurchaseOperationNumber { get; set; } = "000";
        public string PurchaseOrderNumber { get; set; } = "000";
        public string CustomerIdentification { get; set; } = "9999999999";
        public string CustomerName { get; set; } = "No Disponible";

        public string Email { get; set; } = "rramirez@ecuanexus.com";
        public DateTime Date { get; set; } = DateTime.Now;
        public decimal Subtotal0 { get; set; } = 0;
        public decimal Subtotal12 { get; set; } = 0;
        public decimal Subtotal { get { return Subtotal0 + Subtotal12; } }
        public decimal IVA { get { return decimal.Round(Subtotal12 * 0.12M, 2); } }
        public decimal ICE { get; set; } = 0;
        public decimal PurchaseAmount
        {
            get { return Subtotal + IVA + ICE; }
        }

    }

    public class PaymentData : VPOSBean
    {
        //public string idCommerce { get { return commerceId; } }
    }
}
