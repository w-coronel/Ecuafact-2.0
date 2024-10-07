using Ecuafact.WebAPI.Dal.Core;
using Ecuafact.WebAPI.Dal.Repository;
using Ecuafact.WebAPI.Dal.Repository.Extensions;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Entities.App;
using Ecuafact.WebAPI.Domain.Entities.Engine;
using Ecuafact.WebAPI.Domain.Entities.SRI;
using Ecuafact.WebAPI.Domain.Pagination;
using Ecuafact.WebAPI.Domain.Repository;
using Ecuafact.WebAPI.Domain.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;


namespace Ecuafact.WebAPI.Dal.Services
{
    public class AppService : IAppService
    {
        private readonly IEntityRepository<Emisor> _emisorRepository;
        private readonly IEntityRepository<INFORUC> _infoRucRepository;
        private readonly IEntityRepository<INFOREGIMEN> _infoRegimenRepository;
        private readonly IEntityRepository<TipoSustento> _sustenanceTypeRepository;
        private readonly IEntityRepository<ApiV3_DocumentosData> _vDocumentosDataRepository;
        private readonly IEntityRepository<SupplierDocument> _supplierDocument;
        private readonly IEntityRepository<SettlementInfo> _settlementInfoRepository; 


        private EcuafactAppContext EcuafactAppDB = new EcuafactAppContext();
        private EcuafactSRIContext EcuafactSRIDB = new EcuafactSRIContext();

        public AppService(IDocumentReceivedRepository documentReceivedRepository,
            IEntityRepository<SettlementInfo> settlementInfoRepository)
        {
            _emisorRepository = new EntityRepository<Emisor>(EcuafactAppDB);
            _sustenanceTypeRepository = new EntityRepository<TipoSustento>(EcuafactAppDB);
            _infoRucRepository = new EntityRepository<INFORUC>(EcuafactSRIDB);
            _infoRegimenRepository = new EntityRepository<INFOREGIMEN>(EcuafactSRIDB);
            _vDocumentosDataRepository = new EntityRepository<ApiV3_DocumentosData>(EcuafactAppDB);
            _supplierDocument = new EntityRepository<SupplierDocument>(EcuafactAppDB);
            _settlementInfoRepository = settlementInfoRepository;
        }

        public OperationResult SRIPassword(string identificacion, string pass)
        {
            try
            {
                var emisores = _emisorRepository.All.Where(pr => pr.RUC.Contains(identificacion.Substring(0, 10)));

                if (emisores != null && emisores.Count()>0)
                {
                    // Configuro la clave del SRI para todos los emisores con esta identificacion.
                    foreach (var item in emisores)
                    {
                        item.SetClaveSRI(pass);
                    }
                    
                    _emisorRepository.Save();
                    // Si no hubo errores entonces todo se guardo!
                    return new OperationResult(true, HttpStatusCode.OK, "OK");
                }

                throw new Exception("El emisor especificado no se encuentra configurado!");
            }
            catch (Exception ex)
            {
                return new OperationResult(false, HttpStatusCode.InternalServerError, ex.Message);
            }
        }          

