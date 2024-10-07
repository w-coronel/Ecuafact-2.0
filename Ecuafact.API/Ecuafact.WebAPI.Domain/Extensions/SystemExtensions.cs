using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Net.Http.Headers;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Entities.SRI;
using System.Reflection;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using Ecuafact.WebAPI.Domain.Entities.App;
using Ecuafact.WebAPI.Domain.Extensions;
using System.Xml.Schema;
using System.Xml;
using System.Xml.Serialization;
using Ecuafact.WebAPI.Domain.Entities.Ats;
using Ecuafact.WebAPI;

namespace System
{
    public static class MySystemExtensions
    {
        public static string Rimpe => "Contribuyente Régimen: RIMPE";
        public static string AgenteRetencion => "Agente de Retención No. Resolución:";       

        // Esto corrige el logo en los formatos de factura y otras imagenes
        public static byte[] ScaleImage(this Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(maxWidth, maxHeight);

            using (var graphics = Graphics.FromImage(newImage))
            {
                // Calculate x and y which center the image
                int y = (maxHeight / 2) - newHeight / 2;
                int x = (maxWidth / 2) - newWidth / 2;

                // Draw image on x and y with newWidth and newHeight
                graphics.DrawImage(image, x, y, newWidth, newHeight);

                return newImage.ToStream()?.ToArray();
            }
        }

        public static MemoryStream ToStream(this Image image)
        {
            var stream = new MemoryStream();

            image.Save(stream, ImageFormat.Png);
            return stream;
        }

        public static string ToDateTimeString(this string text)
        {
            return text.ToDateTime()
                       .ToString("dd/MM/yyyy");
        }

        public static string ToDateTimeString2(this string text)
        {
            return text.ToDateTime()
                       .ToString("yyyy/MM/dd");
        }

        public static DateTime ToDateTime(this string text)
        {
            try
            {
                if (DateTime.TryParse(text, new CultureInfo("es-ES"), DateTimeStyles.AllowWhiteSpaces, out DateTime date))
                {
                    return date;
                }

                // Caso contrario entonces analizo el texto para determinar año, mes y dia:
                var dates = text?.Split(new char[] { '/', '-', '.' }, StringSplitOptions.RemoveEmptyEntries);

                if (dates != null && dates.Length >= 3)
                {
                    int year, month, day;

                    if (dates[0].Length == 4)
                    {
                        year = Convert.ToInt32(dates[0]);
                        month = Convert.ToInt32(dates[1]);
                        day = Convert.ToInt32(dates[2]);
                    }
                    else
                    {
                        day = Convert.ToInt32(dates[0]);
                        month = Convert.ToInt32(dates[1]);
                        year = Convert.ToInt32(dates[2]);
                    }

                    return new DateTime(year, month, day);
                }
            }
            catch { }

            return DateTime.Today.Date;
        }

        public static string GetValue(this HttpResponseHeaders header, string name)
        {
            IEnumerable<string> results;

            if (header.TryGetValues(name, out results) && results.Count() > 0)
            {
                var value = "";
                for (int i = 0; i < results.Count(); i++)
                {
                    if (i > 0) value += ",";
                    value += results.ElementAt(i);
                }
                return value;
            }

            return string.Empty;
        }

        public static string Encode(this string text)
        {
            text = $"{text}|{DateTime.Now.ToString("MMyyddd-yyddMM-dyyMM-ddMM-yy-yy")}";

            var byteArray = Encoding.UTF8.GetBytes(text);

            return Convert.ToBase64String(byteArray);
        }

        public static string Decode(this string text)
        {
            var byteArray = Convert.FromBase64String(text);
            var result = Encoding.UTF8.GetString(byteArray);
            return result.Split('|')[0];
        }

        public static string Ago(this TimeSpan time)
        {
            string text;

            if (time.Days > 0)
            {
                text = $"Hace {time.Days} dias";
            }
            else if (time.Hours > 0)
            {
                text = $"Hace {time.Hours} horas";
            }
            else if (time.Minutes > 0)
            {
                text = $"Hace {time.Minutes} minutos";
            }
            else
            {
                text = $"Hace {time.Seconds} segundos";
            }

            return text;
        }

        public static byte[] GetBytes(this HttpPostedFileBase file)
        {
            if (file.InputStream != null && file.InputStream.Length > 0)
            {
                file.InputStream.Position = 0;
                BinaryReader reader = new BinaryReader(file.InputStream);
                var result = reader.ReadBytes((int)file.ContentLength);
                return result;
            }

            return default;
        }

        public static string GetMainAddress(this SRIContrib contrib)
        {
            var establishment = contrib.Establishments.FirstOrDefault();

            StringBuilder builder = new StringBuilder();

            if (establishment != null)
            {

                builder.Append(establishment.Street);

                if (!string.IsNullOrEmpty(builder.ToString()))
                {
                    builder.Append(" ");
                }

                if (!string.IsNullOrEmpty(establishment.AddressNumber))
                {
                    builder.Append($"#{establishment.AddressNumber}");
                }

                if (!string.IsNullOrEmpty(builder.ToString()))
                {
                    builder.Append(" ");

                    if (!string.IsNullOrEmpty(establishment.Intersection))
                    {
                        builder.Append("Y ");
                    }
                }

                builder.Append(establishment.Intersection);

                if (!string.IsNullOrEmpty(builder.ToString()))
                {
                    builder.Append(" / ");
                }

                builder.Append(establishment.Town);
                 
                if (!string.IsNullOrEmpty(builder.ToString()))
                {
                    builder.Append(" / ");
                }

                builder.Append(establishment.City);

                if (!string.IsNullOrEmpty(builder.ToString()))
                {
                    builder.Append(" / ");
                }

                builder.Append(establishment.Province);

            }

            return $"{builder}";
        }

