IF NOT EXISTS (SELECT 1 FROM MonitoreoServicioExterno WHERE Nombre = 'MOA PAY')
BEGIN
    INSERT INTO MonitoreoServicioExterno
        (Nombre, HealthCheckUrl, UltimoEstado, UltimaVerificacion, HealthCheckConfig, KeyJob)
    VALUES
        ('MOA PAY', 'http://localhost/Scato.WebApiCore/MOAPayApi/ObtenerPagos', 2, GETDATE(), '{"Method":"POST","ContentType":"application/json","TimeoutMs":5000,"ExpectedStatusCodes":[200],"Body":"{\"Id\":null,\"Dominio\":null,\"NumeroDocumento\":null,\"TipoFecha\":\"E\",\"FechaDesde\":\"2025-09-10T16:33:22\",\"FechaHasta\":\"2025-09-11T16:33:22\",\"Disponible\":\"S\",\"Pagado\":\"S\"}","Headers":{"Authorization":"Basic U2NhdG9Mb2dpc3RpY2E6U2VydmljaW9FeHRlcm5vUGFzcw=="},"QueryParameters":{}}', 'VerificarHealthCheckMOAPayHealth');

    PRINT 'Registro insertado en MonitoreoServicioExterno.';
END
