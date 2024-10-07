using Ecuafact.WebAPI.Domain.Entities;
using Newtonsoft.Json;
using System;

namespace Ecuafact.WebAPI.PayMe
{
    /// <summary>
    /// Modelo para el Requerimiento de Pagos para VPOS2
    /// </summary>
    public class PaymentRequestModel
    {
        public PaymentRequestModel(PurchasePayment payment)
        {
            this.Payment = payment;
        }

        [JsonProperty("subscriptionActiva")]
        public bool SubscriptionActiva { get; set; }
        [JsonProperty("tipoProceso")]
        public long TipoProceso { get; set; }
        private PurchasePayment Payment { get; set; }
        public PurchaseOrder PurchaseOrder => Payment?.PurchaseOrder;
        [JsonProperty("acquirerId")] public string AcquirerId => Payment.AcquirerId;
        [JsonProperty("idCommerce")] public string IdCommerce => Payment.IdCommerce;
        [JsonProperty("purchaseOperationNumber")] public string PurchaseOperationNumber => Payment.Id.ToString("000000000");
        [JsonProperty("purchaseAmount")] public string PurchaseAmount => Convert.ToInt64(Payment.PurchaseAmount * 100).ToString();
        [JsonProperty("purchaseCurrencyCode")] public string PurchaseCurrencyCode => Payment.PurchaseCurrencyCode;
        [JsonProperty("language")] public string Language => Constants.VPOS2.LANGUAGE_CODE;
        [JsonProperty("shippingFirstName")] public string ShippingFirstName => PurchaseOrder.FirstName;
        [JsonProperty("shippingLastName")] public string ShippingLastName => PurchaseOrder.LastName;
        [JsonProperty("shippingEmail")] public string ShippingEmail => PurchaseOrder.Email;
        [JsonProperty("shippingAddress")] public string ShippingAddress => PurchaseOrder.Address;
        [JsonProperty("shippingZIP")] public string ShippingZIP => PurchaseOrder.ZIP;
        [JsonProperty("shippingCity")] public string ShippingCity => PurchaseOrder.City;
        [JsonProperty("shippingState")] public string ShippingState => PurchaseOrder.Province;
        [JsonProperty("shippingCountry")] public string ShippingCountry => PurchaseOrder.Country;
        [JsonProperty("userCommerce")] public string UserCommerce => PurchaseOrder.Identification;
        [JsonProperty("userCodePayme")] public string UserCodePayme => Payment.UserCodePayme;
        [JsonProperty("descriptionProducts")] public string DescriptionProducts => PurchaseOrder.Products;
        [JsonProperty("programmingLanguage")] public string ProgrammingLanguage => Constants.VPOS2.PROGRAM_LANGUAGE;
        [JsonProperty("purchaseVerification")] public string PurchaseVerification => getPasarelaSHA();
        [JsonProperty("reserved1")] public string Reserved1 => Convert.ToInt64(PurchaseOrder.Subtotal12 * 100).ToString(); //	Neto
        [JsonProperty("reserved2")] public string Reserved2 => Convert.ToInt64(PurchaseOrder.IVA * 100).ToString();//	IVA
        [JsonProperty("reserved3")] public string Reserved3 => Constants.VPOS2.LANGUAGE_CODE; //	Idioma
        [JsonProperty("reserved4")] public string Reserved4 => Convert.ToInt64(PurchaseOrder.Additional * 100).ToString();   //	Condiciones Entrega
        [JsonProperty("reserved5")] public string Reserved5 => "000";  //	Opcional
        [JsonProperty("reserved6")] public string Reserved6 => "000";   //	Transporte Mercaderia
        [JsonProperty("reserved9")] public string Reserved9 => Convert.ToInt64(PurchaseOrder.Interests * 100).ToString();   //	Intereses
        [JsonProperty("reserved11")] public string Reserved11 => Convert.ToInt64(PurchaseOrder.Subtotal0 * 100).ToString(); //	No Graba IVA
        [JsonProperty("reserved12")] public string Reserved12 => Convert.ToInt64(PurchaseOrder.ICE * 100).ToString();	//	ICE