        public OperationResult<SRIContrib> SearchByRUC(string ruc)
        {
            try
            {
                var rucInfo = _infoRucRepository.All?.Where(m => m.NUMERO_RUC == ruc)?.OrderBy(m => m.NUMERO_ESTABLECIMIENTO)?.ToList();
                if (rucInfo != null && rucInfo.Count > 0)
                {
                    SRIContrib contrib = null;
                    foreach (var item in rucInfo)
                    {
                        if (contrib == null)
                        {
                            contrib = new SRIContrib
                            {
                                RUC = item.NUMERO_RUC,
                                BusinessName = item.RAZON_SOCIAL,
                                TradeName = item.NOMBRE_COMERCIAL,
                                AccountingRequired = ((item.OBLIGADO ?? "N") != "N"),
                                ContributorClass = (item.CLASE_CONTRIBUYENTE == "RISE" ? SRIContribClass.RISE : item.CLASE_CONTRIBUYENTE == "ESPECIAL" ? SRIContribClass.Special : SRIContribClass.Other),
                                Status = (item.ESTADO_CONTRIBUYENTE == "PASIVO" ? SRIContribStatus.Pasive : item.ESTADO_CONTRIBUYENTE == "SUSPENDIDO" ? SRIContribStatus.Suspended : SRIContribStatus.Active),
                                ContributorType = ((item.TIPO_CONTRIBUYENTE ?? "NATURAL").Contains("SOCIEDAD") ? SRIContribType.Juridical : SRIContribType.Natural),
                                Establishments = new List<Establishment>()
                            };

                            contrib.CreatedOn = GetDate(item.FECHA_INICIO_ACTIVIDADES);
                            contrib.LastUpdated = GetDate(item.FECHA_ACTUALIZACION);
                            contrib.ReactivationDate = GetDate(item.FECHA_REINICIO_ACTIVIDADES);
                            contrib.SuspensionDate = GetDate(item.FECHA_SUSPENSION_DEFINITIVA);
                        }

                        var estab = new Establishment
                        {
                            Id = item.UID,
                            EstablishmentNumber = item.NUMERO_ESTABLECIMIENTO,
                            CommercialName = item.NOMBRE_FANTASIA_COMERCIAL,
                            Street = item.CALLE,
                            AddressNumber = item.NUMERO,
                            City = item.DESCRIPCION_CANTON,
                            Intersection = item.INTERSECCION,
                            Town = item.DESCRIPCION_PARROQUIA,
                            Province = item.DESCRIPCION_PROVINCIA,
                            CIIU = item.CODIGO_CIIU,
                            Activity = item.ACTIVIDAD_ECONOMICA,
                            Status = ((item.ESTADO_ESTABLECIMIENTO ?? "ABI").Contains("CER") ? SRIEstabStatus.Closed : SRIEstabStatus.Open)
                        };

                        contrib.Establishments.Add(estab);
                    }

                    return new OperationResult<SRIContrib>(true, HttpStatusCode.OK, contrib);
                }

                throw new Exception("El RUC especificado no existe!");
            }
            catch (Exception ex)
            {
                return new OperationResult<SRIContrib>(ex);
            }
        }

        public OperationResult<INFOREGIMEN> IsRimpe(string ruc)
        {
            try
            {
                var rucInfo = _infoRegimenRepository.All?.Where(m => m.NUMERO_RUC == ruc && m.REGIMEN == "RIMPE").FirstOrDefault();
                if (rucInfo != null)
                {
                    return new OperationResult<INFOREGIMEN>(true, HttpStatusCode.OK, rucInfo);
                }

                throw new Exception("El RUC especificado no existe!");
            }
            catch (Exception ex)
            {
                return new OperationResult<INFOREGIMEN>(ex);
            }
        }

        public OperationResult<List<TipoSustento>> GetSustenanceType()
        {
            try
            {
                var _sustenanceType = _sustenanceTypeRepository.FindBy(data => data.Activo).ToList();
                if (_sustenanceType != null)
                {
                    return new OperationResult<List<TipoSustento>>(true, HttpStatusCode.OK, _sustenanceType);
                }

                throw new Exception("Error al listar los tipos sustentos del comprobante!");
            }
            catch (Exception ex)
            {
                return new OperationResult<List<TipoSustento>>(ex);
            }
        }

        public long SyncUpReceived(string ruc, string yearMont, DateTime dateRequest, string status)
        {
            SqlParameter identificacion = new SqlParameter("@identificacion", ruc);
            SqlParameter anioMes = new SqlParameter("@AnioMes", yearMont);           
            SqlParameter fechaSolicitud = new SqlParameter("@FechaSolicitud", dateRequest);
            SqlParameter estado = new SqlParameter("@Estado", status);

            var idOutParam = new SqlParameter
            {
                ParameterName = "@Id",
                SqlDbType = SqlDbType.Int,
                Direction = ParameterDirection.Output
            };

            _emisorRepository.Database.ExecuteSqlCommand("EXEC BOT_COLA_SINCRONIZACION_INSERT @Id out, @identificacion, @AnioMes, @FechaSolicitud, @Estado",
                idOutParam,
                identificacion,
                anioMes,
                fechaSolicitud,
                estado);
            return Convert.ToInt64(idOutParam.Value);
        }

