CREATE TABLE [dbo].[AutorizacionCamion] (
    [Id]                       INT             IDENTITY (1, 1) NOT NULL,
    [Fecha]                    DATETIME        NOT NULL,
    [Comentario]               NVARCHAR (1000) NOT NULL,
    [InhabilitacionCamion_Id]  INT             NOT NULL,
    [NombreUsuarioResponsable] NVARCHAR (30)   NOT NULL,
    CONSTRAINT [PK_dbo.AutorizacionCamion] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (FILLFACTOR = 90, STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.AutorizacionCamion_dbo.InhabilitacionChofer_InhabilitacionChofer_Id] FOREIGN KEY ([InhabilitacionCamion_Id]) REFERENCES [dbo].[InhabilitacionCamion] ([Id])
);






GO
CREATE NONCLUSTERED INDEX [IX_InhabilitacionCamion_Id]
    ON [dbo].[AutorizacionCamion]([InhabilitacionCamion_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



