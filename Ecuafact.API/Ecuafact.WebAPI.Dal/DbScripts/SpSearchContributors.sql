USE [EcuafactExpress]
GO

IF EXISTS(SELECT 1 FROM sys.procedures WHERE Name = 'SpSearchContributors')
BEGIN
    DROP PROCEDURE dbo.SpSearchContributors
END

/****** Object:  StoredProcedure [dbo].[SpSearchContributors]    Script Date: 17/12/2018 11:59:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SpSearchContributors]
(
	@searchTerm NVARCHAR(200),
	@issuerId BIGINT
)
AS
BEGIN
	select * from Contributor (NOLOCK)
	where LOWER(Identification+'-'+BussinesName) like '%'+LOWER(@searchTerm)+'%'
	and IssuerId = @issuerId
END;

GO

