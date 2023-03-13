CREATE TABLE [dbo].[BajaCTG] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
	[CodigoDeBaja]      NVARCHAR (400) NULL,
	[CodigoDeBajaDefinitivo]      NVARCHAR (400) NULL,
	[Fecha]       DATETIME NOT NULL,
	[CartaPorte_Id]          INT            NULL,
    [WorkflowId]   UNIQUEIDENTIFIER NULL,
    [OrdenDeDescargaFason_Id] INT NULL, 
    CONSTRAINT [PK_dbo.BajaCTG] PRIMARY KEY CLUSTERED ([Id] ASC),                                                                                  
	CONSTRAINT [FK_dbo.BajaCTG_dbo.CartaPorte_CartaPorte_Id] FOREIGN KEY ([CartaPorte_Id]) REFERENCES [dbo].[CartaPorte] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_dbo.BajaCTG_dbo.OrdenDeDescargaFason_OrdenDeDescargaFason_Id] FOREIGN KEY ([OrdenDeDescargaFason_Id]) REFERENCES [dbo].[OrdenDeDescargaFason] ([Id]) ON DELETE CASCADE
);

GO
CREATE NONCLUSTERED INDEX [IX_CartaPorte_Id]
    ON [dbo].[BajaCTG]([CartaPorte_Id] ASC);
GO
CREATE NONCLUSTERED INDEX [IX_WorkflowId] 
     ON [dbo].[BajaCTG]([WorkflowId] ASC);