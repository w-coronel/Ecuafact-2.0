using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities
{
    public class AdditionalFieldModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public int LineNumber { get; set; }
        public short? IsCarrier { get; set; }        
    }

    public enum TextoInfoAdicionalEnum : short
    {
        [EcuafactEnum("Agente de Retención", "Agente de Retención")]
        IsRetentionAgent = 1,
        [EcuafactEnum("Contribuyente Régimen RIMPE", "Rimpe")]
        IsRimpe = 2,
        [EcuafactEnum("Contribuyente Régimen General", "General")]
        IsGeneralRegime = 3,
        [EcuafactEnum("Régimen Sociedades Simplificadas", "Sociedades Simplificadas")]
        IsSimplifiedCompaniesRegime = 4,
        [EcuafactEnum("Artesano Calificado Nº", "Artesano Calificado Nº")]
        IsSkilledCraftsman = 5,
        [EcuafactEnum("Contribuyente Negocio Popular - Regimen RIMPE", "Negocio Popular")]
        IsPopularBusiness = 6,
    }
}
