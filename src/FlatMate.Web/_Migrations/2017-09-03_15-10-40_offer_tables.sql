--
-- Script
--

CREATE TABLE [Infrastructure].[Image](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Guid] [uniqueidentifier] NOT NULL,
    [ContentType] [varchar](50) NOT NULL,
	[File] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_Image] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE SCHEMA [Offers]
GO

-- Company

CREATE TABLE [Offers].[Company](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[ImageGuid] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


-- Market
CREATE TABLE [Offers].[Market](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[City] [varchar](100) NULL,
	[CompanyId] [int] NOT NULL,
	[ExternalId] [varchar](255) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[PostalCode] [varchar](10) NULL,
	[Street] [varchar](255) NULL,
 CONSTRAINT [PK_Market] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [Offers].[Market]  WITH CHECK ADD  CONSTRAINT [FK_Market_Company] FOREIGN KEY([CompanyId])
REFERENCES [Offers].[Company] ([Id])
GO

ALTER TABLE [Offers].[Market] CHECK CONSTRAINT [FK_Market_Company]
GO

-- Product
CREATE TABLE [Offers].[Product](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Brand] [varchar](255) NOT NULL,
    [Description] [text] NULL,
	[ExternalId] [varchar](255) NOT NULL,
	[ImageUrl] [text] NULL,
	[MarketId] [int] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Price] [decimal](7, 2) NOT NULL,
	[SizeInfo] [varchar](255) NULL,
 CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [Offers].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_Market] FOREIGN KEY([MarketId])
REFERENCES [Offers].[Market] ([Id])
GO

ALTER TABLE [Offers].[Product] CHECK CONSTRAINT [FK_Product_Market]
GO

-- Offer
CREATE TABLE [Offers].[Offer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExternalId] [varchar](255) NOT NULL,
	[From] [date] NOT NULL,
	[ImageUrl] [text] NULL,
	[MarketId] [int] NOT NULL,
	[Price] [decimal](7, 2) NOT NULL,
	[ProductId] [int] NOT NULL,
	[To] [date] NOT NULL,
 CONSTRAINT [PK_Offer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [Offers].[Offer]  WITH CHECK ADD  CONSTRAINT [FK_Offer_Market] FOREIGN KEY([MarketId])
REFERENCES [Offers].[Market] ([Id])
GO

ALTER TABLE [Offers].[Offer] CHECK CONSTRAINT [FK_Offer_Market]
GO

ALTER TABLE [Offers].[Offer]  WITH CHECK ADD  CONSTRAINT [FK_Offer_Product] FOREIGN KEY([ProductId])
REFERENCES [Offers].[Product] ([Id])
GO

ALTER TABLE [Offers].[Offer] CHECK CONSTRAINT [FK_Offer_Product]
GO

-- PriceHistory
CREATE TABLE [Offers].[PriceHistory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Date] [date] NOT NULL,
	[Price] [decimal](7, 2) NOT NULL,
	[ProductId] [int] NOT NULL,
 CONSTRAINT [PK_PriceHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [Offers].[PriceHistory]  WITH CHECK ADD  CONSTRAINT [FK_PriceHistory_Product] FOREIGN KEY([ProductId])
REFERENCES [Offers].[Product] ([Id])
GO

ALTER TABLE [Offers].[PriceHistory] CHECK CONSTRAINT [FK_PriceHistory_Product]
GO

--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName]) 
VALUES ('2017-09-03_15-10-40_offer_tables');