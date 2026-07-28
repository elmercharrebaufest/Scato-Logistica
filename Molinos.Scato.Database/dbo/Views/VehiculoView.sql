
CREATE VIEW [dbo].[VehiculoView]
AS
    SELECT NULLIF(LTRIM(RTRIM(v.Patente)), '')          AS Patente1,
           NULLIF(LTRIM(RTRIM(v.PatenteAcoplado)), '')  AS Patente2,
           NULLIF(LTRIM(RTRIM(v.PatenteAcoplado2)), '') AS Patente3,
           r.Id                                         AS Recorrido_Id
	  FROM Vehiculo v
	 INNER JOIN Recorrido r ON r.Vehiculo_Id = v.Id
	
	UNION 

	SELECT NULLIF(LTRIM(RTRIM(ocfas.PatenteCamion)), ''), 
	       NULLIF(LTRIM(RTRIM(ocfas.PatenteAcoplado)), ''), 
		   NULL, 
		   ocfas.Recorrido_Id
	  FROM OrdenCargaFas ocfas

	UNION 

	SELECT NULLIF(LTRIM(RTRIM(ocfason.PatenteCamion)), ''), 
	       NULLIF(LTRIM(RTRIM(ocfason.PatenteAcoplado)), ''), 
		   NULL, 
		   ocfason.Recorrido_Id
	  FROM OrdenCargaInternaFason ocfason

	UNION 

	SELECT NULLIF(LTRIM(RTRIM(ocNoProductivos.PatenteCamion)), ''), 
	       NULLIF(LTRIM(RTRIM(ocNoProductivos.PatenteAcoplado)), ''), 
		   NULL, 
		   ocNoProductivos.Recorrido_Id
	  FROM OrdenCargaInterna ocNoProductivos

    --UNION

	--SELECT occ.PatenteCamion, occ.PatenteAcoplado, NULL, occ.Recorrido_Id
	--  FROM OrdenDeCargaContenedor occ
-- select * from OrdenEntrePlantas
	UNION

	SELECT NULLIF(LTRIM(RTRIM(odd.PatenteCamion)), ''), 
	       NULLIF(LTRIM(RTRIM(odd.PatenteAcoplado)), ''), 
		   NULL, 
		   odd.Recorrido_Id
	  FROM OrdenDeDescarga odd

	UNION

	SELECT NULLIF(LTRIM(RTRIM(oddf.PatenteCamion)), ''), 
	       NULLIF(LTRIM(RTRIM(oddf.PatenteAcoplado)), ''), 
		   NULL, 
		   oddf.Recorrido_Id
	  FROM OrdenDeDescargaFason oddf;