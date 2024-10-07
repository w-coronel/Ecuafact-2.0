/****** Object:  StoredProcedure [dbo].[SpSearchProducts]    Script Date: 17/12/2018 13:13:03 ******/
DROP PROCEDURE [dbo].[SpSearchProducts]
GO
/****** Object:  StoredProcedure [dbo].[SpSearchContributors]    Script Date: 17/12/2018 13:13:03 ******/
DROP PROCEDURE [dbo].[SpSearchContributors]
GO
/****** Object:  StoredProcedure [dbo].[SpGetNextDocumentSequential]    Script Date: 17/12/2018 13:13:03 ******/
DROP PROCEDURE [dbo].[SpGetNextDocumentSequential]
GO
ALTER TABLE [dbo].[TotalTax] DROP CONSTRAINT [FK_dbo.TotalTax_dbo.Document_DocumentId]
GO
ALTER TABLE [dbo].[Tax] DROP CONSTRAINT [FK_dbo.Tax_dbo.DocumentDetail_DocumentDetailId]
GO
ALTER TABLE [dbo].[RequestSession] DROP CONSTRAINT [FK_dbo.RequestSession_dbo.Issuer_IssuerId]
GO
ALTER TABLE [dbo].[Product] DROP CONSTRAINT [FK_dbo.Product_dbo.ProductType_ProductTypeId]
GO
ALTER TABLE [dbo].[Product] DROP CONSTRAINT [FK_dbo.Product_dbo.IVARate_IvaRateId]
GO
ALTER TABLE [dbo].[Product] DROP CONSTRAINT [FK_dbo.Product_dbo.Issuer_IssuerId]
GO
ALTER TABLE [dbo].[Product] DROP CONSTRAINT [FK_dbo.Product_dbo.ICERate_IceRateId]
GO
ALTER TABLE [dbo].[Payment] DROP CONSTRAINT [FK_dbo.Payment_dbo.Document_DocumentId]
GO
ALTER TABLE [dbo].[InvoiceInfo] DROP CONSTRAINT [FK_dbo.InvoiceInfo_dbo.Document_InvoiceInfoId]
GO
ALTER TABLE [dbo].[DocumentDetail] DROP CONSTRAINT [FK_dbo.DocumentDetail_dbo.Document_DocumentId]
GO
ALTER TABLE [dbo].[Contributor] DROP CONSTRAINT [FK_dbo.Contributor_dbo.Issuer_IssuerId]
GO
ALTER TABLE [dbo].[Contributor] DROP CONSTRAINT [FK_dbo.Contributor_dbo.IdentificationType_IdentificationTypeId]
GO
ALTER TABLE [dbo].[Contributor] DROP CONSTRAINT [FK_dbo.Contributor_dbo.ContributorType_ContributorTypeId]
GO
ALTER TABLE [dbo].[AdditionalField] DROP CONSTRAINT [FK_dbo.AdditionalField_dbo.Document_DocumentId]
GO
/****** Object:  Index [IX_DocumentId]    Script Date: 17/12/2018 13:13:03 ******/
DROP INDEX [IX_DocumentId] ON [dbo].[TotalTax]
GO
/****** Object:  Index [IX_DocumentDetailId]    Script Date: 17/12/2018 13:13:03 ******/
DROP INDEX [IX_DocumentDetailId] ON [dbo].[Tax]
GO
/****** Object:  Index [IX_IssuerId]    Script Date: 17/12/2018 13:13:03 ******/
DROP INDEX [IX_IssuerId] ON [dbo].[RequestSession]
GO
/****** Object:  Index [IX_ProductTypeId]    Script Date: 17/12/2018 13:13:03 ******/
DROP INDEX [IX_ProductTypeId] ON [dbo].[Product]
GO
/****** Object:  Index [IX_PROD_CODE]    Script Date: 17/12/2018 13:13:03 ******/
DROP INDEX [IX_PROD_CODE] ON [dbo].[Product]
GO
/****** Object:  Index [IX_IvaRateId]    Script Date: 17/12/2018 13:13:03 ******/
DROP INDEX [IX_IvaRateId] ON [dbo].[Product]
GO
/****** Object:  Index [IX_IssuerId]    Script Date: 17/12/2018 13:13:03 ******/
DROP INDEX [IX_IssuerId] ON [dbo].[Product]
GO
/****** Object:  Index [IX_IceRateId]    Script Date: 17/12/2018 13:13:03 ******/
DROP INDEX [IX_IceRateId] ON [dbo].[Product]
GO
/****** Object:  Index [IX_DocumentId]    Script Date: 17/12/2018 13:13:03 ******/
DROP INDEX [IX_DocumentId] ON [dbo].[Payment]
GO
/****** Object:  Index [IX_ISSUER_RUC]    Script Date: 17/12/2018 13:13:03 ******/
DROP INDEX [IX_ISSUER_RUC] ON [dbo].[Issuer]
GO
/****** Object:  Index [IX_InvoiceInfoId]    Script Date: 17/12/2018 13:13:03 ******/
DROP INDEX [IX_InvoiceInfoId] ON [dbo].[InvoiceInfo]
GO
/****** Object:  Index [IX_DocumentId]    Script Date: 17/12/2018 13:13:03 ******/
DROP INDEX [IX_DocumentId] ON [dbo].[DocumentDetail]
GO
/****** Object:  Index [IX_DOCUMENT_RUC]    Script Date: 17/12/2018 13:13:03 ******/
DROP INDEX [IX_DOCUMENT_RUC] ON [dbo].[Document]
GO
/****** Object:  Index [IX_IssuerId]    Script Date: 17/12/2018 13:13:03 ******/
DROP INDEX [IX_IssuerId] ON [dbo].[Contributor]
GO
/****** Object:  Index [IX_IdentificationTypeId]    Script Date: 17/12/2018 13:13:03 ******/
DROP INDEX [IX_IdentificationTypeId] ON [dbo].[Contributor]
GO
/****** Object:  Index [IX_ContributorTypeId]    Script Date: 17/12/2018 13:13:03 ******/
DROP INDEX [IX_ContributorTypeId] ON [dbo].[Contributor]
GO
/****** Object:  Index [IX_DocumentId]    Script Date: 17/12/2018 13:13:03 ******/
DROP INDEX [IX_DocumentId] ON [dbo].[AdditionalField]
GO
/****** Object:  Table [dbo].[TotalTax]    Script Date: 17/12/2018 13:13:03 ******/
DROP TABLE [dbo].[TotalTax]
GO
/****** Object:  Table [dbo].[Tax]    Script Date: 17/12/2018 13:13:03 ******/
DROP TABLE [dbo].[Tax]
GO
/****** Object:  Table [dbo].[RequestSession]    Script Date: 17/12/2018 13:13:03 ******/
DROP TABLE [dbo].[RequestSession]
GO
/****** Object:  Table [dbo].[ProductType]    Script Date: 17/12/2018 13:13:03 ******/
DROP TABLE [dbo].[ProductType]
GO
/****** Object:  Table [dbo].[Product]    Script Date: 17/12/2018 13:13:03 ******/
DROP TABLE [dbo].[Product]
GO
/****** Object:  Table [dbo].[PaymentMethod]    Script Date: 17/12/2018 13:13:03 ******/
DROP TABLE [dbo].[PaymentMethod]
GO
/****** Object:  Table [dbo].[Payment]    Script Date: 17/12/2018 13:13:03 ******/
DROP TABLE [dbo].[Payment]
GO
/****** Object:  Table [dbo].[IVARate]    Script Date: 17/12/2018 13:13:03 ******/
DROP TABLE [dbo].[IVARate]
GO
/****** Object:  Table [dbo].[Issuer]    Script Date: 17/12/2018 13:13:03 ******/
DROP TABLE [dbo].[Issuer]
GO
/****** Object:  Table [dbo].[InvoiceInfo]    Script Date: 17/12/2018 13:13:03 ******/
DROP TABLE [dbo].[InvoiceInfo]
GO
/****** Object:  Table [dbo].[IdentificationType]    Script Date: 17/12/2018 13:13:03 ******/
DROP TABLE [dbo].[IdentificationType]
GO
/****** Object:  Table [dbo].[ICERate]    Script Date: 17/12/2018 13:13:03 ******/
DROP TABLE [dbo].[ICERate]
GO
/****** Object:  Table [dbo].[DocumentType]    Script Date: 17/12/2018 13:13:03 ******/
DROP TABLE [dbo].[DocumentType]
GO
/****** Object:  Table [dbo].[DocumentSequential]    Script Date: 17/12/2018 13:13:03 ******/
DROP TABLE [dbo].[DocumentSequential]
GO
/****** Object:  Table [dbo].[DocumentDetail]    Script Date: 17/12/2018 13:13:03 ******/
DROP TABLE [dbo].[DocumentDetail]
GO
/****** Object:  Table [dbo].[Document]    Script Date: 17/12/2018 13:13:03 ******/
DROP TABLE [dbo].[Document]
GO
/****** Object:  Table [dbo].[ContributorType]    Script Date: 17/12/2018 13:13:03 ******/
DROP TABLE [dbo].[ContributorType]
GO
/****** Object:  Table [dbo].[Contributor]    Script Date: 17/12/2018 13:13:03 ******/
DROP TABLE [dbo].[Contributor]
GO
/****** Object:  Table [dbo].[AdditionalField]    Script Date: 17/12/2018 13:13:03 ******/
DROP TABLE [dbo].[AdditionalField]
GO


