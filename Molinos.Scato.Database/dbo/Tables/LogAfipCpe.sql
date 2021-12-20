CREATE TABLE [dbo].[LogAfipCpe] (
    [Id]                     INT           IDENTITY (1, 1) NOT NULL,
    [Servicio]               NVARCHAR (100)    NOT NULL,
    [Consulta]				 NVARCHAR (MAX)    NOT NULL,
	[Respuesta]				 NVARCHAR (MAX)    NOT NULL,
    [Fecha]                  DATETIME          NULL
);