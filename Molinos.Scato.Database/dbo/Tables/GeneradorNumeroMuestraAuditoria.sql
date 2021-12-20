CREATE TABLE [dbo].[GeneradorNumeroMuestraAuditoria] (
    [Id]          INT  IDENTITY (1, 1) NOT NULL,
    [SeqVal]      NVARCHAR (10) NOT NULL,
    CONSTRAINT [PK_dbo.GeneradorNumeroMuestraAuditoria] PRIMARY KEY CLUSTERED ([Id] ASC)
);
