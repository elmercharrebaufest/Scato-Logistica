CREATE TABLE [dbo].[AutorizacionChofer] (
    [Id]                       INT             IDENTITY (1, 1) NOT NULL,
    [Fecha]                    DATETIME        NOT NULL,
    [Comentario]               NVARCHAR (1000) NOT NULL,
    [InhabilitacionChofer_Id]  INT             NOT NULL,
    [NombreUsuarioResponsable] NVARCHAR (30)   NOT NULL,
    CONSTRAINT [PK_dbo.AutorizacionChofer] PRIMARY KEY CLUSTERED ([Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON),
    CONSTRAINT [FK_dbo.AutorizacionChofer_dbo.InhabilitacionChofer_InhabilitacionChofer_Id] FOREIGN KEY ([InhabilitacionChofer_Id]) REFERENCES [dbo].[InhabilitacionChofer] ([Id])
);






GO
CREATE NONCLUSTERED INDEX [IX_InhabilitacionChofer_Id]
    ON [dbo].[AutorizacionChofer]([InhabilitacionChofer_Id] ASC) WITH (STATISTICS_NORECOMPUTE = ON);



