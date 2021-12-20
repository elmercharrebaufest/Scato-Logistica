-- ================================================
-- Template generated from Template Explorer using:
-- Create Inline Function (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the function.
-- ================================================
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[CalcularSaldo]
(@FechaDesde DATETIME, @TipoBIN INT, @TablaCoP VARCHAR(2), @CentProd INT)
RETURNS  @Saldos_Iniciales TABLE(SaldoInicial INT,MaterialBin INT,ProveedorId INT,CentroINV_Id INT,VinedoId INT)
AS
BEGIN
DECLARE @Saldos_Temp TABLE(Ajuste INT,Carga INT,Descarga INT,MaterialBin INT,ProveedorId INT,CentroINV_Id INT,VinedoId INT)
INSERT INTO @Saldos_Temp (Ajuste,Carga,Descarga,MaterialBin,ProveedorId,CentroINV_Id,VinedoId)

(SELECT 0 as Ajuste, CDB.CantidadBines as Carga,0 as Descarga,CDB.Tipo_Id as Material,Proveedor.Id as Proveedor, Rec.Centro_Id as CentroINV, Vinedo.Id as Vinedo
								FROM CargaDeBines CDB
								inner join RemitoBodegaUva RBUV on CDB.RemitoBodegaUva_Id = RBUV.Id
								inner join recorrido Rec on Rec.Id = RBUV.Recorrido_Id
								inner join Vinedo on Vinedo.Id = RBUV.Vinedo_Id
								left join Proveedor on Proveedor.Id = Vinedo.Proveedor_Id
								WHERE Rec.FechaEgreso < @FechaDesde AND (CDB.Tipo_Id = @TipoBIN OR @TipoBIN IS NULL)
								and (1 = (SELECT CASE WHEN (@TablaCoP = 'P' and Proveedor.Id = CAST(@CentProd as INT)) or @CentProd IS Null THEN 1
													  WHEN (@TablaCoP = 'C' and Rec.Centro_Id = CAST(@CentProd as INT)) or @CentProd IS Null THEN 1
													  WHEN (@TablaCoP = 'V' and Vinedo.Id = CAST(@CentProd as INT) and Vinedo.Discriminator = 'VinedoPropio') or @CentProd IS Null THEN 1
													  ELSE 0
													END))
UNION ALL(
SELECT 0 as Ajuste, 0 AS Carga,DDB.CantidadBines as Descarga,DDB.Tipo_Id AS Material,Proveedor.Id as Proveedor, Rec.Centro_Id as CentroINV, Vinedo.Id as Vinedo
FROM DescargaDeBines DDB
inner join RemitoBodegaUva RBUV on DDB.RemitoBodegaUva_Id = RBUV.Id
inner join recorrido Rec on Rec.Id = RBUV.Recorrido_Id
inner join Vinedo on Vinedo.Id = RBUV.Vinedo_Id
left join Proveedor on Proveedor.Id = Vinedo.Proveedor_Id
WHERE Rec.FechaEgreso < @FechaDesde AND (DDB.Tipo_Id = @TipoBIN OR @TipoBIN IS NULL)
and (1 = (SELECT CASE WHEN @TablaCoP = 'P' and Proveedor.Id = CAST(@CentProd as INT) or @CentProd IS Null THEN 1
						WHEN @TablaCoP = 'C' and Rec.Centro_Id = CAST(@CentProd as INT) or @CentProd IS Null THEN 1
						WHEN (@TablaCoP = 'V' and Vinedo.Id = CAST(@CentProd as INT) and Vinedo.Discriminator = 'VinedoPropio') or @CentProd IS Null THEN 1
						ELSE 0
						END))
)
UNION ALL(
SELECT  (CASE Movimiento WHEN  1      THEN 0 - Stock
                       WHEN 0 THEN Stock
       END) AS Ajuste,0 as Carga ,0 as Descarga,	AjusteStockBines.Material_Id as Material,Proveedor.Id as Proveedor,AjusteStockBines.Centro_Id as CentroINV, Vinedo.Id as Vinedo
FROM AjusteStockBines
left join Proveedor on Proveedor.Id = AjusteStockBines.Proveedor_Id
left join Vinedo on Vinedo.Id = AjusteStockBines.VinedoPropio_Id
WHERE  Fecha < @FechaDesde AND (AjusteStockBines.Material_Id = @TipoBIN OR @TipoBIN IS NULL)
and (1 = (SELECT CASE WHEN @TablaCoP = 'P' and Proveedor.Id = CAST(@CentProd as INT) or @CentProd IS Null THEN 1
					WHEN @TablaCoP = 'C' and AjusteStockBines.Centro_Id = CAST(@CentProd as INT) or @CentProd IS Null THEN 1
					WHEN (@TablaCoP = 'V' and Vinedo.Id = CAST(@CentProd as INT) and Vinedo.Discriminator = 'VinedoPropio') or @CentProd IS Null THEN 1
					ELSE 0
				END))
)
)
UNION ALL(
SELECT  (CASE Movimiento WHEN  1      THEN 0 - Cantidad
                       WHEN 0 THEN Cantidad
       END) AS Ajuste,0 as Carga ,0 as Descarga,	MovimientoDeBines.Material_Id as Material,Proveedor.Id as Proveedor,MovimientoDeBines.Centro_Id as CentroINV, Vinedo.Id as Vinedo
FROM MovimientoDeBines
left join Proveedor on Proveedor.Id = MovimientoDeBines.Proveedor_Id
left join Vinedo on Vinedo.Id = MovimientoDeBines.VinedoPropio_Id
WHERE  Fecha < @FechaDesde AND (MovimientoDeBines.Material_Id = @TipoBIN OR @TipoBIN IS NULL)
and (1 = (SELECT CASE WHEN @TablaCoP = 'P' and Proveedor.Id = CAST(@CentProd as INT) or @CentProd IS Null THEN 1
					WHEN @TablaCoP = 'C' and MovimientoDeBines.Centro_Id = CAST(@CentProd as INT) or @CentProd IS Null THEN 1
					WHEN (@TablaCoP = 'V' and Vinedo.Id = CAST(@CentProd as INT) and Vinedo.Discriminator = 'VinedoPropio') or @CentProd IS Null THEN 1
					ELSE 0
				END))
)

INSERT INTO @Saldos_Iniciales (SaldoInicial,MaterialBin,ProveedorId,CentroINV_Id,VinedoId)
			select	SUM(Ajuste) + SUM(Descarga) - SUM(Carga) AS SaldoInicial,
						S.MaterialBin,
						NULL AS ProveedorId,
						S.CentroINV_Id,
						NULL AS VinedoId					 
						from @Saldos_Temp AS S						
				GROUP BY S.MaterialBin,S.CentroINV_Id
				HAVING S.CentroINV_Id IS NOT NULL AND (@TablaCoP = 'C' OR @TablaCoP IS NULL)
				UNION ALL(
				select	SUM(Ajuste) - SUM(Descarga) + SUM(Carga) as SaldoInicial,
						S.MaterialBin,
						S.ProveedorId,
						NULL AS CentroINV_Id,
						NULL AS VinedoId
						from @Saldos_Temp AS S
				GROUP BY S.ProveedorId,S.MaterialBin
				HAVING S.ProveedorId IS NOT NULL AND (@TablaCoP = 'P' OR @TablaCoP IS NULL))
				UNION ALL(
				select	SUM(Ajuste) - SUM(Descarga) + SUM(Carga) as SaldoInicial,
						S.MaterialBin,
						NULL AS ProveedorId,
						NULL AS CentroINV_Id,
						s.VinedoId
						from @Saldos_Temp AS S
				GROUP BY S.VinedoId,S.MaterialBin
				HAVING S.VinedoId IS NOT NULL AND (@TablaCoP = 'V' OR @TablaCoP IS NULL))
return
END





