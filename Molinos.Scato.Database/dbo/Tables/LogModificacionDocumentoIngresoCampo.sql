CREATE TABLE [dbo].[LogModificacionDocumentoIngresoCampo]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
	[Nombre] NVARCHAR (50) NOT NULL,
	[ValorOriginal] NVARCHAR (250)  NULL,
	[ValorNuevo] NVARCHAR (250)  NULL,
    [LogModificacionDocumentoIngreso_Id] INT NOT NULL,
	[NombreUsuario]      NVARCHAR (40)  NOT NULL,
	[Fecha]			   DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_dbo.LogModificacionDocumentoIngresoCampo_dbo.LogModificacionDocumentoIngreso_LogModificacionDocumentoIngreso_Id] FOREIGN KEY ([LogModificacionDocumentoIngreso_Id]) REFERENCES [dbo].[LogModificacionDocumentoIngreso] ([Id]),
)
