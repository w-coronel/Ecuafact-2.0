USE [EcuafactExpress]
GO

/****** Object:  StoredProcedure [dbo].[SpSearchProducts]    Script Date: 15/12/2018 20:38:03 ******/

IF EXISTS(SELECT 1 FROM sys.procedures WHERE Name = 'SpSearchProducts')
BEGIN
    DROP PROCEDURE dbo.SpSearchProducts
END

/****** Object:  StoredProcedure [dbo].[SpSearchProducts]    Script Date: 15/12/2018 20:38:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SpSearchProducts]
(
	@searchTerm NVARCHAR(100),
	@issuerId BIGINT
)
AS
BEGIN
	select top(20) * from Product (NOLOCK)
	where LOWER(MainCode+'-'+Name) like '%'+LOWER(@searchTerm)+'%' AND IssuerId = @issuerId
END;
GO


