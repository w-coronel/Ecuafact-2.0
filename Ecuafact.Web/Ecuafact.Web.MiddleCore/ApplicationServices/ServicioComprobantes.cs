using Ecuafact.Web.MiddleCore.NexusApiServices;
using System.Net.Http;
using Ecuafact.Web.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Ecuafact.Web.Domain.Services;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Ecuafact.Web.MiddleCore.ApplicationServices
{
    public static class ServicioComprobantes
    { 
        public static List<DeductibleType> ObtenerTiposDeducibles(string token)
        {
            try
            {
                var httpClient = ClientHelper.GetClient(token);

                var serviceUrl = $"{Constants.ServiceAppUrl}/documents/deductibles";

                var response = httpClient.GetAsync(serviceUrl).Result;

                if (response.IsSuccessStatusCode)
                {
                    var deductibles = response.GetContent<DeductibleTypeResponse>();

                    return deductibles?.deductibles ?? new List<DeductibleType>();
                }
            }
            catch (Exception) { }

            return new List<DeductibleType>();
        }    
        public static saveDefaultDeductiblesRespones AutoClasificarGastos(string token, saveDefaultDeductiblesRequest request)
        {
            try
            {
                var httpClient = ClientHelper.GetClient(token);

                var serviceUrl = $"{Constants.ServiceAppUrl}/SaveDefaultDeductibles";

                var response = httpClient.PostAsync(serviceUrl, request.ToContent()).Result;

                if (response.IsSuccessStatusCode)
                {
                    return response.GetContent<saveDefaultDeductiblesRespones>();
                }
            }
            catch (Exception) { }

            return default;
        }
        public static SetDocumentsDeductiblesResponse ClasificarGastosDocumento(string token, long id, SetDocumentsDeductiblesRequest request)
        {
            try
            {
                var httpClient = ClientHelper.GetClient(token);
                var serviceUrl = $"{Constants.ServiceAppUrl}/documents/{id}/deductible";
                var response = httpClient.PostAsync(serviceUrl, request.ToContent()).Result;
                if (response.IsSuccessStatusCode)
                {
                    return response.GetContent<SetDocumentsDeductiblesResponse>();
                }
            }
            catch (Exception) { }

            return default;
        }
        public static async Task<HttpResponseMessage> TipoSustentoDocumento(string token, long id, string supportTypeCode, int emissionType = 1)
        {
            try
            {
                var httpClient = ClientHelper.GetClient(token);
                var serviceUrl = $"{Constants.WebApiUrl}/documents/received/{id}/sustenancetype/{supportTypeCode}/{emissionType}";
                var response = await httpClient.PostAsync(serviceUrl, default);
                return response;
            }
            catch (Exception) { }

            return default;
        }
        public static async Task<DocumentReceived> ObtenerComprobanteRecibidoById(string token, string id)
        {
            try
            {
                var httpClient = ClientHelper.GetClient(token);
                var serviceUrl = $"{Constants.WebApiUrl}/documents/{id}/received";
                var response = await httpClient.GetAsync(serviceUrl);
                if (response.IsSuccessStatusCode)
                {
                    var result = response.GetContent<DocumentReceived>();
                    return result;
                }
            }
            catch (Exception) { }

            return default;
        }
        public static Document ObtenerComprobanteByIdOld(string token, string id)
        {
            try
            {
                var httpClient = ClientHelper.GetClient(token);

                var serviceUrl = $"{Constants.ServiceAppUrl}/documents/{id}/detail?client_token={token}&client_type=1";

                var response = httpClient.GetAsync(serviceUrl).Result;

                if (response.IsSuccessStatusCode)
                {
                    var result = response.GetContent<DocumentUnicoResponse>();

                    return result?.document;
                }
            }
            catch (Exception) { }

            return default;
        }
        public static DocumentReceived ObtenerComprobanteById(string token, string id)
        {
            try
            {
                var httpClient = ClientHelper.GetClient(token);
                var serviceUrl = $"{Constants.WebApiUrl}/documents/{id}/received/detail";
                var response = httpClient.GetAsync(serviceUrl).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = response.GetContent<DocumentResponseDetail>();
                    return result?.document;
                }
            }
            catch (Exception) { }

            return default;
        }
        public static async Task<HttpResponseMessage> DescargarComprobanteAsync(string token, object id, string type)
        { 
            return await ClientHelper.GetClient(token).GetAsync($"{Constants.WebApiUrl}/documents/{id}/{type}"); 
        }
        private const string CodigoDocumentosEmitidos = "0";
        private const string CodigoDocumentosRecibidos = "0";

        public static DocumentResponse ObtenerComprobantesRecibidos(string token, ConsultaDocumentosParamsDto paramsDto)
        {
            try
            {
                var star = paramsDto.FechaInicio.ToString("yyyy-MM-dd");
                var end = paramsDto.FechaHasta.ToString("yyyy-MM-dd");
                var codDoc = string.IsNullOrEmpty(paramsDto.TipoDocumento) ? "0" : paramsDto.TipoDocumento;
                var search = paramsDto.Contenido ?? string.Empty;
                var state = CodigoDocumentosRecibidos;
                var deductible = paramsDto.Deducible ?? "-1";
                var pageNumber = paramsDto.NumeroPagina ?? 1;
                var clientType = "1"; // TIPO DE METODO -- RECIBIDOS
                var httpClient = ClientHelper.GetClient(token);                              
                var serviceUrl = $"{Constants.ServiceAppUrl}/documents?client_token={token}&from={star}&to={end}&cod_doc={codDoc}&state={state}&deductible={deductible}&page_number={pageNumber}&search={search}&client_Type={clientType}";
                var response = httpClient.GetAsync(serviceUrl).Result;
                if (response.IsSuccessStatusCode)
                {
                    return response.GetContent<DocumentResponse>();
                }
            }
            catch (Exception ex)
            {
                return new DocumentResponse() { documents = new List<Document>(), result = new Error() { code = "400", message = ex.ToString() } };
            }

            return new DocumentResponse() { documents = new List<Document>(), result = new Error() { code = "400", message = "Los datos ingresados son incorrectos" } };

        }
        public static async Task<DatatableList<DocumentReceived>> ObtenerComprobantesRecibidosPagedAsync(string token, ConsultaDocumentosParamsDto paramsDto)
        {
            try
            {
                var star = paramsDto.FechaInicio;
                var end = paramsDto.FechaHasta;
                var codDoc = string.IsNullOrEmpty(paramsDto.TipoDocumento) ? "0" : paramsDto.TipoDocumento;
                var search = paramsDto.Contenido ?? string.Empty;                
                var deductible = paramsDto.Deducible ?? "-1";
                var pageNumber = paramsDto.NumeroPagina ?? 1;
                var pageSize = paramsDto.CantidadPorPagina ?? 10;
                var httpClient = ClientHelper.GetClient(token);
                var serviceUrl = $"{Constants.WebApiUrl}/documents/received/page?search={search}&startDate={star}&endDate={end}&documentType={codDoc}&deductible={deductible}&page={pageNumber}&pageSize={pageSize}";
                var response = await httpClient.GetAsync(new Uri(serviceUrl));
                if (response.IsSuccessStatusCode)
                {
                    var _result = response.GetContent<DocumentRecivedResponse>();
                    var documentReceived = _result.documents;
                    int pageCount = 0;
                    if (_result.meta.count > 0)
                    {
                        pageCount = Convert.ToInt32(_result.meta.count);
                    }                   

                    var result = new DatatableList<DocumentReceived>(documentReceived) { recordsFiltered = pageCount, recordsTotal = pageCount };
                    return result;
                }
            }
            catch (Exception ex)
            {
                return new DatatableList<DocumentReceived>(ex.Message);
                
            }

            return new DatatableList<DocumentReceived>();
           

        }
        public static async Task<DocumentResponse> ObtenerComprobantesRecibidosAsync(string token, ConsultaDocumentosParamsDto paramsDto)
        {
            try
            {
                var star = paramsDto.FechaInicio.ToString("yyyy-MM-dd");
                var end = paramsDto.FechaHasta.ToString("yyyy-MM-dd");
                var codDoc = string.IsNullOrEmpty(paramsDto.TipoDocumento) ? "0" : paramsDto.TipoDocumento;
                var search = paramsDto.Contenido ?? string.Empty;
                var state = CodigoDocumentosRecibidos;
                var deductible = -1; // TODOS
                var pageNumber = paramsDto.NumeroPagina;
                var clientType = "1"; // TIPO DE METODO -- RECIBIDOS

                var httpClient = ClientHelper.GetClient(token);

                var serviceUrl = $"{Constants.ServiceAppUrl}/documents?client_token={token}&from={star}&to={end}&cod_doc={codDoc}&state={state}&deductible={deductible}&page_number={pageNumber}&search={search}&client_Type={clientType}";

                var response = await httpClient.GetAsync(serviceUrl);

                if (response.IsSuccessStatusCode)
                {
                    return await response.GetContentAsync<DocumentResponse>();
                }
            }
            catch (Exception ex)
            {
                return new DocumentResponse() { documents = new List<Document>(), result = new Error() { code = "400", message = ex.ToString() } };
            }

            return new DocumentResponse() { documents = new List<Document>(), result = new Error() { code = "400", message = "Los datos ingresados son incorrectos" } };

        }
        public static DocumentResponse ObtenerComprobantesEmitidos(string token, ConsultaDocumentosParamsDto paramsDto)
        {
            var star = paramsDto.FechaInicio.ToString("yyyy-MM-dd");
            var end = paramsDto.FechaHasta.ToString("yyyy-MM-dd");
            var codDoc = string.IsNullOrEmpty(paramsDto.TipoDocumento) ? "0" : paramsDto.TipoDocumento;
            var search = paramsDto.Contenido;
            var state = CodigoDocumentosEmitidos;
            var deductible = -1;
            var pageNumber = paramsDto.NumeroPagina;
            var clientType = "-1"; // TIPO DE METODO -- EMITIDOS

            var httpClient = ClientHelper.GetClient(token);
            var serviceUrl = $"{Constants.ServiceEngineUrl}/documents?client_token={token}&from={star}&to={end}&cod_doc={codDoc}&state={state}&deductible={deductible}&page_number={pageNumber}&search={search}&client_Type={clientType}";
            var response = httpClient.GetAsync(serviceUrl).Result;

            if (response.IsSuccessStatusCode)
            {
                return response.GetContent<DocumentResponse>();
            }

            return new DocumentResponse() { result = new Error() { code = "400", message = "Los datos ingresados son incorrectos" } };

        }
        public static async Task<HttpResponseMessage> DescargarNotaVentaAsync(string token, object id)
        {
            return await ClientHelper.GetClient(token).GetAsync($"{Constants.WebApiUrl}/documents/{id}/salesnote/download");
        }
        public static async Task<HttpResponseMessage> DescargarRecibidosAsync(string token, string filtro = null, DateTime? startDate = null, DateTime? endDate = null, string documentType = "", string deductible ="")
        {
            try
            {
                if (string.IsNullOrEmpty(filtro))
                {
                    filtro = string.Empty;
                } // Si el filtro esta nulo, se lo envia vacio.

                var qs = $"search={filtro}";

                if (!string.IsNullOrEmpty(documentType) && documentType != "0")
                {
                    qs += $"&documentType={documentType}";
                }

                if (!string.IsNullOrEmpty(deductible) && deductible != "0")
                {
                    qs += $"&deductible={deductible}";
                }
                if (startDate != null)
                {
                    qs += $"&startdate={startDate.Value:yyyy-MM-dd}";
                }

                if (endDate != null)
                {
                    qs += $"&endDate={endDate.Value:yyyy-MM-dd}";
                }
                return await ClientHelper.GetClient(token).GetAsync($"{Constants.WebApiUrl}/documents/export/received?{qs}");
            }
            catch (Exception ex)
            {
                ex.GetType();
                return new HttpResponseMessage();
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="claveAcceso"></param>
        /// <returns></returns>
        public static string ObtenerUrlRecibidos(string claveAcceso, string tipo)
        {
            tipo = tipo.ToUpper();
              
            if (!string.IsNullOrEmpty(claveAcceso))
            {
                if (tipo == "PDF")
                {
                    return $"{Constants.ServiceUrl}/GetPDF.aspx?CA={claveAcceso}";
                }
                else if (tipo == "XML")
                {
                    return $"{Constants.ServiceUrl}/GetXML.aspx?CA={claveAcceso}";
                }
                else if (tipo == "ZIP")
                {
                    return $"{Constants.ServiceUrl}/GetZIP.aspx?CA={claveAcceso}";
                }
            }

            return string.Empty;
        }

        public static async Task<DocumentStatusInfo> ObtenerAutorizacionAsync(string token, string ruc, string docType, string docNumber)
        {
            // http://nube.ecuafact.com/ApiDevTest/Service.svc/documents/status?token=TESTTOKEN&ruc=0992882549001&docType=01&docNumber=001-001-000000001


            var document = new DocumentStatusInfo();

            var httpClient = ClientHelper.GetClient(token);
            {
                var response = await httpClient.GetAsync(new Uri($"{Constants.ServiceEngineUrl}/documents/status?token={token}&ruc={ruc}&docType={docType}&docNumber={docNumber}"));

                if (response.IsSuccessStatusCode)
                {
                    document = response.GetContent<DocumentStatusInfo>();
                }
            }

            return document;

        }

        public static async Task<OperationResult> EnviarCorreoAsync(string token, string email, string uid)
        { 
            var httpClient = ClientHelper.GetClient(token);
            {
                var request = new resendRequest
                {
                    client_token = token,
                    resend = new resend { email = email }
                };

                var response = await httpClient.PostAsync(new Uri($"{Constants.ServiceAppUrl}/documents/{uid}/Resend"), request.ToContent());

                if (response.IsSuccessStatusCode)
                {
                    var result = response.GetContent<resendResponse>();

                    if (result != null && result?.result.code == "1000")
                    {
                        return await Task.FromResult(new OperationResult(true, System.Net.HttpStatusCode.OK, result.result?.message ?? "El reenvío se ha realizado exitosamente"));
                    }
                }
            }

            return await Task.FromResult(new OperationResult(false, System.Net.HttpStatusCode.InternalServerError, "Error al reenviar el documento"));
        }

        public static DocumentRecividModel ObtenerComprobanteByXml(UploadFileRequest request)
        {
            try
            {

                var response = JsonWebApiHelper.ExecuteJsonWebApi<UploadFileResponseModel>($"{Constants.ServiceAppUrl}/documents/uploadXMLInvoice", HttpMethod.Post, request);               
                if (response.Result.Code == "100")
                {
                    return response.Document;
                }
            }
            catch (Exception ex) {

                var msaje = ex.ToString();
            }

            return default;
        }

        public static long SincronizarRecibidos(string token)
        {
            try
            {
                var httpClient = ClientHelper.GetClient(token);
                var serviceUrl = $"{Constants.WebApiUrl}/documents/received/syncup";
                var response = httpClient.GetAsync(serviceUrl).Result;
                if (response.IsSuccessStatusCode)
                { 
                    return 1;
                }
            }
            catch (Exception) { }

            return 0;
        }

        public static async Task<HttpResponseMessage> GuardarNotaVentaAsync(string issuerToken, SalesNoteRequestModel model)
        {
            var httpClient = ClientHelper.GetClient(issuerToken);
            var content = new StringContent(JsonConvert.SerializeObject(model));
            //var _model = JsonConvert.SerializeObject(model);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response;

            response = await httpClient.PostAsync($"{Constants.WebApiUrl}/documents/received/salesnote", content).ConfigureAwait(false);

            return response;
        }

        public static async Task<OperationResult> AnularDocumentoRecibidoAsync(string token, int id)
        {

            var httpClient = ClientHelper.GetClient(token);
            var serviceUrl = $"{Constants.WebApiUrl}/documents/received/{id}/delete";
            var response = await httpClient.DeleteAsync(serviceUrl, default);
            if (response.IsSuccessStatusCode)
            {
                var result = response.GetContent<OperationResult>();
                if (result.IsSuccess)
                {
                    return await Task.FromResult(new OperationResult(true, System.Net.HttpStatusCode.OK, "El documento fue anulado correctamente"));
                }
            }

            return await Task.FromResult(new OperationResult(false, System.Net.HttpStatusCode.InternalServerError, "Error al anular el documento"));
        }

    }
}