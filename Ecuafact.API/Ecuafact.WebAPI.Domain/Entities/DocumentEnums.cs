using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public enum DocumentTypeEnum : short
    {
        [EcuafactEnum("01", "FACTURA", "FA")]
        Invoice = 1,

        [EcuafactEnum("02", "NOTA DE VENTA", "NV")]
        SaleNote = 2,

        [EcuafactEnum("03", "LIQUIDACION DE COMPRAS", "LC")]
        PurchaseSettlement = 3,

        [EcuafactEnum("04", "NOTA DE CRÉDITO", "NC")]
        CreditNote = 4,

        [EcuafactEnum("05", "NOTA DE DÉBITO", "ND")]
        DebitNote = 5,

        [EcuafactEnum("06", "GUIA DE REMISIÓN", "GR")]
        ReferralGuide = 6,

        [EcuafactEnum("07", "COMPROBANTE DE RETENCIÓN", "RE")]
        RetentionReceipt = 7,

        [EcuafactEnum("08", "BOLETOS O ENTRADAS A ESPECTÁCULOS", "BO")]
        Ticket = 8
    }

    public enum IdentificationTypeEnum : short
    {
        [EcuafactEnum("05", "CEDULA")]
        IdentityCard = 1,

        [EcuafactEnum("04", "RUC")]
        RUC = 4,

        [EcuafactEnum("06", "PASAPORTE")]
        Passport = 6
    }
}