        public DateTime? GetDate(string dateText)
        {
            DateTime date; 
            if(DateTime.TryParse(dateText, CultureInfo.GetCultureInfo("es-ES"), DateTimeStyles.None, out date))
            {
                return date;
            }
            return default;
        }

        public bool AssignSupportType(long documentoId, string supportTypeCode, int emissionType = 1)
        {
            SqlParameter pkDocument = new SqlParameter("@pkDocumento", documentoId);
            SqlParameter codTipoSustento = new SqlParameter("@codSustento", supportTypeCode);
            var idOutParam = new SqlParameter
            {
                ParameterName = "@result",
                SqlDbType = SqlDbType.Bit,
                Direction = ParameterDirection.Output
            };

            if (emissionType > 2)
            {
                _settlementInfoRepository.Database.ExecuteSqlCommand("EXEC Pr_document_SupportTypeCode @pkDocumento, @codSustento, @result out",
                pkDocument,
                codTipoSustento,
                idOutParam);
            }
            else {
                _sustenanceTypeRepository.Database.ExecuteSqlCommand("EXEC Pr_documento_AsignaTipoSustento @pkDocumento, @codSustento, @result out",
                pkDocument,
                codTipoSustento,
                idOutParam);
            }
            
            return Convert.ToBoolean(idOutParam.Value);
        }

        public OperationResult<PaginationList<ApiV3_DocumentosData>> GetDocumentosReceivedPagination(FilterDocumentReceived filter)
        {
            try
            {
                #region Consulta documentos
                var deducibleId = new SqlParameter { ParameterName = "@idDeducible", IsNullable = true, SqlDbType = SqlDbType.Int };
                var codDoc = new SqlParameter { ParameterName = "@codDoc", IsNullable = true, SqlDbType = SqlDbType.VarChar };
                var search = new SqlParameter { ParameterName = "@search", IsNullable = true, SqlDbType = SqlDbType.VarChar };
                //deducibles
                if (filter.DeductibleId == null)
                    deducibleId.Value = DBNull.Value;
                else
                    deducibleId.Value = filter.DeductibleId;
                //tipo de documento
                if (string.IsNullOrWhiteSpace(filter.CodTypeDoc))
                    codDoc.Value = DBNull.Value;
                else
                    codDoc.Value = filter.CodTypeDoc;
                //parametros de busqueda
                if (string.IsNullOrWhiteSpace(filter.Search))
                    search.Value = DBNull.Value;
                else
                    search.Value = filter.Search;
                // listado de parametros
                var _params = new[]{
                    new SqlParameter{ParameterName="@ruc", IsNullable = true,SqlValue= filter.Ruc},
                    new SqlParameter{ParameterName="@dateStart", IsNullable = true, SqlValue= filter.DateStart },
                    new SqlParameter{ParameterName="@dateEnd",IsNullable = true, SqlValue = filter.DateEnd },
                    deducibleId,
                    codDoc,
                    search,
                    new SqlParameter("@pageNumber", filter.PageNumber),
                    new SqlParameter("@pageSize", filter.PageSize)
                };
                var _sqlDoc = "EXEC DocumentsReceivedPaged @ruc, @dateStart, @dateEnd, @idDeducible, @codDoc, @search, @pageNumber, @pageSize";
                var documents = EcuafactAppDB.Database.SqlQuery<ApiV3_DocumentosData>(_sqlDoc, _params).ToList();
                if (documents.Count > 0)
                {
                    var _deducibleId = new SqlParameter { ParameterName = "@idDeducible", IsNullable = true, SqlDbType = SqlDbType.Int };
                    var _codDoc = new SqlParameter { ParameterName = "@codDoc", IsNullable = true, SqlDbType = SqlDbType.VarChar };
                    var _search = new SqlParameter { ParameterName = "@search", IsNullable = true, SqlDbType = SqlDbType.VarChar };
                    //deducibles
                    if (filter.DeductibleId == null)
                        _deducibleId.Value = DBNull.Value;
                    else
                        _deducibleId.Value = filter.DeductibleId;
                    //tipo de documento
                    if (string.IsNullOrWhiteSpace(filter.CodTypeDoc))
                        _codDoc.Value = DBNull.Value;
                    else
                        _codDoc.Value = filter.CodTypeDoc;
                    //parametros de busqueda
                    if (string.IsNullOrWhiteSpace(filter.Search))
                        _search.Value = DBNull.Value;
                    else
                        _search.Value = filter.Search;
                    // listado de parametros
                    var _params2 = new[] {
                        new SqlParameter("@numberElementBypage", filter.PageSize),
                        new SqlParameter("@ruc", filter.Ruc),
                        new SqlParameter("@dateStart", filter.DateStart),
                        new SqlParameter("@dateEnd", filter.DateEnd),
                        _search,
                        _deducibleId,
                        _codDoc                        
                    };
                    var _sqlTotalDoc = "exec DocumentsReceivedNumberPages @numberElementBypage, @ruc, @dateStart, @dateEnd, @search, @idDeducible, @codDoc";
                    var totalDoc = EcuafactAppDB.Database.SqlQuery<ElementByPage>(_sqlTotalDoc, _params2).FirstOrDefault();
                    var _paginationList = new PaginationList<ApiV3_DocumentosData>
                    {
                        Data = documents,
                        PageNumber = filter.PageNumber,
                        PageSize = filter.PageSize,
                        TotalPages = totalDoc.TotalPages,
                        TotalCount = totalDoc.TotalRows
                    };

                    return new OperationResult<PaginationList<ApiV3_DocumentosData>>(true, HttpStatusCode.OK, _paginationList);
                }
                #endregion Consulta documentos

                throw new Exception("No se encontraron registros, con los filtros especificados");
            }
            catch (Exception ex)
            {
                return new OperationResult<PaginationList<ApiV3_DocumentosData>>(ex);
            }
        }

