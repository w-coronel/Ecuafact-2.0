using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ecuafact.WebAPI.Dal.Repository;
using Ecuafact.WebAPI.Dal.Repository.Extensions;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Entities.Engine;
using Ecuafact.WebAPI.Domain.Repository;
using Ecuafact.WebAPI.Domain.Services;

namespace Ecuafact.WebAPI.Dal.Services
{
    public class DocumentsService : IDocumentsService
    {
        private readonly IDocumentRepository _documentRepository;
        private IEntityRepository<DocumentType> _documentTypeRepository;

        public DocumentsService(IDocumentRepository documentRepository, IEntityRepository<DocumentType> documentTypeRepository)
        {
            _documentRepository = documentRepository;
            _documentTypeRepository = documentTypeRepository;
        }

        public Document GetIssuerDocumentById(long issuerId, long documentId)
        {
            return _documentRepository.GetDocumentById(issuerId, documentId);
        }

        public Document GetIssuerDocument(long issuerId, string documentType,
            string documentNumber, string establishmentCode, string issuePointCode)
        {
            return _documentRepository.GetDocument(issuerId, documentType, documentNumber, establishmentCode, issuePointCode);
        }

        public IQueryable<Document> GetIssuerDocuments(long issuerId, string documentType, DocumentStatusEnum? status, bool authorizeDate)
        {
            return _documentRepository.GetDocumentsByIssuer(issuerId, documentType, status, authorizeDate);
        }

        public IQueryable<Document> GetAllIssuerDocuments(long issuerId)
        {
            return _documentRepository.GetAllDocumentsByIssuer(issuerId);
        }

        public OperationResult<Document> AddDocument(long issuerId, Document document)
        {
            try
            {
                var docType = GetDocumentTypeDescription(document.DocumentTypeCode);

                if (docType == null)
                {
                    throw new Exception("El tipo de documento enviado es incorrecto!");
                }
                document.Sequential = Guid.NewGuid().ToString().Substring(0, 6).PadLeft(9, 'X');
                if (document.Status.HasValue && document.Status != DocumentStatusEnum.Draft)
                {
                    var seq = _documentRepository.GetNextDocumentSequential(issuerId, document.EstablishmentCode, document.IssuePointCode, docType.Id, docType.Name);
                    document.Sequential = seq.ToString().PadLeft(9, '0');
                    var accessKey = GetClaveAcceso(document);
                    document.AccessKey = document.AuthorizationNumber = accessKey;
                    document.StatusMsg = !string.IsNullOrWhiteSpace(accessKey) ? DocumentStatusEnum.Authorized.GetDisplayValue().ToUpper() : "";
                    document.Status = !string.IsNullOrWhiteSpace(accessKey) ? DocumentStatusEnum.Authorized : document.Status;
                    _documentRepository.GetCounterDocument(issuerId);
                    //document.Processed = true;
                    if (string.IsNullOrWhiteSpace(document.PurchaseOrder))
                        document.PurchaseOrder = accessKey;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(document.PurchaseOrder))
                        document.PurchaseOrder = $"{DateTime.Now:ddMMyyyy}{document.DocumentTypeCode}{document.RUC}2{document.EstablishmentCode}{document.IssuePointCode}{document.Sequential}";
                }
                document.IsEnabled = true;
                document.CreatedOn = DateTime.Now;
                document.IssuerId = issuerId;
                _documentRepository.AddDocument(document);              

                return new OperationResult<Document>(true, HttpStatusCode.OK)
                {
                    Entity = document,
                    DevMessage = $"Se guardo el documento #{document.Id} con el estado {document.Status?.GetDisplayValue() ?? "No Disponible"}",
                    UserMessage = $"Se guardo el documento! {docType.Name} #:{document.EstablishmentCode}-{document.IssuePointCode}-{document.Sequential}"
                };
            }
            catch (DbEntityValidationException ex)
            {
                var msg = "";
                foreach (var error in ex.EntityValidationErrors)
                    foreach (var valError in error.ValidationErrors)
                    {
                        msg += $"{valError.PropertyName}: {valError.ErrorMessage}{Environment.NewLine}";
                    }

                msg += $"{ex.Message}-{ex.InnerException?.Message}-{ex.InnerException?.InnerException?.Message}";


                return new OperationResult<Document>(false, HttpStatusCode.InternalServerError)
                {
                    DevMessage = msg,

                    UserMessage = "Ocurrio un error al grabar el documento"
                };
            }
            catch (Exception ex)
            {
                return new OperationResult<Document>(false, HttpStatusCode.InternalServerError)
                {
                    DevMessage = $"{ex.Message}-{ex.InnerException?.Message}-{ex.InnerException?.InnerException?.Message}",
                    UserMessage = "Ocurrio un error al grabar el documento"
                };
            }
        }

