using Ecuafact.Web.Domain.Entities;
using Ecuafact.Web.MiddleCore.NexusApiServices;
using Microsoft.Reporting.WebForms;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ecuafact.Web.Reporting
{
    public static class ExportsExtensions
    { 
        public static byte[] ToExcel(this DocumentosQueryModel model)
        {
            var documents = model?.Data ?? new List<DocumentModel>();
             
            using (var excelPkg = new ExcelPackage())
            {
                var Sheet = excelPkg.Workbook.Worksheets.Add("Emitidos");
                
                Sheet.Cells[1, 1].Value = "Documentos Emitidos";
                Sheet.Cells[1, 1].Style.Font.Size = 20;
                Sheet.Cells[1, 1, 1, 5].Merge = true;
                Sheet.Cells[1, 1, 1, 5].Style.Font.Color.SetColor(Color.Navy);

                Sheet.Cells[5, 1].Value = "ID";
                Sheet.Cells[5, 2].Value = "Fecha Emision";
                Sheet.Cells[5, 3].Value = "Cod. Tipo";
                Sheet.Cells[5, 4].Value = "Tipo Documento";
                Sheet.Cells[5, 5].Value = "No. Documento";
                Sheet.Cells[5, 6].Value = "Identificacion";
                Sheet.Cells[5, 7].Value = "Razon Social";

                Sheet.Cells[5, 8].Value = "Subtotal 0%";
                Sheet.Cells[5, 9].Value = "Subtotal 12%";
                Sheet.Cells[5, 10].Value = "Descuento";
                Sheet.Cells[5, 11].Value = "Subtotal";
                Sheet.Cells[5, 12].Value = "Tipo IVA";
                Sheet.Cells[5, 13].Value = "IVA";
                Sheet.Cells[5, 14].Value = "Total";
                Sheet.Cells[5, 15].Value = "Estado";
                Sheet.Cells[5, 16].Value = "Num. Autorizacion";
                Sheet.Cells[5, 17].Value = "Fecha Autorizacion";
                Sheet.Cells[5, 18].Value = "Clave Acceso"; 
                Sheet.Cells[5, 19].Value = "MSG"; 

                const string stringFormat = "@";
                //const string percentFormat = "0 \"%\"";
                const string dateFormat = "dd/mm/yyyy";
                const string moneyFormat = "$ * #,##0.00;$ * - #,##0.00";

                Sheet.Column(2).Style.Numberformat.Format = dateFormat;

                Sheet.Column(5).Style.Numberformat.Format = stringFormat;
                Sheet.Column(6).Style.Numberformat.Format = stringFormat;

                
                Sheet.Column(8).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(9).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(10).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(11).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(12).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(13).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(14).Style.Numberformat.Format = moneyFormat;

                Sheet.Column(16).Style.Numberformat.Format = stringFormat;
                Sheet.Column(17).Style.Numberformat.Format = dateFormat;
                Sheet.Column(18).Style.Numberformat.Format = stringFormat;
                 
                int row = 6;

                foreach (var document in documents)
                {
                    Sheet.Cells[row, 1].Value = document.Id;
                    Sheet.Cells[row, 2].Value = document.IssuedOn;
                    Sheet.Cells[row, 3].Value = document.DocumentTypeCode;
                    Sheet.Cells[row, 4].Value = GetDocType( document.DocumentTypeCode);
                    Sheet.Cells[row, 5].Value = document.DocumentNumber;
                    Sheet.Cells[row, 6].Value = document.ContributorIdentification;
                    Sheet.Cells[row, 7].Value = document.ContributorName;

                    Sheet.Cells[row, 8].Value = document.InvoiceInfo?.SubtotalVatZero;
                    Sheet.Cells[row, 9].Value = document.InvoiceInfo?.SubtotalVat;
                    Sheet.Cells[row, 10].Value = document.InvoiceInfo?.TotalDiscount;
                    Sheet.Cells[row, 11].Value = document.InvoiceInfo?.Subtotal ?? document.RetentionInfo?.FiscalAmount;
                    Sheet.Cells[row, 13].Value = document.InvoiceInfo?.ValueAddedTax;
                    
                    Sheet.Cells[row, 14].Value = document.Total;
                    Sheet.Cells[row, 15].Value = document.Status?.GetDisplayValue() ?? "BORRADOR";
                    Sheet.Cells[row, 16].Value = document.AuthorizationNumber;
                    Sheet.Cells[row, 17].Value = document.AuthorizationDate;
                    Sheet.Cells[row, 18].Value = document.AccessKey; 
                    Sheet.Cells[row, 19].Value = document.StatusMsg;
                     
                    row++;
                }


                var table = Sheet.Tables.Add(new ExcelAddressBase(5, 1, row - 1, 19), "Recibidos");
                table.ShowFilter = false;
                table.ShowHeader = true;
                table.ShowRowStripes = true;
                table.TableStyle = TableStyles.Medium2;

                Sheet.Cells["A:AZ"].AutoFitColumns();
                 

                return excelPkg.GetAsByteArray();
            }


        }
        public static byte[] ToExcel(this DocumentResponse model)
        {
            var documents = model?.documents ?? new List<Document>();
             
            using (var excelPkg = new ExcelPackage())
            {
                var Sheet = excelPkg.Workbook.Worksheets.Add("Recibidos");
                
                Sheet.Cells[1, 1].Value = "Documentos Recibidos";
                Sheet.Cells[1, 1].Style.Font.Size = 20;
                Sheet.Cells[1, 1, 1, 5].Merge = true;
                Sheet.Cells[1, 1, 1, 5].Style.Font.Color.SetColor(Color.Navy);

                Sheet.Cells[5, 1].Value = "Id";
                Sheet.Cells[5, 2].Value = "Fecha Emision";
                Sheet.Cells[5, 3].Value = "Tipo Deducible";
                Sheet.Cells[5, 4].Value = "Tipo Documento";
                Sheet.Cells[5, 5].Value = "No. Documento";
                Sheet.Cells[5, 6].Value = "Identificacion";
                Sheet.Cells[5, 7].Value = "Razon Social";

                Sheet.Cells[5, 8].Value = "Subtotal 0%";
                Sheet.Cells[5, 9].Value = "Subtotal IVA%";
                Sheet.Cells[5, 10].Value = "Descuento";
                Sheet.Cells[5, 11].Value = "Subtotal";
                Sheet.Cells[5, 12].Value = "Tipo IVA";
                Sheet.Cells[5, 13].Value = "IVA";
                Sheet.Cells[5, 14].Value = "Total";
                Sheet.Cells[5, 15].Value = "Estado";
                Sheet.Cells[5, 16].Value = "Num. Autorizacion";
                Sheet.Cells[5, 17].Value = "Fecha Autorizacion";
                Sheet.Cells[5, 18].Value = "Clave Acceso"; 
                Sheet.Cells[5, 19].Value = "Mensaje"; 

                const string stringFormat = "@";
                //const string percentFormat = "0 \"%\"";
                const string dateFormat = "dd/mm/yyyy";
                const string moneyFormat = "$ * #,##0.00;$ * - #,##0.00";

                Sheet.Column(2).Style.Numberformat.Format = dateFormat;

                Sheet.Column(5).Style.Numberformat.Format = stringFormat;
                Sheet.Column(6).Style.Numberformat.Format = stringFormat;

                
                Sheet.Column(8).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(9).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(10).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(11).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(12).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(13).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(14).Style.Numberformat.Format = moneyFormat;

                Sheet.Column(16).Style.Numberformat.Format = stringFormat;
                Sheet.Column(17).Style.Numberformat.Format = dateFormat;
                Sheet.Column(18).Style.Numberformat.Format = stringFormat;
                 
                int row = 6;

                foreach (var document in documents ?? new List<Document>())
                {
                    decimal _iva = document.iva.ToDecimal();
                    decimal _subTotal0 = document.subTotal0.ToDecimal();
                    decimal _subTotal = document.subTotal.ToDecimal();
                    decimal _subTotal12 = document.subTotal12.ToDecimal();
                    decimal _total = document.total.ToDecimal();    
                    
                    if(_subTotal0 > 0 && _subTotal > 0)
                        _subTotal12 = _subTotal - _subTotal0;
                    else
                        _subTotal12 = _subTotal;

                    Sheet.Cells[row, 1].Value = document.pk;
                    Sheet.Cells[row, 2].Value = document.date;
                    Sheet.Cells[row, 3].Value = GetDeductibleName(document.deductibleId);
                    Sheet.Cells[row, 4].Value = document.typeDoc;
                    Sheet.Cells[row, 5].Value = document.sequence;
                    Sheet.Cells[row, 6].Value = document.identificationNumber;
                    Sheet.Cells[row, 7].Value = document.name;
                    Sheet.Cells[row, 8].Value = document.subTotal0.ToDecimal();
                    Sheet.Cells[row, 9].Value = _subTotal12;
                    Sheet.Cells[row, 10].Value = document.discount.ToDecimal();
                    Sheet.Cells[row, 11].Value = document.subTotal.ToDecimal();
                    Sheet.Cells[row, 12].Value = document.ivaType;
                    Sheet.Cells[row, 13].Value = _iva;
                    Sheet.Cells[row, 14].Value = document.total.ToDecimal();
                    Sheet.Cells[row, 15].Value = "AUTORIZADO";
                    Sheet.Cells[row, 16].Value = document.authorizationNumber;
                    Sheet.Cells[row, 17].Value = document.authorizationDate;
                    Sheet.Cells[row, 18].Value = document.accessKey; 
                    Sheet.Cells[row, 19].Value = document.MSG;   
                    row++;
                }


                var table = Sheet.Tables.Add(new ExcelAddressBase(5, 1, row - 1, 19), "Recibidos");
                table.ShowFilter = false;
                table.ShowHeader = true;
                table.ShowRowStripes = true;
                table.TableStyle = TableStyles.Medium2;

                Sheet.Cells["A:AZ"].AutoFitColumns();
                  
                return excelPkg.GetAsByteArray();
            }


        }

        private static string GetDeductibleName(int? deductibleId)
        {  
            if (deductibleId == 1)
            {
                return "Vivienda";
            }

            if (deductibleId == 2)
            {
                return "Educación";
            }

            if (deductibleId == 3)
            {
                return "Alimentación";
            }

            if (deductibleId == 4)
            {
                return "Vestimenta";
            }

            if (deductibleId == 5)
            {
                return "Salud";
            }

            if (deductibleId == 6)
            {
                return "Arte y Cultura";
            }

            return "Sin Clasificar"; 
        }

        public static decimal ToDecimal(this string value)
        {
            var result = 0M;
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Contains("$"))
                {
                    value = value.Replace("$","");
                }

                if (value.Contains(","))
                {
                    value = value.Replace(",", "");
                }

                decimal.TryParse(value.Trim(), out result);
            }

            return result;
        }

        public static string GetDocType(string code)
        {
            if (code == "01")
            {
                return "Factura";
            }

            if (code == "04")
            {
                return "NotaCredito";
            }

            if (code == "07")
            {
                return "Retencion";
            }

            if (code == "06")
            {
                return "GuiaRemision";
            }

            if (code == "03")
            {
                return "Liquidacion de Compra";
            }

            return "Comprobantes";
        }

    }
}
