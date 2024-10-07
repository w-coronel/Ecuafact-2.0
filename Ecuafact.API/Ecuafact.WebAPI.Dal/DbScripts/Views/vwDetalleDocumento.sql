
CREATE VIEW dbo.vwDetalleDocumento
AS

	SELECT
		dd.DocumentId as IdERP,
		dd.Id as IdDetalle,
		dd.MainCode as codigoPrincipal,
		dd.AuxCode as codigoAuxiliar,
		dd.Name1 as Descripcion,
		dd.Amount as cantidad,
		dd.UnitPrice as precioUnitario,
		dd.Discount as descuento,
		dd.SubTotal as precioTotalSinImpuestos,
		dd.Name1 as nom1,
		dd.Name2 as nom2,
		dd.Name3 as nom3,
		dd.Value1 as val1,
		dd.Value2 as val2,
		dd.Value3 as val3 --,dd.* 
	FROM DocumentDetail dd (NOLOCK)

GO