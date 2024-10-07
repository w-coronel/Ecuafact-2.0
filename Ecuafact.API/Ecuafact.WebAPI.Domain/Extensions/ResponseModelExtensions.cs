using Ecuafact.WebAPI.Domain.Entities;
using Ecuafact.WebAPI.Domain.Entities.Ats;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ecuafact.WebAPI.Domain.Extensions
{
    internal static class ResponseModelExtensions
    {        
        internal static List<AtsCompras> ToAtsCompras(this AtsReporte atsModel)
        {
            var _compras = new List<AtsCompras>();
            if (atsModel != null)
            {
                if (atsModel.AtsCompraReporte.Count > 0)
                {
                    atsModel.AtsCompraReporte.ForEach(comp =>
                    {
                        var model = new AtsCompras
                        {
                            TipoIDInformante = "R",
                            IdInformante = comp.IdInformante,
                            RazonSocial = comp.RazonSocial,
                            Anio = comp.Anio,
                            Mes = comp.Mes,
                            NumEstabRuc = comp.NumEstabRuc,
                            TotalVentas = 0,
                            CodigoOperativo = "IVA",
                            CodSustento = !string.IsNullOrWhiteSpace(comp.CodSustento) ? comp.CodSustento : "",
                            TpIdProv = string.IsNullOrWhiteSpace(comp.TpIdProv) ? "01" : comp.TpIdProv,
                            IdProv = comp.IdProv,
                            ParteRel = "NO",
                            TipoComprobante = comp.TipoComprobante,
                            FechaRegistro = comp.FechaRegistro,
                            Establecimiento = comp.Establecimiento,
                            PuntoEmision = comp.PuntoEmision,
                            Secuencial = comp.Secuencial,
                            FechaEmision = comp.FechaEmision,
                            Autorizacion = comp.Autorizacion,
                            BaseNoGraIva = comp.BaseNoGraIva,
                            BaseImponible = comp.BaseImponible,
                            BaseImpGrav = comp.BaseImpGrav,
                            BaseImpExe = comp.BaseImpExe,
                            MontoIce = comp.MontoIce,
                            MontoIva = comp.MontoIva,
                            TotbasesImpReemb = (comp.BaseNoGraIva + comp.BaseImponible + comp.BaseImpGrav + comp.BaseImpExe),
                            PagoLocExt = "01",
                            NumCajBan = 0,
                            PrecCajBan = 0,
                            DocModificado = !string.IsNullOrWhiteSpace(comp.DocModificado) ? comp.DocModificado : "0",
                            EstabModificado = !string.IsNullOrWhiteSpace(comp.DocModificado) ? comp.EstabModificado : "000",
                            PtoEmiModificado = !string.IsNullOrWhiteSpace(comp.DocModificado) ? comp.PtoEmiModificado : "000",
                            SecModificado = !string.IsNullOrWhiteSpace(comp.DocModificado) ? comp.SecModificado : "0",
                            AutModificado = !string.IsNullOrWhiteSpace(comp.DocModificado) ? comp.AutModificado : "000",
                            DenoProv = comp.DenoProv,
                            FormaPago = comp.TipoComprobante != "04" ? comp.FormaPago : "",
                        };                       
                        #region retencion de las compras
                        if (atsModel.AtsComprasRetncionReporte.Count > 0) 
                        {
                            var _ret = atsModel.AtsComprasRetncionReporte
                                                 .Where(r => r.RetentionReferenceDocumentAuth == comp.Autorizacion)
                                                 .FirstOrDefault() ?? null;
                            if (_ret != null)
                            {
                                model.ValRetBien10 = _ret.ValRetBien10;
                                model.ValRetServ20 = _ret.ValRetServ20;
                                model.ValorRetBienes = _ret.ValRetServ30;
                                model.ValRetServ50 = _ret.ValRetServ50;
                                model.ValorRetServicios = _ret.ValRetServ70;
                                model.ValRetServ100 = _ret.ValRetServ100;
                                model.TotbasesImpReemb = _ret.TotbasesImpReemb;
                                model.CodRetAir = _ret.CodRetAir;
                                model.BaseImpAir = _ret.BaseImpAir;
                                model.PorcentajeAir = _ret.PorcentajeAir;
                                model.ValRetAir = _ret.ValRetAir;
                                model.EstabRetencion1 = _ret.EstabRetencion1;
                                model.PtoEmiRetencion1 = _ret.PtoEmiRetencion1;
                                model.SecRetencion1 = _ret.SecRetencion1;
                                model.AutRetencion1 = _ret.AutRetencion1;
                                model.FechaEmiRet1 = $"{_ret.FechaEmiRet1:dd/MM/yyyy}";
                            }
                            else
                            {
                                if (comp.TipoComprobante != "04" && comp.TipoComprobante != "05")
                                {
                                    decimal _base = comp.BaseNoGraIva + comp.BaseImponible + comp.BaseImpGrav + comp.BaseImpExe;
                                    model.BaseImpAir = _base;
                                    model.PorcentajeAir = 0;
                                    model.ValRetAir = 0.00M;
                                    model.CodRetAir = "332";
                                }
                            }
                               
                        }
                        else
                        {
                            decimal _base = comp.BaseNoGraIva + comp.BaseImponible + comp.BaseImpGrav + comp.BaseImpExe;
                            model.BaseImpAir = _base;
                            model.PorcentajeAir = 0;
                            model.ValRetAir = 0.00M;
                        }
                        #endregion retencion de las compras

                        _compras.Add(model);
                    });
                }
            }
            return _compras;
        }
        internal static List<AtsVentas> ToAtsVentass(this AtsReporte atsModel)
        {
            var _ventas = new List<AtsVentas>();
            if (atsModel != null)
            {
                atsModel.AtsVentasReporte.ForEach(vent =>
                {
                    var model = new AtsVentas
                    {
                        TipoIDInformante = "R",
                        IdInformante = vent.IdInformante,
                        RazonSocial = vent.RazonSocial,
                        Anio = vent.Anio.Value,
                        Mes = vent.Mes.Value,
                        NumEstabRuc = vent.NumEstabRuc,
                        TotalVentas = 0,
                        CodigoOperativo = "IVA",
                        TpIdCliente = vent.TpIdCliente,
                        IdCliente = vent.IdCliente,
                        TipoComprobante = vent.TipoComprobante,
                        TipoEmision = "E",
                        NumeroComprobantes = vent.NumeroComprobantes,
                        BaseNoGraIva = vent.BaseNoGraIva.Value,
                        BaseImponible = vent.BaseImponible.Value,
                        BaseImpGrav = vent.BaseImpGrav.Value,
                        MontoIva = vent.MontoIva.Value,
                        MontoIce = 0
                    };

                    if(vent.TipoComprobante == "01")
                    {
                        model.TipoComprobante = vent.TipoComprobante;
                    }

                    #region retencion de las ventas
                    if (atsModel.AtsRetencionVentaReport.Count > 0)
                    {
                        var _retVenta = atsModel.AtsRetencionVentaReport
                                         .Where(ret => ret.IdCliente == vent.IdCliente && ret.DocumentoCodigoReferencia == vent.TipoComprobante)
                                         .FirstOrDefault();
                        if (_retVenta != null)
                        {
                            model.ValorRetIva = _retVenta.ValorRetIva.Value;
                            model.ValorRetRenta = _retVenta.ValorRetRenta.Value;
                        }
                    }
                    #endregion retencion de las ventas

                    if (model.TipoComprobante == "01")
                    {
                        model.TipoComprobante = "18";
                    }

                    _ventas.Add(model);
                });
            }

            return _ventas;
        }
        internal static List<AtsAnulados> ToAtsAnulados(this AtsReporte atsModel)
        {
            var _anulados = new List<AtsAnulados>();
            atsModel.AtsAnuladosReporte.ForEach(anu => {
                var model = new AtsAnulados
                {
                    TipoIDInformante = "R",
                    IdInformante = anu.IdInformante,
                    RazonSocial = anu.RazonSocial,
                    Anio = anu.Anio.Value,
                    Mes = anu.Mes.Value,
                    NumEstabRuc = anu.EstablishmentCode,
                    TotalVentas = 0,
                    CodigoOperativo = "IVA",
                    TipoComprobante = anu.TipoComprobante == "01" ? "18": anu.TipoComprobante,
                    Establecimiento = anu.EstablishmentCode,
                    PuntoEmision = anu.PuntoEmision,
                    SecuencialInicio = anu.SecuencialInicio,
                    SecuencialFin = anu.SecuencialFin,
                    Autorizacion = anu.Autorizacion,

                };

                _anulados.Add(model);
            });

            return _anulados;
        }
        internal static List<AtsVentasEstablecimiento> ToAtsVentasEstablecimiento(this AtsReporte atsModel)
        {
            
           var _ventEst = atsModel.AtsVentasReporte
                                 .GroupBy(g => new{
                                     g.IdInformante,
                                     g.RazonSocial,
                                     g.Anio,
                                     g.Mes,
                                     g.NumEstabRuc})
                                 .Select(dat => new AtsVentasEstablecimiento{
                                     TipoIDInformante = "R",
                                     IdInformante = dat.Key.IdInformante,
                                     RazonSocial = dat.Key.RazonSocial,
                                     Anio = dat.Key.Anio.Value,
                                     Mes = dat.Key.Mes.Value,
                                     NumEstabRuc = dat.Key.NumEstabRuc,
                                     TotalVentas = 0,
                                     CodigoOperativo ="IVA",
                                     CodEstab = dat.Key.NumEstabRuc,
                                     VentasEstab = 0                                 
                                 }).ToList();    

            return _ventEst;
        }
        internal static ATS ToXmlAts(this AtsReporte atsModel)
        {
            var _ats = new ATS();
            if (atsModel != null)
            {
                #region Cmpras 
                if (atsModel.AtsCompraReporte.Count > 0)
                {
                    _ats.TipoIDInformante = "R";
                    _ats.IdInformante = atsModel.AtsCompraReporte.FirstOrDefault().IdInformante;
                    _ats.razonSocial = atsModel.AtsCompraReporte.FirstOrDefault().RazonSocial.Replace('-',' ').Trim();
                    _ats.Anio = atsModel.AtsCompraReporte.FirstOrDefault().Anio;
                    _ats.Mes = atsModel.AtsCompraReporte.FirstOrDefault().Mes.ToString("D2");
                    _ats.numEstabRuc = atsModel.AtsCompraReporte.FirstOrDefault().NumEstabRuc;
                    _ats.totalVentas = 0.00M;
                    _ats.codigoOperativo = "IVA";
                    _ats.compras = new List<ATS_Compras>();
                    atsModel.AtsCompraReporte.ForEach(comp =>
                    {
                        var _atsComp = new ATS_Compras
                        {
                            codSustento = !string.IsNullOrWhiteSpace(comp.CodSustento) ? comp.CodSustento:"0",
                            tpIdProv = string.IsNullOrWhiteSpace(comp.TpIdProv) ? "01" : comp.TpIdProv,
                            idProv = comp.IdProv,
                            tipoComprobante = comp.TipoComprobante,
                            parteRel = "NO",
                            fechaRegistro = $"{comp.FechaRegistro:dd/MM/yyyy}",
                            establecimiento = comp.Establecimiento,
                            puntoEmision = comp.PuntoEmision,
                            secuencial = comp.Secuencial,
                            fechaEmision = $"{comp.FechaEmision:dd/MM/yyyy}",
                            autorizacion = comp.Autorizacion,
                            baseNoGraIva = decimal.Round(comp.BaseNoGraIva,2),
                            baseImponible = decimal.Round(comp.BaseImponible, 2, MidpointRounding.AwayFromZero),
                            baseImpGrav = decimal.Round(comp.BaseImpGrav, 2, MidpointRounding.AwayFromZero),
                            baseImpExe = decimal.Round(comp.BaseImpExe, 2, MidpointRounding.AwayFromZero),
                            montoIce = 0.00M,
                            montoIva = decimal.Round(comp.MontoIva, 2),
                            docModificado = !string.IsNullOrWhiteSpace(comp.DocModificado) ? comp.DocModificado : "00",
                            estabModificado = !string.IsNullOrWhiteSpace(comp.DocModificado) ? comp.EstabModificado : "000",
                            ptoEmiModificado = !string.IsNullOrWhiteSpace(comp.DocModificado) ? comp.PtoEmiModificado : "000",
                            secModificado = !string.IsNullOrWhiteSpace(comp.DocModificado) ? comp.SecModificado : "0",
                            autModificado = !string.IsNullOrWhiteSpace(comp.DocModificado) ? comp.AutModificado : "000",
                            pagoExterior = new PagoExteriorType
                            {
                                pagoLocExt = "01",
                                paisEfecPago = "NA",
                                aplicConvDobTrib = "NA",
                                pagExtSujRetNorLeg = "NA"
                            }
                        };

                        #region forma pago
                        if (comp.TipoComprobante != "04")
                        {
                            _atsComp.formasDePago =  new FormasDePagoType { formaPago = comp.FormaPago };
                        }
                        #endregion forma pago
                                              
                        #region retencion de las compras
                        if (atsModel.AtsComprasRetncionReporte.Count > 0)
                        {
                            var ret = atsModel.AtsComprasRetncionReporte
                                     .Where(r => r.RetentionReferenceDocumentAuth == comp.Autorizacion)
                                     .FirstOrDefault() ?? null;

                            if (ret != null)
                            {
                                _atsComp.valRetBien10 = decimal.Round(ret.ValRetBien10, 2, MidpointRounding.AwayFromZero);
                                _atsComp.valRetServ20 = decimal.Round(ret.ValRetServ20, 2, MidpointRounding.AwayFromZero);
                                _atsComp.valorRetBienes = decimal.Round(ret.ValRetServ30, 2, MidpointRounding.AwayFromZero);
                                _atsComp.valRetServ50 = decimal.Round(ret.ValRetServ50, 2, MidpointRounding.AwayFromZero);
                                _atsComp.valorRetServicios = decimal.Round(ret.ValRetServ70, 2, MidpointRounding.AwayFromZero);
                                _atsComp.valRetServ100 = decimal.Round(ret.ValRetServ100, 2, MidpointRounding.AwayFromZero);
                                _atsComp.totbasesImpReemb = decimal.Round(ret.TotbasesImpReemb, 2, MidpointRounding.AwayFromZero);
                                _atsComp.air = new List<AirType> {
                                        new AirType {
                                            codRetAir = ret.CodRetAir,
                                            baseImpAir = decimal.Round(ret.BaseImpAir, 2, MidpointRounding.AwayFromZero),
                                            porcentajeAir = decimal.Round(ret.PorcentajeAir, 2, MidpointRounding.AwayFromZero),
                                            valRetAir = decimal.Round(ret.ValRetAir, 2, MidpointRounding.AwayFromZero)
                                        }
                                    };
                                _atsComp.estabRetencion1 = ret.EstabRetencion1;
                                _atsComp.ptoEmiRetencion1 = ret.PtoEmiRetencion1;
                                _atsComp.secRetencion1 = ret.SecRetencion1;
                                _atsComp.autRetencion1 = ret.AutRetencion1;
                                _atsComp.fechaEmiRet1 = $"{ret.FechaEmiRet1:dd/MM/yyyy}";
                            }
                            else
                            {
                                if (comp.TipoComprobante != "04" && comp.TipoComprobante != "05")
                                {
                                    decimal _base = _atsComp.baseNoGraIva + _atsComp.baseImponible + _atsComp.baseImpGrav + _atsComp.baseImpExe;
                                    _atsComp.air = new List<AirType> {
                                            new AirType {
                                                codRetAir = "332",
                                                baseImpAir = decimal.Round(_base, 2, MidpointRounding.AwayFromZero),
                                                porcentajeAir = decimal.Round(0, 2, MidpointRounding.AwayFromZero),
                                                valRetAir = decimal.Round(0, 2, MidpointRounding.AwayFromZero)
                                            }
                                    };
                                }
                            }
                        }
                        else
                        {
                            decimal _base = _atsComp.baseNoGraIva + _atsComp.baseImponible + _atsComp.baseImpGrav + _atsComp.baseImpExe;
                            _atsComp.air = new List<AirType> {
                                        new AirType {
                                            codRetAir = "332",
                                            baseImpAir = decimal.Round(_base, 2, MidpointRounding.AwayFromZero),
                                            porcentajeAir = decimal.Round(0, 2, MidpointRounding.AwayFromZero),
                                            valRetAir = decimal.Round(0, 2, MidpointRounding.AwayFromZero)
                                        }
                             };
                        }
                        #endregion retencion de las compras

                        _ats.compras.Add(_atsComp);
                    });
                }
                #endregion Compras

                #region ventas
                if(atsModel.AtsVentasReporte.Count > 0)
                {
                    _ats.ventas = new List<ATS_Ventas>();
                    atsModel.AtsVentasReporte.ForEach(vent=>
                    {
                        var _ventAts = new ATS_Ventas
                        {
                            tpIdCliente = vent.TpIdCliente,
                            idCliente = vent.IdCliente,
                            parteRelVtas = "NO",
                            tipoEmision = "E",
                            numeroComprobantes = vent.NumeroComprobantes.Value,
                            baseNoGraIva = decimal.Round(vent.BaseNoGraIva.Value, 2, MidpointRounding.AwayFromZero),
                            baseImponible = decimal.Round(vent.BaseImponible.Value, 2, MidpointRounding.AwayFromZero),
                            baseImpGrav = decimal.Round(vent.BaseImpGrav.Value, 2, MidpointRounding.AwayFromZero),
                            montoIva = decimal.Round(vent.MontoIva.Value, 2, MidpointRounding.AwayFromZero),
                            tipoComprobante = vent.TipoComprobante,
                            montoIce = 0.00M,
                            
                        };
                        #region forma pago
                        if (vent.TipoComprobante !="04")
                        {
                            _ventAts.formasDePago = new FormasDePagoType { formaPago = vent.FormaPago };
                        }
                        #endregion forma pago
                        #region retencion de las ventas
                        if (atsModel.AtsRetencionVentaReport.Count > 0)
                        {
                            var _retVenta = atsModel.AtsRetencionVentaReport
                                             .Where(ret => ret.IdCliente == vent.IdCliente && ret.DocumentoCodigoReferencia == vent.TipoComprobante)
                                             .FirstOrDefault();
                            if (_retVenta != null)
                            {
                                _ventAts.valorRetIva = decimal.Round(_retVenta.ValorRetIva.Value, 2, MidpointRounding.AwayFromZero);
                                _ventAts.valorRetRenta = decimal.Round(_retVenta.ValorRetRenta.Value, 2, MidpointRounding.AwayFromZero);
                            }
                        }
                        #endregion retencion de las ventas
                        if (vent.TipoComprobante == "01")
                        {
                            _ventAts.tipoComprobante = "18";
                        }                       
                        _ats.ventas.Add(_ventAts);
                    });
                }
                #endregion ventas

                #region anulados
                _ats.anulados = new List<ATS_Anulados>();
                atsModel.AtsAnuladosReporte.ForEach(anu =>{
                    var model = new ATS_Anulados{
                        tipoComprobante = anu.TipoComprobante == "01" ? "18" : anu.TipoComprobante,
                        establecimiento = anu.EstablishmentCode,
                        puntoEmision = anu.PuntoEmision,
                        secuencialInicio = Convert.ToInt32(anu.SecuencialInicio),
                        secuencialFin = Convert.ToInt32(anu.SecuencialFin),
                        autorizacion = anu.Autorizacion
                    };
                    _ats.anulados.Add(model);
                });
                #endregion anulados

                #region Ventas Establecimiento
                if(atsModel.AtsVentasReporte.Count > 0)
                {                    
                    var _ventEst = atsModel.AtsVentasReporte
                                 .GroupBy(g => new {
                                     g.IdInformante,
                                     g.RazonSocial,
                                     g.Anio,
                                     g.Mes,
                                     g.NumEstabRuc
                                 })
                                 .Select(dat => new AtsVentasEstablecimiento
                                 {
                                     TipoIDInformante = "R",
                                     IdInformante = dat.Key.IdInformante,
                                     RazonSocial = dat.Key.RazonSocial,
                                     Anio = dat.Key.Anio.Value,
                                     Mes = dat.Key.Mes.Value,
                                     NumEstabRuc = dat.Key.NumEstabRuc,
                                     TotalVentas = 0.00M,
                                     CodigoOperativo = "IVA",
                                     CodEstab = dat.Key.NumEstabRuc,
                                     VentasEstab = 0
                                 }).ToList();
                    if(_ventEst.Count > 0)
                    {
                        _ats.ventasEstablecimiento = new List<ATS_VentasEstablecimiento>();
                        _ventEst.ForEach(ve => {
                            var _vEst = new ATS_VentasEstablecimiento
                            {
                                codEstab = ve.CodEstab,
                                ventasEstab = 0.00M,
                                ivaComp = 0.00M
                            };
                            _ats.ventasEstablecimiento.Add(_vEst);
                        });
                    }
                    
                }
                
                #endregion Ventas Establecimiento
            }

            return _ats;
        }
              
    }
    public static class ModelExtensions
    {
        public static VwAtsCompra ToVwAtsCompra(this VwAtsPurchasesSettlement model)
        {
            return new VwAtsCompra
            {
                IdInformante = model.IdInformante,
                RazonSocial = model.RazonSocial,
                Anio = model.Anio,
                Mes = model.Mes,
                NumEstabRuc = model.NumEstabRuc,
                CodSustento = model.CodSustento,
                TpIdProv = model.TpIdProv,
                IdProv = model.IdProv,
                DenoProv = model.DenoProv,
                TipoComprobante = model.TipoComprobante,
                FechaRegistro = model.FechaRegistro,
                Establecimiento = model.Establecimiento,
                PuntoEmision = model.PuntoEmision,
                Secuencial = model.Secuencial,
                FechaEmision = model.FechaEmision,
                Autorizacion = model.Autorizacion,
                BaseNoGraIva = model.BaseNoGraIva,
                BaseImpExe = model.BaseImpExe,
                BaseImponible = model.BaseImponible,
                BaseImpGrav = model.BaseImpGrav,
                MontoIce = model.MontoIce,
                MontoIva = model.MontoIva,
                FormaPago = model.FormaPago,
            };
        }
        public static List<AtsFactura> ToAtsFactura(this List<VwAtsFactura> model)
        {
            var _compras = new List<AtsFactura>();
            if (model?.Count > 0)
            {
                model.ForEach(comp =>
                {
                    _compras.Add(new AtsFactura
                    {
                        Id = comp.Id,
                        FechaEmision = comp.FechaEmision,
                        TipoDocumento = comp.TipoDocumento,
                        Anio = comp.Anio,
                        Mes = comp.Mes,
                        DocumentoType = comp.DocumentoType,
                        NumeroDocumento = comp.NumeroDocumento,
                        Numeroautorizacion = comp.Numeroautorizacion,
                        FechaAutorizacion = comp.FechaAutorizacion,
                        Rucproveedor = comp.Rucproveedor,
                        RazonSocialProveedor = comp.RazonSocialProveedor,
                        Total = comp.Total,
                        Ruc = comp.Ruc,
                        RazonSocial = comp.RazonSocial,
                        CodSustento = comp.CodSustento,
                        TipoEmision = comp.TipoEmision
                        
                    });
                });
            }
            return _compras;
        }
        public static AtsFactura ToAtsFactura(this VwAtsFactura model)
        {            
            return new AtsFactura{
                Id = model.Id,
                FechaEmision = model.FechaEmision,
                TipoDocumento = model.TipoDocumento,
                Anio = model.Anio,
                Mes = model.Mes,
                DocumentoType = model.DocumentoType,
                NumeroDocumento = model.NumeroDocumento,
                Numeroautorizacion = model.Numeroautorizacion,
                FechaAutorizacion = model.FechaAutorizacion,
                Rucproveedor = model.Rucproveedor,
                RazonSocialProveedor = model.RazonSocialProveedor,
                Total = model.Total,
                Ruc = model.Ruc,
                RazonSocial = model.RazonSocial,
                CodSustento = model.CodSustento,
                TipoEmision = model.TipoEmision
            };
        }
    }
}
