using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ecuafact.WebAPI.Domain.Entities.Ats
{
    [XmlRoot("iva")]
    public class ATS
    {
        public string TipoIDInformante { get; set; }
        public string IdInformante { get; set; }
        public string razonSocial { get; set; }
        public int Anio { get; set; }
        public string Mes { get; set; }
        public string regimenMicroempresa { get; set; }
        public string numEstabRuc { get; set; }
        public decimal totalVentas { get; set; }
        public string codigoOperativo { get; set; }
        [XmlArrayItem("detalleCompras")]
        public List<ATS_Compras> compras { get; set; }

        [XmlArrayItem("detalleVentas")]
        public List<ATS_Ventas> ventas { get; set; }

        [XmlArrayItem("ventaEst")]
        public List<ATS_VentasEstablecimiento> ventasEstablecimiento { get; set; } //Maximo 1 objeto

        [XmlArrayItem("detalleExportaciones")]
        public List<ATS_DetalleExportaciones> exportaciones { get; set; }

        [XmlArrayItem("detalleRecap")]
        public List<detalleRecapType> recap { get; set; }

        [XmlArrayItem("detalleFideicomisos")]
        public List<detalleFideicomisosType> fideicomisos { get; set; }

        [XmlArrayItem("detalleAnulados")]
        public List<ATS_Anulados> anulados { get; set; }

        [XmlArrayItem("detalleRendFinancieros")]
        public List<detalleRendFinancierosType> rendFinancieros { get; set; }
    }

    public class ATS_Compras
    {
        public string codSustento { get; set; } = ""; //
        public string tpIdProv { get; set; } = ""; //
        public string idProv { get; set; } = ""; //
        public string tipoComprobante { get; set; } = ""; //
        public string tipoProv { get; set; }
        public string denoProv { get; set; } //
        public string parteRel { get; set; } = ""; //
        public string fechaRegistro { get; set; } //
        public string establecimiento { get; set; } = ""; //
        public string puntoEmision { get; set; } = ""; //
        public string secuencial { get; set; } //
        public string fechaEmision { get; set; } //
        public string autorizacion { get; set; } //
        public decimal baseNoGraIva { get; set; }
        public decimal baseImponible { get; set; }
        public decimal baseImpGrav { get; set; }
        public decimal baseImpExe { get; set; }
        public decimal montoIce { get; set; }
        public decimal montoIva { get; set; }
        public decimal valRetBien10 { get; set; }
        public decimal valRetServ20 { get; set; }
        public decimal valorRetBienes { get; set; }
        public decimal valRetServ50 { get; set; }
        public decimal valorRetServicios { get; set; }
        public decimal valRetServ100 { get; set; }
        public decimal valorRetencionNc { get; set; }
        public decimal totbasesImpReemb { get; set; }
        public PagoExteriorType pagoExterior { get; set; }
        public FormasDePagoType formasDePago { get; set; }

        [XmlArrayItem("detalleAir")]
        public List<AirType> air { get; set; } //SOlo 1
        public string estabRetencion1 { get; set; }
        public string ptoEmiRetencion1 { get; set; }
        public string secRetencion1 { get; set; }
        public string autRetencion1 { get; set; }
        public string fechaEmiRet1 { get; set; }
        public string estabRetencion2 { get; set; }
        public string ptoEmiRetencion2 { get; set; }
        public string secRetencion2 { get; set; }
        public string autRetencion2 { get; set; }
        public string fechaEmiRet2 { get; set; }
        public string docModificado { get; set; }
        public string estabModificado { get; set; }
        public string ptoEmiModificado { get; set; }
        public string secModificado { get; set; }
        public string autModificado { get; set; }
        [XmlArrayItem("reembolso")]
        public List<ReembolsosType> reembolsos { get; set; }
    }

    public class ATS_Ventas
    {
        public string tpIdCliente { get; set; }
        public string idCliente { get; set; }
        public string parteRelVtas { get; set; }
        public string tipoCliente { get; set; }
        public string denoCli { get; set; }
        public string tipoComprobante { get; set; }
        public string tipoEmision { get; set; }
        public int numeroComprobantes { get; set; }
        public decimal baseNoGraIva { get; set; }
        public decimal baseImponible { get; set; }
        public decimal baseImpGrav { get; set; }
        public decimal montoIva { get; set; }
        public string compensaciones { get; set; }
        public decimal montoIce { get; set; }
        public decimal valorRetIva { get; set; }
        public decimal valorRetRenta { get; set; }
        public FormasDePagoType formasDePago { get; set; }
    }

    public class ATS_VentasEstablecimiento
    {
       
        public string codEstab { get; set; }
        public decimal ventasEstab { get; set; }


        public decimal ivaComp { get; set; }
    }

    public class ATS_DetalleExportaciones
    {
        public int? tpIdClienteEx { get; set; }
        public string idClienteEx { get; set; }
        public string parteRelExp { get; set; }
        public string tipoCli { get; set; }
        public string denoExpCli { get; set; }
        public string tipoRegi { get; set; }
        public string paisEfecPagoGen { get; set; }
        public string paisEfecPagoParFis { get; set; }
        public string denopagoRegFis { get; set; }
        public string paisEfecExp { get; set; }
        public string pagoRegFis { get; set; }
        public string exportacionDe { get; set; }
        public string tipIngExt { get; set; }
        public string ingExtGravOtroPais { get; set; }


        public decimal impuestoOtroPais { get; set; }
        public string tipoComprobante { get; set; }
        public string distAduanero { get; set; }
        public int? anio { get; set; }
        public string regimen { get; set; }
        public string correlativo { get; set; }
        public string verificador { get; set; }
        public string docTransp { get; set; }
        public string fechaEmbarque { get; set; }
        public string fue { get; set; }
        public decimal valorFOB { get; set; }
        public decimal valorFOBComprobante { get; set; }
        public string establecimiento { get; set; }
        public string puntoEmision { get; set; }
        public string secuencial { get; set; }
        public string autorizacion { get; set; }
        public string fechaEmision { get; set; }
    }
    public class ATS_Anulados
    {
       
        public string tipoComprobante { get; set; }
        public string establecimiento { get; set; }
        public string puntoEmision { get; set; }
        public int secuencialInicio { get; set; }
        public int secuencialFin { get; set; }
        public string autorizacion { get; set; }
    }

    public class ReembolsosType
    {
        public string tipoComprobanteReemb { get; set; }
        public string tpIdProvReemb { get; set; }
        public string idProvReemb { get; set; }
        public string establecimientoReemb { get; set; }
        public string puntoEmisionReemb { get; set; }
        public string secuencialReemb { get; set; }
        public DateTime fechaEmisionReemb { get; set; }
        public string autorizacionReemb { get; set; }
        public decimal baseImponibleReemb { get; set; }
        public decimal baseImpGravReemb { get; set; }
        public decimal baseNoGraIvaReemb { get; set; }
        public decimal baseImpExeReemb { get; set; }
        public decimal montoIceRemb { get; set; }
        public decimal montoIvaRemb { get; set; }
    }

    public class FormasDePagoType
    {
        public string formaPago { get; set; }
    }

    public class AirType
    {        
        public string codRetAir { get; set; }
        public decimal baseImpAir { get; set; }
        public decimal porcentajeAir { get; set; }
        public decimal valRetAir { get; set; }

    }

    public class PagoExteriorType
    {
        
        public string pagoLocExt { get; set; }
        public string tipoRegi { get; set; }
        public string paisEfecPagoGen { get; set; }
        public string paisEfecPagoParFis { get; set; }
        public string denopagoRegFis { get; set; }
        public string paisEfecPago { get; set; }
        public string aplicConvDobTrib { get; set; } = "NA";
        public string pagExtSujRetNorLeg { get; set; } = "NA";
        public string pagoRegFis { get; set; }
    }

    public class detalleFideicomisosType
    {
        public string tipoBeneficiario { get; set; }
        public string idBeneficiario { get; set; }
        public string parteRelExp { get; set; }
        public string tipoBeneficiarioCli { get; set; }
        public string denoBenefi { get; set; }
        public string rucFideicomiso { get; set; }
        [XmlArrayItem("detallefValor")]
        public List<detallefValorType> fValor { get; set; } //Solo 1
    }

    public class detallefValorType
    {
        public int tipoFideicomiso { get; set; }
        public decimal totalF { get; set; }
        public decimal individualF { get; set; }
        public decimal porRetF { get; set; }
        public decimal valorRetF { get; set; }
        public string fechaPagoDiv { get; set; }

        public decimal imRentaSoc { get; set; }
        public int anioUtDiv { get; set; }
        public PagoExteriorType pagoExterior { get; set; }
    }

    public class detalleRecapType
    {
        public string establecimientoRecap { get; set; }
        public string identificacionRecap { get; set; }
        public string parteRelRec { get; set; }
        public string tipoEst { get; set; }
        public string denoCliRecaps { get; set; }
        public string tipoComprobante { get; set; }
        public string numeroRecap { get; set; }
        public string fechaPago { get; set; }
        public string tarjetaCredito { get; set; }
        public string fechaEmisionRecap { get; set; }
        public decimal consumoCero { get; set; }
        public decimal consumoGravado { get; set; }
        public decimal totalConsumo { get; set; }
        public decimal montoIva { get; set; }
        [XmlArrayItem("compensacion")]
        public List<compensacionType> compensaciones { get; set; } //Solo 1
        public decimal comision { get; set; }
        public int numeroVouchers { get; set; }

        public decimal valRetBien10 { get; set; }

        public decimal valRetServ20 { get; set; }
        public decimal valorRetBienes { get; set; }

        public decimal valRetServ50 { get; set; }
        public decimal valorRetServicios { get; set; }
        public decimal valRetServ100 { get; set; }
        public PagoExteriorType pagoExterior { get; set; }
        [XmlArrayItem("detalleAir")]
        public List<AirType> air { get; set; } //Solo 1
        public string establecimiento { get; set; }
        public string puntoEmision { get; set; }
        public int secuencial { get; set; }
        public string autorizacion { get; set; }
        public string fechaEmision { get; set; }
    }

    public class detalleRendFinancierosType
    {
        public string retenido { get; set; }
        public string idRetenido { get; set; }
        public string parteRelFid { get; set; }
        public string tipoRete { get; set; }
        public string denoBenefi { get; set; }
        public ahorroPNType ahorroPN { get; set; }
        public ahorroPNType ctaExenta { get; set; }

        [XmlArrayItem("detRet")]
        public List<retencionesType> retenciones { get; set; }
    }

    public class ahorroPNType
    {
        public decimal totalDep { get; set; }
        public decimal rendGen { get; set; }
    }

    public class retencionesType
    {
        public PagoExteriorType pagoExterior { get; set; }
        public int estabRetencion { get; set; }
        public int ptoEmiRetencion { get; set; }
        public int secRetencion { get; set; }
        public string autRetencion { get; set; }
        public DateTime fechaEmiRet { get; set; }
        //detalleAirRen
        [XmlArrayItem("detalleAirRen")]
        public List<detalleAirRenType> airRend { get; set; }
    }

    public class detalleAirRenType
    {
        public string codRetAir { get; set; }
        public decimal deposito { get; set; }
        public decimal baseImpAir { get; set; }
        public decimal porcentajeAir { get; set; }
        public decimal valRetAir { get; set; }
    }

    public class compensacionType
    {
        public string tipoCompe { get; set; }
        public decimal monto { get; set; }
    }
}
