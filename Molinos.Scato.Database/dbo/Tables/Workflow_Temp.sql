CREATE TABLE [dbo].[Workflow_Temp]
(
	[Codigo] NVARCHAR(100) NOT NULL PRIMARY KEY,
	[FechaCreacion]			DATETIME NOT NULL,
	[Comentario]			NVARCHAR (1000) NULL,
    [NombreUsuario]			NVARCHAR (20) NOT NULL,
	[Activa]				BIT NOT NULL CONSTRAINT DF_Workflow_Temp_Activa DEFAULT(0),
	[ActividadInicial]		NVARCHAR (40) NOT NULL,
	[Definicion]			VARBINARY(MAX) NOT NULL,
)
