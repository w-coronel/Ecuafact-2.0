
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ecuafact.Web.Domain.Entities
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

        [Display(Name = "Año")]
        public string Year { get; set; } = "";

        [Display(Name = "Mes")]
        public string Month { get; set; } = "";

        [Display(Name = "Establecimiento")]
        public string EstablishmentCode { get; set; } = "";

        [Display(Name = "Punto Emisión")]
        public string IssuePointCode { get; set; } = "";

        [Display(Name = "Periodo")]
        public int Period { get; set; } = 0;        

        public DocumentStatusEnum? Status { get; set; }

        public TModelType Data { get; set; } = default(TModelType);

        public string FormatType { get; set; } = "excel";
    }



    public class DocumentosQueryModel : ObjectQueryModel<IEnumerable<DocumentModel>>
    {
        [Display(Name = "Estado del Documento")]
        public DocumentStatusEnum? Status { get; set; } = null;

        public DocumentosQueryModel()
        {
            Data = new List<DocumentModel>();
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


    public class ImportDataModel
    {       

        /// <summary>
        /// Formato del archivo txt
        /// </summary>    
        public string FormatType { get; set; }

        [JsonIgnore]
        public HttpPostedFileBase ImportDataFile { get; set; }

        /// <summary>
        /// Archivo importar 
        /// </summary>   
        [Required]
        public byte[] FileImportRaw => fileimportRaw ?? (fileimportRaw = ImportDataFile.GetBytes());

        private byte[] fileimportRaw;
    }
}
