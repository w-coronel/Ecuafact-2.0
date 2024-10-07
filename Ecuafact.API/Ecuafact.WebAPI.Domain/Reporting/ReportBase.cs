using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.WebAPI.Domain.Reporting
{
    public abstract class ReportBase<TDocument>
    {
        protected List<ReportParameter> Parameters { get; set; } = new List<ReportParameter>();

        protected abstract string ReportName { get; }
         
        protected abstract List<ReportDataSource> GetDataSources(TDocument document);

        public ReportResult Render(TDocument document, string reportType)
        {
            // Cargamos los reportes personalizados para este cliente
            string reportPath = Path.Combine(Constants.ServerPath, "Reports", $"{ReportName}.rdlc");
              
            var viewer = new ReportViewer
            {
                ProcessingMode = ProcessingMode.Local
            };
             
            if (File.Exists(reportPath) && document != null)
            {
                viewer.LocalReport.ReportPath = reportPath;
                viewer.LocalReport.EnableExternalImages = true;
                
                if (Parameters.Count > 0)
                {
                    viewer.LocalReport.SetParameters(Parameters);
                }

                viewer.LocalReport.DataSources.Clear();

                // Cargamos los data sources desde un dataset.

                var dataSources = GetDataSources(document);
                foreach (var item in dataSources)
                {
                    viewer.LocalReport.DataSources.Add(item);
                }

                viewer.LocalReport.Refresh();
                string deviceInfo = string.Empty;

                if (reportType.ToUpper().Contains("POS")){
                    var pageWidth = reportType.ToUpper().Contains("POS72") ? "3.3in" : "2.6in";
                    reportType = "PDF";                   
                    deviceInfo ="<DeviceInfo>" +
                    " <OutputFormat>" + reportType + "</OutputFormat>" +
                    $" <PageWidth>{pageWidth}</PageWidth>" +
                    " <PageHeight>11in</PageHeight>" +
                    " <MarginTop>0.5cm</MarginTop>" +
                    " <MarginRight>0.5cm</MarginRight>" +
                    " <MarginLeft>0.5cm</MarginLeft>" +
                    " <MarginBottom>0.5cm</MarginBottom>" +
                    "</DeviceInfo>";
                }
                else {
                    deviceInfo ="<DeviceInfo>" +
                    " <OutputFormat>" + reportType + "</OutputFormat>" +
                    " <PageWidth>8.5in</PageWidth>" +
                    " <PageHeight>11in</PageHeight>" +
                    " <MarginTop>1cm</MarginTop>" +
                    " <MarginRight>1cm</MarginRight>" +
                    " <MarginLeft>1cm</MarginLeft>" +
                    " <MarginBottom>1cm</MarginBottom>" +
                    "</DeviceInfo>";
                }
                

                string mimeType;
                string encoding;
                string fileNameExt;
                string[] streams;
                Warning[] warnings;


                byte[] content = viewer.LocalReport.Render(reportType, deviceInfo,
                    out mimeType,
                    out encoding,
                    out fileNameExt,
                    out streams,
                    out warnings);

                var result = new ReportResult(content)
                {
                    MimeType = mimeType,
                    Encoding = encoding,
                    Extension = fileNameExt,
                    Streams = streams,
                    Warnings = warnings
                };

                return result;
            }

            return null;

        }
         
    }
}
