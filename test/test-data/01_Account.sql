USE [flatmate]
GO

SET IDENTITY_INSERT [Account].[User] ON 
GO

-- Password: 12345678
INSERT [Account].[User] ([Id], [Created], [Email], [PasswordHash], [Salt], [UserName])
VALUES (1, CAST(N'2017-01-01T00:00:00.000' AS DateTime), N'user1@russianbee.de', N'nmiVz6ju+do07UxMxjkYuiAj4s8CA0OB0AJhQulGBl6PG2xcxbvKbTdE/4C4uvv6upAORT/dJuf6ySEARSVsxg==', N'E1Uzdr7JKZ96JrOItvWFzg==', N'user1')
GO

-- Password: 12345678
INSERT [Account].[User] ([Id], [Created], [Email], [PasswordHash], [Salt], [UserName])
VALUES (2, CAST(N'2017-01-01T00:00:00.000' AS DateTime), N'user2@russianbee.de', N'nmiVz6ju+do07UxMxjkYuiAj4s8CA0OB0AJhQulGBl6PG2xcxbvKbTdE/4C4uvv6upAORT/dJuf6ySEARSVsxg==', N'E1Uzdr7JKZ96JrOItvWFzg==', N'user2')
GO

-- Password: 12345678
INSERT [Account].[User] ([Id], [Created], [Email], [PasswordHash], [Salt], [UserName])
VALUES (3, CAST(N'2017-01-01T00:00:00.000' AS DateTime), N'user3@russianbee.de', N'nmiVz6ju+do07UxMxjkYuiAj4s8CA0OB0AJhQulGBl6PG2xcxbvKbTdE/4C4uvv6upAORT/dJuf6ySEARSVsxg==', N'E1Uzdr7JKZ96JrOItvWFzg==', N'user3')
GO

SET IDENTITY_INSERT [Account].[User] OFF
GO
