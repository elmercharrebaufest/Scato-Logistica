--Actualiza columna Recorrido_Id en los documentos de ingreso

UPDATE OrdenDeDescarga SET Recorrido_Id = (SELECT Id FROM Recorrido WHERE NumeroDocumentoIngreso = Numero AND TipoDocumentoIngreso = 2 AND Patente = PatenteCamion)
UPDATE OrdenDeDescargaFason SET Recorrido_Id = (SELECT Id FROM Recorrido WHERE NumeroDocumentoIngreso = Numero AND TipoDocumentoIngreso = 4)
UPDATE Remito SET Recorrido_Id = (SELECT Id FROM Recorrido WHERE NumeroDocumentoIngreso = OrdenDeDescarga AND TipoDocumentoIngreso = 7)

