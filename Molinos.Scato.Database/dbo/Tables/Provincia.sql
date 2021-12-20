CREATE TABLE [dbo].[Provincia] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
	[CodigoAfip] INT            NOT NULL,
    [Descripcion] NVARCHAR (50) COLLATE SQL_Latin1_General_CP1_CI_AI NOT NULL,
    [DescripcionCollate_CI_AS] NVARCHAR (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL default '',
	[Pais_Id] INT            NOT NULL,
    CONSTRAINT [PK_dbo.Provincia] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.Provincia_dbo.Pais_Pais_Id] FOREIGN KEY ([Pais_Id]) REFERENCES [dbo].[Pais] ([Id])
);

GO
CREATE NONCLUSTERED INDEX [IX_Pais_Id]
    ON [dbo].[Provincia]([Pais_Id] ASC);
