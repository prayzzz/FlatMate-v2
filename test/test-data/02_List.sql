USE [flatmate]
GO

---------------
-- ItemLists --
SET IDENTITY_INSERT [List].[ItemList] ON 
GO

-- User1
INSERT [List].[ItemList] ([Id], [Created], [Description], [IsPublic], [LastEditorId], [Modified], [Name], [OwnerId])
VALUES (1, CAST(N'2017-01-01T00:00:00.000' AS DateTime), N'User1, Private', 0, 1, CAST(N'2017-01-01T00:00:00.000' AS DateTime), N'List01', 1)

INSERT [List].[ItemList] ([Id], [Created], [Description], [IsPublic], [LastEditorId], [Modified], [Name], [OwnerId])
VALUES (2, CAST(N'2017-01-01T00:00:00.000' AS DateTime), N'User1, Public', 1, 1, CAST(N'2017-01-01T00:00:00.000' AS DateTime), N'List02', 1)
GO

-- User2
INSERT [List].[ItemList] ([Id], [Created], [Description], [IsPublic], [LastEditorId], [Modified], [Name], [OwnerId])
VALUES (3, CAST(N'2017-01-01T00:00:00.000' AS DateTime), N'User2, Private', 0, 2, CAST(N'2017-01-01T00:00:00.000' AS DateTime), N'List03', 2)

INSERT [List].[ItemList] ([Id], [Created], [Description], [IsPublic], [LastEditorId], [Modified], [Name], [OwnerId])
VALUES (4, CAST(N'2017-01-01T00:00:00.000' AS DateTime), N'User2, Public', 1, 2, CAST(N'2017-01-01T00:00:00.000' AS DateTime), N'List04', 2)
GO

SET IDENTITY_INSERT [List].[ItemList] OFF
GO

------------
-- Groups --
SET IDENTITY_INSERT [List].[ItemGroup] ON 
GO

-- User1
INSERT [List].[ItemGroup] ([Id], [Created], [ItemListId], [LastEditorId], [Modified], [Name], [OwnerId], [SortIndex])
VALUES (1, CAST(N'2017-01-01T00:00:00.000' AS DateTime), 1, 1, CAST(N'2017-01-01T00:00:00.000' AS DateTime), N'Group01', 1, 0)

INSERT [List].[ItemGroup] ([Id], [Created], [ItemListId], [LastEditorId], [Modified], [Name], [OwnerId], [SortIndex])
VALUES (2, CAST(N'2017-01-01T00:00:00.000' AS DateTime), 2, 1, CAST(N'2017-01-01T00:00:00.000' AS DateTime), N'Group02', 1, 1)
GO

-- User2
INSERT [List].[ItemGroup] ([Id], [Created], [ItemListId], [LastEditorId], [Modified], [Name], [OwnerId], [SortIndex])
VALUES (3, CAST(N'2017-01-01T00:00:00.000' AS DateTime), 3, 3, CAST(N'2017-01-01T00:00:00.000' AS DateTime), N'Group03', 2, 0)

INSERT [List].[ItemGroup] ([Id], [Created], [ItemListId], [LastEditorId], [Modified], [Name], [OwnerId], [SortIndex])
VALUES (4, CAST(N'2017-01-01T00:00:00.000' AS DateTime), 4, 3, CAST(N'2017-01-01T00:00:00.000' AS DateTime), N'Group04', 2, 1)
GO

SET IDENTITY_INSERT [List].[ItemGroup] OFF
GO

-----------
-- Items --
SET IDENTITY_INSERT [List].[Item] ON 
GO

-- User1
INSERT [List].[Item] ([Id], [Created], [ItemListId], [ItemGroupId], [LastEditorId], [Modified], [Name], [OwnerId], [SortIndex])
VALUES (1, CAST(N'2017-01-01T00:00:00.000' AS DateTime), 1, 1, 1, CAST(N'2017-01-01T00:00:00.000' AS DateTime), N'Item01', 1, 0)

INSERT [List].[Item] ([Id], [Created], [ItemListId], [ItemGroupId], [LastEditorId], [Modified], [Name], [OwnerId], [SortIndex])
VALUES (2, CAST(N'2017-01-01T00:00:00.000' AS DateTime), 2, 2, 1, CAST(N'2017-01-01T00:00:00.000' AS DateTime), N'Item02', 1, 1)
GO

-- User2
INSERT [List].[Item] ([Id], [Created], [ItemListId], [ItemGroupId], [LastEditorId], [Modified], [Name], [OwnerId], [SortIndex])
VALUES (3, CAST(N'2017-01-01T00:00:00.000' AS DateTime), 1, 3, 1, CAST(N'2017-01-01T00:00:00.000' AS DateTime), N'Item01', 2, 0)

INSERT [List].[Item] ([Id], [Created], [ItemListId], [ItemGroupId], [LastEditorId], [Modified], [Name], [OwnerId], [SortIndex])
VALUES (4, CAST(N'2017-01-01T00:00:00.000' AS DateTime), 2, 4, 1, CAST(N'2017-01-01T00:00:00.000' AS DateTime), N'Item02', 2, 1)
GO

SET IDENTITY_INSERT [List].[Item] OFF
GO
