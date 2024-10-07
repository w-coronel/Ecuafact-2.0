using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public enum DocumentStatusEnum : short
    {
        /// <summary>
        /// Borrador
        /// </summary>
        [EcuafactEnum("0", "Borrador")]
        Draft = 0,
        
        /// <summary>
        /// Emitido
        /// </summary>
        [EcuafactEnum("1", "Emitido")]
        Issued = 1,
        
        /// <summary>
        /// Error
        /// </summary>
        [EcuafactEnum("10", "Error")]
        Error = 10,

        /// <summary>
        /// Eliminado
        /// </summary>
        [EcuafactEnum("-1", "Eliminado")]
        Deleted = -1,

        /// <summary>
        /// Anulado
        /// </summary>
        [EcuafactEnum("-77", "Anulado")]
        Revoked = -77,
        
        /// <summary>
        /// Autorizado
        /// </summary>
        [EcuafactEnum("100", "Validado SRI")]
        Validated = 100,

        /// <summary>
        /// Autorizado
        /// </summary>
        [EcuafactEnum("101", "Autorizado Offline")]
        Authorized = 101
    }

    public enum NewDocumentStatusEnum : short
    {
        /// <summary>
        /// Borrador
        /// </summary>
        [EcuafactEnum("0", "Borrador")]
        Draft = 0,

        /// <summary>
        /// Emitido
        /// </summary>
        [EcuafactEnum("1", "Emitido")]
        Issued = 1
    }
}
