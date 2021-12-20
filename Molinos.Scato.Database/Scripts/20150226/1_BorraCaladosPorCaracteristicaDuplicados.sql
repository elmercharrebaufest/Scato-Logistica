DELETE
FROM CaladoPorCaracteristica
WHERE Id NOT IN
    (SELECT MIN(Id)
     FROM CaladoPorCaracteristica AS c
     WHERE EXISTS
         ( SELECT 1
          FROM CaladoPorCaracteristica AS cc
          WHERE c.Calado_Id = cc.Calado_Id
            AND c.CaracteristicaDeCalidad_Id = cc.CaracteristicaDeCalidad_Id
          GROUP BY Calado_Id,
                   CaracteristicaDeCalidad_Id
          HAVING COUNT(*) > 1)
     GROUP BY CaracteristicaDeCalidad_Id,
              Calado_Id)
  AND Id IN
    (SELECT Id
     FROM CaladoPorCaracteristica AS c
     WHERE EXISTS
         ( SELECT 1
          FROM CaladoPorCaracteristica AS cc
          WHERE c.Calado_Id = cc.Calado_Id
            AND c.CaracteristicaDeCalidad_Id = cc.CaracteristicaDeCalidad_Id
          GROUP BY Calado_Id,
                   CaracteristicaDeCalidad_Id
          HAVING COUNT(*) > 1));