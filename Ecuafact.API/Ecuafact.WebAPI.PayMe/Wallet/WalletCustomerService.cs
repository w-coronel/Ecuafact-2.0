using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Entities.VPOS2;
using Ecuafact.WebAPI.PayMe.WalletService;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.PayMe
{
    public static class WalletCustomerService
    {
        private static WalletCommerce _walletClient;

        static WalletCustomerService()
        {
            _walletClient = new WalletCommerceClient("WalletCommerceSOAP", Constants.VPOS2.WalletServiceURL);
        }
         
        /// <summary>
        /// Proceso de Registro del Cliente:
        /// </summary>
        /// <returns></returns>
        public static WalletCustomerResult RegisterCustomer(WalletCustomerRequest request)
        {
            var body = new RegisterCardHolderRequestBody
            {
                idEntCommerce = Constants.VPOS2.IDWalletCode,
                codCardHolderCommerce = request.CustomerCode,
                names = request.FirstName,
                lastNames = request.LastName,
                mail = request.Email,
                registerVerification = GetVerificationHash(request.CustomerCode, request.Email),
                reserved1 = "",
                reserved2 = "",
                reserved3 = ""
            };
             
            ////////////////////////////////////////////////////////////////////
            ///         MUY IMPORTANTE 
            ////////////////////////////////////////////////////////////////////
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.Expect100Continue = true;
            ////////////////////////////////////////////////////////////////////

            var response = _walletClient?.RegisterCardHolder(new RegisterCardHolderRequest(body))?.Body;

            if (response != null)
            { 
                return new WalletCustomerResult
                {
                    CodAsoCardHolderWallet = response.codAsoCardHolderWallet,
                    StatusCode = response.ansCode,
                    Description = response.ansDescription,
                    Date = response.date,
                    Hour = response.hour 
                };
            }

            return new WalletCustomerResult
            {
                CodAsoCardHolderWallet = "**************",
                StatusCode = "000",
                Description="No se genero el codigo de Payme para el Cliente.",
                Date = DateTime.Now.ToString("yyyyMMdd"),
                Hour = DateTime.Now.ToString("HHmmss")
            };
        }

        public static async Task<OperationResult<OperationQueryResponse>> GetPaymentStatus(PaymentRequestModel request)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var jsonRequest = JsonConvert.SerializeObject(new OperationQueryRequest
                    {
                        idAcquirer = request.AcquirerId,
                        idCommerce = request.IdCommerce,
                        operationNumber = request.PurchaseOperationNumber,
                        purchaseVerification = request.PurchaseVerification
                    });

                    var response = await client.PostAsync($"{Constants.VPOS2.UrlAPI}/operationAcquirer/consulte", new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                    var jsonResult = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<OperationQueryResponse>(jsonResult);

                    return new OperationResult<OperationQueryResponse>(response.IsSuccessStatusCode, response.StatusCode, response.ReasonPhrase) { Entity = result };
                }
            }
            catch (Exception ex)
            {
                return new OperationResult<OperationQueryResponse>(false, HttpStatusCode.InternalServerError, ex.Message) { DevMessage = ex.ToString() };
            }
        } 

        private static string GetVerificationHash(string codCardHolderCommerce, string email)
        { 
            return Constants.VPOS2.GetStringSHA($"{Constants.VPOS2.IDWalletCode}{codCardHolderCommerce}{email}{Constants.VPOS2.WalletSecretKey}");
        }
    }
}