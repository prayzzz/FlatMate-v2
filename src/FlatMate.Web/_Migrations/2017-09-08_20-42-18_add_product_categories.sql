--
-- Script
--


-- create product categories
CREATE TABLE [Offers].[ProductCategory](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[SortWeight] [int] NOT NULL,
 CONSTRAINT [PK_ProductCategory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [Offers].[ProductCategory] ADD  CONSTRAINT [DF_ProductCategory_SortWeight]  DEFAULT ((0)) FOR [SortWeight]
GO


SET IDENTITY_INSERT [Offers].[ProductCategory] ON
GO

INSERT INTO [Offers].[ProductCategory] ([Id], [Name], [SortWeight]) 
			VALUES (1, 'Sonstiges', 0),
				   (2, 'Obst & Gemüse', 1000),
				   (3, 'Frische & Convenience', 990),
				   (4, 'Kühlung', 980),
				   (5, 'Tiefkühl', 970),
				   (6, 'Frühstück', 960),
				   (7, 'Kochen & Backen', 950),
				   (8, 'Süßigkeiten', 940),
				   (9, 'Getränke', 930),
				   (10, 'Baby & Kind', 920),
				   (11, 'Haushalt', 910),
				   (12, 'Drogerie', 900)
GO

SET IDENTITY_INSERT [Offers].[ProductCategory] OFF
GO


-- add product categories to product
ALTER TABLE [Offers].[Product] ADD [ProductCategoryId] [int] NULL, 
								   [ExternalProductCategoryId] [varchar](255) NULL,
								   [ExternalProductCategory] [varchar](255) NULL  
GO

ALTER TABLE [Offers].[Product]   WITH CHECK ADD  CONSTRAINT [FK_Product_ProductCategory] FOREIGN KEY([ProductCategoryId])
REFERENCES [Offers].[ProductCategory] ([Id])
GO

ALTER TABLE [Offers].[Product] CHECK CONSTRAINT [FK_Product_ProductCategory]
GO

-- set default category for existing products
UPDATE [Offers].[Product] SET [ProductCategoryId] = 1
GO

-- category to not null
ALTER TABLE [Offers].[Product] ALTER COLUMN [ProductCategoryId] [int] NOT NULL
GO


--
-- Migration
--


INSERT INTO [Infrastructure].[Migrations] ([FileName]) 
VALUES ('2017-09-08_20-42-18_add_product_categories');