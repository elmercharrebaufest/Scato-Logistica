CREATE TABLE [dbo].[InhabilitacionChofer] (
    [Id]                       INT            IDENTITY (1, 1) NOT NULL,
    [FechaDesde]               DATETIME       NOT NULL,
    [FechaHasta]               DATETIME       NOT NULL,
    [Motivo]                   NVARCHAR (1000) NOT NULL,
    [NombreUsuarioResponsable] NVARCHAR (30) NOT NULL,
    [Chofer_Id]                INT            NOT NULL,
    [Centro_Id]                INT            NOT NULL,
    CONSTRAINT [PK_dbo.InhabilitacionChofer] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_dbo.InhabilitacionChofer_dbo.Centro_Centro_Id] FOREIGN KEY ([Centro_Id]) REFERENCES [dbo].[Centro] ([Id]),
    CONSTRAINT [FK_dbo.InhabilitacionChofer_dbo.Chofer_Chofer_Id] FOREIGN KEY ([Chofer_Id]) REFERENCES [dbo].[Chofer] ([Id])
);




GO
CREATE NONCLUSTERED INDEX [IX_Chofer_Id]
    ON [dbo].[InhabilitacionChofer]([Chofer_Id] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Centro_Id]
    ON [dbo].[InhabilitacionChofer]([Centro_Id] ASC);

