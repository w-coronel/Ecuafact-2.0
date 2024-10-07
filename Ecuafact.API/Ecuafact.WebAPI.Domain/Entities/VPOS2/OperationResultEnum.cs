using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Entities.VPOS2
{
    public enum OperationResultEnum : short
    {
        Registrado = 0,
        Invalido = 1,
        Denegado = 2,
        Autorizado = 3,
        Eliminado = 4,
        Depositado = 5,
        Anulado = 6,
        Liquidado = 7,
        Extornado = 8,
        Desaprobado = 9,
        DepositadoDevolucion = 10,
        AnuladoDevolucion = 11,
        LiquidadoDevolucion = 12,
        Incompleto = 13,
        ExtornoFallido = 14,
        AutorizadoSinLiquidacion = 15,
        Contracargado = 16
    }
    
}
