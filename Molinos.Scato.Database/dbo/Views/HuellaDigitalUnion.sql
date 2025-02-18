CREATE VIEW dbo.HuellaDigitalUnion
AS
SELECT 
    r.Id, 
    r.Patente, 
    NULLIF(v.PatenteAcoplado, '') AS Acoplado, -- Devuelve NULL si está vacío 
    r.Transportista_Id AS IdTransportista,
    t.RazonSocial AS Transportista, 
    r.Chofer_Id AS IdChofer, 
    CONCAT(ch.Apellido, ' ', ch.Nombre) AS Chofer, -- Nombre completo del chofer
    r.BalanzaTara_Id AS IdBalanza, 
    b.Nombre AS Balanza, 
    r.PesoTara, 
    r.Centro_Id AS IdCentro, 
    c.Descripcion AS LugarPesaje, 
    r.PesoTaraFecha AS FechaHoraPesaje,
    r.Usuario,
    '' AS Observaciones,
    1 AS Tipo,
    1 AS Estado
FROM 
    Recorrido r 
    INNER JOIN Vehiculo v ON r.Vehiculo_Id = v.Id 
    INNER JOIN Transportista t ON r.Transportista_Id = t.Id
    INNER JOIN Chofer ch ON r.Chofer_Id = ch.Id
    INNER JOIN Balanza b ON r.BalanzaTara_Id = b.Id
    INNER JOIN Centro c ON r.Centro_Id = c.Id
WHERE 
    r.PesoTaraFecha >= DATEADD(MONTH, -12, GETDATE()) -- Filtra solo los últimos 12 meses


UNION

SELECT 
    h.Id,
    h.Patente,
    h.Acoplado,
    h.Transportista_Id AS IdTransportista,
    t.RazonSocial AS Transportista,
    h.Chofer_Id AS IdChofer,
    CONCAT(ch.Apellido, ' ', ch.Nombre) AS Chofer,
    h.Balanza_Id AS IdBalanza,
    b.Nombre AS Balanza,
    h.PesoTara,
    h.Centro_Id AS IdCentro,
    c.Descripcion AS LugarPesaje,
    h.FechaHoraPesaje AS FechaHoraPesaje,
    h.Usuario,
    h.Observaciones,
    2 AS Tipo,
    h.Estado
FROM 
    HuellaDigital h
    INNER JOIN Chofer ch ON h.Chofer_Id = ch.Id
    INNER JOIN Transportista t ON h.Transportista_Id = t.Id
    INNER JOIN Centro c ON h.Centro_Id = c.Id
    INNER JOIN Balanza b ON h.Balanza_Id = b.Id;