        public static string GetFullAddress(this Establishment establishment)
        {
            StringBuilder builder = new StringBuilder();

            
            builder.Append(establishment.Street); 
            
            if (!string.IsNullOrEmpty(builder.ToString()))
            {
                builder.Append(" ");
            }
            
            if (!string.IsNullOrEmpty(establishment.AddressNumber))
            {
                builder.Append($"#{establishment.AddressNumber}");
            }

            if (!string.IsNullOrEmpty(builder.ToString()))
            {
                builder.Append(" ");

                if (!string.IsNullOrEmpty(establishment.Intersection))
                {
                    builder.Append("Y ");
                }
            }
            
            builder.Append(establishment.Intersection);

            if (!string.IsNullOrEmpty(builder.ToString()))
            {
                builder.Append(", ");
            }

            builder.Append(establishment.Town);

            
            return $"{builder}";
        }

        public static List<T> ImportData<T>(Stream stream, List<VatRate> ivaRates = null)
        {
            var collection = new List<T>();
            var type = typeof(T);

            try
            {
                #region read excel
                using (stream)
                {
                    ExcelPackage package = new ExcelPackage(stream);

                    ExcelWorksheet sheet = package.Workbook.Worksheets[1];

                    #region check excel format
                    if (sheet == null)
                    {
                        return collection;
                    }
                    #endregion

                    #region get last row index
                    int lastRow = sheet.Dimension.End.Row;
                    while (sheet.Cells[lastRow, 1].Value == null)
                    {
                        lastRow--;
                    }
                    #endregion

                    #region read datas

                    string strName = typeof(T).Name;

                    if (strName == "Product")
                    {

                        //T obj = Activator.CreateInstance<T>();
                        for (int i = 2; i <= lastRow; i++)
                        {
                            T obj = Activator.CreateInstance<T>();

                            PropertyInfo property = type.GetProperty("MainCode");
                            property.SetValue(obj, sheet.Cells[i, 1].Value?.ToString().Trim());

                            property = type.GetProperty("AuxCode");
                            property.SetValue(obj, sheet.Cells[i, 2].Value?.ToString().Trim());

                            property = type.GetProperty("Name");
                            property.SetValue(obj, sheet.Cells[i, 3].Value?.ToString().Trim());

                            property = type.GetProperty("UnitPrice");
                            property.SetValue(obj, Convert.ToDecimal(decimal.TryParse(sheet.Cells[i, 4].Value?.ToString(), out _) ? sheet.Cells[i, 4].Value.ToString() : "1"));

                            property = type.GetProperty("ProductTypeId");
                            property.SetValue(obj, Convert.ToInt16(int.TryParse(sheet.Cells[i, 5].Value?.ToString(), out _) ? sheet.Cells[i, 5].Value.ToString() : "1"));

                            var ivaRateId = 0;
                            property = type.GetProperty("IvaRateId");                            
                            if (ivaRates != null){
                                var ivaCode = int.TryParse(sheet.Cells[i, 6].Value?.ToString(), out _) ? sheet.Cells[i, 6].Value.ToString() : "0";
                                ivaRateId = ivaRates.Where(cod => cod.SriCode == ivaCode).FirstOrDefault().Id;
                            }
                            property.SetValue(obj, Convert.ToInt16(ivaRateId));
                            // property.SetValue(obj, Convert.ToInt16(int.TryParse(sheet.Cells[i, 6].Value?.ToString(), out _) ? sheet.Cells[i, 6].Value.ToString() : "1"));

                            property = type.GetProperty("IceRateId");
                            //property.SetValue(obj, Convert.ToInt16(int.TryParse(sheet.Cells[i, 7].Value?.ToString(), out _) ? sheet.Cells[i, 7].Value.ToString() : "1"));
                            property.SetValue(obj, Convert.ToInt16(1));

                            property = type.GetProperty("IsEnabled");
                            property.SetValue(obj, true);

                            property = type.GetProperty("Name1");
                            property.SetValue(obj, sheet.Cells[i, 7].Value?.ToString().Trim());

                            property = type.GetProperty("Value1");
                            property.SetValue(obj, sheet.Cells[i, 8].Value?.ToString().Trim());

                            property = type.GetProperty("Name2");
                            property.SetValue(obj, sheet.Cells[i, 9].Value?.ToString().Trim());

                            property = type.GetProperty("Value2");
                            property.SetValue(obj, sheet.Cells[i, 10].Value?.ToString().Trim());

                            property = type.GetProperty("CreatedOn");
                            property.SetValue(obj, DateTime.Now);

                            collection.Add(obj);
                        }
                    }
                    else if (strName == "Contributor")
                    {

                        //T obj = Activator.CreateInstance<T>();
                        for (int i = 2; i <= lastRow; i++)
                        {
                            T obj = Activator.CreateInstance<T>();

                            var perNatural = sheet.Cells[i, 3].Value?.ToString();
                            var perJuridica = sheet.Cells[i, 4].Value?.ToString();
                            var tradeName = sheet.Cells[i, 5].Value?.ToString();
                            var bussinesName = "";
                            if (!String.IsNullOrEmpty(perNatural)) { bussinesName = perNatural; }
                            if (!String.IsNullOrEmpty(perJuridica)) { bussinesName = perJuridica; }

                            PropertyInfo property = type.GetProperty("Identification");
                            property.SetValue(obj, (sheet.Cells[i, 1].Value?.ToString().Length < 10 && Convert.ToInt16(int.TryParse(sheet.Cells[i, 2].Value?.ToString(), out _) ? sheet.Cells[i, 2].Value.ToString() : "2") == 1) ? sheet.Cells[i, 1].Value?.ToString().PadLeft(10, '0') : sheet.Cells[i, 1].Value?.ToString());

                            property = type.GetProperty("IdentificationTypeId");
                            property.SetValue(obj, Convert.ToInt16(int.TryParse(sheet.Cells[i, 2].Value?.ToString(), out _) ? sheet.Cells[i, 2].Value.ToString() : "2"));

                            property = type.GetProperty("BussinesName");
                            property.SetValue(obj, bussinesName.Trim());

                            property = type.GetProperty("TradeName");
                            property.SetValue(obj, tradeName ?? bussinesName.Trim());

                            property = type.GetProperty("Address");
                            property.SetValue(obj, sheet.Cells[i, 6].Value?.ToString().Trim());

                            property = type.GetProperty("Phone");
                            property.SetValue(obj, sheet.Cells[i, 7].Value?.ToString().Trim());

                            property = type.GetProperty("EmailAddresses");
                            property.SetValue(obj, sheet.Cells[i, 8].Value?.ToString().Trim());

                            property = type.GetProperty("IsSupplier");
                            property.SetValue(obj, false);

                            property = type.GetProperty("IsCustomer");
                            property.SetValue(obj, true);

                            property = type.GetProperty("IsDriver");
                            property.SetValue(obj, false);

                            property = type.GetProperty("IsEnabled");
                            property.SetValue(obj, true);

                            property = type.GetProperty("CreatedOn");
                            property.SetValue(obj, DateTime.Now);

                            collection.Add(obj);
                        }
                    }


                    #endregion
                }
                #endregion
            }
            catch (Exception ex)
            {
                var errors = ex.Message;
            }

            return collection;
        }

