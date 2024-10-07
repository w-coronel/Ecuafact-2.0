

CREATE VIEW dbo.vwFormaDePago
AS
	SELECT py.DocumentId,
	py.PaymentMethodCode as CodFormaPago,
	py.Total,
	py.Term as Plazo,
	py.TimeUnit as UnidadTiempo
	FROM Payment py (NOLOCK)
GO