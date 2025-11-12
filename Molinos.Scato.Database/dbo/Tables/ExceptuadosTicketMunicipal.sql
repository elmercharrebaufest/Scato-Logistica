CREATE TABLE [dbo].[ExceptuadosTicketMunicipal]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
	[Patente] NVARCHAR(50) NOT NULL, 
	[NombreUsuario]			 NVARCHAR (100)   NULL, 
    [NumeroDocumentoIngreso] NVARCHAR(100) NOT NULL, 
	[WorkflowCodigo] NVARCHAR(100) NOT NULL,
	[WorkflowDescripcion] NVARCHAR(100) NOT NULL,
	[FechaCreacionExcepcion] DATETIME NOT NULL, 
	[Activo] BIT NOT NULL DEFAULT 1, 
    [PermiteAcciones] BIT NOT NULL DEFAULT 1
   
)
