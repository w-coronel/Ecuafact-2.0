using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Entities
{
    public class ObjectQueryModel<TModelType>
    {
        public ObjectQueryModel() { }

        public ObjectQueryModel(TModelType model)
        {
            Data = model;
        }

        [Display(Name = "Tipo")]
        public string QueryType { get; set; } = "0";

        [Display(Name = "Desde")]
        public string From { get; set; } = DateTime.Today.Date.AddDays(-29).ToString("dd/MM/yyyy");

        [Display(Name = "Hasta")]
        public string To { get; set; } = DateTime.Today.Date.AddHours(23).AddMinutes(59).ToString("dd/MM/yyyy");

        [Display(Name = "Tipo Comprobante")]
        public string DocumentType { get; set; } = "";

        [Display(Name = "Buscar")]
        public string SearchTerm { get; set; } = "";

        public DocumentStatusEnum? Status { get; set; }

        public TModelType Data { get; set; } = default(TModelType);
    }



    public class DocumentosQueryModel : ObjectQueryModel<IEnumerable<Document>>
    {
        [Display(Name = "Estado del Documento")]
        public DocumentStatusEnum? Status { get; set; } = null;

        public DocumentosQueryModel()
        {
            Data = new List<Document>();
        }

        public DocumentosQueryModel(string documentType, DocumentStatusEnum? status = null) : this()
        {
            this.DocumentType = documentType;
            this.Status = status;
        }
    }


    public class ObjectQueryModel : ObjectQueryModel<object>
    {
        public ObjectQueryModel() : base() { }
        public ObjectQueryModel(object model) : base(model) { }
    }
}
