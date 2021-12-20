CREATE TABLE [dbo].[HistoricoInhabilitacionCamion] (
    [Id]                       INT            IDENTITY (1, 1) NOT NULL,
    [Fecha]               DATETIME       NOT NULL,
    [Comentario]                   NVARCHAR (1000) NOT NULL,	
    [InhabilitacionCamion_Id]                INT            NOT NULL,
    [NombreUsuarioResponsable] NVARCHAR (30) NOT NULL,
	[NombreUsuarioCambio] NVARCHAR (30) NOT NULL , 
	 [FechaDesde]               DATETIME       NOT NULL,
    [FechaHasta]               DATETIME       NOT NULL,
    [Motivo]                   NVARCHAR (1000) NOT NULL,
    CONSTRAINT [PK_dbo.HistoricoInhabilitacionCamion] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.HistoricoInhabilitacionCamion_dbo.InhabilitacionCamion_InhabilitacionCamion_Id] FOREIGN KEY ([InhabilitacionCamion_Id]) REFERENCES [dbo].[InhabilitacionCamion] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_InhabilitacionCamion_Id]
    ON [dbo].[HistoricoInhabilitacionCamion]([InhabilitacionCamion_Id] ASC);

