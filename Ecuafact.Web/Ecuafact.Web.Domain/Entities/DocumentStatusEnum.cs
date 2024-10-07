using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{
    /// <summary>
    /// Document Status
    /// </summary>
    public enum DocumentStatusEnum : short
    {
        /// <summary>
        /// Borrador
        /// </summary>
        [EcuafactEnum("0", "Borrador", "info")]
        Draft = 0,
        /// <summary>
        /// Enviado
        /// </summary>
        [EcuafactEnum("1", "En Proceso", "warning")]
        Issued = 1,
        /// <summary>
        /// Error
        /// </summary>
        [EcuafactEnum("10", "Error", "danger")]
        Error = 10,
        /// <summary>
        /// Anulado
        /// </summary>
        [EcuafactEnum("-77", "Anulado", "dark")]
        Revoked = -77, 
        /// <summary>
        /// Anulado
        /// </summary>
        [EcuafactEnum("-1", "Eliminado", "light")]
        Deleted = -1,
        /// <summary>
        /// Validated
        /// </summary>
        [EcuafactEnum("100", "Validado SRI", "success")]
        Validated = 100,
        /// <summary>
        /// Autorizado
        /// </summary>
        [EcuafactEnum("101", "Autorizado Offline", "info")]
        Authorized = 101
    }

    public struct  AnulacionMsg
    {
        #region Métodos

        public static string MensajeStatus(DocumentStatusEnum? status)
        {
            var msg = "";
            if (DocumentStatusEnum.Draft == status)
            {
                msg = "<small>(Este proceso no se puede revertir) </small>";
            }
            else if (DocumentStatusEnum.Validated == status)
            {
                msg = "<p>Estimado cliente antes de anular un documento en Ecuafact, previamente debe anular el documento en la plataforma del " +
                       $"<a href='{Constants.SriUrl}' target='_blank'>SRI</a>. " +
                       "Si ya lo realizó de clic en continuar, caso contrario " +
                       "en cancelar</p > <br>";
            }

            return msg;
        }
        #endregion 
    }


}