        public OperationResult<Document> IssueDocument(long issuerId, long id, string reason, DateTime? issueDate)
        {
            try
            {
                var document = GetIssuerDocumentById(issuerId, id);

                // Antes que todo debemos validar que en realidad sea un documento no emitido:
                if (document.Status != DocumentStatusEnum.Draft)
                {
                    throw new Exception("El estado del documento no permite su emisión o ya fue emitido.");
                }

                if (!string.IsNullOrEmpty(document.Reason))
                {
                    reason = $"{document.Reason}- {reason}";
                }

                document.Reason = reason;
                document.Status = DocumentStatusEnum.Issued;

                // Solo se puede emitir un documento dentro del año en curso
                // y no menor que la fecha actual.
                if (issueDate != null && issueDate > DateTime.Now.AddMonths(-12) && issueDate < DateTime.Now.AddMinutes(23))
                {
                    document.IssuedOn = issueDate.Value;
                }

                return UpdateDocument(document, false);
            }
            catch (Exception ex)
            {
                return new OperationResult<Document>(false, HttpStatusCode.InternalServerError)
                {
                    DevMessage = $"{ex.Message}-{ex.InnerException?.Message}-{ex.InnerException?.InnerException?.Message}",
                    UserMessage = $"Ocurrio un error al actualizar el documento: {ex.Message}"
                };
            }
        }

        public OperationResult DeleteDocument(long issuerId, long id, string reason)
        {
            try
            {

                var document = GetIssuerDocumentById(issuerId, id);
                var anula = document.Status > 0 ? "anula" : "elimina";
                if (document.Status == DocumentStatusEnum.Validated || document.Status == DocumentStatusEnum.Draft)
                {
                    if (document.Status > 0 && string.IsNullOrEmpty(reason))
                    {
                        return new OperationResult(false, HttpStatusCode.BadRequest,
                            $"No se puede {anula}r el documento porque no se ha confirmado la petición del usuario.");
                    }

                    reason = $"Documento {anula}do: {DateTime.Now} - {reason}";

                    if (!string.IsNullOrEmpty(document.Reason))
                    {
                        reason += $"{Environment.NewLine}{document.Reason}";
                    }

                    document.IsEnabled = false;
                    document.Reason = reason;
                    document.Status = document.Status > 0 ? DocumentStatusEnum.Revoked : DocumentStatusEnum.Deleted;
                    document.StatusMsg = $"Documento {anula}do: {reason} {DateTime.Now}";

                    var result = UpdateDocument(document, false);

                    if (result.IsSuccess)
                    {
                        result.UserMessage = result.DevMessage = $"El Documento fue {anula}do correctamente!";
                    }

                    return result;
                }

                return new OperationResult(false, HttpStatusCode.BadRequest, $"Solo se pueden anuluar o eliminar docuemntos autorizados o borrador.");
            }
            catch (Exception ex)
            {
                return new OperationResult(false, HttpStatusCode.InternalServerError)
                {
                    DevMessage = $"{ex.Message}-{ex.InnerException?.Message}-{ex.InnerException?.InnerException?.Message}",
                    UserMessage = $"Ocurrio un error al actualizar el documento: {ex.Message}"
                };
            }
        }

        public OperationResult<Document> UpdateDocument(Document document)
        {
            return UpdateDocument(document, true);
        }

