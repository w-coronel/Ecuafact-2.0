using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{
    public class DashboardInfo
    {
        /// <summary>
        /// Emisor Id
        /// </summary>
        public long IssuerId { get; set; } = 0;

        /// <summary>
        /// Cantidad de Documentos Emitidos
        /// </summary>
        public long DocumentsCount { get; set; } = 0;

        /// <summary>
        /// Cantidad de Documentos Borradores
        /// </summary>
        public long DraftsCount { get; set; } = 0;

        /// <summary>
        /// Cantidad de Productos
        /// </summary>
        public long ProductsCount { get; set; } = 0;

        /// <summary>
        /// Cantidad de Clientes
        /// </summary>
        public long CustomersCount { get; set; } = 0;

        /// <summary>
        /// Cantidad de Facturas efectuadas
        /// </summary>
        public long InvoicesCount { get; set; } = 0;

        /// <summary>
        /// Cantidad de Notas de Credito efectuadas
        /// </summary>
        public long CreditsCount { get; set; } = 0;

        /// <summary>
        /// Cantidad de Notas de Debito efectuadas
        /// </summary>
        public long DebitsCount { get; set; } = 0;

        /// <summary>
        /// Cantidad de Guias de remision efectuadas
        /// </summary>
        public long ReferralsCount { get; set; } = 0;

        /// <summary>
        /// Cantidad de Retenciones efectuadas
        /// </summary>
        public long RetentionsCount { get; set; } = 0;

        /// <summary>
        /// Cantidad de Liquidación de compras
        /// </summary>
        public long SettlementsCount { get; set; } = 0;        

        /// <summary>
        /// Total Facturado
        /// </summary>
        public decimal TotalSales { get; set; } = 0;
        /// <summary>
        /// Total Notas de Credito
        /// </summary>
        public decimal TotalCredits { get; set; } = 0;
        /// <summary>
        /// Total Debitos
        /// </summary>
        public decimal TotalDebits { get; set; } = 0;
        /// <summary>
        /// Total Retenciones
        /// </summary>
        public decimal TotalRetentions { get; set; } = 0;
        /// <summary>
        /// Total Guias de Remision
        /// </summary>
        public decimal TotalReferrals { get; set; } = 0;

        /// <summary>
        /// Total Liquidación de Compras
        /// </summary>
        public decimal TotalSettlements { get; set; } = 0;
    }
}
