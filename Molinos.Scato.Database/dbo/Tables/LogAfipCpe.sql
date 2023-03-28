CREATE TABLE [dbo].[LogAfipCpe] (
    [Id]                     INT           IDENTITY (1, 1) NOT NULL,
    [Servicio]               NVARCHAR (100)    NOT NULL,
    [Consulta]				 NVARCHAR (MAX)    NOT NULL,
	[Respuesta]				 NVARCHAR (MAX)    NOT NULL,
    [Fecha]                  DATETIME          NULL
);
GO
CREATE NONCLUSTERED INDEX ndx_Id_Servicio_Consulta_Respuesta_Fecha ON [dbo].[LogAfipCpe] (
[Id]
) include([Servicio],[Consulta],[Respuesta],[Fecha])
WITH (SORT_IN_TEMPDB = ON, DROP_EXISTING = OFF, ONLINE = OFF, FILLFACTOR = 90)
ON [PRIMARY]
GO