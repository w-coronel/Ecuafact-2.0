
/****** Object:  View [dbo].[vwCabFactura]    Script Date: 18/12/2018 13:16:12 ******/
IF EXISTS(SELECT 1 FROM sys.views WHERE Name = 'vwCabFactura')
BEGIN
    DROP PROCEDURE dbo.vwCabFactura
END


/****** Object:  View [dbo].[vwCabFactura]    Script Date: 18/12/2018 13:16:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/****** Object:  View [dbo].[vwPayments]    Script Date: 18/12/2018 2:10:27 ******/
CREATE VIEW [dbo].[vwCabFactura]
AS

SELECT d.Id as IdERP,
	   d.RUC,
	   d.DocumentTypeCode as codDoc, 
	   d.EstablishmentCode as estab,
	   d.IssuePointCode as ptoEmi,
	   d.Sequential as secuencial,
	   d.Emails as emails,
	   d.CreatedOn as fechaEmision,
	   i.EstablishmentAddress as dirEstablecimiento,
	   i.IdentificationType as tipoIdentificacionComprador,
	   i.BussinesName as razonSocialComprador,
	   i.Identification as identificacionComprador,
	   i.Currency as moneda,
	   i.SubtotalVat as subtotoal12,
	   i.SubtotalVatZero as subtotal0,
	   i.SubtotalNotSubject as subtotalNoSujeto,
	   i.Subtotal as totalSinImpuestos,
	   i.TotalDiscount as totalDescuentos,
	   i.SpecialConsumTax as ICE,
	   i.ValueAddedTax as IVA12,
	   i.Total as importeTotal,
	   i.Tip as propina,
	   i.ReferralGuide as guiaRemision,
	   i.Address as direccionComprador,
	   i.InvoiceIncoTerm as IncoTermFactura,
	   i.PlaceIncoTerm as lugarIncoTerm,
	   i.OriginCountry as paisOrigen,
	   i.PortBoarding as puertoEmbarque,
	   i.DestinationPort as puertoDestino,
	   i.DestinationCountry as paisDestino,
	   i.CountryAcquisition as paisAdquisicion,
	   i.TotalWithoutTaxesIncoTerm as incoTermTotalSinImpuestos
from Document d (NOLOCK) inner join InvoiceInfo i (NOLOCK)
on d.Id = i.InvoiceInfoId

GO


