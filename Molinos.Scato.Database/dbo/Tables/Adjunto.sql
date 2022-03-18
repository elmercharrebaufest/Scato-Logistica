CREATE TABLE [dbo].[Adjunto] (
    [Id]                      INT            IDENTITY (1, 1) NOT NULL,
    [Archivo]                 NVARCHAR (MAX) NOT NULL,
    [Descripcion]             NVARCHAR (100) NULL,
    [InhabilitacionCamion_Id] INT            NULL,
    [InhabilitacionChofer_Id] INT            NULL,
    CONSTRAINT [PK_dbo.Adjunto] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.Adjunto_dbo.InhabilitacionCamion_InhabilitacionCamion_Id] FOREIGN KEY ([InhabilitacionCamion_Id]) REFERENCES [dbo].[InhabilitacionCamion] ([Id]),
    CONSTRAINT [FK_dbo.Adjunto_dbo.InhabilitacionChofer_InhabilitacionChofer_Id] FOREIGN KEY ([InhabilitacionChofer_Id]) REFERENCES [dbo].[InhabilitacionChofer] ([Id])
);




