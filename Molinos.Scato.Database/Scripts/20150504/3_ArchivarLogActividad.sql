DELETE LogActividad
OUTPUT
    DELETED.Id,
    DELETED.WorkflowInstanceId,
    DELETED.Actividad,
    DELETED.Fecha,
    DELETED.ActividadXaml
INTO LogActividadHistorico (Id, WorkflowInstanceId, Actividad, Fecha, ActividadXaml)
WHERE 
	EXISTS (SELECT TOP 1 1 
				FROM Recorrido r 
				WHERE r.InstanciaWorkflow = WorkflowInstanceId AND r.Terminado = 1)