        public OperationResult<List<ApiV3_DocumentosData>> GetDocumentosReceived(FilterDocumentReceived filter)
        {
            try
            {
                #region Consulta documentos
                var deducibleId = new SqlParameter { ParameterName = "@idDeducible", IsNullable = true, SqlDbType = SqlDbType.Int };
                var codDoc = new SqlParameter { ParameterName = "@codDoc", IsNullable = true, SqlDbType = SqlDbType.VarChar };
                var search = new SqlParameter { ParameterName = "@search", IsNullable = true, SqlDbType = SqlDbType.VarChar };
                //deducibles
                if (filter.DeductibleId == null)
                    deducibleId.Value = DBNull.Value;
                else
                    deducibleId.Value = filter.DeductibleId;
                //tipo de documento
                if (string.IsNullOrWhiteSpace(filter.CodTypeDoc))
                    codDoc.Value = DBNull.Value;
                else
                    codDoc.Value = filter.CodTypeDoc;
                //parametros de busqueda
                if (string.IsNullOrWhiteSpace(filter.Search))
                    search.Value = DBNull.Value;
                else
                    search.Value = filter.Search;
                // listado de parametros
                var _params = new[]{
                    new SqlParameter{ParameterName="@ruc", IsNullable = true,SqlValue= filter.Ruc},
                    new SqlParameter{ParameterName="@dateStart", IsNullable = true, SqlValue= filter.DateStart },
                    new SqlParameter{ParameterName="@dateEnd",IsNullable = true, SqlValue = filter.DateEnd },
                    deducibleId,
                    codDoc,
                    search
                };
                var _sqlDoc = "EXEC DocumentsReceived @ruc, @dateStart, @dateEnd, @idDeducible, @codDoc, @search";
                var documents = EcuafactAppDB.Database.SqlQuery<ApiV3_DocumentosData>(_sqlDoc, _params).ToList();
                if (documents.Count > 0)
                {
                    return new OperationResult<List<ApiV3_DocumentosData>>(true, HttpStatusCode.OK, documents);
                }
                #endregion Consulta documentos

                throw new Exception("No se encontraron registros, con los filtros especificados");
            }
            catch (Exception ex)
            {
                return new OperationResult<List<ApiV3_DocumentosData>>(ex);
            }
        }

        public OperationResult<ApiV3_DocumentosData> GetDocumentosReceivedById(long id)
        {
            try
            {
                var sql =  $"select *from ApiV3_DocumentosData WITH (NOLOCK) where pk=@pk";
                var pkParam = new SqlParameter("@pk", id);
                var document = _vDocumentosDataRepository.ExecSearchesWithStoreProcedure(sql, pkParam).FirstOrDefault();
                return new OperationResult<ApiV3_DocumentosData>(true, HttpStatusCode.OK, document); 
            }
            catch (Exception ex)
            {
                return new OperationResult<ApiV3_DocumentosData>(ex);
            }
        }

