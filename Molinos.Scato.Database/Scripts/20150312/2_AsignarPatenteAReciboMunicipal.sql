--Le asigna una patente a las impresiones que no la tengan (ImpresionReciboMunicipal)
UPDATE [Impresion]
   SET [Patente] =  (select R.Patente FROM Recorrido as R where R.InstanciaWorkflow = Impresion.WorkflowId)
 WHERE Patente is null
GO