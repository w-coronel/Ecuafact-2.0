using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.PayMe
{

    public class WalletCustomerResult
    {

        /// <summary>
        /// Codigo de Respuesta
        /// </summary>
        public string StatusCode { get; internal set; }

        /// <summary>
        /// Descripcion de la Respuesta
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// Codigo de Asociacion de la Tarjeta en el Servicio Wallet
        /// </summary>
        public string CodAsoCardHolderWallet { get; internal set; }

        /// <summary>
        /// Fecha de Registro
        /// </summary>
        public string Date { get; internal set; }

        /// <summary>
        /// Hora de Registro
        /// </summary>
        public string Hour { get; internal set; }  
    }

}
