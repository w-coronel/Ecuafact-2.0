using Ecuafact.Web.Domain.Entities;
using Microsoft.Reporting.WebForms;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Reporting
{
    public class SalesReport : ReportBase<ObjectQueryModel<SalesReportResponse>>
    {
        public SalesReport(IssuerDto issuer, string path)
            : base(issuer, path)
        {
        }

        protected override string ReportName => "SalesReport";

        protected override List<ReportDataSource> GetDataSources(ObjectQueryModel<SalesReportResponse> document)
        {
            return new List<ReportDataSource>
            {
                new ReportDataSource("ReportsDataSet", document)
            };
        }

        public static byte[] GenerateExcel(ObjectQueryModel<SalesReportResponse> model)
        {
            var reportList = model.Data.Entity;
             
            using (var excelPkg = new ExcelPackage())
            {
                var Sheet = excelPkg.Workbook.Worksheets.Add("Ventas");
                
                Sheet.Cells[1, 1].Value = "REPORTE DE VENTAS";
                Sheet.Cells[1, 1].Style.Font.Size = 20;
                Sheet.Cells[1, 1, 1, 5].Merge = true;
                Sheet.Cells[1, 1, 1, 5].Style.Font.Color.SetColor(Color.Navy);

                Sheet.Cells[2, 1].Value = "Fecha Inicio:";
                Sheet.Cells[2, 1].Style.Font.Bold = true;
                Sheet.Cells[2, 2].Value = model.From;

                Sheet.Cells[2, 2, 2, 3].Merge = true;

                Sheet.Cells[2, 4].Value = "Fecha Fin:";
                Sheet.Cells[2, 4].Style.Font.Bold = true;
                Sheet.Cells[2, 5].Value = model.To;

                if (!string.IsNullOrEmpty(model.SearchTerm))
                {
                    Sheet.Cells[3, 1].Value = "Filtrado por:";
                    Sheet.Cells[3, 2].Value = model.SearchTerm;
                    Sheet.Cells[3, 2, 3, 5].Merge = true;
                    Sheet.Cells[3, 1, 3, 5].Style.Font.Color.SetColor(Color.Gray);
                }

                Sheet.Cells[5, 1].Value = "Codigo";
                Sheet.Cells[5, 2].Value = "Tipo";
                Sheet.Cells[5, 3].Value = "Establecimiento";
                Sheet.Cells[5, 4].Value = "Pto. Emision";
                Sheet.Cells[5, 5].Value = "Secuencial";
                Sheet.Cells[5, 6].Value = "No. Documento";
                Sheet.Cells[5, 7].Value = "Fecha Emision";

                Sheet.Cells[5, 8].Value = "RUC";
                Sheet.Cells[5, 9].Value = "Cliente";

                Sheet.Cells[5, 10].Value = "No. Autorizacion";
                Sheet.Cells[5, 11].Value = "Fecha Autorizacion";
                 
                Sheet.Cells[5, 12].Value = "Base 0%";
                Sheet.Cells[5, 13].Value = "Base Iva%";
                Sheet.Cells[5, 14].Value = "IVA";
                Sheet.Cells[5, 15].Value = "Total";
                Sheet.Cells[5, 16].Value = "Descripcion";
                
                Sheet.Cells[5, 17].Value = "Id Retencion";
                Sheet.Cells[5, 18].Value = "RUC Retencion";
                Sheet.Cells[5, 19].Value = "Nombre Retencion";
                Sheet.Cells[5, 20].Value = "Fecha Retencion";
                Sheet.Cells[5, 21].Value = "No. Retencion";
                Sheet.Cells[5, 22].Value = "Autorizacion Retencion";
                Sheet.Cells[5, 23].Value = "Tipo Ref.";
                Sheet.Cells[5, 24].Value = "No. Ref.";
                Sheet.Cells[5, 25].Value = "Fecha Ref.";
                Sheet.Cells[5, 26].Value = "Codigo 104";
                Sheet.Cells[5, 27].Value = "Base Renta";
                Sheet.Cells[5, 28].Value = "% Renta";
                Sheet.Cells[5, 29].Value = "Valor Renta";
                Sheet.Cells[5, 30].Value = "Codigo IVA";
                Sheet.Cells[5, 31].Value = "Base IVA";
                Sheet.Cells[5, 32].Value = "% IVA Ret.";
                Sheet.Cells[5, 33].Value = "IVA Retenido";
                Sheet.Cells[5, 34].Value = "Codigo ISD";
                Sheet.Cells[5, 35].Value = "Base ISD";
                Sheet.Cells[5, 36].Value = "% ISD Ret.";
                Sheet.Cells[5, 37].Value = "ISD Retenido";
                Sheet.Cells[5, 38].Value = "Motivo Retencion";
                Sheet.Cells[5, 39].Value = "Descripcion Retencion";
                Sheet.Cells[5, 40].Value = "Observaciones Retencion";
                Sheet.Cells[5, 41].Value = "Observaciones";



                const string percentFormat = "0 \"%\"";
                const string dateFormat = "dd/mm/yyyy";
                const string moneyFormat = "$ * #,##0.00;$ * - #,##0.00";


                Sheet.Column(7).Style.Numberformat.Format = dateFormat;

                Sheet.Column(12).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(13).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(14).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(15).Style.Numberformat.Format = moneyFormat;
                
                Sheet.Column(20).Style.Numberformat.Format = dateFormat;

                Sheet.Column(27).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(28).Style.Numberformat.Format = percentFormat;
                Sheet.Column(29).Style.Numberformat.Format = moneyFormat;

                Sheet.Column(31).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(32).Style.Numberformat.Format = percentFormat;
                Sheet.Column(33).Style.Numberformat.Format = moneyFormat;

                Sheet.Column(35).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(36).Style.Numberformat.Format = percentFormat;
                Sheet.Column(37).Style.Numberformat.Format = moneyFormat;


                int row = 6;

                foreach (var item in reportList)
                {
                    Sheet.Cells[row, 1].Value = item.DocumentTypeCode;
                    Sheet.Cells[row, 2].Value = item.DocumentType;
                    Sheet.Cells[row, 3].Value = item.EstablishmentCode;
                    Sheet.Cells[row, 4].Value = item.IssuePointCode;
                    Sheet.Cells[row, 5].Value = item.Sequential;
                    Sheet.Cells[row, 6].Value = item.DocumentNumber;
                    Sheet.Cells[row, 7].Value = item.IssuedOn;

                    Sheet.Cells[row, 8].Value = item.ContributorRUC;
                    Sheet.Cells[row, 9].Value = item.ContributorName;

                    Sheet.Cells[row, 10].Value = item.AuthorizationNumber;
                    Sheet.Cells[row, 11].Value = item.AuthorizationDate;
                    
                    Sheet.Cells[row, 12].Value = item.Base0;
                    Sheet.Cells[row, 13].Value = item.Base12;
                    Sheet.Cells[row, 14].Value = item.IVA;
                    Sheet.Cells[row, 15].Value = item.Total;
                    Sheet.Cells[row, 16].Value = item.Description;
                    
                    Sheet.Cells[row, 17].Value = item.RetentionId;

                    Sheet.Cells[row, 18].Value = item.RetentionContributorRUC;
                    Sheet.Cells[row, 19].Value = item.RetentionContributor;
                    Sheet.Cells[row, 20].Value = item.RetentionIssuedOn;

                    Sheet.Cells[row, 21].Value = item.RetentionNumber;
                    Sheet.Cells[row, 22].Value = item.RetentionAuthorizationNumber;
                    Sheet.Cells[row, 23].Value = item.RetentionReferenceType;
                    Sheet.Cells[row, 24].Value = item.RetentionReferenceNumber;
                    Sheet.Cells[row, 25].Value = item.RetentionReferenceDate;

                    Sheet.Cells[row, 26].Value = item.Retention104TaxCode;
                    Sheet.Cells[row, 27].Value = item.RetentionTaxBase;
                    Sheet.Cells[row, 28].Value = item.RetentionTaxRate;
                    Sheet.Cells[row, 29].Value = item.RetentionTaxValue;

                    Sheet.Cells[row, 30].Value = item.RetentionVatCode;
                    Sheet.Cells[row, 31].Value = item.RetentionVatBase;
                    Sheet.Cells[row, 32].Value = item.RetentionVatRate;
                    Sheet.Cells[row, 33].Value = item.RetentionVatValue;

                    Sheet.Cells[row, 34].Value = item.RetentionISDCode;
                    Sheet.Cells[row, 35].Value = item.RetentionISDBase;
                    Sheet.Cells[row, 36].Value = item.RetentionISDRate;
                    Sheet.Cells[row, 37].Value = item.RetentionISDValue;

                    Sheet.Cells[row, 38].Value = item.RetentionReason;
                    Sheet.Cells[row, 39].Value = item.RetentionDescription;
                    Sheet.Cells[row, 40].Value = item.RetentionNotes;

                    Sheet.Cells[row, 41].Value = item.Notes;


                    row++;
                }


                var table = Sheet.Tables.Add(new ExcelAddressBase(5, 1, row - 1, 41), "Ventas");
                table.ShowFilter = false;
                table.ShowHeader = true;
                table.ShowRowStripes = true;
                table.TableStyle = TableStyles.Medium2;

                Sheet.Cells["A:AZ"].AutoFitColumns();

                Sheet.Column(17).Hidden = true;
                Sheet.Column(18).Hidden = true;
                Sheet.Column(19).Hidden = true;

                Sheet.Column(34).Hidden = true;
                Sheet.Column(35).Hidden = true;
                Sheet.Column(36).Hidden = true;
                Sheet.Column(37).Hidden = true;

                Sheet.Column(41).Hidden = true;
                Sheet.Column(41).Width = 20;

                return excelPkg.GetAsByteArray();
            }


        }
    }
}
