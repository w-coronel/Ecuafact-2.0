using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

  using VPOS20_PLUGIN;
 
namespace Ecuafact.Web.Payment
{
    public class PaymentService
    { 
        public const string CURRENCY_DEFAULT = "840"; // 
        public const string LANGUAGE_DEFAULT = "SP"; //  Tiene tres posibles valores: Español (SP), Inglés (EN), Portugués (PT)

        public static PaymentResponse Generate(PaymentRequest request)
        {
            try
            {
                var oVposData = new PaymentData();
                oVposData.acquirerId = VPOSSettings.Current.IdAcquirer;
                oVposData.commerceId = VPOSSettings.Current.IdCommerce;
                oVposData.purchaseCurrencyCode = CURRENCY_DEFAULT; // DOLLAR
                oVposData.purchaseAmount = Convert.ToInt64(request.PurchaseAmount * 100).ToString("#####0");
                oVposData.purchaseOperationNumber = request.PurchaseOperationNumber;
                //oVposData.billingEMail = oVposData.shippingEMail = request.Email;
                //oVposData.billingFirstName = oVposData.shippingFirstName = request.CustomerName;

                //Valor proporcionado por el comercio. No incluye el valor del IVA.
                oVposData.reserved1 = Convert.ToInt64(request.Subtotal * 100).ToString("#####0"); // (Pago.subtotal + Pago.adicional).ToString();

                //Valor total del IVA reportado por el comercio. Si no hay valor de IVA debe de indicarse el valor 0.
                oVposData.reserved2 = Convert.ToInt64(request.IVA * 100).ToString("#####0");

                // Idioma a ser utilizado por el V-POS.
                oVposData.reserved3 = LANGUAGE_DEFAULT;

                // SOLICITADO POR PACIFICARD
                oVposData.reserved9 = "000";

                // Monto No Grava IVA
                oVposData.reserved11 = (request.Subtotal0 * 100).ToString("#####0");

                //Impuesto a los Consumos Especiales
                oVposData.reserved12 = (request.ICE * 100).ToString("#####0");

                var srVposLlaveCifradoPublica = new StreamReader(Path.Combine(VPOSSettings.Current.RootDirectory, VPOSSettings.Current.LlaveCifradoVPOSPublic));
                var srComercioLlaveFirmaPrivada = new StreamReader(Path.Combine(VPOSSettings.Current.RootDirectory, VPOSSettings.Current.LlaveFirmaComPrivate));

                var oVposBean = oVposData as VPOSBean;
                var oVposSend = new VPOSSend(srVposLlaveCifradoPublica, srComercioLlaveFirmaPrivada, VPOSSettings.Current.VectorInitialize);
                oVposSend.execute(ref oVposBean);
                
                var result = new PaymentResponse
                {
                    IDACQUIRER = VPOSSettings.Current.IdAcquirer,
                    IDCOMMERCE = VPOSSettings.Current.IdCommerce,
                    SESSIONKEY = oVposBean.cipheredSessionKey,
                    XMLREQ = oVposBean.cipheredXML,
                    DIGITALSIGN = oVposBean.cipheredSignature,
                    PAYMENTURL = VPOSSettings.Current.UrlVPOS,
                    PURCHASEORDER = request,
                    VPOSDATA = (PaymentData)oVposBean
                };

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Hubo un problema al generar el requerimiento de pago", ex);
            } 
        }
    }
}