        public OperationResult<SupplierDocument> AddSaleNote(SupplierDocument document)
        {
            try
            {

                AddDocument(document);
                return new OperationResult<SupplierDocument>(true, HttpStatusCode.OK)
                {
                    Entity = document,
                    DevMessage = $"Se guardo la nota de venta con #{document.DocumentId}",
                    UserMessage = $"Se guardo la nota de venta #:{document.DocumentId}"
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
                return new OperationResult<SupplierDocument>(false, HttpStatusCode.InternalServerError)
                {
                    DevMessage = msg,
                    UserMessage = "Ocurrio un error al grabar la nota de venta"
                };
            }
            catch (Exception ex)
            {
                return new OperationResult<SupplierDocument>(false, HttpStatusCode.InternalServerError)
                {
                    DevMessage = $"{ex.Message}-{ex.InnerException?.Message}-{ex.InnerException?.InnerException?.Message}",
                    UserMessage = "Ocurrio un error al grabar la nota de venta"
                };
            }
        }

        public void AddDocument(SupplierDocument document)
        {
            using (DbContextTransaction transaction = EcuafactAppDB.Database.BeginTransaction())
            {
                try
                {
                    EcuafactAppDB.Set<SupplierDocument>().Add(document);
                    EcuafactAppDB.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
        public OperationResult<SupplierDocument> GetDocumentReceivedDetailById(long documentId, string claveAcceso)
        {
            try
            {
                var _doc = !string.IsNullOrWhiteSpace(claveAcceso) ?
                             _supplierDocument.FindBy(doc => doc.AccessKey == claveAcceso)
                                              .LoadDocumentReceivedReferences()
                                              .FirstOrDefault() :
                             _supplierDocument.FindBy(doc => doc.DocumentPk == documentId)
                                              .LoadDocumentReceivedReferences()
                                               .FirstOrDefault();

                if (_doc != null)
                {
                    return new OperationResult<SupplierDocument>(true, HttpStatusCode.OK) { Entity = _doc };
                }

                throw new Exception("Documento especificado no existe");
            }
            catch (Exception ex)
            {
                return new OperationResult<SupplierDocument>(false, HttpStatusCode.InternalServerError)
                {
                    DevMessage = $"{ex.Message}-{ex.InnerException?.Message}-{ex.InnerException?.InnerException?.Message}",
                    UserMessage = "Ocurrio error al listar el documento."
                };
            }
             
        }

        public OperationResult CancelDocument(long id)
        {
            try
            {
                var _pkDocument = new SqlParameter("@pkDocumento", id);
                var _motivo = new SqlParameter("@motivo", $"Documento anulado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                var idOutParam = new SqlParameter
                {
                    ParameterName = "@result",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };

                _sustenanceTypeRepository.Database.ExecuteSqlCommand("EXEC Pr_documento_anular @pkDocumento, @motivo, @result out",
                    _pkDocument,
                    _motivo,
                    idOutParam);

                var result = Convert.ToBoolean(idOutParam.Value);
                if (result)
                {
                    return new OperationResult(true, HttpStatusCode.OK){                     
                        DevMessage = $"Se anulado el documento con exíto #:{id}",
                        UserMessage = $"Se anulado el documento con exíto #:{id}"
                    };
                }

                return new OperationResult(false, HttpStatusCode.InternalServerError)
                {
                    DevMessage = $"Error al anular el documento :{id}",
                    UserMessage = $"Error al anular el documento :{id}"
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
                return new OperationResult(false, HttpStatusCode.InternalServerError)
                {
                    DevMessage = msg,
                    UserMessage = "Ocurrio un error al anular el documento"
                };
            }
            catch (Exception ex)
            {
                return new OperationResult(false, HttpStatusCode.InternalServerError)
                {
                    DevMessage = $"{ex.Message}-{ex.InnerException?.Message}-{ex.InnerException?.InnerException?.Message}",
                    UserMessage = "Ocurrio un error al anular el documento"
                };
            }
        }

    }
}
