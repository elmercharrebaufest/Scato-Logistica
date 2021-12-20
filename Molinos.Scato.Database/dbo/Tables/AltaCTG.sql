CREATE TABLE [dbo].[AltaCTG] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
	[CodigoCTG]      NVARCHAR (15) NOT NULL,
	[Fecha]       DATETIME NOT NULL,
	[CartaPorte_Id]          INT            NULL,
    [WorkflowId]   UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_dbo.AltaCTG] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_dbo.AltaCTG_dbo.CartaPorte_CartaPorte_Id] FOREIGN KEY ([CartaPorte_Id]) REFERENCES [dbo].[CartaPorte] ([Id]) ON DELETE CASCADE
);

GO
CREATE NONCLUSTERED INDEX [IX_CartaPorte_Id]
    ON [dbo].[AltaCTG]([CartaPorte_Id] ASC);