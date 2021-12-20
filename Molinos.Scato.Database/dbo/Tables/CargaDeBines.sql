CREATE TABLE [dbo].[CargaDeBines] (
    [Id]                              INT             IDENTITY (1, 1) NOT NULL,
    [CantidadBines]					 INT             NOT NULL,
    [Tipo_Id]                       INT             NOT NULL,
    [RemitoBodegaUva_Id]                       INT             NOT NULL,
    CONSTRAINT [PK_dbo.CargaDeBines] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.CargaDeBines_dbo.Tipo_Tipo_Id] FOREIGN KEY ([Tipo_Id]) REFERENCES [dbo].[Material] ([Id]),
    CONSTRAINT [FK_dbo.CargaDeBines_dbo.RemitoBodegaUva_RemitoBodegaUva_Id] FOREIGN KEY ([RemitoBodegaUva_Id]) REFERENCES [dbo].[RemitoBodegaUva] ([Id]) ON DELETE CASCADE,
);


GO
CREATE NONCLUSTERED INDEX [IX_Tipo_Id]
    ON [dbo].[CargaDeBines]([Tipo_Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_Calado_Id]
    ON [dbo].[CargaDeBines]([RemitoBodegaUva_Id] ASC);