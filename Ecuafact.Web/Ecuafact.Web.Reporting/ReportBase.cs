using Ecuafact.Web.Domain.Entities;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Reporting
{
    public abstract class ReportBase<TDocument>
    {
        public ReportBase(IssuerDto issuer, string path = null)
        {
            this.Issuer = issuer;
            this.LocalPath = path;
        }

        protected abstract string ReportName { get; }

        public string LocalPath { get; set; }

        public IssuerDto Issuer { get; private set; }

        protected abstract List<ReportDataSource> GetDataSources(TDocument document);
         
        public ReportResult Render(TDocument document,  string reportType)
        {
            // Cargamos los reportes personalizados para este cliente
            string reportPath = Path.Combine(LocalPath, "Reports", $"{ReportName}_{Issuer.RUC}.rdlc");

            if (!File.Exists(reportPath))
            {
                reportPath = Path.Combine(LocalPath, "Reports", $"{ReportName}.rdlc");
            }
             
            var viewer = new ReportViewer
            {
                ProcessingMode = ProcessingMode.Local
            };


            if (File.Exists(reportPath) && document != null)
            {
                viewer.LocalReport.ReportPath = reportPath;
                 
                viewer.LocalReport.EnableExternalImages = true;
                viewer.LocalReport.DataSources.Clear();

                // Cargamos los data sources desde un dataset.
                var dataSources = GetDataSources(document);
                foreach (var item in dataSources)
                {
                    viewer.LocalReport.DataSources.Add(item);
                }

                viewer.LocalReport.DataSources.Add(new ReportDataSource("DsLogo", new[] { new { Ruc = Issuer.RUC, Logo = GetIssuerLogo() } }));
                 
                viewer.LocalReport.Refresh();

                string deviceInfo =
                    "<DeviceInfo>" +
                    " <OutputFormat>" + reportType + "</OutputFormat>" +
                    " <PageWidth>8.5in</PageWidth>" +
                    " <PageHeight>11in</PageHeight>" +
                    " <MarginTop>1cm</MarginTop>" +
                    " <MarginRight>1cm</MarginRight>" +
                    " <MarginLeft>1cm</MarginLeft>" +
                    " <MarginBottom>1cm</MarginBottom>" +
                    "</DeviceInfo>";

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
                    Enconding = encoding,
                    Extension = fileNameExt,
                    Streams = streams,
                    Warnings = warnings
                };

                return result;
            }

            return null;

        }
        
        protected byte[] GetIssuerLogo()
        {
            string logoIssuerFile = Path.Combine(LocalPath, "Logos", $"{Issuer.RUC}_logo.jpg");

            if (!File.Exists(logoIssuerFile))
            {
                logoIssuerFile = Path.Combine(LocalPath, "Logos", $"no_logo.jpg");
            }

            byte[] logoInfo = null;

            if (File.Exists(logoIssuerFile))
            {
                try // Si hubo error no se muestra la imagen
                {
                    // Tamaño predeterminado del logo es 200x100 pixeles
                    logoInfo = Image.FromFile(logoIssuerFile)?.ScaleImage(800, 400);
                }
                catch
                {
                    logoInfo = Image.FromFile(logoIssuerFile).ToStream().ToArray();
                }
            }

            return logoInfo;
        }
         

    }
}
