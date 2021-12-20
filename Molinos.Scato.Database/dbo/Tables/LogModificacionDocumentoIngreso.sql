CREATE TABLE [dbo].[LogModificacionDocumentoIngreso]
(
	[Id] INT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
	[Numero] VARCHAR(20) NOT NULL,
    [TipoDocumentoIngreso]   INT              NOT NULL,
	[NombreUsuarioUltimaModificacion]      NVARCHAR (40)  NOT NULL,
	[FechaUltimaModificacion]			   DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT [FK_dbo.LogModificacionDocumentoIngreso_dbo.TipoDocumentoIngreso_TipoDocumentoIngreso_Id] FOREIGN KEY ([TipoDocumentoIngreso]) REFERENCES [dbo].[TipoDocumentoIngreso] ([Id])
)
