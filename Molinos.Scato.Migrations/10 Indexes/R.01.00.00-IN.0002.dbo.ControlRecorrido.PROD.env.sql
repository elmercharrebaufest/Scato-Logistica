CREATE INDEX [IX_ControlRecorrido_WorkflowInstanceId_ActividadXaml] ON [dbo].[ControlRecorrido] ([WorkflowInstanceId],ActividadXaml, puestodetrabajo_id)
GO
CREATE NONCLUSTERED INDEX IX_ActividadXaml_PuestoDeTrabajo_id ON ControlRecorrido (
                ActividadXaml,
                PuestoDeTrabajo_id
) INCLUDE ([WorkflowInstanceId])

WITH (PAD_INDEX = ON, STATISTICS_NORECOMPUTE = ON, SORT_IN_TEMPDB = ON, DROP_EXISTING = OFF, ONLINE = ON, ALLOW_ROW_LOCKS = ON,
ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF)
ON [PRIMARY]
GO