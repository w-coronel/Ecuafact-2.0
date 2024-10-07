using Ecuafact.WebAPI.Dal.Core;
using Ecuafact.WebAPI.Dal.Repository;
using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Repository;
using Ecuafact.WebAPI.Domain.Services;
using Ecuafact.WebAPI.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Net;

namespace Ecuafact.WebAPI.Dal.Services
{
    public class  AtsService: IAtsService
    {
        private readonly IEntityRepository<VwAtsPurchasesSettlement> _vwAtsPurchasesSettlementRepository;        
        private readonly IEntityRepository<VwAtsSale> _vwAtsSaleRepository;
        private readonly IEntityRepository<VwAtsSettlement> _vwAtsSettlementRepository;
        private readonly IEntityRepository<VwAtsVoidedDocument> _vwAtsVoidedDocumentRepository;
        private readonly IEntityRepository<VwAtsPurchasesRetetion> _vwAtsPurchasesRetetionRepository;
        private readonly IEntityRepository<VwAtsCompra> _vwAtsCompraRepository;
        private readonly IEntityRepository<VwAtsFactura> _vwAtsFacturasRepository;
        private readonly IEntityRepository<VwAtsRetencionVenta> _vwAtsRetencionVentaRepository;
        private EcuafactAppContext DB = new EcuafactAppContext();
        public AtsService(
            IEntityRepository<VwAtsSale> vwAtsSaleRepository,
            IEntityRepository<VwAtsSettlement> vwAtsSettlementRepository,            
            IEntityRepository<VwAtsVoidedDocument> vwAtsVoidedDocumentRepository,
            IEntityRepository<VwAtsPurchasesRetetion> vwAtsPurchasesRetetionRepository,
            IEntityRepository<VwAtsPurchasesSettlement> vwAtsPurchasesSettlementRepository
            )
        {

            vwAtsSaleRepository.Timeout = 300; // Seconds
            vwAtsVoidedDocumentRepository.Timeout = 300; // Seconds
            vwAtsPurchasesRetetionRepository.Timeout = 300; // Seconds
            vwAtsPurchasesSettlementRepository.Timeout = 300; // Seconds
            vwAtsSettlementRepository.Timeout = 300; // Seconds
            _vwAtsSaleRepository = vwAtsSaleRepository;
            _vwAtsVoidedDocumentRepository = vwAtsVoidedDocumentRepository;
            _vwAtsPurchasesRetetionRepository = vwAtsPurchasesRetetionRepository;
            _vwAtsPurchasesSettlementRepository = vwAtsPurchasesSettlementRepository;
            _vwAtsSettlementRepository = vwAtsSettlementRepository;
            _vwAtsCompraRepository = new EntityRepository<VwAtsCompra>(DB);
            _vwAtsRetencionVentaRepository = new EntityRepository<VwAtsRetencionVenta>(DB);
            _vwAtsFacturasRepository = new EntityRepository<VwAtsFactura>(DB); ;
        }

        public OperationResult<AtsReporte> GetReportAts(string ruc, PeriodTypeEnum periodType, int year, int month, int semester)
        {
            if (periodType == PeriodTypeEnum.Biyearly)
            {
                return GetReporBiyearlytAts(ruc, year, semester);
            }

            return GetReporMonthlytAts(ruc, year, month);
        }