        public static byte[] ToExcelProduct(this List<Product> model)
        {
            var product = model ?? new List<Product>();

            using (var excelPkg = new ExcelPackage())
            {
                var Sheet = excelPkg.Workbook.Worksheets.Add("Productos");

                Sheet.Cells[1, 1].Value = "Ecuafact | Productos";
                Sheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                Sheet.Cells[1, 1].Style.Font.Size = 20;
                Sheet.Cells[1, 1, 1, 5].Merge = true;
                Sheet.Cells[1, 1, 1, 5].Style.Font.Color.SetColor(Color.Navy);

                Sheet.Cells[5, 1].Value = "Código";
                Sheet.Cells[5, 2].Value = "Cod. Aux.";
                Sheet.Cells[5, 3].Value = "Descripción";
                Sheet.Cells[5, 4].Value = "PVP";
                Sheet.Cells[5, 5].Value = "Tipo";
                Sheet.Cells[5, 6].Value = "IVA";
                Sheet.Cells[5, 7].Value = "ICE";               

                const string stringFormat = "@";
                const string percentFormat = "0\"%\"";
                const string dateFormat = "dd/mm/yyyy";
                const string moneyFormat = "$ #,##0.00";

              
                Sheet.Column(4).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(6).Style.Numberformat.Format = percentFormat;               
                Sheet.Column(7).Style.Numberformat.Format = percentFormat;               

                int row = 6;

                foreach (var p in product)
                {
                    Sheet.Cells[row, 1].Value = p.MainCode;
                    Sheet.Cells[row, 2].Value = p.AuxCode;
                    Sheet.Cells[row, 3].Value = p.Name;
                    Sheet.Cells[row, 4].Value = p.UnitPrice;
                    Sheet.Cells[row, 5].Value = p.ProductType.Name;
                    Sheet.Cells[row, 6].Value = p.IvaRate.RateValue;
                    Sheet.Cells[row, 7].Value = p.IceRate.Rate;                   

                    row++;
                }


                var table = Sheet.Tables.Add(new ExcelAddressBase(5, 1, row - 1, 7), "Productos");
                table.ShowFilter = false;
                table.ShowHeader = true;
                table.ShowRowStripes = true;
                table.TableStyle = TableStyles.Medium2;

                Sheet.Cells["A:AZ"].AutoFitColumns();


                return excelPkg.GetAsByteArray();
            }


        }

