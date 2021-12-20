CREATE TABLE [dbo].[TransaccionSAP] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [DescripcionCorta] NVARCHAR (8)   NULL,
    [CentroOrigen_Id]  INT            NULL,
    [Material_Id]	   INT            NOT NULL,
	[TipoComercial_Id] INT            NOT NULL,
    [FuncionSAP]	   INT            NOT NULL,
    CONSTRAINT [PK_dbo.TransaccionSAP] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.TransaccionSAP_dbo.Centro_CentroOrigen_Id] FOREIGN KEY ([CentroOrigen_Id]) REFERENCES [dbo].[Centro] ([Id]),
    CONSTRAINT [FK_dbo.TransaccionSAP_dbo.Material_Material_Id] FOREIGN KEY ([Material_Id]) REFERENCES [dbo].[Material] ([Id]),
    CONSTRAINT [FK_dbo.TransaccionSAP_dbo.TipoComercial_TipoComercial_Id] FOREIGN KEY ([TipoComercial_Id]) REFERENCES [dbo].[TipoComercial] ([Id])
);


GO
CREATE NONCLUSTERED INDEX [IX_CentroOrigen_Id]
    ON [dbo].[TransaccionSAP]([CentroOrigen_Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_Material_Id]
    ON [dbo].[TransaccionSAP]([Material_Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_TipoComercial_Id]
    ON [dbo].[TransaccionSAP]([TipoComercial_Id] ASC);