        [JsonProperty("development")] public bool DevelopmentMode => Constants.DevelopmentMode;

        [JsonProperty("paymentId")] public long PaymentId => Payment.Id;
        [JsonProperty("carrierCode")] public string ErrorCode => Payment.ErrorCode;
        [JsonProperty("currentStatus")] public string ErrorMessage => Payment.ErrorMessage;
        [JsonProperty("transactionId")] public string TransactionId => Payment.TransactionId;
        [JsonProperty("paymentDate")] public string PaymentDate => Payment.CreatedOn.ToString();
        [JsonProperty("authorizationCode")] public string AuthorizationCode => Payment.AuthorizationCode;
        [JsonProperty("status")] public string Status => Payment.Status;

        string getPasarelaSHA()
        {
            return Constants.VPOS2.GetStringSHA($"{Constants.VPOS2.IDAcquirer}{IdCommerce}{PurchaseOperationNumber}{PurchaseAmount}{PurchaseCurrencyCode}{Constants.VPOS2.PasarelaSecretKey}");
        }
    }

    public static class PurchasePaymentExtensions
    {

        public static PurchasePayment ToPurchasePayment(this PaymentTransaction model)
        {
            return new PurchasePayment
            {
                AcquirerId = "0",
                IdCommerce = "0",
                PurchaseOrderId = model.PurchaseOrder.PurchaseOrderId,
                PurchaseOrder = model.PurchaseOrder,
                PurchaseAmount = model.PurchaseOrder.Total,
                PurchaseCurrencyCode = "0",
                CardType = TypeCardName(model.card.type),
                CardNumber = model.card.number,
                Carrier = model.transaction.carrier,
                Installments = model.transaction.installments.ToString(),
                InstallmentsType = model.transaction.installments_type,
                Origin = model.card.origin,
                ErrorCode = model.transaction.carrier_code,
                AuthorizationCode = model.transaction.authorization_code,
                ErrorMessage = $"{model.transaction.current_status}-{GetStatusDetail((StatusDetailEnum)model.transaction.status_detail)}",
                TransactionId = model.transaction.id,
                Status = model.transaction.status,
                DevReference = model.transaction.dev_reference,
                CreatedOn = DateTime.Now,
                Processed = model.transaction.carrier_code == "00",
                PaymentMethodType = model.transaction.payment_method_type,
                LastModifiedOn = DateTime.Now,
                OriginId = model?.ECommerce?.Id,
                StatusDetail = model.transaction.status_detail
            };

        }

        internal static string TypeCardName(string type)
        {
            var name = "";
            switch (type)
            {
                case "vi":
                    name = "Visa";
                    break;
                case "mc":
                    name = "Mastercard";
                    break;
                case "ax":
                    name = "American Express";
                    break;
                case "di":
                    name = "Diners";
                    break;
                case "dc":
                    name = "Discover";
                    break;
                case "ms":
                    name = "Maestro";
                    break;
                case "cs":
                    name = "Credisensa";
                    break;
                case "so":
                    name = "Solidario";
                    break;
                case "up":
                    name = "Union Pay";
                    break;

            }
            return name;
        }

