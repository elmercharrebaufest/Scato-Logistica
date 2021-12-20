CREATE TABLE [dbo].[DescargaDeBines] (
    [Id]                              INT             IDENTITY (1, 1) NOT NULL,
    [CantidadBines]					 INT             NOT NULL,
    [Cuartel_Id]                       INT             NULL,
    [Tipo_Id]                       INT             NOT NULL,
    [RemitoBodegaUva_Id]                       INT             NOT NULL,
    CONSTRAINT [PK_dbo.DescargaDeBines] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.DescargaDeBines_dbo.Cuartel_Cuartel_Id] FOREIGN KEY ([Cuartel_Id]) REFERENCES [dbo].[Cuartel] ([Id]),
    CONSTRAINT [FK_dbo.DescargaDeBines_dbo.Tipo_Tipo_Id] FOREIGN KEY ([Tipo_Id]) REFERENCES [dbo].[Material] ([Id]),
    CONSTRAINT [FK_dbo.DescargaDeBines_dbo.RemitoBodegaUva_RemitoBodegaUva_Id] FOREIGN KEY ([RemitoBodegaUva_Id]) REFERENCES [dbo].[RemitoBodegaUva] ([Id]) ON DELETE CASCADE,
);


GO
CREATE NONCLUSTERED INDEX [IX_Cuartel_Id]
    ON [dbo].[DescargaDeBines]([Cuartel_Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_Tipo_Id]
    ON [dbo].[DescargaDeBines]([Tipo_Id] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_Calado_Id]
    ON [dbo].[DescargaDeBines]([RemitoBodegaUva_Id] ASC);