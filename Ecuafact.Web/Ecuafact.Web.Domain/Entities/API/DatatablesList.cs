using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Domain.Entities 
{
    public class DatatableList<T>
    {
        /// <summary>
        /// List of Data
        /// </summary>
        public IEnumerable<T> data { get; private set; }

        /// <summary>
        /// Error Message
        /// </summary>
        public string error { get; set; }

        /// <summary>
        /// Numero de peticion Actual
        /// </summary>
        public int draw { get; set; } = 1;

        /// <summary>
        /// Current Page
        /// </summary>
        public int page { get; set; } = 1;


        /// <summary>
        /// Total Records at DB Table
        /// </summary>
        public int recordsTotal { get; set; } = 0;

        /// <summary>
        /// Total Records filtered from DB Table
        /// </summary>
        public int recordsFiltered { get; set; } = 0;
        /// <summary>
        /// Class Style for Row when DataTable are rendered to HTML
        /// </summary>
        public string rowClass { get; set; } = "";
        /// <summary>
        /// Row Attributes when DataTables are rendered to HTML
        /// </summary>
        public string rowAttribute { get; set; } = "";
         
        /// <summary>
        /// Start Row number 
        /// </summary>
        public int start => (((page * length) - length) + 1);

        public bool more => (page * length) < recordsTotal;

        /// <summary>
        /// Rows per Page
        /// </summary>
        public int length => data?.Count() ?? 0;

        public DatatableList(IEnumerable<T> list)
        {
            data = list;
            recordsFiltered = recordsTotal = list.Count();
        }               

        public DatatableList(string errorMsg)
        {
            error = errorMsg;
        }
        public DatatableList() : this("No hay datos disponibles aún.")
        {
        }       
    }
}
