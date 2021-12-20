-- Este script requiere tener los workflows actualizados en la Scato_Soporte del SQLEXPRESS local
-- Además hay que tener agregado como linked server el servidor de SCATO QA: vicsqltest

DELETE FROM [VICSQLTEST].[SCATO].[dbo].[Workflow_Temp]
GO

INSERT INTO [VICSQLTEST].[SCATO].[dbo].[Workflow_Temp]
           ([Codigo]
           ,[FechaCreacion]
           ,[Comentario]
           ,[NombreUsuario]
           ,[Activa]
           ,[ActividadInicial]
           ,[Definicion])
SELECT [Codigo]
      ,[FechaCreacion]
      ,[Comentario]
      ,[NombreUsuario]
      ,[Activa]
      ,[ActividadInicial]
      ,[Definicion]
  FROM [Scato_Soporte].[dbo].[Workflow_Temp]
GO

INSERT INTO  [VICSQLTEST].[Scato].[dbo].[WorkflowDefinicion] (Workflow_Id, FechaCreacion, Comentario, NombreUsuario, Activa, ActividadInicial, Definicion)
SELECT w.Id, wt.FechaCreacion, wt.Comentario, wt.NombreUsuario, wt.Activa, wt.ActividadInicial, wt.Definicion
FROM [VICSQLTEST].[Scato].[dbo].[Workflow] w INNER JOIN [VICSQLTEST].[Scato].[dbo].[Workflow_Temp] wt 
	ON w.Codigo like wt.Codigo + '%' and LEN(w.Codigo) <= LEN(wt.Codigo)+1
GO