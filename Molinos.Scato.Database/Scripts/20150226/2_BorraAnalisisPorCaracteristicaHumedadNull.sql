DELETE
FROM AnalisisPorCaracteristica
WHERE AnalisisPorCaracteristica.ValorAnalisis IS NULL
  AND EXISTS
    ( SELECT 1
     FROM CaracteristicaDeCalidad AS cc
     WHERE cc.Id = AnalisisPorCaracteristica.CaracteristicaDeCalidad_Id
       AND EsHumedad = 1)