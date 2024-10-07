
using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.MiddleCore.NexusApiServices;
using Newtonsoft.Json;

namespace Ecuafact.Web.Models
{
    public class RecibidosQueryModel : ObjectQueryModel<DocumentResponse>
    {
        public string DeductibleType { get; set; } = "-1";

        public RecibidosQueryModel()
        {
            Data = new DocumentResponse();
        }
    }

}