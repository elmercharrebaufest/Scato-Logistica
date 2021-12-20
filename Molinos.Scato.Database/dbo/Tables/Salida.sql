CREATE TABLE [dbo].[Salida] (
    [Id]                         INT            IDENTITY (1, 1) NOT NULL,
    [Codigo]                NVARCHAR (40)  NOT NULL,
	[Descripcion]                NVARCHAR (40)  NOT NULL,
    CONSTRAINT [PK_dbo.Salida] PRIMARY KEY CLUSTERED ([Id] ASC),
);

GO
