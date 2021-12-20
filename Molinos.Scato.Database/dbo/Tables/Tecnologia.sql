CREATE TABLE [dbo].[Tecnologia] (
    [Id]                              INT             IDENTITY (1, 1) NOT NULL,
    [Empresa_Id]      INT             NOT NULL,
    [Nombre]                          NVARCHAR (30)   NOT NULL,
	[Codigo]                          NVARCHAR (30)   NOT NULL,
    CONSTRAINT [PK_dbo.Tecnologia] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Tecnologia_dbo.Empresa_Empresa_Id] FOREIGN KEY ([Empresa_Id]) REFERENCES [dbo].[Empresa] ([Id]),
);


GO
CREATE NONCLUSTERED INDEX [IX_Empresa_Id]
    ON [dbo].[Tecnologia]([Empresa_Id] ASC);
