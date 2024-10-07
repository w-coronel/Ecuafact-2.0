
CREATE VIEW dbo.vwImpuestos
AS
	SELECT	dd.DocumentId as IdERP,
			t.DocumentDetailId as IdDetalle,
			t.Code as impuestoCodigo,
			t.PercentageCode as impuestoCodigoPorcentaje,
			t.TaxableBase as impuestoBaseImponible,
			t.Rate as impuestoTarifa,
			t.TaxValue as impuestoValor,
			t.* 
	FROM Tax t (NOLOCK) inner join DocumentDetail dd (NOLOCK)
	on t.DocumentDetailId = dd.Id
GO