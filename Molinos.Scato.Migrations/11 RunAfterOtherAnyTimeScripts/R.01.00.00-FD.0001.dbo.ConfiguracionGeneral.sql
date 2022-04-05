IF NOT EXISTS (SELECT 1 FROM ConfiguracionGeneral WHERE Pantalla = 'AFIP' AND  Nombre = 'ConsultasParalelas' AND  Centro_Id IS NULL) 
BEGIN 
INSERT INTO ConfiguracionGeneral(Pantalla,Nombre,Valor,Centro_Id,FechaCreacion,UsuarioCreacion) VALUES ('AFIP', 'ConsultasParalelas', '1', NULL, GETDATE(), 'SCATO')
END
