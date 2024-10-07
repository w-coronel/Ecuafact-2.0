
CREATE VIEW dbo.vwInfoAdicional
AS
	SELECT	af.DocumentId as IdERP,
			af.Name as Nombre,
			af.Value as Valor,
			af.LineNumber as NumLinea
	FROM AdditionalField af (NOLOCK)
GO