        internal static string GetStatusDetail(StatusDetailEnum? status)
        {
            var statusDetail = "info";
            if (status.HasValue)
            {
                switch (status.Value)
                {

                    case StatusDetailEnum.Esperando_Pagada:
                        statusDetail = "Esperando para ser Pagada.";
                        break;
                    case StatusDetailEnum.Sección_Verificar:
                        statusDetail = "Se requiere verificación, por favor revise la sección de Verificar";
                        break;
                    case StatusDetailEnum.Pagada_Parcialmente:
                        statusDetail = "Pagada Parcialmente";
                        break;
                    case StatusDetailEnum.Pagada:
                        statusDetail = "Pagada";
                        break;
                    case StatusDetailEnum.En_Disputa:
                        statusDetail = "En Disputa";
                        break;
                    case StatusDetailEnum.Sobrepagada:
                        statusDetail = "Sobrepagada";
                        break;
                    case StatusDetailEnum.Fraude:
                        statusDetail = "Fraude";
                        break;
                    case StatusDetailEnum.Reverso:
                        statusDetail = "Reverso";
                        break;
                    case StatusDetailEnum.Contracargo:
                        statusDetail = "Contracargo";
                        break;
                    case StatusDetailEnum.Rechazada_procesador:
                        statusDetail = "Rechazada por el procesador";
                        break;
                    case StatusDetailEnum.Error_sistema:
                        statusDetail = "Error en el sistema";
                        break;
                    case StatusDetailEnum.Detectado_Fraude:
                        statusDetail = "Fraude detectado por Paymentez";
                        break;
                    case StatusDetailEnum.Blacklist_Paymentez:
                        statusDetail = "Blacklist de Paymentez";
                        break;
                    case StatusDetailEnum.Tiempo_tolerancia:
                        statusDetail = "Tiempo de tolerancia";
                        break;
                    case StatusDetailEnum.Expirada_Paymentez:
                        statusDetail = "Expirada por Paymentez";
                        break;
                    case StatusDetailEnum.Expirado_carrier:
                        statusDetail = "Expirado por el carrier";
                        break;
                    case StatusDetailEnum.Rechazado_Paymentez:
                        statusDetail = "Rechazado por Paymentez";
                        break;
                    case StatusDetailEnum.Abandonada_Paymentez:
                        statusDetail = "Abandonada por Paymentez";
                        break;
                    case StatusDetailEnum.Abandonada_cliente:
                        statusDetail = "Abandonada por el cliente";
                        break;
                    case StatusDetailEnum.Código_autorización_invalido:
                        statusDetail = "Código de autorización invalido";
                        break;
                    case StatusDetailEnum.Código_autorización_expirado:
                        statusDetail = "Código de autorización expirado";
                        break;
                    case StatusDetailEnum.Paymentez_Fraude:
                        statusDetail = "Fraude Paymentez - Reverso pendiente";
                        break;
                    case StatusDetailEnum.AuthCode_Inválido:
                        statusDetail = "AuthCode Inválido - Reverso pendiente";
                        break;
                    case StatusDetailEnum.AuthCode_Expirado:
                        statusDetail = "AuthCode Expirado - Reverso pendiente";
                        break;
                    case StatusDetailEnum.Fraude_Paymentez:
                        statusDetail = "Fraude Paymentez - Reverso solicitado";
                        break;
                    case StatusDetailEnum.Inválido_AuthCode:
                        statusDetail = "AuthCode Inválido - Reverso solicitado";
                        break;
                    case StatusDetailEnum.Expirado_AuthCode:
                        statusDetail = "AuthCode Expirado - Reverso solicitado";
                        break;
                    case StatusDetailEnum.Comercio_pendiente:
                        statusDetail = "Comercio - Reverso pendiente";
                        break;
                    case StatusDetailEnum.Comercio_solicitado:
                        statusDetail = "Comercio - Reverso solicitado";
                        break;
                    case StatusDetailEnum.Anulada:
                        statusDetail = "Anulada";
                        break;
                    case StatusDetailEnum.Transacción_asentada:
                        statusDetail = "Transacción asentada";
                        break;
                    case StatusDetailEnum.Esperando_OTP:
                        statusDetail = "Esperando OTP";
                        break;
                    case StatusDetailEnum.OTP_validado_correctamente:
                        statusDetail = "OTP validado correctamente";
                        break;
                    case StatusDetailEnum.OTP_no_validado:
                        statusDetail = "OTP no validado";
                        break;
                    case StatusDetailEnum.Reverso_parcial:
                        statusDetail = "Reverso parcial";
                        break;
                    case StatusDetailEnum.Método3DS:
                        statusDetail = "Método 3DS solicitado, esperando para continuar";
                        break;
                    case StatusDetailEnum.Desafío3DS:
                        statusDetail = "Desafío 3DS solicitado, esperando el CRES";
                        break;
                    case StatusDetailEnum.Rechazada3DS:
                        statusDetail = "Rechazada por 3DS";
                        break;
                }
            }

            return statusDetail;
        }

    }

    public class transaction
    {
        public string id { get; set; }
    }

    public class response
    {
        public string status { get; set; }
        public string detail { get; set; }
    }

}