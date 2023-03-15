CREATE TABLE [dbo].[Almacen] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [Descripcion]      NVARCHAR (30)  NOT NULL,
    [DescripcionCorta] NVARCHAR (8)   NULL,
    [CodigoSAP]        NVARCHAR (20)            NULL,
    [CodigoONCCA]      NVARCHAR (20)            NULL,
    [EsTanqueVino]     BIT            NOT NULL,
	[EsSojaSustentable]     BIT            NOT NULL default 0,
    [Centro_Id]        INT            NULL,
    [EPA]     BIT            NOT NULL default 0,
    CONSTRAINT [PK_dbo.Almacen] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.Almacen_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_Centro_Id]
    ON [dbo].[Almacen]([Centro_Id] ASC);

