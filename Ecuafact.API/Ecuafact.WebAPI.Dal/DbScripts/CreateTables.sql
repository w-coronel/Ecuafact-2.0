/****** Object:  Table [dbo].[AdditionalField]    Script Date: 17/12/2018 13:13:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdditionalField](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](300) NULL,
	[Value] [nvarchar](300) NULL,
	[LineNumber] [int] NULL,
	[DocumentId] [bigint] NOT NULL,
 CONSTRAINT [PK_dbo.AdditionalField] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[Contributor]    Script Date: 17/12/2018 13:13:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contributor](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Identification] [nvarchar](50) NULL,
	[IdentificationTypeId] [smallint] NOT NULL,
	[BussinesName] [nvarchar](300) NOT NULL,
	[TradeName] [nvarchar](300) NULL,
	[Address] [nvarchar](300) NULL,
	[Phone] [nvarchar](300) NULL,
	[EmailAddresses] [nvarchar](1500) NULL,
	[ContributorTypeId] [smallint] NOT NULL,
	[IssuerId] [bigint] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModifiedOn] [datetime] NULL,
	[IsEnabled] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Contributor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[ContributorType]    Script Date: 17/12/2018 13:13:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ContributorType](
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](500) NULL,
	[IsEnabled] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.ContributorType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[Document]    Script Date: 17/12/2018 13:13:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Document](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[RUC] [nvarchar](13) NOT NULL,
	[DocumentTypeCode] [nvarchar](2) NULL,
	[EstablishmentCode] [nvarchar](5) NULL,
	[IssuePointCode] [nvarchar](5) NULL,
	[Sequential] [nvarchar](9) NULL,
	[Emails] [nvarchar](500) NULL,
	[CreatedOn] [datetime] NOT NULL,
	[BussinesName] [nvarchar](300) NOT NULL,
	[TradeName] [nvarchar](300) NULL,
	[IsEnabled] [bit] NOT NULL,
	[IssuerId] [long] NOT NULL,
	[Status] [smallint] NOT NULL,
 CONSTRAINT [PK_dbo.Document] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[DocumentDetail]    Script Date: 17/12/2018 13:13:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentDetail](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[MainCode] [nvarchar](500) NULL,
	[AuxCode] [nvarchar](500) NULL,
	[Description] [nvarchar](500) NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
	[Discount] [decimal](18, 2) NOT NULL,
	[SubTotal] [decimal](18, 2) NOT NULL,
	[DocumentId] [bigint] NOT NULL,
 CONSTRAINT [PK_dbo.DocumentDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[DocumentSequential]    Script Date: 17/12/2018 13:13:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentSequential](
	[IssuerId] [bigint] NOT NULL,
	[EstablishmentCode] [nvarchar](5) NOT NULL,
	[IssuePointCode] [nvarchar](5) NOT NULL,
	[DocumentTypeId] [smallint] NOT NULL,
	[DocumentType] [nvarchar](100) NULL,
	[Sequential] [bigint] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NULL,
 CONSTRAINT [PK_dbo.DocumentSequential] PRIMARY KEY CLUSTERED 
(
	[IssuerId] ASC,
	[EstablishmentCode] ASC,
	[IssuePointCode] ASC,
	[DocumentTypeId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[DocumentType]    Script Date: 17/12/2018 13:13:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocumentType](
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[SriCode] [nvarchar](10) NULL,
	[Name] [nvarchar](300) NOT NULL,
	[IsEnabled] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.DocumentType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[ICERate]    Script Date: 17/12/2018 13:13:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ICERate](
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[Rate] [decimal](18, 2) NULL,
	[EspecificRate] [decimal](18, 2) NULL,
	[EspecificRateDescription] [nvarchar](max) NULL,
	[SriCode] [nvarchar](10) NULL,
	[Name] [nvarchar](300) NOT NULL,
	[IsEnabled] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.ICERate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[IdentificationType]    Script Date: 17/12/2018 13:13:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IdentificationType](
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[SriCode] [nvarchar](10) NULL,
	[Name] [nvarchar](300) NOT NULL,
	[IsEnabled] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.IdentificationType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[InvoiceInfo]    Script Date: 17/12/2018 13:13:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InvoiceInfo](
	[InvoiceInfoId] [bigint] NOT NULL,
	[IssuedOn] [nvarchar](10) NOT NULL,
	[EstablishmentAddress] [nvarchar](300) NOT NULL,
	[IdentificationType] [nvarchar](2) NOT NULL,
	[Identification] [nvarchar](100) NOT NULL,
	[BussinesName] [nvarchar](300) NOT NULL,
	[Currency] [nvarchar](max) NULL,
	[SubtotalVat] [decimal](18, 2) NOT NULL,
	[SubtotalVatZero] [decimal](18, 2) NOT NULL,
	[SubtotalNotSubject] [decimal](18, 2) NOT NULL,
	[SubtotalExempt] [decimal](18, 2) NOT NULL,
	[Subtotal] [decimal](18, 2) NOT NULL,
	[TotalDiscount] [decimal](18, 2) NOT NULL,
	[SpecialConsumTax] [decimal](18, 2) NOT NULL,
	[ValueAddedTax] [decimal](18, 2) NOT NULL,
	[Tip] [decimal](18, 2) NOT NULL,
	[Total] [decimal](18, 2) NOT NULL,
	[ReferralGuide] [nvarchar](20) NULL,
	[Address] [nvarchar](300) NULL,
	[ModifiedDocumentCode] [nvarchar](2) NULL,
	[ModifiedDocumentNumber] [nvarchar](17) NULL,
	[SupportDocumentIssueDate] [nvarchar](10) NULL,
 CONSTRAINT [PK_dbo.InvoiceInfo] PRIMARY KEY CLUSTERED 
(
	[InvoiceInfoId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[Issuer]    Script Date: 17/12/2018 13:13:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Issuer](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[RUC] [nvarchar](13) NOT NULL,
	[BussinesName] [nvarchar](300) NOT NULL,
	[TradeName] [nvarchar](300) NULL,
	[MainAddress] [nvarchar](300) NULL,
	[EstablishmentCode] [nvarchar](5) NULL,
	[IssuePointCode] [nvarchar](5) NULL,
	[IsSpecialContributor] [bit] NOT NULL,
	[ResolutionNumber] [nvarchar](50) NULL,
	[IsAccountingRequired] [bit] NOT NULL,
	[EnvironmentType] [int] NOT NULL,
	[IssueType] [int] NOT NULL,
	[IsEnabled] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Issuer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[IVARate]    Script Date: 17/12/2018 13:13:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IVARate](
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[RateValue] [decimal](18, 2) NOT NULL,
	[SriCode] [nvarchar](10) NULL,
	[Name] [nvarchar](300) NOT NULL,
	[IsEnabled] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.IVARate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[Payment]    Script Date: 17/12/2018 13:13:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Payment](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[PaymentMethodCode] [nvarchar](2) NULL,
	[Total] [decimal](18, 2) NULL,
	[Term] [int] NULL,
	[TimeUnit] [nvarchar](30) NULL,
	[DocumentId] [bigint] NOT NULL,
 CONSTRAINT [PK_dbo.Payment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[PaymentMethod]    Script Date: 17/12/2018 13:13:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentMethod](
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[SriCode] [nvarchar](10) NULL,
	[Name] [nvarchar](300) NOT NULL,
	[IsEnabled] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.PaymentMethod] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[Product]    Script Date: 17/12/2018 13:13:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Product](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[MainCode] [nvarchar](500) NULL,
	[AuxCode] [nvarchar](500) NULL,
	[Name] [nvarchar](500) NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
	[ProductTypeId] [smallint] NOT NULL,
	[IssuerId] [bigint] NOT NULL,
	[IvaRateId] [smallint] NOT NULL,
	[IceRateId] [smallint] NOT NULL,
	[IsEnabled] [bit] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[LastModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_dbo.Product] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[ProductType]    Script Date: 17/12/2018 13:13:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductType](
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](500) NULL,
	[IsEnabled] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.ProductType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[RequestSession]    Script Date: 17/12/2018 13:13:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RequestSession](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Token] [nvarchar](100) NULL,
	[IssuerId] [bigint] NOT NULL,
	[Username] [nvarchar](50) NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ClosedOn] [datetime] NULL,
	[IsEnabled] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.RequestSession] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[Tax]    Script Date: 17/12/2018 13:13:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tax](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](1) NULL,
	[PercentageCode] [nvarchar](4) NULL,
	[Rate] [decimal](18, 2) NOT NULL,
	[TaxableBase] [decimal](18, 2) NOT NULL,
	[TaxValue] [decimal](18, 2) NOT NULL,
	[DocumentDetailId] [bigint] NOT NULL,
 CONSTRAINT [PK_dbo.Tax] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
/****** Object:  Table [dbo].[TotalTax]    Script Date: 17/12/2018 13:13:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TotalTax](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[TaxCode] [nvarchar](1) NULL,
	[PercentageTaxCode] [nvarchar](5) NULL,
	[TaxableBase] [decimal](18, 2) NOT NULL,
	[TaxValue] [decimal](18, 2) NOT NULL,
	[TaxRate] [decimal](18, 2) NOT NULL,
	[AditionalDiscount] [decimal](18, 2) NOT NULL,
	[DocumentId] [bigint] NOT NULL,
 CONSTRAINT [PK_dbo.TotalTax] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF)
)

GO
SET IDENTITY_INSERT [dbo].[ContributorType] ON 

GO
INSERT [dbo].[ContributorType] ([Id], [Name], [IsEnabled]) VALUES (1, N'CLIENTE', 1)
GO
INSERT [dbo].[ContributorType] ([Id], [Name], [IsEnabled]) VALUES (2, N'SUJETO RETENIDO', 1)
GO
INSERT [dbo].[ContributorType] ([Id], [Name], [IsEnabled]) VALUES (3, N'DESTINATARIO', 1)
GO
SET IDENTITY_INSERT [dbo].[ContributorType] OFF
GO
SET IDENTITY_INSERT [dbo].[DocumentType] ON 

GO
INSERT [dbo].[DocumentType] ([Id], [SriCode], [Name], [IsEnabled]) VALUES (1, N'01', N'FACTURA', 1)
GO
INSERT [dbo].[DocumentType] ([Id], [SriCode], [Name], [IsEnabled]) VALUES (2, N'04', N'NOTA DE CREDITO', 1)
GO
INSERT [dbo].[DocumentType] ([Id], [SriCode], [Name], [IsEnabled]) VALUES (3, N'05', N'NOTA DE DEBITO', 1)
GO
INSERT [dbo].[DocumentType] ([Id], [SriCode], [Name], [IsEnabled]) VALUES (4, N'06', N'GUIA DE REMISION', 1)
GO
INSERT [dbo].[DocumentType] ([Id], [SriCode], [Name], [IsEnabled]) VALUES (5, N'07', N'COMPROBANTE DE RETENCION', 1)
GO
SET IDENTITY_INSERT [dbo].[DocumentType] OFF
GO
SET IDENTITY_INSERT [dbo].[ICERate] ON 

GO
INSERT [dbo].[ICERate] ([Id], [Rate], [EspecificRate], [EspecificRateDescription], [SriCode], [Name], [IsEnabled]) VALUES (1, CAST(0.00 AS Decimal(18, 2)), NULL, NULL, N'0', N'NO APLICA', 1)
GO
INSERT [dbo].[ICERate] ([Id], [Rate], [EspecificRate], [EspecificRateDescription], [SriCode], [Name], [IsEnabled]) VALUES (2, CAST(100.00 AS Decimal(18, 2)), NULL, NULL, N'3670', N'Cocinas, calefones y otros de uso doméstico a gas SRI', 1)
GO
INSERT [dbo].[ICERate] ([Id], [Rate], [EspecificRate], [EspecificRateDescription], [SriCode], [Name], [IsEnabled]) VALUES (3, CAST(100.00 AS Decimal(18, 2)), NULL, NULL, N'3640', N'Focos incandescentes excepto aquellos utilizados como insumos automotrices', 1)
GO
INSERT [dbo].[ICERate] ([Id], [Rate], [EspecificRate], [EspecificRateDescription], [SriCode], [Name], [IsEnabled]) VALUES (4, CAST(15.00 AS Decimal(18, 2)), NULL, NULL, N'3092', N'Servicios de televisión pagada', 1)
GO
SET IDENTITY_INSERT [dbo].[ICERate] OFF
GO
SET IDENTITY_INSERT [dbo].[IdentificationType] ON 

GO
INSERT [dbo].[IdentificationType] ([Id], [SriCode], [Name], [IsEnabled]) VALUES (1, N'05', N'CEDULA', 1)
GO
INSERT [dbo].[IdentificationType] ([Id], [SriCode], [Name], [IsEnabled]) VALUES (2, N'04', N'RUC', 1)
GO
INSERT [dbo].[IdentificationType] ([Id], [SriCode], [Name], [IsEnabled]) VALUES (3, N'06', N'PASAPORTE', 1)
GO
INSERT [dbo].[IdentificationType] ([Id], [SriCode], [Name], [IsEnabled]) VALUES (4, N'07', N'CONSUMIDOR FINAL', 1)
GO
INSERT [dbo].[IdentificationType] ([Id], [SriCode], [Name], [IsEnabled]) VALUES (5, N'08', N'IDENTIFICACION DEL EXTERIOR', 1)
GO
INSERT [dbo].[IdentificationType] ([Id], [SriCode], [Name], [IsEnabled]) VALUES (6, N'09', N'PLACA', 1)
GO
SET IDENTITY_INSERT [dbo].[IdentificationType] OFF
GO
SET IDENTITY_INSERT [dbo].[Issuer] ON 

GO
INSERT [dbo].[Issuer] ([Id], [RUC], [BussinesName], [TradeName], [MainAddress], [EstablishmentCode], [IssuePointCode], [IsSpecialContributor], [ResolutionNumber], [IsAccountingRequired], [EnvironmentType], [IssueType], [IsEnabled]) VALUES (1, N'0992882549001', N'Consultora Informatica Ecuadorian Nexus - Ecuanexus IT', N'Ecuanexus', N'Av. del Periodista 420 entre Calle Olimpo y Calle 10ma', N'001', N'001', 0, NULL, 1, 0, 0, 1)
GO
INSERT [dbo].[Issuer] ([Id], [RUC], [BussinesName], [TradeName], [MainAddress], [EstablishmentCode], [IssuePointCode], [IsSpecialContributor], [ResolutionNumber], [IsAccountingRequired], [EnvironmentType], [IssueType], [IsEnabled]) VALUES (2, N'0919821850001', N'MORANTE ARREAGA CHRISTIAN ANDRES', N'MORANTE ARREAGA CHRISTIAN ANDRES', N'GUAYAQUIL', N'001', N'001', 0, NULL, 0, 0, 0, 1)
GO
SET IDENTITY_INSERT [dbo].[Issuer] OFF
GO
SET IDENTITY_INSERT [dbo].[IVARate] ON 

GO
INSERT [dbo].[IVARate] ([Id], [RateValue], [SriCode], [Name], [IsEnabled]) VALUES (1, CAST(0.00 AS Decimal(18, 2)), N'0', N'0%', 1)
GO
INSERT [dbo].[IVARate] ([Id], [RateValue], [SriCode], [Name], [IsEnabled]) VALUES (2, CAST(12.00 AS Decimal(18, 2)), N'2', N'12%', 1)
GO
INSERT [dbo].[IVARate] ([Id], [RateValue], [SriCode], [Name], [IsEnabled]) VALUES (3, CAST(14.00 AS Decimal(18, 2)), N'3', N'14%', 1)
GO
INSERT [dbo].[IVARate] ([Id], [RateValue], [SriCode], [Name], [IsEnabled]) VALUES (4, CAST(0.00 AS Decimal(18, 2)), N'6', N'NO OBJETO DE IMPUESTO', 1)
GO
INSERT [dbo].[IVARate] ([Id], [RateValue], [SriCode], [Name], [IsEnabled]) VALUES (5, CAST(0.00 AS Decimal(18, 2)), N'7', N'EXENTO DE IVA', 1)
GO
SET IDENTITY_INSERT [dbo].[IVARate] OFF
GO
SET IDENTITY_INSERT [dbo].[PaymentMethod] ON 

GO
INSERT [dbo].[PaymentMethod] ([Id], [SriCode], [Name], [IsEnabled]) VALUES (1, N'01', N'SIN UTILIZACION DEL SISTEMA FINANCIERO', 1)
GO
INSERT [dbo].[PaymentMethod] ([Id], [SriCode], [Name], [IsEnabled]) VALUES (2, N'15', N'COMPENSACION DE DEUDAS', 1)
GO
INSERT [dbo].[PaymentMethod] ([Id], [SriCode], [Name], [IsEnabled]) VALUES (3, N'16', N'TARJETAS DE DEBITO', 1)
GO
INSERT [dbo].[PaymentMethod] ([Id], [SriCode], [Name], [IsEnabled]) VALUES (4, N'17', N'DINERO ELECTRONICO', 1)
GO
INSERT [dbo].[PaymentMethod] ([Id], [SriCode], [Name], [IsEnabled]) VALUES (5, N'18', N'TARJETA PREPAGO', 1)
GO
INSERT [dbo].[PaymentMethod] ([Id], [SriCode], [Name], [IsEnabled]) VALUES (6, N'19', N'TARJETA DE CREDITO', 1)
GO
INSERT [dbo].[PaymentMethod] ([Id], [SriCode], [Name], [IsEnabled]) VALUES (7, N'20', N'OTROS CON UTILIZACION DEL SISTEMA FINANCIERO', 1)
GO
INSERT [dbo].[PaymentMethod] ([Id], [SriCode], [Name], [IsEnabled]) VALUES (8, N'21', N'ENDOSO DE TITULOS', 1)
GO
SET IDENTITY_INSERT [dbo].[PaymentMethod] OFF
GO
SET IDENTITY_INSERT [dbo].[Product] ON 

GO
INSERT [dbo].[Product] ([Id], [MainCode], [AuxCode], [Name], [UnitPrice], [ProductTypeId], [IssuerId], [IvaRateId], [IceRateId], [IsEnabled], [CreatedOn], [LastModifiedOn]) VALUES (1, N'PR01', N'PR001', N'PRODUCTO 1', CAST(5.50 AS Decimal(18, 2)), 1, 1, 2, 1, 1, CAST(N'2018-12-17T13:01:53.843' AS DateTime), NULL)
GO
SET IDENTITY_INSERT [dbo].[Product] OFF
GO
SET IDENTITY_INSERT [dbo].[ProductType] ON 

GO
INSERT [dbo].[ProductType] ([Id], [Name], [IsEnabled]) VALUES (1, N'BIEN', 1)
GO
INSERT [dbo].[ProductType] ([Id], [Name], [IsEnabled]) VALUES (2, N'SERVICIO', 1)
GO
SET IDENTITY_INSERT [dbo].[ProductType] OFF
GO
SET IDENTITY_INSERT [dbo].[RequestSession] ON 

GO
INSERT [dbo].[RequestSession] ([Id], [Token], [IssuerId], [Username], [CreatedOn], [ClosedOn], [IsEnabled]) VALUES (1, N'33A08B05-0E2C-47F6-B0A1-D0F50F7A7850', 1, N'TESTUSER', CAST(N'2018-12-17T12:57:09.117' AS DateTime), NULL, 1)
GO
INSERT [dbo].[RequestSession] ([Id], [Token], [IssuerId], [Username], [CreatedOn], [ClosedOn], [IsEnabled]) VALUES (2, N'E82A4349-6130-4890-A2FF-D0302EFD0B4C', 2, N'TESTUSER', CAST(N'2018-12-17T12:57:09.120' AS DateTime), NULL, 1)
GO
SET IDENTITY_INSERT [dbo].[RequestSession] OFF
GO
/****** Object:  Index [IX_DocumentId]    Script Date: 17/12/2018 13:13:03 ******/
CREATE NONCLUSTERED INDEX [IX_DocumentId] ON [dbo].[AdditionalField]
(
	[DocumentId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_ContributorTypeId]    Script Date: 17/12/2018 13:13:03 ******/
CREATE NONCLUSTERED INDEX [IX_ContributorTypeId] ON [dbo].[Contributor]
(
	[ContributorTypeId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_IdentificationTypeId]    Script Date: 17/12/2018 13:13:03 ******/
CREATE NONCLUSTERED INDEX [IX_IdentificationTypeId] ON [dbo].[Contributor]
(
	[IdentificationTypeId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_IssuerId]    Script Date: 17/12/2018 13:13:03 ******/
CREATE NONCLUSTERED INDEX [IX_IssuerId] ON [dbo].[Contributor]
(
	[IssuerId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_DOCUMENT_RUC]    Script Date: 17/12/2018 13:13:03 ******/
CREATE NONCLUSTERED INDEX [IX_DOCUMENT_RUC] ON [dbo].[Document]
(
	[RUC] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_DocumentId]    Script Date: 17/12/2018 13:13:03 ******/
CREATE NONCLUSTERED INDEX [IX_DocumentId] ON [dbo].[DocumentDetail]
(
	[DocumentId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_InvoiceInfoId]    Script Date: 17/12/2018 13:13:03 ******/
CREATE NONCLUSTERED INDEX [IX_InvoiceInfoId] ON [dbo].[InvoiceInfo]
(
	[InvoiceInfoId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_ISSUER_RUC]    Script Date: 17/12/2018 13:13:03 ******/
CREATE NONCLUSTERED INDEX [IX_ISSUER_RUC] ON [dbo].[Issuer]
(
	[RUC] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_DocumentId]    Script Date: 17/12/2018 13:13:03 ******/
CREATE NONCLUSTERED INDEX [IX_DocumentId] ON [dbo].[Payment]
(
	[DocumentId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_IceRateId]    Script Date: 17/12/2018 13:13:03 ******/
CREATE NONCLUSTERED INDEX [IX_IceRateId] ON [dbo].[Product]
(
	[IceRateId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_IssuerId]    Script Date: 17/12/2018 13:13:03 ******/
CREATE NONCLUSTERED INDEX [IX_IssuerId] ON [dbo].[Product]
(
	[IssuerId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_IvaRateId]    Script Date: 17/12/2018 13:13:03 ******/
CREATE NONCLUSTERED INDEX [IX_IvaRateId] ON [dbo].[Product]
(
	[IvaRateId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_PROD_CODE]    Script Date: 17/12/2018 13:13:03 ******/
CREATE NONCLUSTERED INDEX [IX_PROD_CODE] ON [dbo].[Product]
(
	[MainCode] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_ProductTypeId]    Script Date: 17/12/2018 13:13:03 ******/
CREATE NONCLUSTERED INDEX [IX_ProductTypeId] ON [dbo].[Product]
(
	[ProductTypeId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_IssuerId]    Script Date: 17/12/2018 13:13:03 ******/
CREATE NONCLUSTERED INDEX [IX_IssuerId] ON [dbo].[RequestSession]
(
	[IssuerId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_DocumentDetailId]    Script Date: 17/12/2018 13:13:03 ******/
CREATE NONCLUSTERED INDEX [IX_DocumentDetailId] ON [dbo].[Tax]
(
	[DocumentDetailId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
/****** Object:  Index [IX_DocumentId]    Script Date: 17/12/2018 13:13:03 ******/
CREATE NONCLUSTERED INDEX [IX_DocumentId] ON [dbo].[TotalTax]
(
	[DocumentId] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO
ALTER TABLE [dbo].[AdditionalField]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AdditionalField_dbo.Document_DocumentId] FOREIGN KEY([DocumentId])
REFERENCES [dbo].[Document] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AdditionalField] CHECK CONSTRAINT [FK_dbo.AdditionalField_dbo.Document_DocumentId]
GO
ALTER TABLE [dbo].[Contributor]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Contributor_dbo.ContributorType_ContributorTypeId] FOREIGN KEY([ContributorTypeId])
REFERENCES [dbo].[ContributorType] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Contributor] CHECK CONSTRAINT [FK_dbo.Contributor_dbo.ContributorType_ContributorTypeId]
GO
ALTER TABLE [dbo].[Contributor]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Contributor_dbo.IdentificationType_IdentificationTypeId] FOREIGN KEY([IdentificationTypeId])
REFERENCES [dbo].[IdentificationType] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Contributor] CHECK CONSTRAINT [FK_dbo.Contributor_dbo.IdentificationType_IdentificationTypeId]
GO
ALTER TABLE [dbo].[Contributor]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Contributor_dbo.Issuer_IssuerId] FOREIGN KEY([IssuerId])
REFERENCES [dbo].[Issuer] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Contributor] CHECK CONSTRAINT [FK_dbo.Contributor_dbo.Issuer_IssuerId]
GO
ALTER TABLE [dbo].[DocumentDetail]  WITH CHECK ADD  CONSTRAINT [FK_dbo.DocumentDetail_dbo.Document_DocumentId] FOREIGN KEY([DocumentId])
REFERENCES [dbo].[Document] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DocumentDetail] CHECK CONSTRAINT [FK_dbo.DocumentDetail_dbo.Document_DocumentId]
GO
ALTER TABLE [dbo].[InvoiceInfo]  WITH CHECK ADD  CONSTRAINT [FK_dbo.InvoiceInfo_dbo.Document_InvoiceInfoId] FOREIGN KEY([InvoiceInfoId])
REFERENCES [dbo].[Document] ([Id])
GO
ALTER TABLE [dbo].[InvoiceInfo] CHECK CONSTRAINT [FK_dbo.InvoiceInfo_dbo.Document_InvoiceInfoId]
GO
ALTER TABLE [dbo].[Payment]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Payment_dbo.Document_DocumentId] FOREIGN KEY([DocumentId])
REFERENCES [dbo].[Document] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Payment] CHECK CONSTRAINT [FK_dbo.Payment_dbo.Document_DocumentId]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Product_dbo.ICERate_IceRateId] FOREIGN KEY([IceRateId])
REFERENCES [dbo].[ICERate] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_dbo.Product_dbo.ICERate_IceRateId]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Product_dbo.Issuer_IssuerId] FOREIGN KEY([IssuerId])
REFERENCES [dbo].[Issuer] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_dbo.Product_dbo.Issuer_IssuerId]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Product_dbo.IVARate_IvaRateId] FOREIGN KEY([IvaRateId])
REFERENCES [dbo].[IVARate] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_dbo.Product_dbo.IVARate_IvaRateId]
GO
ALTER TABLE [dbo].[Product]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Product_dbo.ProductType_ProductTypeId] FOREIGN KEY([ProductTypeId])
REFERENCES [dbo].[ProductType] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Product] CHECK CONSTRAINT [FK_dbo.Product_dbo.ProductType_ProductTypeId]
GO
ALTER TABLE [dbo].[RequestSession]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RequestSession_dbo.Issuer_IssuerId] FOREIGN KEY([IssuerId])
REFERENCES [dbo].[Issuer] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RequestSession] CHECK CONSTRAINT [FK_dbo.RequestSession_dbo.Issuer_IssuerId]
GO
ALTER TABLE [dbo].[Tax]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Tax_dbo.DocumentDetail_DocumentDetailId] FOREIGN KEY([DocumentDetailId])
REFERENCES [dbo].[DocumentDetail] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Tax] CHECK CONSTRAINT [FK_dbo.Tax_dbo.DocumentDetail_DocumentDetailId]
GO
ALTER TABLE [dbo].[TotalTax]  WITH CHECK ADD  CONSTRAINT [FK_dbo.TotalTax_dbo.Document_DocumentId] FOREIGN KEY([DocumentId])
REFERENCES [dbo].[Document] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TotalTax] CHECK CONSTRAINT [FK_dbo.TotalTax_dbo.Document_DocumentId]
GO
/****** Object:  StoredProcedure [dbo].[SpGetNextDocumentSequential]    Script Date: 17/12/2018 13:13:03 ******/
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
/****** Object:  StoredProcedure [dbo].[SpSearchContributors]    Script Date: 17/12/2018 13:13:03 ******/
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
	select * from Contributor
	where (Identification+'-'+BussinesName) like '%'+@searchTerm+'%'
	and IssuerId = @issuerId
END;


GO
/****** Object:  StoredProcedure [dbo].[SpSearchProducts]    Script Date: 17/12/2018 13:13:03 ******/
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
	select top(20) * from Product
	where (MainCode+'-'+Name) like '%'+@searchTerm+'%' AND IssuerId = @issuerId
END;

GO