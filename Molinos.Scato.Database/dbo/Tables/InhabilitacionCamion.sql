CREATE TABLE [dbo].[InhabilitacionCamion] (
    [Id]                       INT             IDENTITY (1, 1) NOT NULL,
    [Patente]                  NVARCHAR (10)   NOT NULL,
    [FechaDesde]               DATETIME        NOT NULL,
    [FechaHasta]               DATETIME        NOT NULL,
    [Motivo]                   NVARCHAR (1000) NOT NULL,
    [NombreUsuarioResponsable] NVARCHAR (30)   NOT NULL,
    [Centro_Id]                INT             NOT NULL,
    CONSTRAINT [PK_dbo.InhabilitacionCamion] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.InhabilitacionCamion_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id])
);






GO
CREATE NONCLUSTERED INDEX [IX_Centro_Id]
    ON [dbo].[InhabilitacionCamion]([Centro_Id] ASC) WITH (FILLFACTOR = 90, PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON);



