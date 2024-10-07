
CREATE VIEW dbo.vwImpuestosTotales
AS
	SELECT tt.DocumentId as IdERP,
		tt.TaxCode as codigo,
		tt.PercentageTaxCode as codigoPorcentaje,
		tt.TaxRate as tarifa,
		tt.TaxableBase as baseImponible,
		tt.AditionalDiscount as DescuentoAdicional,
		tt.TaxValue as valor
	FROM dbo.TotalTax tt (NOLOCK)
GO