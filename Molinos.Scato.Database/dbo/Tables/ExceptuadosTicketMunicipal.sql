CREATE TABLE [dbo].[ExceptuadosTicketMunicipal] 
(
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [Patente] NVARCHAR(50) NOT NULL, 
    [NombreUsuario] NVARCHAR(100) NULL, 
    [FechaCreacionExcepcion] DATETIME NOT NULL, 
    [WorkflowInstanceId] UNIQUEIDENTIFIER NULL,
    
    CONSTRAINT [PK_ExceptuadosTicketMunicipal] PRIMARY KEY CLUSTERED ([Id])
)
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_ExceptuadosTicketMunicipal_Patente_WorkflowInstanceId]
    ON [dbo].[ExceptuadosTicketMunicipal]([Patente], [WorkflowInstanceId])
    WHERE [WorkflowInstanceId] IS NOT NULL;
GO