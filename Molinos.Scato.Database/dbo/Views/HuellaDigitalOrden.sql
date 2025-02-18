CREATE VIEW dbo.HuellaDigitalOrden
AS
WITH HuellaDigital AS (
    SELECT 
        Id,
        Patente,
        Acoplado,
        IdTransportista,
        Transportista,
        IdChofer,
        Chofer,
        IdBalanza,
        Balanza,
        PesoTara,
        IdCentro,
        LugarPesaje,
        FechaHoraPesaje,
        Usuario,
        Observaciones,
        Tipo,
        Estado,
        ROW_NUMBER() OVER (
            PARTITION BY Patente, Acoplado, Transportista
            ORDER BY FechaHoraPesaje DESC
        ) AS Orden
    FROM dbo.HuellaDigitalUnion
)
SELECT *
FROM HuellaDigital