        public OperationResult<AtsReporte> GetReporMonthlytAts(string ruc, int year, int month)
        {

            try
            {
                var report = new AtsReporte();

                #region consultar compras                
                var sqlDocumentosCompras = $"select *from vwAtsCompras WITH (NOLOCK) where (IdInformante = @ruc or IdInformante = @ruc2) and anio = @anio and mes = @mes";
                var sqlPurchasesSettlement = $"select *from vwAtsPurchasesSettlement WITH (NOLOCK) where IdInformante = @ruc and anio = @anio and mes = @mes";
                var sqlComprasRetenciones = $"select *from vwAtsPurchasesRetetions WITH (NOLOCK) where IdInformante = @ruc and anio = @anio and mes = @mes";                
                var paramCompras = new List<SqlParameter> {
                    new SqlParameter("@ruc", ruc),
                    new SqlParameter("@ruc2", ruc.Substring(0, 10)),
                    new SqlParameter("@anio", year),
                    new SqlParameter("@mes", month)
                };
                var _compras = _vwAtsCompraRepository.ExecSearchesWithStoreProcedure(sqlDocumentosCompras, paramCompras.ToArray()).ToList();
                var paramCompras2 = new List<SqlParameter> {
                    new SqlParameter("@ruc", ruc),                   
                    new SqlParameter("@anio", year),
                    new SqlParameter("@mes", month)
                };               
                var _purchasesSettlement = _vwAtsPurchasesSettlementRepository.ExecSearchesWithStoreProcedure(sqlPurchasesSettlement, paramCompras2.ToArray()).ToList();
                if(_compras.Count > 0)
                {
                    if (_purchasesSettlement.Count > 0)
                    {
                        _purchasesSettlement.ForEach(sett => { _compras.Add(sett);});
                    }
                }
                else
                {                    
                    if (_purchasesSettlement.Count > 0)
                    {
                        _compras = new List<VwAtsCompra>();
                        _purchasesSettlement.ForEach(sett => { _compras.Add(sett);});
                    }
                }
                if (_compras.Count > 0)
                {
                    report.AtsCompraReporte = _compras;
                    var _paramPurchases = new[] {
                         new SqlParameter("@ruc", ruc),
                         new SqlParameter("@anio", year),
                         new SqlParameter("@mes", month)
                    };
                    var purchases = _vwAtsPurchasesRetetionRepository.ExecSearchesWithStoreProcedure(sqlComprasRetenciones, _paramPurchases).ToList();
                    if(purchases.Count > 0)
                    {
                        report.AtsComprasRetncionReporte = purchases;
                    }

                }
                #endregion consultar compras

                #region consultar ventas
                var sqlDocumentoVentas = $"select *from vwAtsSales WITH (NOLOCK) where IdInformante = @ruc and anio = @anio and mes = @mes";
                var sqlVentaRetenciones = $"select *from vwAtsRetencionVentas WITH (NOLOCK) where (IdInformante = @ruc or IdInformante = @ruc2) and anio = @anio and mes = @mes";
                var paramVentas = new List<SqlParameter> {
                    new SqlParameter("@ruc", ruc),                   
                    new SqlParameter("@anio", year),
                    new SqlParameter("@mes", month)
                };

                var _ventas = _vwAtsSaleRepository.ExecSearchesWithStoreProcedure(sqlDocumentoVentas, paramVentas.ToArray()).ToList();
                if (_ventas.Count > 0)
                {
                    report.AtsVentasReporte = _ventas;
                    var _paramSales = new[] {
                         new SqlParameter("@ruc", ruc),
                         new SqlParameter("@ruc2", ruc.Substring(0, 10)),
                         new SqlParameter("@anio", year),
                         new SqlParameter("@mes", month)
                    };
                    var salesRetention = _vwAtsRetencionVentaRepository.ExecSearchesWithStoreProcedure(sqlVentaRetenciones, _paramSales).ToList();
                    if (salesRetention.Count > 0)
                    {
                        report.AtsRetencionVentaReport = salesRetention;
                    }

                }
                #endregion consultar ventas

                #region consultar Anulados
                var sqlAnulados = $"select *from vwAtsVoidedDocuments WITH (NOLOCK) where IdInformante = @ruc and anio = @anio and mes = @mes";                
                var paramAnulado = new List<SqlParameter> {
                    new SqlParameter("@ruc", ruc),
                    new SqlParameter("@anio", year),
                    new SqlParameter("@mes", month)
                };

                var _anulados = _vwAtsVoidedDocumentRepository.ExecSearchesWithStoreProcedure(sqlAnulados, paramAnulado.ToArray()).ToList();
                if (_anulados.Count > 0)
                {
                    report.AtsAnuladosReporte = _anulados;                    

                }
                #endregion consultar ventas

                return new OperationResult<AtsReporte>(true, HttpStatusCode.OK)
                {
                    Entity = report
                };

            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                var msg = "";
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }
                return new OperationResult<AtsReporte>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error al listar reporte ats" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<AtsReporte>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error al listar el reporte ats" };
            }
            catch (Exception e)
            {
                return new OperationResult<AtsReporte>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error al listar el punto de emisisión" };
            }
        }

        public OperationResult<AtsReporte> GetReporBiyearlytAts(string ruc, int year, int semester)
        {

            try
            {
                var report = new AtsReporte();
                var startMonth = 1;
                var endMonth = 6;
                #region consultar compras
                var sqlDocumentosCompras = $"select *from vwAtsCompras WITH (NOLOCK) where (IdInformante = @ruc or IdInformante = @ruc2) and anio = @anio and mes between @mes1 and @mes2 order by mes";
                var sqlComprasRetenciones = $"select *from vwAtsPurchasesRetetions WITH (NOLOCK) where IdInformante = @ruc  and anio = @anio and mes between @mes1 and @mes2 order by mes";
                if (semester == 2)
                {
                    startMonth = 7;
                    endMonth = 12;
                }
                var paramCompras = new List<SqlParameter> {
                    new SqlParameter("@ruc", ruc),
                    new SqlParameter("@ruc2", ruc.Substring(0, 10)),
                    new SqlParameter("@anio", year),
                    new SqlParameter("@mes1", startMonth),
                    new SqlParameter("@mes2", endMonth)
                };
                var _compras = _vwAtsCompraRepository.ExecSearchesWithStoreProcedure(sqlDocumentosCompras, paramCompras.ToArray()).ToList();
                if (_compras.Count > 0)
                {
                    report.AtsCompraReporte = _compras;
                    var _paramPurchases = new[] {
                         new SqlParameter("@ruc", ruc),
                         new SqlParameter("@anio", year),
                         new SqlParameter("@mes1", startMonth),
                         new SqlParameter("@mes2", endMonth)
                    };
                    var purchases = _vwAtsPurchasesRetetionRepository.ExecSearchesWithStoreProcedure(sqlComprasRetenciones, _paramPurchases).ToList();
                    if (purchases.Count > 0)
                    {
                        report.AtsComprasRetncionReporte = purchases;
                    }

                }
                #endregion consultar compras

                #region consultar ventas
                var sqlDocumentoVentas = $"select *from vwAtsSales WITH (NOLOCK) where IdInformante = @ruc and anio = @anio and mes between @mes1 and @mes2 order by mes";
                var sqlVentaRetenciones = $"select *from vwAtsRetencionVentas WITH (NOLOCK) where (IdInformante = @ruc or IdInformante = @ruc2) and anio = @anio and mes between @mes1 and @mes2 order by mes";
                var paramVentas = new List<SqlParameter> {
                    new SqlParameter("@ruc", ruc),
                    new SqlParameter("@anio", year),
                    new SqlParameter("@mes1", startMonth),
                    new SqlParameter("@mes2", endMonth)
                };

                var _ventas = _vwAtsSaleRepository.ExecSearchesWithStoreProcedure(sqlDocumentoVentas, paramVentas.ToArray()).ToList();
                if (_ventas.Count > 0)
                {
                    report.AtsVentasReporte = _ventas;
                    var _paramSales = new[] {
                         new SqlParameter("@ruc", ruc),
                         new SqlParameter("@ruc2", ruc.Substring(0, 10)),
                         new SqlParameter("@anio", year),
                         new SqlParameter("@mes1", startMonth),
                         new SqlParameter("@mes2", endMonth)
                    };
                    var salesRetention = _vwAtsRetencionVentaRepository.ExecSearchesWithStoreProcedure(sqlVentaRetenciones, _paramSales).ToList();
                    if (salesRetention.Count > 0)
                    {
                        report.AtsRetencionVentaReport = salesRetention;
                    }

                }
                #endregion consultar ventas

                #region consultar Anulados
                var sqlAnulados = $"select *from vwAtsVoidedDocuments WITH (NOLOCK) where IdInformante = @ruc and anio = @anio and mes between @mes1 and @mes2 order by mes";
                var paramAnulado = new List<SqlParameter> {
                    new SqlParameter("@ruc", ruc),
                    new SqlParameter("@anio", year),
                    new SqlParameter("@mes1", startMonth),
                    new SqlParameter("@mes2", endMonth)
                };

                var _anulados = _vwAtsVoidedDocumentRepository.ExecSearchesWithStoreProcedure(sqlAnulados, paramAnulado.ToArray()).ToList();
                if (_anulados.Count > 0)
                {
                    report.AtsAnuladosReporte = _anulados;

                }
                #endregion consultar ventas

                return new OperationResult<AtsReporte>(true, HttpStatusCode.OK)
                {
                    Entity = report
                };

            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                var msg = "";
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }
                return new OperationResult<AtsReporte>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error al listar el reporte ats" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<AtsReporte>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error al listar el reporte ats" };
            }
            catch (Exception e)
            {
                return new OperationResult<AtsReporte>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error al listar el reporte ats" };
            }
        }

        public OperationResult<List<VwAtsFactura>> GetReportInvoiceAts(string ruc, PeriodTypeEnum periodType, int year, int month, int semester)
        {
            try
            {
                var _facturas = new List<VwAtsFactura>();
                var paramFactCompras = new List<SqlParameter>();
                var paramLiquCompras = new List<SqlParameter>();
                var startMonth = 1;
                var endMonth = 6;

                #region consultar compras

                var sqlFacturasCompras = $"select *from vwAtsFacturas WITH (NOLOCK) where (Ruc = @ruc or Ruc = @ruc2) and anio = @anio and mes = @mes";
                var sqlLiquidacionCompras = $"select *from VwAtsSettlement WITH (NOLOCK) where Ruc = @ruc and anio = @anio and mes = @mes";

                //parametros para consultar las facturas y nota de ventas recibidas
                paramFactCompras = new List<SqlParameter> {
                    new SqlParameter("@ruc", ruc),
                    new SqlParameter("@ruc2", ruc.Substring(0, 10)),
                    new SqlParameter("@anio", year),
                    new SqlParameter("@mes", month)                  
                };

                //parametros para consultar las liquidaciones emitidas
                paramLiquCompras = new List<SqlParameter> {
                    new SqlParameter("@ruc", ruc),                   
                    new SqlParameter("@anio", year),
                    new SqlParameter("@mes", month)
                };

                if (periodType == PeriodTypeEnum.Biyearly)
                {
                    sqlFacturasCompras = $"select *from vwAtsFacturas WITH (NOLOCK) where (Ruc = @ruc or Ruc = @ruc2) and anio = @anio and mes between @mes1 and @mes2 order by mes";
                    sqlLiquidacionCompras = $"select *from VwAtsSettlement WITH (NOLOCK) where Ruc = @ruc and anio = @anio and mes between @mes1 and @mes2 order by mes";
                    if (semester == 2)
                    {
                        startMonth = 7;
                        endMonth = 12;
                    }

                    paramFactCompras = new List<SqlParameter> {
                        new SqlParameter("@ruc", ruc),
                        new SqlParameter("@ruc2", ruc.Substring(0, 10)),
                        new SqlParameter("@anio", year),
                        new SqlParameter("@mes1", startMonth),
                        new SqlParameter("@mes2", endMonth)
                     };

                    paramLiquCompras = new List<SqlParameter> {
                        new SqlParameter("@ruc", ruc),
                        new SqlParameter("@anio", year),
                        new SqlParameter("@mes1", startMonth),
                        new SqlParameter("@mes2", endMonth)
                    };
                }

                _facturas = _vwAtsFacturasRepository.ExecSearchesWithStoreProcedure(sqlFacturasCompras, paramFactCompras.ToArray()).ToList();
                var _liquidacion = _vwAtsSettlementRepository.ExecSearchesWithStoreProcedure(sqlLiquidacionCompras, paramLiquCompras.ToArray()).ToList();
                if(_facturas?.Count > 0)
                {
                    if(_liquidacion?.Count > 0)
                        _liquidacion.ForEach(liq => { _facturas.Add(liq); });
                }
                else
                {
                    if (_liquidacion?.Count > 0)
                        _liquidacion.ForEach(liq => { _facturas.Add(liq); });
                }
                #endregion consultar compras

                return new OperationResult<List<VwAtsFactura>>(true, HttpStatusCode.OK)
                {
                    Entity = _facturas
                };

            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                var msg = "";
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }
                return new OperationResult<List<VwAtsFactura>>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error al listar las facturas" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<List<VwAtsFactura>>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error al listar facturas" };
            }
            catch (Exception e)
            {
                return new OperationResult<List<VwAtsFactura>>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error al listar las facturas" };
            }
        }

        public OperationResult<VwAtsFactura> GetPurchasesAtsById(int id, int typeIssuance)
        {
            try
            {

                #region consultar compras               
                var sqlFacturasCompras = $"select *from vwAtsFacturas WITH (NOLOCK) where Id = @id";
                //parametros para consultar las facturas y nota de ventas recibidas
                var paramCompras = new List<SqlParameter> {
                    new SqlParameter("@id", id)
                };

                if (typeIssuance > 1)
                {
                    sqlFacturasCompras = $"select *from VwAtsSettlement WITH (NOLOCK) where where Id = @id";
                    var _settlement = _vwAtsSettlementRepository.ExecSearchesWithStoreProcedure(sqlFacturasCompras, paramCompras.ToArray()).FirstOrDefault();
                    return new OperationResult<VwAtsFactura>(true, HttpStatusCode.OK)
                    {
                        Entity = _settlement
                    };
                }
                
                var _factura = _vwAtsFacturasRepository.ExecSearchesWithStoreProcedure(sqlFacturasCompras, paramCompras.ToArray()).FirstOrDefault();
                return new OperationResult<VwAtsFactura>(true, HttpStatusCode.OK)
                {
                    Entity = _factura
                };

                #endregion consultar compras

            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                var msg = "";
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }
                return new OperationResult<VwAtsFactura>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error al listar el documento" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<VwAtsFactura>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error al listar el documento" };
            }
            catch (Exception e)
            {
                return new OperationResult<VwAtsFactura>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error al listar el documento" };
            }
        }

        public  OperationResult<StatisticsAts> GetStatisticsAts(string ruc, PeriodTypeEnum periodType, int year, int month, int semester)
        {
            try
            {

                // listado de parametros
                var _params = new[]{
                    new SqlParameter("@ruc", ruc.Substring(0, 10)),
                     new SqlParameter("@year",year),
                    new SqlParameter("@month", month),                                   
                    new SqlParameter("@period", (int)periodType),
                    new SqlParameter("@semester", semester)
                };

                var _sqlDoc = "EXEC Ats_CountDocuments @ruc, @year, @month, @period, @semester";
                var _result = DB.Database.SqlQuery<StatisticsAts>(_sqlDoc, _params).FirstOrDefault();
                return  new OperationResult<StatisticsAts>(true, HttpStatusCode.OK)
                {
                    Entity = _result
                }; 

            }
            catch (DbEntityValidationException ex) // Errores de validacion al guardar
            {
                var msg = "";
                if (ex.EntityValidationErrors.Count() > 0)
                {
                    var er = ex.EntityValidationErrors?.FirstOrDefault()?.ValidationErrors?.FirstOrDefault();
                    msg = $"{er.PropertyName}: {er.ErrorMessage}";
                }
                else
                {
                    msg = $"{ex.Message} - {ex.InnerException?.InnerException?.Message}";
                }
                return new OperationResult<StatisticsAts>(false, HttpStatusCode.BadRequest) { DevMessage = msg, UserMessage = "Ocurrio un error al listar la cantidad de recibidos" };

            }
            catch (DbUpdateException dbex)
            {
                return new OperationResult<StatisticsAts>(false, HttpStatusCode.BadRequest) { DevMessage = $"{dbex.Message} - {dbex.InnerException?.InnerException?.Message}", UserMessage = "Ocurrio un error al listar la cantidad de recibidos" };
            }
            catch (Exception e)
            {
                return new OperationResult<StatisticsAts>(false, HttpStatusCode.InternalServerError) { DevMessage = e.ToString(), UserMessage = "Ocurrio un error al listar la cantidad de recibidos" };
            }
        }
    }
}
