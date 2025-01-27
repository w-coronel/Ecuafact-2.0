USE [EcuafactExpress]
GO

/****** Object:  StoredProcedure [dbo].[SpGetNextDocumentSequential]    Script Date: 15/12/2018 21:03:32 ******/
IF EXISTS(SELECT 1 FROM sys.procedures WHERE Name = 'SpGetNextDocumentSequential')
BEGIN
    DROP PROCEDURE dbo.SpGetNextDocumentSequential
END

/****** Object:  StoredProcedure [dbo].[SpGetNextDocumentSequential]    Script Date: 15/12/2018 21:03:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SpGetNextDocumentSequential]
(
	@IssuerId BIGINT,
	@EstablishmentCode NVARCHAR(3),
	@IssuePointCode NVARCHAR(3),
	@DocumentTypeId SMALLINT,
	@DocumentType NVARCHAR(100),
	@Sequential BIGINT OUTPUT
)
AS
BEGIN
	BEGIN TRANSACTION SEQUENTIAL;
	BEGIN TRY
		SET @Sequential = 0;

		SELECT @Sequential = Sequential
        FROM dbo.DocumentSequential WITH (XLOCK, ROWLOCK)
        WHERE IssuerId = @IssuerId AND
			  EstablishmentCode = @EstablishmentCode AND
			  IssuePointCode = @IssuePointCode AND
			  DocumentTypeId = @DocumentTypeId

		IF (@Sequential = 0)
        BEGIN
            SET @Sequential = 1;
            INSERT INTO dbo.DocumentSequential
            VALUES
            (@IssuerId, @EstablishmentCode, @IssuePointCode, @DocumentTypeId,@DocumentType,@Sequential, GETDATE(), NULL);
        END;

		ELSE
        BEGIN
            SET @Sequential = @Sequential + 1;
            UPDATE dbo.DocumentSequential
            SET Sequential = @Sequential, UpdatedOn=GETDATE()
            WHERE IssuerId = @IssuerId AND
				  EstablishmentCode = @EstablishmentCode AND
				  IssuePointCode = @IssuePointCode AND
				  DocumentTypeId = @DocumentTypeId
        END;
        COMMIT TRANSACTION SEQUENTIAL;

	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION SEQUENTIAL;
	END CATCH
END
GO