CREATE VIEW dbo.LogActividadUnion
AS
SELECT Id, WorkflowInstanceId, Actividad, Fecha, ActividadXaml
FROM dbo.LogActividadHistorico
UNION ALL
SELECT Id, WorkflowInstanceId, Actividad, Fecha, ActividadXaml
FROM dbo.LogActividad

