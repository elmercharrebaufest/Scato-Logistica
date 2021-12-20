--Actualiza columna Recorrido_Id de ordenes de carga fas que no tengan duplicados

UPDATE OrdenCargaFas SET Recorrido_Id = (SELECT Id FROM Recorrido WHERE NumeroDocumentoIngreso = NumeroOrden)
WHERE NumeroOrden not in (SELECT NumeroOrden from 
(select PatenteCamion, NumeroOrden , COUNT(*) as Cantidad from OrdenCargaFas
GROUP BY PatenteCamion, NumeroOrden) as repetidos
where repetidos.Cantidad > 1)

--**RECORDAR ACTUALIZAR MANUALMENTE LA COLUMNA RECORRIDO_ID DE LAS ORDENES DE CARGA FAS DUPLICADAS**


--update OrdenCargaFas set recorrido_id = 5371  where id = 21
--update OrdenCargaFas set recorrido_id = 5444  where id = 22
--update OrdenCargaFas set recorrido_id = 5477  where id = 23
--update OrdenCargaFas set recorrido_id = 5504  where id = 24