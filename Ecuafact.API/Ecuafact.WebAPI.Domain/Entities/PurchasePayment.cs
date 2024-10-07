using Ecuafact.WebAPI.Entities.VPOS2;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class PurchasePayment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int Id { get; set; }

        [ForeignKey("PurchaseOrder")]
        public int PurchaseOrderId { get; set; }
        
        public string AcquirerId { get; set; }
        public string IdCommerce { get; set; }
        
        public decimal PurchaseAmount { get; set; }
        public string PurchaseCurrencyCode { get; set; }

        public string PurchaseVerification { get; set; }

        public OperationResultEnum? AuthorizationResult { get; set; }
        public string AuthorizationCode { get; set; }
        public string CardNumber { get; set; }

        public string UserCodePayme { get; set; }
        
        public string AuthenticationECI { get; set; }
        public string BillingCountry { get; set; }
        public string BillingState { get; set; }
        public string CardType { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string Language { get; set; }
        public string PurchaseIPAddress { get; set; }
        public string Reserved1 { get; set; }
        public string Reserved11 { get; set; }
        public string Reserved12 { get; set; }
        public string Reserved2 { get; set; }
        public string Reserved3 { get; set; }
        public string Reserved4 { get; set; }
        public string Reserved5 { get; set; }
        public string Reserved6 { get; set; }
        public string Reserved9 { get; set; }       
        public string ShippingAddress { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingCountry { get; set; }
        public string ShippingEMail { get; set; }
        public string ShippingFirstName { get; set; }
        public string ShippingLastName { get; set; }
        public string ShippingState { get; set; }
        public string ShippingZIP { get; set; }
        public string TerminalCode { get; set; }
        public bool Processed { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        public PurchaseOrder PurchaseOrder { get; set; }

        public string Result { get; set; }

        public long? OriginId { get; set; }
        public string Status { get; set; }
        public StatusDetailEnum? StatusDetail { get; set; }
        public string TransactionId { get; set; }
        public string PaymentMethodType { get; set; }
        public string DevReference { get; set; }
        public string Carrier { get; set; }
        public string Installments { get; set; }
        public string InstallmentsType { get; set; }
        public string Origin { get; set; }
        public bool? Refund { get; set; }
    }

    public enum StatusDetailEnum : short
    {
        [EcuafactEnum("0", "Esperando para ser Pagada.")]
        Esperando_Pagada = 0,
        [EcuafactEnum("1", "Se requiere verificación, por favor revise la sección de Verificar")]
        Sección_Verificar = 1,
        [EcuafactEnum("2", "Pagada Parcialmente")]
        Pagada_Parcialmente = 2,
        [EcuafactEnum("3", "Pagada")]
        Pagada = 3,
        [EcuafactEnum("4", "En Disputa")]
        En_Disputa = 4,
        [EcuafactEnum("5", "Sobrepagada")]
        Sobrepagada = 5,
        [EcuafactEnum("6", "Fraude")]
        Fraude = 6,
        [EcuafactEnum("7", "Reverso")]
        Reverso = 7,
        [EcuafactEnum("8", "Contracargo")]
        Contracargo = 8,
        [EcuafactEnum("9", "Rechazada por el procesador")]
        Rechazada_procesador = 9,
        [EcuafactEnum("10", "Error en el sistema")]
        Error_sistema = 10,
        [EcuafactEnum("11", "Fraude detectado por Paymentez")]
        Detectado_Fraude = 11,
        [EcuafactEnum("12", "Blacklist de Paymentez")]
        Blacklist_Paymentez = 12,
        [EcuafactEnum("13", "Tiempo de tolerancia")]
        Tiempo_tolerancia = 13,
        [EcuafactEnum("14", "Expirada por Paymentez")]
        Expirada_Paymentez = 14,
        [EcuafactEnum("15", "Expirado por el carrier")]
        Expirado_carrier = 15,
        [EcuafactEnum("16", "Rechazado por Paymentez")]
        Rechazado_Paymentez = 16,
        [EcuafactEnum("17", "Abandonada por Paymentez")]
        Abandonada_Paymentez = 17,
        [EcuafactEnum("18", "Abandonada por el cliente")]
        Abandonada_cliente = 18,
        [EcuafactEnum("19", "Código de autorización invalido")]
        Código_autorización_invalido = 19,
        [EcuafactEnum("20", "Código de autorización expirado")]
        Código_autorización_expirado = 20,
        [EcuafactEnum("21", "Fraude Paymentez - Reverso pendiente")]
        Paymentez_Fraude = 21,
        [EcuafactEnum("22", "AuthCode Inválido - Reverso pendiente")]
        AuthCode_Inválido = 22,
        [EcuafactEnum("23", "AuthCode Expirado - Reverso pendiente")]
        AuthCode_Expirado = 23,
        [EcuafactEnum("24", "Fraude Paymentez - Reverso solicitado")]
        Fraude_Paymentez = 24,
        [EcuafactEnum("25", "AuthCode Inválido - Reverso solicitado")]
        Inválido_AuthCode = 25,
        [EcuafactEnum("26", "AuthCode Expirado - Reverso solicitado")]
        Expirado_AuthCode = 26,
        [EcuafactEnum("27", "Comercio - Reverso pendiente")]
        Comercio_pendiente = 27,
        [EcuafactEnum("28", "Comercio - Reverso solicitado")]
        Comercio_solicitado = 28,
        [EcuafactEnum("29", "Anulada")]
        Anulada = 29,
        [EcuafactEnum("30", "Transacción asentada")]
        Transacción_asentada = 30,
        [EcuafactEnum("31", "Esperando OTP")]
        Esperando_OTP = 31,
        [EcuafactEnum("32", "OTP validado correctamente")]
        OTP_validado_correctamente = 32,
        [EcuafactEnum("33", "OTP no validado")]
        OTP_no_validado = 33,
        [EcuafactEnum("34", "Reverso parcial")]
        Reverso_parcial = 34,
        [EcuafactEnum("35", "Método 3DS solicitado, esperando para continuar")]
        Método3DS = 35,
        [EcuafactEnum("36", "Desafío 3DS solicitado, esperando el CRES")]
        Desafío3DS = 36,
        [EcuafactEnum("37", "Rechazada por 3DS")]
        Rechazada3DS = 37
    }


}