        public static byte[] ToExcelContributors(this List<Contributor> model)
        {
            var contributor = model ?? new List<Contributor>();

            using (var excelPkg = new ExcelPackage())
            {
                var Sheet = excelPkg.Workbook.Worksheets.Add("Clientes");

                Sheet.Cells[1, 1].Value = "Ecuafact | Clientes";
                Sheet.Cells[1, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                Sheet.Cells[1, 1].Style.Font.Size = 20;
                Sheet.Cells[1, 1, 1, 6].Merge = true;
                Sheet.Cells[1, 1, 1, 6].Style.Font.Color.SetColor(Color.Navy);
               


                Sheet.Cells[5, 1].Value = "Identificación No.";
                Sheet.Cells[5, 2].Value = "Razón Social";
                Sheet.Cells[5, 3].Value = "Nombre Comercial";
                Sheet.Cells[5, 4].Value = "Dirección";
                Sheet.Cells[5, 5].Value = "Teléfono";
                Sheet.Cells[5, 6].Value = "E-mail";               

                const string stringFormat = "@";

                Sheet.Column(1).Style.Numberformat.Format = stringFormat;
                Sheet.Column(5).Style.Numberformat.Format = stringFormat;
                
                int row = 6;

                foreach (var p in contributor)
                {
                    Sheet.Cells[row, 1].Value = p.Identification;
                    Sheet.Cells[row, 2].Value = p.BussinesName;
                    Sheet.Cells[row, 3].Value = p.TradeName;
                    Sheet.Cells[row, 4].Value = p.Address;
                    Sheet.Cells[row, 5].Value = p.Phone;
                    Sheet.Cells[row, 6].Value = p.EmailAddresses;                   

                    row++;
                }


                var table = Sheet.Tables.Add(new ExcelAddressBase(5, 1, row - 1, 6), "Clientes");
                table.ShowFilter = false;
                table.ShowHeader = true;
                table.ShowRowStripes = true;
                table.TableStyle = TableStyles.Medium2;

                Sheet.Cells["A:AZ"].AutoFitColumns();


                return excelPkg.GetAsByteArray();
            }


        }

        public static byte[] ToExcelDocuments(this List<Document> model)
        {
            var documents = model ?? new List<Document>();

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
                Sheet.Cells[5, 9].Value = "Subtotal IVA";
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
                    Sheet.Cells[row, 4].Value = GetDocType(document.DocumentTypeCode);
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


                var table = Sheet.Tables.Add(new ExcelAddressBase(5, 1, row - 1, 19), "Emitidos");
                table.ShowFilter = false;
                table.ShowHeader = true;
                table.ShowRowStripes = true;
                table.TableStyle = TableStyles.Medium2;

                Sheet.Cells["A:AZ"].AutoFitColumns();


                return excelPkg.GetAsByteArray();
            }


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

        public static byte[] ToExcelDocumentsReceived(this List<ApiV3_DocumentosData> model)
        {
            var documents = model ?? new List<ApiV3_DocumentosData>();
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
                Sheet.Cells[5, 9].Value = "Subtotal IVA";
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
                Sheet.Column(12).Style.Numberformat.Format = stringFormat;
                Sheet.Column(13).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(14).Style.Numberformat.Format = moneyFormat;

                Sheet.Column(16).Style.Numberformat.Format = stringFormat;
                Sheet.Column(17).Style.Numberformat.Format = dateFormat;
                Sheet.Column(18).Style.Numberformat.Format = stringFormat;

                int row = 6;

                foreach (var document in documents)
                {
                    var typeRate = "0";
                    var baseImp = document.Subtotal15 > 0 ? document.Subtotal15 : document.Subtotal12;
                    var total = document.Total - document.Subtotal0;
                    if (baseImp > 0 && total > 0)
                    {
                        decimal valor = decimal.Round(((total - baseImp) / (baseImp / 100)), 0);
                        typeRate = $"{valor}";
                    }
                    Sheet.Cells[row, 1].Value = document.Pk;
                    Sheet.Cells[row, 2].Value = document.FechaEmision;
                    Sheet.Cells[row, 3].Value = document.CodTipoDoc;
                    Sheet.Cells[row, 4].Value = GetDocType(document.CodTipoDoc);
                    Sheet.Cells[row, 5].Value = document.Secuencia;
                    Sheet.Cells[row, 6].Value = document.NumeroIdentificacionEmisor;
                    Sheet.Cells[row, 7].Value = document.NombreEmisor;

                    Sheet.Cells[row, 8].Value = document.Subtotal0;
                    Sheet.Cells[row, 9].Value = document.Subtotal15 > 0 ? document.Subtotal15: document.Subtotal12;
                    Sheet.Cells[row, 10].Value = 0;
                    Sheet.Cells[row, 11].Value = document.Subtotal;
                    Sheet.Cells[row, 12].Value = typeRate;
                    Sheet.Cells[row, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    Sheet.Cells[row, 13].Value = document.Iva;

                    Sheet.Cells[row, 14].Value = document.Total;
                    Sheet.Cells[row, 15].Value = document.Estado.Equals("100") ? "Validado SRI":"";
                    Sheet.Cells[row, 16].Value = document.Numeroautorizacion;
                    Sheet.Cells[row, 17].Value = document.FechaAut;
                    Sheet.Cells[row, 18].Value = document.ClaveAcceso;
                    Sheet.Cells[row, 19].Value = "";

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

        public static byte[] ToAtsReports(this AtsReporte model)
        {

            using (var excelPkg = new ExcelPackage())
            {
                #region Compras
                var _compras = model.ToAtsCompras();
                var Sheet = excelPkg.Workbook.Worksheets.Add("ATS");

                Sheet.Cells[1, 1].Value = "COMPRAS";
                Sheet.Cells[1, 1].Style.Font.Size = 16;
                Sheet.Cells[1, 1, 1, 5].Merge = true;
                Sheet.Cells[1, 1, 1, 5].Style.Font.Color.SetColor(Color.Navy);

                Sheet.Cells[2, 1].Value = "TipoIDInformante";
                Sheet.Cells[2, 2].Value = "IdInformante";
                Sheet.Cells[2, 3].Value = "RazonSocial";
                Sheet.Cells[2, 4].Value = "Anio";
                Sheet.Cells[2, 5].Value = "Mes";
                Sheet.Cells[2, 6].Value = "NumEstabRuc";
                Sheet.Cells[2, 7].Value = "TotalVentas";
                Sheet.Cells[2, 8].Value = "CodigoOperativo";
                Sheet.Cells[2, 9].Value = "CodSustento";
                Sheet.Cells[2, 10].Value = "TpIdProv";
                Sheet.Cells[2, 11].Value = "IdProv";
                Sheet.Cells[2, 12].Value = "ParteRel";
                Sheet.Cells[2, 13].Value = "TipoComprobante";
                Sheet.Cells[2, 14].Value = "FechaRegistro";
                Sheet.Cells[2, 15].Value = "Establecimiento";
                Sheet.Cells[2, 16].Value = "PuntoEmision";
                Sheet.Cells[2, 17].Value = "Secuencial";
                Sheet.Cells[2, 18].Value = "FechaEmision";
                Sheet.Cells[2, 19].Value = "Autorizacion";
                Sheet.Cells[2, 20].Value = "BaseNoGraIva";
                Sheet.Cells[2, 21].Value = "BaseImponible";
                Sheet.Cells[2, 22].Value = "BaseImpGrav";
                Sheet.Cells[2, 23].Value = "BaseImpExe";
                Sheet.Cells[2, 24].Value = "MontoIce";
                Sheet.Cells[2, 25].Value = "MontoIva";
                Sheet.Cells[2, 26].Value = "ValRetBien10";
                Sheet.Cells[2, 27].Value = "ValRetServ20";
                Sheet.Cells[2, 28].Value = "ValorRetBienes";
                Sheet.Cells[2, 29].Value = "ValRetServ50";
                Sheet.Cells[2, 30].Value = "ValorRetServicios";
                Sheet.Cells[2, 31].Value = "ValRetServ100";
                Sheet.Cells[2, 32].Value = "TotbasesImpReemb";
                Sheet.Cells[2, 33].Value = "PagoLocExt";
                Sheet.Cells[2, 34].Value = "PaisEfecPago";
                Sheet.Cells[2, 35].Value = "AplicConvDobTrib";
                Sheet.Cells[2, 36].Value = "PagExtSujRetNorLeg";
                Sheet.Cells[2, 37].Value = "PagoRegFis";
                Sheet.Cells[2, 38].Value = "FormaPago";
                Sheet.Cells[2, 39].Value = "CodRetAir";
                Sheet.Cells[2, 40].Value = "BaseImpAir";
                Sheet.Cells[2, 41].Value = "PorcentajeAir";
                Sheet.Cells[2, 42].Value = "ValRetAir";
                Sheet.Cells[2, 43].Value = "NumCajBan";
                Sheet.Cells[2, 44].Value = "PrecCajBan";
                Sheet.Cells[2, 45].Value = "EstabRetencion1";
                Sheet.Cells[2, 46].Value = "PtoEmiRetencion1";
                Sheet.Cells[2, 47].Value = "SecRetencion1";
                Sheet.Cells[2, 48].Value = "AutRetencion1";
                Sheet.Cells[2, 49].Value = "FechaEmiRet1";
                Sheet.Cells[2, 50].Value = "DocModificado";
                Sheet.Cells[2, 51].Value = "EstabModificado";
                Sheet.Cells[2, 52].Value = "PtoEmiModificado";
                Sheet.Cells[2, 53].Value = "SecModificado";
                Sheet.Cells[2, 54].Value = "AutModificado";
                Sheet.Cells[2, 55].Value = "DenoProv";

                const string stringFormat = "@";
                //const string percentFormat = "0 \"%\"";
                const string dateFormat = "dd/mm/yyyy";
                const string moneyFormat = "#,##0.00;- #,##0.00";

                Sheet.Column(2).Style.Numberformat.Format = stringFormat;
                Sheet.Column(5).Style.Numberformat.Format = stringFormat;
                Sheet.Column(6).Style.Numberformat.Format = stringFormat;
                Sheet.Column(7).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(9).Style.Numberformat.Format = stringFormat;
                Sheet.Column(10).Style.Numberformat.Format = stringFormat;
                Sheet.Column(11).Style.Numberformat.Format = stringFormat;
                Sheet.Column(13).Style.Numberformat.Format = stringFormat;
                Sheet.Column(14).Style.Numberformat.Format = dateFormat;
                Sheet.Column(15).Style.Numberformat.Format = stringFormat;
                Sheet.Column(16).Style.Numberformat.Format = stringFormat;
                Sheet.Column(17).Style.Numberformat.Format = stringFormat;
                Sheet.Column(18).Style.Numberformat.Format = dateFormat;
                Sheet.Column(20).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(21).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(22).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(23).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(24).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(25).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(26).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(27).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(28).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(29).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(30).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(31).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(32).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(33).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(38).Style.Numberformat.Format = stringFormat;
                Sheet.Column(39).Style.Numberformat.Format = stringFormat;
                Sheet.Column(40).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(41).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(42).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(45).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(46).Style.Numberformat.Format = stringFormat;
                Sheet.Column(47).Style.Numberformat.Format = stringFormat;
                Sheet.Column(48).Style.Numberformat.Format = stringFormat;
                Sheet.Column(49).Style.Numberformat.Format = dateFormat;
                Sheet.Column(50).Style.Numberformat.Format = stringFormat;
                Sheet.Column(51).Style.Numberformat.Format = stringFormat;
                Sheet.Column(52).Style.Numberformat.Format = stringFormat;
                Sheet.Column(53).Style.Numberformat.Format = stringFormat;
                Sheet.Column(54).Style.Numberformat.Format = stringFormat;

                int row = 3;

                foreach (var comp in _compras)
                {
                    Sheet.Cells[row, 1].Value = comp.TipoIDInformante;
                    Sheet.Cells[row, 2].Value = comp.IdInformante;
                    Sheet.Cells[row, 3].Value = comp.RazonSocial;
                    Sheet.Cells[row, 4].Value = comp.Anio;
                    Sheet.Cells[row, 5].Value = comp.Mes;
                    Sheet.Cells[row, 6].Value = comp.NumEstabRuc;
                    Sheet.Cells[row, 7].Value = comp.TotalVentas;
                    Sheet.Cells[row, 8].Value = comp.CodigoOperativo;
                    Sheet.Cells[row, 9].Value = comp.CodSustento;
                    Sheet.Cells[row, 10].Value = comp.TpIdProv;
                    Sheet.Cells[row, 11].Value = comp.IdProv;
                    Sheet.Cells[row, 12].Value = comp.ParteRel;
                    Sheet.Cells[row, 13].Value = comp.TipoComprobante;
                    Sheet.Cells[row, 14].Value = comp.FechaRegistro;
                    Sheet.Cells[row, 15].Value = comp.Establecimiento;
                    Sheet.Cells[row, 16].Value = comp.PuntoEmision;
                    Sheet.Cells[row, 17].Value = comp.Secuencial;
                    Sheet.Cells[row, 18].Value = comp.FechaEmision;
                    Sheet.Cells[row, 19].Value = comp.Autorizacion;
                    Sheet.Cells[row, 20].Value = comp.BaseNoGraIva;
                    Sheet.Cells[row, 21].Value = comp.BaseImponible;
                    Sheet.Cells[row, 22].Value = comp.BaseImpGrav;
                    Sheet.Cells[row, 23].Value = comp.BaseImpExe;
                    Sheet.Cells[row, 24].Value = comp.MontoIce;
                    Sheet.Cells[row, 25].Value = comp.MontoIva;
                    Sheet.Cells[row, 26].Value = comp.ValRetBien10;
                    Sheet.Cells[row, 27].Value = comp.ValRetServ20;
                    Sheet.Cells[row, 28].Value = comp.ValorRetBienes;
                    Sheet.Cells[row, 29].Value = comp.ValRetServ50;
                    Sheet.Cells[row, 30].Value = comp.ValorRetServicios;
                    Sheet.Cells[row, 31].Value = comp.ValRetServ100;
                    Sheet.Cells[row, 32].Value = comp.TotbasesImpReemb;
                    Sheet.Cells[row, 33].Value = comp.PagoLocExt;
                    Sheet.Cells[row, 34].Value = comp.PaisEfecPago;
                    Sheet.Cells[row, 35].Value = comp.AplicConvDobTrib;
                    Sheet.Cells[row, 36].Value = comp.PagExtSujRetNorLeg;
                    Sheet.Cells[row, 37].Value = comp.PagoRegFis;
                    Sheet.Cells[row, 38].Value = comp.FormaPago;
                    Sheet.Cells[row, 39].Value = comp.CodRetAir;
                    Sheet.Cells[row, 40].Value = comp.BaseImpAir;
                    Sheet.Cells[row, 41].Value = comp.PorcentajeAir;
                    Sheet.Cells[row, 42].Value = comp.ValRetAir;
                    Sheet.Cells[row, 43].Value = comp.NumCajBan;
                    Sheet.Cells[row, 44].Value = comp.PrecCajBan;
                    Sheet.Cells[row, 45].Value = comp.EstabRetencion1;
                    Sheet.Cells[row, 46].Value = comp.PtoEmiRetencion1;
                    Sheet.Cells[row, 47].Value = comp.SecRetencion1;
                    Sheet.Cells[row, 48].Value = comp.AutRetencion1;
                    Sheet.Cells[row, 49].Value = comp.FechaEmiRet1;
                    Sheet.Cells[row, 50].Value = comp.DocModificado;
                    Sheet.Cells[row, 51].Value = comp.EstabModificado;
                    Sheet.Cells[row, 52].Value = comp.PtoEmiModificado;
                    Sheet.Cells[row, 53].Value = comp.SecModificado;
                    Sheet.Cells[row, 54].Value = comp.AutModificado;
                    Sheet.Cells[row, 55].Value = comp.DenoProv;

                    row++;
                }
                #endregion Compras

                #region ventas
                var _ventas = model.ToAtsVentass();
                row++;
                Sheet.Cells[row, 1].Value = "VENTAS";
                Sheet.Cells[row, 1].Style.Font.Size = 16;
                Sheet.Cells[row, 1, row, 5].Merge = true;
                Sheet.Cells[row, 1, row, 5].Style.Font.Color.SetColor(Color.Navy);

                row++;

                Sheet.Cells[row, 1].Value = "TipoIDInformante";
                Sheet.Cells[row, 2].Value = "IdInformante";
                Sheet.Cells[row, 3].Value = "RazonSocial";
                Sheet.Cells[row, 4].Value = "Anio";
                Sheet.Cells[row, 5].Value = "Mes";
                Sheet.Cells[row, 6].Value = "NumEstabRuc";
                Sheet.Cells[row, 7].Value = "TotalVentas";
                Sheet.Cells[row, 8].Value = "CodigoOperativo";
                Sheet.Cells[row, 9].Value = "TpIdCliente";
                Sheet.Cells[row, 10].Value = "IdCliente";
                Sheet.Cells[row, 11].Value = "TipoComprobante";
                Sheet.Cells[row, 12].Value = "TipoEmision";
                Sheet.Cells[row, 13].Value = "NumeroComprobantes";
                Sheet.Cells[row, 14].Value = "BaseNoGraIva";
                Sheet.Cells[row, 15].Value = "BaseImponible";
                Sheet.Cells[row, 16].Value = "BaseImpGrav";
                Sheet.Cells[row, 17].Value = "MontoIva";
                Sheet.Cells[row, 18].Value = "MontoIce";
                Sheet.Cells[row, 19].Value = "ValorRetIva";
                Sheet.Cells[row, 20].Value = "ValorRetRenta";

                Sheet.Column(2).Style.Numberformat.Format = stringFormat;
                Sheet.Column(5).Style.Numberformat.Format = stringFormat;
                Sheet.Column(6).Style.Numberformat.Format = stringFormat;
                Sheet.Column(7).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(9).Style.Numberformat.Format = stringFormat;
                Sheet.Column(10).Style.Numberformat.Format = stringFormat;
                Sheet.Column(11).Style.Numberformat.Format = stringFormat;
                Sheet.Column(14).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(15).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(16).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(17).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(18).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(19).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(20).Style.Numberformat.Format = moneyFormat;

                row++;
                foreach (var vent in _ventas)
                {
                    Sheet.Cells[row, 1].Value = vent.TipoIDInformante;
                    Sheet.Cells[row, 2].Value = vent.IdInformante;
                    Sheet.Cells[row, 3].Value = vent.RazonSocial;
                    Sheet.Cells[row, 4].Value = vent.Anio;
                    Sheet.Cells[row, 5].Value = vent.Mes;
                    Sheet.Cells[row, 6].Value = vent.NumEstabRuc;
                    Sheet.Cells[row, 7].Value = vent.TotalVentas;
                    Sheet.Cells[row, 8].Value = vent.CodigoOperativo;
                    Sheet.Cells[row, 9].Value = vent.TpIdCliente;
                    Sheet.Cells[row, 10].Value = vent.IdCliente;
                    Sheet.Cells[row, 11].Value = vent.TipoComprobante;
                    Sheet.Cells[row, 12].Value = vent.TipoEmision;
                    Sheet.Cells[row, 13].Value = vent.NumeroComprobantes;
                    Sheet.Cells[row, 14].Value = vent.BaseNoGraIva;
                    Sheet.Cells[row, 15].Value = vent.BaseImponible;
                    Sheet.Cells[row, 16].Value = vent.BaseImpGrav;
                    Sheet.Cells[row, 17].Value = vent.MontoIva;
                    Sheet.Cells[row, 18].Value = vent.MontoIce;
                    Sheet.Cells[row, 19].Value = vent.ValorRetIva;
                    Sheet.Cells[row, 20].Value = vent.ValorRetRenta;
                    row++;
                }
                #endregion ventas

                #region Ventas por establecimiento
                var _ventEst = model.ToAtsVentasEstablecimiento();

                row++;
                Sheet.Cells[row, 1].Value = "VENTAS POR ESTABLECIMIENTO";
                Sheet.Cells[row, 1].Style.Font.Size = 16;
                Sheet.Cells[row, 1, row, 5].Merge = true;
                Sheet.Cells[row, 1, row, 5].Style.Font.Color.SetColor(Color.Navy);

                row++;

                Sheet.Cells[row, 1].Value = "TipoIDInformante";
                Sheet.Cells[row, 2].Value = "IdInformante";
                Sheet.Cells[row, 3].Value = "RazonSocial";
                Sheet.Cells[row, 4].Value = "Anio";
                Sheet.Cells[row, 5].Value = "Mes";
                Sheet.Cells[row, 6].Value = "NumEstabRuc";
                Sheet.Cells[row, 7].Value = "TotalVentas";
                Sheet.Cells[row, 8].Value = "CodigoOperativo";
                Sheet.Cells[row, 9].Value = "CodEstab";
                Sheet.Cells[row, 10].Value = "VentasEstab";

                Sheet.Column(2).Style.Numberformat.Format = stringFormat;
                Sheet.Column(5).Style.Numberformat.Format = stringFormat;
                Sheet.Column(6).Style.Numberformat.Format = stringFormat;
                Sheet.Column(7).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(9).Style.Numberformat.Format = stringFormat;
                Sheet.Column(10).Style.Numberformat.Format = moneyFormat;

                 row++;
                foreach (var vest in _ventEst)
                {
                    Sheet.Cells[row, 1].Value = vest.TipoIDInformante;
                    Sheet.Cells[row, 2].Value = vest.IdInformante;
                    Sheet.Cells[row, 3].Value = vest.RazonSocial;
                    Sheet.Cells[row, 4].Value = vest.Anio;
                    Sheet.Cells[row, 5].Value = vest.Mes;
                    Sheet.Cells[row, 6].Value = vest.NumEstabRuc;
                    Sheet.Cells[row, 7].Value = vest.TotalVentas;
                    Sheet.Cells[row, 8].Value = vest.CodigoOperativo;
                    Sheet.Cells[row, 9].Value = vest.CodEstab;
                    Sheet.Cells[row, 10].Value = vest.VentasEstab;
                    row++;
                }

                #endregion  Ventas por establecimiento

                #region Exportaciones                

                row++;
                Sheet.Cells[row, 1].Value = "EXPORTACIONES";
                Sheet.Cells[row, 1].Style.Font.Size = 16;
                Sheet.Cells[row, 1, row, 5].Merge = true;
                Sheet.Cells[row, 1, row, 5].Style.Font.Color.SetColor(Color.Navy);

                row++;

                Sheet.Cells[row, 1].Value = "TipoIDInformante";
                Sheet.Cells[row, 2].Value = "IdInformante";
                Sheet.Cells[row, 3].Value = "RazonSocial";
                Sheet.Cells[row, 4].Value = "Anio";
                Sheet.Cells[row, 5].Value = "Mes";
                Sheet.Cells[row, 6].Value = "NumEstabRuc";
                Sheet.Cells[row, 7].Value = "TotalVentas";
                Sheet.Cells[row, 8].Value = "CodigoOperativo";
                Sheet.Cells[row, 9].Value = "TpIdClienteEx";
                Sheet.Cells[row, 10].Value = "IdClienteEx";
                Sheet.Cells[row, 11].Value = "TipoRegi";
                Sheet.Cells[row, 12].Value = "ParteRelExp";
                Sheet.Cells[row, 13].Value = "PaisEfecPagoGen";
                Sheet.Cells[row, 14].Value = "PaisEfecExp";
                Sheet.Cells[row, 15].Value = "ExportacionDe";
                Sheet.Cells[row, 16].Value = "TipoComprobante";
                Sheet.Cells[row, 17].Value = "DistAduanero";
                Sheet.Cells[row, 18].Value = "Anio";
                Sheet.Cells[row, 19].Value = "Regimen";
                Sheet.Cells[row, 20].Value = "Correlativo";
                Sheet.Cells[row, 21].Value = "DocTransp";
                Sheet.Cells[row, 22].Value = "FechaEmbarque";
                Sheet.Cells[row, 23].Value = "ValorFOB";
                Sheet.Cells[row, 24].Value = "ValorFOBComprobante";
                Sheet.Cells[row, 25].Value = "Establecimiento";
                Sheet.Cells[row, 26].Value = "PuntoEmision";
                Sheet.Cells[row, 27].Value = "Secuencial";
                Sheet.Cells[row, 28].Value = "Autorizacion";
                Sheet.Cells[row, 29].Value = "FechaEmision";
                Sheet.Cells[row, 30].Value = "TipoCliente";
                Sheet.Cells[row, 31].Value = "DenominacionExportador";
                Sheet.Cells[row, 32].Value = "TipoIngExt";
                Sheet.Cells[row, 33].Value = "GravaImpExterior";
                Sheet.Cells[row, 34].Value = "ImpuestoOtroPais";
                row++;
                #endregion  Exportaciones

                #region Comporbantes Anulados
                var _anulados = model.ToAtsAnulados();
                row++;
                Sheet.Cells[row, 1].Value = "COMPROBANTES ANULADOS";
                Sheet.Cells[row, 1].Style.Font.Size = 16;
                Sheet.Cells[row, 1, row, 5].Merge = true;
                Sheet.Cells[row, 1, row, 5].Style.Font.Color.SetColor(Color.Navy);

                row++;
                Sheet.Cells[row, 1].Value = "TipoIDInformante";
                Sheet.Cells[row, 2].Value = "IdInformante";
                Sheet.Cells[row, 3].Value = "RazonSocial";
                Sheet.Cells[row, 4].Value = "Anio";
                Sheet.Cells[row, 5].Value = "Mes";
                Sheet.Cells[row, 6].Value = "NumEstabRuc";
                Sheet.Cells[row, 7].Value = "TotalVentas";
                Sheet.Cells[row, 8].Value = "CodigoOperativo";
                Sheet.Cells[row, 9].Value = "TipoComprobante";
                Sheet.Cells[row, 10].Value = "Establecimiento";
                Sheet.Cells[row, 11].Value = "PuntoEmision";
                Sheet.Cells[row, 12].Value = "SecuencialInicio";
                Sheet.Cells[row, 13].Value = "SecuencialFin";
                Sheet.Cells[row, 14].Value = "Autorizacion";

                Sheet.Column(2).Style.Numberformat.Format = stringFormat;
                Sheet.Column(5).Style.Numberformat.Format = stringFormat;
                Sheet.Column(6).Style.Numberformat.Format = stringFormat;
                Sheet.Column(7).Style.Numberformat.Format = moneyFormat;
                Sheet.Column(9).Style.Numberformat.Format = stringFormat;
                Sheet.Column(10).Style.Numberformat.Format = stringFormat;
                Sheet.Column(11).Style.Numberformat.Format = stringFormat;
                Sheet.Column(12).Style.Numberformat.Format = stringFormat;
                Sheet.Column(13).Style.Numberformat.Format = stringFormat;
                Sheet.Column(14).Style.Numberformat.Format = stringFormat;

                row++;
                foreach (var anu in _anulados)
                {
                    Sheet.Cells[row, 1].Value = anu.TipoIDInformante;
                    Sheet.Cells[row, 2].Value = anu.IdInformante;
                    Sheet.Cells[row, 3].Value = anu.RazonSocial;
                    Sheet.Cells[row, 4].Value = anu.Anio;
                    Sheet.Cells[row, 5].Value = anu.Mes;
                    Sheet.Cells[row, 6].Value = anu.NumEstabRuc;
                    Sheet.Cells[row, 7].Value = anu.TotalVentas;
                    Sheet.Cells[row, 8].Value = anu.CodigoOperativo;
                    Sheet.Cells[row, 9].Value = anu.TipoComprobante;
                    Sheet.Cells[row, 10].Value = anu.Establecimiento;
                    Sheet.Cells[row, 11].Value = anu.PuntoEmision;
                    Sheet.Cells[row, 12].Value = anu.SecuencialInicio;
                    Sheet.Cells[row, 13].Value = anu.SecuencialFin;
                    Sheet.Cells[row, 14].Value = anu.Autorizacion;
                    row++;
                }

                #endregion Comporbantes Anulados
                Sheet.Cells["A:AZ"].AutoFitColumns();
               // var _result = excelPkg.GetAsByteArray();
               //System.IO.File.WriteAllBytes(@"E:\Ats\ats.xlsx", _result);
               // return _result;
               return excelPkg.GetAsByteArray();
            }
        }

        public static byte[] ToAtsXml(this AtsReporte model)
        {
            string xmlContent = "";           
            var ats = model.ToXmlAts();
            try
            {                
                var serializer = new XmlSerializer(typeof(ATS));

                using (StringWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, ats);
                    xmlContent = writer.ToString();
                }
            }
            catch (Exception ex)
            {
                
                return null;
            }

            if (!String.IsNullOrEmpty(xmlContent)) //Validamos XML
            {
                string xsdPath = Path.Combine(Constants.ServerPath, "ATSXml", $"ats.xsd"); 
                // Cargar el esquema XSD
                XmlSchemaSet schemaSet = new XmlSchemaSet();
                schemaSet.Add(null, xsdPath);

                // Configurar las configuraciones del lector XML
                var settings = new XmlReaderSettings
                {
                    Schemas = schemaSet,
                    ValidationType = ValidationType.Schema
                };

                // Suscribirse al evento de validación
                settings.ValidationEventHandler += new ValidationEventHandler(ValidationEventHandler);

                // Validar el XML
                try
                {
                    // Validar el XML
                    using (StringReader stringReader = new StringReader(xmlContent))
                    {
                        using (XmlReader xmlReader = XmlReader.Create(stringReader, settings))
                        {
                            while (xmlReader.Read()) { }
                        }
                    }

                    return ToBytesXml(xmlContent);
                }
                catch (XmlException ex)
                {
                    return ToBytesXml(xmlContent); 
                }
                catch (Exception ex)
                {                    
                    return ToBytesXml(xmlContent); 
                }
            }

            return null;
        }

        private static byte[] ToBytesXml(string xmlcontent)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlcontent);
                MemoryStream ms = new MemoryStream();
                doc.Save(ms);
                return ms.ToArray();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                throw new XmlException($"Advertencia: {e.Message}");
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                throw new XmlException($"Error: {e.Message}");
            }
        }

