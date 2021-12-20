CREATE TABLE [dbo].[HistoricoInhabilitacionChofer] (
    [Id]                       INT            IDENTITY (1, 1) NOT NULL,
    [Fecha]               DATETIME       NOT NULL,
    [Comentario]                   NVARCHAR (1000) NOT NULL,	
    [InhabilitacionChofer_Id]                INT            NOT NULL,
    [NombreUsuarioResponsable] NVARCHAR (30) NOT NULL,
	[NombreUsuarioCambio] NVARCHAR (30) NOT NULL , 
	 [FechaDesde]               DATETIME       NOT NULL,
    [FechaHasta]               DATETIME       NOT NULL,
    [Motivo]                   NVARCHAR (1000) NOT NULL,
    CONSTRAINT [PK_dbo.HistoricoInhabilitacionChofer] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.HistoricoInhabilitacionChofer_dbo.InhabilitacionChofer_InhabilitacionChofer_Id] FOREIGN KEY ([InhabilitacionChofer_Id]) REFERENCES [dbo].[InhabilitacionChofer] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_InhabilitacionChofer_Id]
    ON [dbo].[HistoricoInhabilitacionChofer]([InhabilitacionChofer_Id] ASC);

