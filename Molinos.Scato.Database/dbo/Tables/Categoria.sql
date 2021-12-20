CREATE TABLE [dbo].[Categoria](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Clasificacion] NVARCHAR(18) NOT NULL,
 CONSTRAINT [PK_Categoria] PRIMARY KEY CLUSTERED ([Id] ASC)
);