        public static string MonthName(this int month)
        {
            var ci = new CultureInfo("es-ES");
            var nameMont = ci.DateTimeFormat.GetMonthName(month);          
            return nameMont;
        }
    }
}

namespace Ecuafact.WebAPI.Domain
{
    internal static class DomainExtensions
    {
        public static byte[] GetLogo(this Issuer issuer)
        {
            string logoIssuerFile = Path.Combine(Constants.EngineLocation, $"{issuer.RUC}_logo.jpg");

            if (!File.Exists(logoIssuerFile))
            {
                logoIssuerFile = Path.Combine(Constants.EngineLocation, $"no_logo.jpg");
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

        public static byte[] GeneraCodigoBarra(this Document document, BarcodeLib.TYPE tipoCodigo)
        {
            byte[] tempCodigo = null;
            var texto = document.AccessKey;
            if (!string.IsNullOrEmpty(texto))
            {
                BarcodeLib.Barcode b = new BarcodeLib.Barcode();
                int W = 350;
                int H = 70;
                b.Alignment = BarcodeLib.AlignmentPositions.CENTER;

                //barcode alignment
                b.Alignment = BarcodeLib.AlignmentPositions.CENTER;


                BarcodeLib.TYPE type = tipoCodigo; //BarcodeLib.TYPE.CODE128;

                try
                {
                    if (type != BarcodeLib.TYPE.UNSPECIFIED)
                    {
                        b.IncludeLabel = false;

                        b.RotateFlipType = System.Drawing.RotateFlipType.RotateNoneFlipNone;// (RotateFlipType)Enum.Parse(typeof(RotateFlipType), this.cbRotateFlip.SelectedItem.ToString(), true);

                        System.Drawing.Image img = b.Encode(type, texto, System.Drawing.Color.Black, System.Drawing.Color.White, W, H);

                        System.IO.MemoryStream ms = new System.IO.MemoryStream();
                        img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);

                        tempCodigo = ms.GetBuffer();

                        ms.Flush();

                    }

                }//try
                catch (Exception ex)
                {
                    throw ex;
                    //MessageBox.Show(ex.Message);
                }//catch

            }

            return tempCodigo;
        }
    }
}
