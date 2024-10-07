using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{
    public enum TipoDocumento : short
    {
        [EcuafactEnum("01", "FACTURA")]
        Factura = 1,
        [EcuafactEnum("02", "NOTA DE VENTA")]
        NotaDeVenta = 2,
        [EcuafactEnum("03", "LIQUIDACION DE COMPRAS")]
        Liquidacion = 3,
        [EcuafactEnum("04", "NOTA DE CRÉDITO")]
        NotaDeCredito = 4,
        [EcuafactEnum("05", "NOTA DE DÉBITO")]
        NotaDeDebito = 5,
        [EcuafactEnum("06", "GUIA DE REMISIÓN")]
        GuiaRemision = 6,
        [EcuafactEnum("07", "COMPROBANTE DE RETENCIÓN")]
        ComprobanteDeRetencion = 7,
        [EcuafactEnum("08", "BOLETOS O ENTRADAS A ESPECTÁCULOS")]
        Boletos = 8
    }

    public enum TipoIdentificacion: short
    {
        [EcuafactEnum("05", "CÉDULA")]
        Cedula=1,
        [EcuafactEnum("04", "RUC")]
        Ruc,
        [EcuafactEnum("06", "PASAPORTE")]
        Pasaporte
    }
}
