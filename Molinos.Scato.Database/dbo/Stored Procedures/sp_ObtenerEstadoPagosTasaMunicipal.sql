CREATE PROCEDURE [dbo].[sp_ObtenerEstadoPagosTasaMunicipal]
    @Patente NVARCHAR(20),
    @NumeroDocumento NVARCHAR(20),
    @DiasFechaDesde INT,
    @Centro INT,
    @TipoVehiculo INT, -- 0: común, 1: escalable
    @CodigoDiferenciaPago NVARCHAR(2)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @FechaLimite DATETIME = DATEADD(DAY, -@DiasFechaDesde, GETDATE());

    ;WITH PagosNormales AS (
        SELECT *
        FROM PagosTasaMunicipal p
        WHERE 
            (p.Dominio = @Patente OR p.NumeroDocumento = @NumeroDocumento)
            AND p.Disponible = 1
            AND RIGHT(p.NumeroDocumento, 2) <> @CodigoDiferenciaPago
            AND p.FechaPago >= @FechaLimite
    ),
    PagosDiferenciaPago AS (
        SELECT *
        FROM PagosTasaMunicipal p
        WHERE 
            (p.Dominio = @Patente OR p.NumeroDocumento = @NumeroDocumento + @CodigoDiferenciaPago)
            AND p.Disponible = 1
            AND p.FechaPago >= @FechaLimite
    ),
    SumaPagos AS (
        SELECT  
            pd.Id AS IdDiferenciaPago,
            pn.Id AS IdPagoNormal,
            pd.MOAPay_Id AS IdPayDiferenciaPago,
            pn.MOAPay_Id AS IdPayPagoNormal,
            pn.NumeroDocumento,
            pn.Dominio,
            pn.FechaPago,
            pn.Importe AS ImporteNormal,
            ISNULL(pd.Importe, 0) AS ImporteDP,
            ISNULL(pd.Importe, 0) + pn.Importe AS TotalPagado
        FROM PagosNormales pn 
        LEFT JOIN PagosDiferenciaPago pd 
            ON pn.NumeroDocumento = LEFT(pd.NumeroDocumento, LEN(pd.NumeroDocumento) - 2)
    )

    SELECT 
        s.*,
        r.Monto AS TarifaTipoVehiculo,
        CASE WHEN s.TotalPagado >= r.Monto THEN 0 ELSE 2 END AS CondicionDePago
    FROM SumaPagos s
    JOIN ReciboMunicipal r ON 
        r.Centro_Id = @Centro
        AND (
            (@TipoVehiculo = 0 AND r.TipoVehiculo = 0)
            OR (@TipoVehiculo <> 0 AND r.TipoVehiculo IS NULL)
        )
        AND r.FechaActivacion = (
            SELECT MAX(r2.FechaActivacion)
            FROM ReciboMunicipal r2
            WHERE 
                r2.Centro_Id = r.Centro_Id
                AND (
                    (@TipoVehiculo = 0 AND r2.TipoVehiculo = 0)
                    OR (@TipoVehiculo <> 0 AND r2.TipoVehiculo IS NULL)
                )
                AND r2.FechaActivacion <= s.FechaPago
        )
    ORDER BY s.FechaPago DESC;
END;
GO