        private OperationResult<Document> UpdateDocument(Document document, bool updateAll)
        {
            try
            {
                var docType = GetDocumentTypeDescription(document.DocumentTypeCode);
                if (docType == null)
                {
                    throw new Exception("El tipo de documento enviado es incorrecto!");
                }                
                // se genera el consecutivo del documento
                if (document.Status == DocumentStatusEnum.Issued && !document.Sequential.IsNumeric())
                {
                    var _sequential = document.Sequential;
                    var seq = _documentRepository.GetNextDocumentSequential(document.IssuerId, document.EstablishmentCode, document.IssuePointCode, docType.Id, docType.Name);
                    var docSequential = seq.ToString().PadLeft(9, '0');
                    document.Sequential = docSequential;
                    //se genera la clave de acceso
                    var accessKey = GetClaveAcceso(document);
                    document.AccessKey = document.AuthorizationNumber = accessKey;
                    document.StatusMsg = !string.IsNullOrWhiteSpace(accessKey) ? DocumentStatusEnum.Authorized.GetDisplayValue().ToUpper() : "";
                    document.Status = !string.IsNullOrWhiteSpace(accessKey) ? DocumentStatusEnum.Authorized : document.Status;
                    //conteo de documentos emitidos
                    _documentRepository.GetCounterDocument(document.IssuerId);
                    if (string.IsNullOrWhiteSpace(document.PurchaseOrder))
                        document.PurchaseOrder = accessKey;
                    else if(!_sequential.IsNumeric())
                        document.PurchaseOrder = accessKey;

                }
                else if(document.Status == DocumentStatusEnum.Issued && document.Sequential.IsNumeric())
                {
                    var accessKey = GetClaveAcceso(document);
                    document.AccessKey = document.AuthorizationNumber = accessKey;
                    document.StatusMsg = !string.IsNullOrWhiteSpace(accessKey) ? DocumentStatusEnum.Authorized.GetDisplayValue().ToUpper() : "";
                    document.Status = !string.IsNullOrWhiteSpace(accessKey) ? DocumentStatusEnum.Authorized : document.Status;
                    if (string.IsNullOrWhiteSpace(document.PurchaseOrder))
                        document.PurchaseOrder = accessKey;
                }
                else if(document.Status == DocumentStatusEnum.Draft)
                {
                    if (string.IsNullOrWhiteSpace(document.PurchaseOrder))
                        document.PurchaseOrder = $"{DateTime.Now:ddMMyyyy}{document.RUC}2{document.EstablishmentCode}{document.IssuePointCode}{document.Sequential}"; ;
                }

                document.LastModifiedOn = DateTime.Now;
                _documentRepository.UpdateDocument(document, updateAll);                

                return new OperationResult<Document>(true, HttpStatusCode.OK)
                {
                    Entity = document,
                    DevMessage = $"Se guardo el documento #{document.Id} con el estado {document.Status}",
                    UserMessage = $"Se guardo el documento! \r\n {docType.Name} #:{document.EstablishmentCode}-{document.IssuePointCode}-{document.Sequential}"
                };
            }
            catch (DbEntityValidationException ex)
            {
                var msg = "";
                foreach (var error in ex.EntityValidationErrors)
                    foreach (var valError in error.ValidationErrors)
                    {
                        msg += $"{valError.PropertyName}: {valError.ErrorMessage}{Environment.NewLine}";
                    }

                msg += $"{ex.Message}-{ex.InnerException?.Message}-{ex.InnerException?.InnerException?.Message}";


                return new OperationResult<Document>(false, HttpStatusCode.InternalServerError)
                {
                    DevMessage = msg,
                    UserMessage = $"Ocurrio un error al actualizar el documento: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new OperationResult<Document>(false, HttpStatusCode.InternalServerError)
                {
                    DevMessage = $"{ex.Message}-{ex.InnerException?.Message}-{ex.InnerException?.InnerException?.Message}",
                    UserMessage = "Ocurrio un error al actualizar el documento"
                };
            }
        }

        private DocumentType GetDocumentTypeDescription(string documentTypeCode)
        {
            var docType = _documentTypeRepository.FindBy(dc => dc.SriCode.Equals(documentTypeCode)).FirstOrDefault();
            return docType;
        }

        public OperationResult<Document> DetailsDependants(Document document)
        {
            try
            {
                _documentRepository.DetailsDependants(document);
                document = this.GetIssuerDocumentById(document.IssuerId, document.Id);
                return new OperationResult<Document>(true, HttpStatusCode.OK)
                {
                    Entity = document,
                    UserMessage = $"OK"
                };

            }
            catch (Exception ex)
            {
                return new OperationResult<Document>(false, HttpStatusCode.InternalServerError)
                {
                    DevMessage = $"{ex.Message}-{ex.InnerException?.Message}-{ex.InnerException?.InnerException?.Message}",
                    UserMessage = $"Ocurrio un error al actualizar el documento: {ex.Message}"
                };
            }
        }

        public void GetAllIssuerDocuments(string accessKey)
        {
             _documentRepository.ReloadEngineDocument(accessKey);
        }


        /// <summary>
        /// Proceso de actualizacion del estado en los documentos
        /// </summary>
        /// <param name="issuerId">Id del Emisor</param>
        /// <param name="documentId">Id del documento</param>
        /// <param name="statusInfo">Informacion del estado para este documento</param>
        /// <returns></returns>
        public OperationResult<Document> UpdateDocumentStatus(long issuerId, long documentId, DocumentStatusInfo statusInfo)
        {
            try
            {

                var document = this.GetIssuerDocumentById(issuerId, documentId);

                // Debe existir el documento para actualizarlo
                if (document != null)
                {

                    // No actualiza el estado si el codigo de error es 600: No Procesado
                    if (statusInfo != null && (statusInfo.Result == null || statusInfo.Result.Code != 600))
                    {
                        // Debido a diversos problemas con el campo en la estructura usamos este:
                        var status = DocumentStatusEnum.Draft;

                        if (!Enum.TryParse(statusInfo.State, out status))
                        {
                            status = DocumentStatusEnum.Error; // Si el estado esta definido de esta forma es que tiene algun error
                            statusInfo.StatusMsg = $"ERROR {statusInfo.Result.Code}:{statusInfo.StatusMsg}  {statusInfo.Result.Message} - Error al interpretar el estado recibido por el servicio.";
                        }

                        document.Status = status;
                        document.StatusMsg = GetDefaultValue(statusInfo.StatusMsg);
                        document.AccessKey = GetDefaultValue(statusInfo.AccessKey, string.Empty.PadRight(49, '0'));
                        document.AuthorizationDate = GetDefaultValue(statusInfo.AuthorizationDate);
                        document.AuthorizationNumber = GetDefaultValue(statusInfo.AuthorizationNumber);
                        //document.XML = statusInfo.XML;
                        //document.PDF = statusInfo.PDF;
                    }

                    return this.UpdateDocument(document, false);

                }

                return new OperationResult<Document>(false, HttpStatusCode.InternalServerError)
                {
                    DevMessage = $"Documento no existe",
                    UserMessage = "Documento especificado no existe"
                };

            }
            catch (Exception ex)
            {
                return new OperationResult<Document>(false, HttpStatusCode.InternalServerError)
                {
                    DevMessage = $"{ex.Message}-{ex.InnerException?.Message}-{ex.ToString()}",
                    UserMessage = "Ocurrio un error al actualizar el documento"
                };
            }
        }

        private string GetDefaultValue(string value, string defaultValue = "")
        {
            if (string.IsNullOrEmpty(value) || value == "-")
            {
                return defaultValue;
            }

            return value;
        }       

        public string GetXmlDocument(long Id)
        {
            return _documentRepository.GenerateDocumentXml(Id);
        }

        #region Metodo para la creación de la clave de acceso
        private  string GetClaveAcceso(Document document)
        {
            var clave = "";
            try
            {
                if (!string.IsNullOrWhiteSpace(document.AccessKey))
                {
                    var length = document.AccessKey.Length;
                    var extFecha = document.AccessKey.Substring(0,8);
                    if ($"{document.IssuedOn:ddMMyyyy}" == extFecha)
                    {
                        clave = document.AccessKey;
                    }
                }

                if (string.IsNullOrWhiteSpace(clave))
                {
                    var tempFecha = $"{document.IssuedOn:dd-MM-yyyy}".Split('-');
                    // Fecha emisión del documento
                    clave += tempFecha[0].ToString().PadLeft(2, '0');
                    clave += tempFecha[1].ToString().PadLeft(2, '0');
                    clave += tempFecha[2].ToString().PadLeft(4, '0');
                    clave += document.DocumentTypeCode.PadLeft(2, '0');
                    clave += document.RUC.PadLeft(13, '0');
                    clave += EnvironmentType.Production.GetCoreValue();
                    clave += document.EstablishmentCode.PadLeft(3, '0');
                    clave += document.IssuePointCode.PadLeft(3, '0');
                    clave += document.Sequential.PadLeft(9, '0');
                    clave += Constants.CodFactorChequeoPonderadoerico;
                    clave += IssueType.Normal.GetCoreValue();
                    clave = GetDigitoVerificadorModulo11(clave);
                }
            }
            catch (Exception ex)
            {
                var msj = ex.Message;              
            }

            return clave;
        }

        private  string GetDigitoVerificadorModulo11(string preClave)
        {
            try
            {
                int i = preClave.Length;
                int factorChequeoPonderado = 2;
                int sumatoria = 0;

                while (i > 0)
                {
                    int digito = Convert.ToInt32(preClave.Substring(i - 1, 1));
                    sumatoria += digito * factorChequeoPonderado;
                    if (factorChequeoPonderado == 7)
                    {
                        factorChequeoPonderado = 2;
                    }
                    else
                    {
                        factorChequeoPonderado++;
                    }
                    i--;
                }
                int modulo11 = sumatoria % 11;
                int dif11 = 11 - modulo11;
                if (dif11 == 10)
                {
                    dif11 = 1;
                }
                else
                {
                    if (dif11 == 11)
                    {
                        dif11 = 0;
                    }
                }
                int resultado = dif11;

                preClave += resultado;
            }
            catch
            {
            }

            return preClave;
        }
        #endregion
    }
}
