	-- Paises
IF EXISTS (select 1 from pais where descripcion = 'Argentina') BEGIN update pais set descripcion = 'ARGENTINA' where id = (select id from pais where descripcion = 'Argentina') END
IF NOT EXISTS (select Descripcion from Pais where Descripcion = ('ARGENTINA')) BEGIN insert into Pais(Descripcion) values ('ARGENTINA'); END
IF NOT EXISTS (select Descripcion from Pais where Descripcion = ('BOLIVIA')) BEGIN insert into Pais(Descripcion) values ('BOLIVIA'); END
IF NOT EXISTS (select Descripcion from Pais where Descripcion = ('BRASIL')) BEGIN insert into Pais(Descripcion) values ('BRASIL'); END
IF NOT EXISTS (select Descripcion from Pais where Descripcion = ('COLOMBIA')) BEGIN insert into Pais(Descripcion) values ('COLOMBIA'); END
IF NOT EXISTS (select Descripcion from Pais where Descripcion = ('CHILE')) BEGIN insert into Pais(Descripcion) values ('CHILE'); END
IF NOT EXISTS (select Descripcion from Pais where Descripcion = ('ECUADOR')) BEGIN insert into Pais(Descripcion) values ('ECUADOR'); END
IF NOT EXISTS (select Descripcion from Pais where Descripcion = ('GUYANA')) BEGIN insert into Pais(Descripcion) values ('GUYANA'); END
IF NOT EXISTS (select Descripcion from Pais where Descripcion = ('PARAGUAY')) BEGIN insert into Pais(Descripcion) values ('PARAGUAY'); END
IF NOT EXISTS (select Descripcion from Pais where Descripcion = ('PERU')) BEGIN insert into Pais(Descripcion) values ('PERU'); END
IF NOT EXISTS (select Descripcion from Pais where Descripcion = ('SURINAM')) BEGIN insert into Pais(Descripcion) values ('SURINAM'); END
IF NOT EXISTS (select Descripcion from Pais where Descripcion = ('URUGUAY')) BEGIN insert into Pais(Descripcion) values ('URUGUAY'); END
IF NOT EXISTS (select Descripcion from Pais where Descripcion = ('VENEZUELA')) BEGIN insert into Pais(Descripcion) values ('VENEZUELA'); END
IF NOT EXISTS (select Descripcion from Pais where Descripcion = ('GUAYANA FRANCESA')) BEGIN insert into Pais(Descripcion) values ('GUAYANA FRANCESA'); END

-- Documentos
IF NOT EXISTS (select 1 from TipoDocumentoIdentidad where descripcionCorta = 'DNI') BEGIN insert into TipoDocumentoIdentidad (Descripcion,DescripcionCorta, CodigoSap) values ('Documento Nacional de Identidad','DNI', ''); END
IF NOT EXISTS (select 1 from TipoDocumentoIdentidad where descripcionCorta = 'LC') BEGIN insert into TipoDocumentoIdentidad (Descripcion,DescripcionCorta, CodigoSap) values ('Libreta Civica','LC', '90'); END
IF NOT EXISTS (select 1 from TipoDocumentoIdentidad where descripcionCorta = 'LE') BEGIN insert into TipoDocumentoIdentidad (Descripcion,DescripcionCorta, CodigoSap) values ('Libreta de Enrolamiento','LE', '12'); END
IF NOT EXISTS (select 1 from TipoDocumentoIdentidad where descripcionCorta = 'CI') BEGIN insert into TipoDocumentoIdentidad (Descripcion,DescripcionCorta, CodigoSap) values ('Cédula de Identidad','CI', '00'); END
IF NOT EXISTS (select 1 from TipoDocumentoIdentidad where descripcionCorta = 'PP') BEGIN insert into TipoDocumentoIdentidad (Descripcion,DescripcionCorta, CodigoSap) values ('Pasaporte','PP', '13'); END

--Tipo Balanza
IF NOT EXISTS (select 1 from TipoBalanza where Descripcion = 'Vehículo') BEGIN insert into TipoBalanza(Descripcion) values ('Vehículo'); END
IF NOT EXISTS (select 1 from TipoBalanza where Descripcion = 'Piso') BEGIN insert into TipoBalanza(Descripcion) values ('Piso'); END
IF NOT EXISTS (select 1 from TipoBalanza where Descripcion = 'Aérea') BEGIN insert into TipoBalanza(Descripcion) values ('Aérea'); END

--Tipo Micromuestras
IF NOT EXISTS (select 1 from TipoMicromuestra where Descripcion = 'Analisis Interno') BEGIN insert into TipoMicromuestra(Id, Descripcion) values (0, 'Analisis Interno'); END
IF NOT EXISTS (select 1 from TipoMicromuestra where Descripcion = 'Camara') BEGIN insert into TipoMicromuestra(Id, Descripcion) values (1, 'Camara'); END
IF NOT EXISTS (select 1 from TipoMicromuestra where Descripcion = 'Casillero') BEGIN insert into TipoMicromuestra(Id, Descripcion) values (2, 'Casillero'); END
IF NOT EXISTS (select 1 from TipoMicromuestra where Descripcion = 'Entregador') BEGIN insert into TipoMicromuestra(Id, Descripcion) values (3, 'Entregador'); END

--Tipo Vehiculo Bodega
IF NOT EXISTS (select 1 from TipoVehiculoBodega where Descripcion = 'Camión') BEGIN insert into TipoVehiculoBodega(Descripcion, DescripcionCorta) values ('Camión','C'); END
IF NOT EXISTS (select 1 from TipoVehiculoBodega where Descripcion = 'Acoplado') BEGIN insert into TipoVehiculoBodega(Descripcion, DescripcionCorta) values ('Acoplado','A'); END
IF NOT EXISTS (select 1 from TipoVehiculoBodega where Descripcion = 'Bines') BEGIN insert into TipoVehiculoBodega(Descripcion, DescripcionCorta) values ('Bines','B'); END
IF NOT EXISTS (select 1 from TipoVehiculoBodega where Descripcion = 'Molienda en Viñedos') BEGIN insert into TipoVehiculoBodega(Descripcion, DescripcionCorta) values ('Molienda en Viñedos','V'); END
IF NOT EXISTS (select 1 from TipoVehiculoBodega where Descripcion = 'Tractor') BEGIN insert into TipoVehiculoBodega(Descripcion, DescripcionCorta) values ('Tractor','T'); END

-- Permisos
IF NOT EXISTS (select 1 from Permiso where Codigo = 1) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Tipo Documento Identidad',0 , 1, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 2) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Material',0 , 2, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 3) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Chofer',0 , 3, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 4) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Almacen',0 , 4, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 5) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Transportista',0 , 5, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 6) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Excepcion Al Control',0 , 6, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 7) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Inhabilitacion Chofer',0 , 7, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 8) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Inhabilitacion Camion',0 , 8, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 9) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Centro',0 , 9, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 10) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Tipo Comercial',0 , 10, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 11) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Tipo Comercial Por Wf',0 , 11, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 12) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Balanza',0 , 12, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 13) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Caracteristica De Calidad',0 , 13, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 14) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Camara',0 , 14, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 15) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Motivo',0 , 15, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 16) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Boca Destino',0 , 16, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 17) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Humedímetro',0 , 17, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 18) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Rol',0 , 18, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 19) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Permiso',0 , 19, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 20) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Usuario',0 , 20, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 21) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Suplencia',0 , 21, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 22) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Excepción de Envío a Cámara',0 , 22, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 23) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Entregador',0 , 23, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 24) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Transaccion SAP',0 , 24, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 25) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Material Por Workflow',0 , 25, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 26) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Panel De Control Transacciones Sap', 0, 26, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 27) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Tara Romaneo', 0, 27, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 28) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Talonario', 0, 28, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 29) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Modificar Documento de Ingreso', 0, 29, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 30) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Reimprimir Documento', 0, 30, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 31) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Exportación de Archivos para AFIP', 0, 31, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 32) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Proveedor', 0, 32, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 33) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Impresiones', 0, 33, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 34) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Formato de Impresion', 0, 34, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 35) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Libro de Movimientos y Existencia de Granos', 0, 35, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 36) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Borrado de Documento', 0, 36, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 37) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Tara Contenedor', 0, 37, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 38) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Calle', 0, 38, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 39) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Calidades Por Material', 0, 39, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 40) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Casillero', 0, 40, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 41) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Tarjetas Bloqueadas', 0, 41, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 42) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Recibo Municipal', 0, 42, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 43) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Tarjetas Rango', 0, 43, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 44) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Tarjetas Supervisor', 0, 44, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 45) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Hidraulica', 0, 45, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 46) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Controles de Tiempo', 0, 46, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 47) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Puestos de Trabajo', 0, 47, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 48) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Actividad Por Dispositivos', 0, 48, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 49) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Actividad Por Barrera Semaforo', 0, 49, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 50) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Motivo de Quiebre de Barrera', 0, 50, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 51) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Editor de Workflow', 0, 51, 'Editor de Workflow'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 52) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Consultar Documento de Ingreso', 0, 52, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 53) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Lista de Tareas Automatizada', 0, 53, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 54) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Modalidad Puesto de Trabajo', 0, 54, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 55) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reimpresion de Documentos Imprimir Documento', 0, 55, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 56) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reimpresion de Documentos Borrar Documento', 0, 56, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 57) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Liberacion de Casilleros', 0, 57, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 58) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Consulta de Casilleros', 0, 58, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 59) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Impresión', 0, 59, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 60) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Exportación de Archivos', 0, 60, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 61) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM CIU Anulados', 0, 61, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 62) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Viñedo Propio', 0, 62, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 63) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Variedad por Vinedo', 0, 63, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 64) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Viñedo de Terceros', 0, 64, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 65) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Asignación de Recorrido', 0, 65, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 66) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Exportación de archivos para CIU', 0, 66, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 67) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Actividad con Carga Automática', 0, 67, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 68) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Establecimientos', 0, 68, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 69) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Consultar Documento por Tarjeta', 0, 69, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 70) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Empresa', 0, 70, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 71) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Tecnologia', 0, 71, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 72) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Proveedor Exceptuado del Sistema Industria', 0, 72, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 73) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Transmisión a sap', 0, 73, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 74) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Impresion Por Centro', 0, 74, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 75) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Borrar Impresion Por Centro', 0, 75, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 76) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Panel de Control Transacciones Cupos', 0, 76, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 77) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Movimiento De Bines', 0, 77, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 78) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Firma', 0, 78, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 79) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Excepción al Descuento', 0, 79, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 80) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Administración de Distancias', 0, 80, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 81) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Borrar Documentos Terminados', 0, 81, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 82) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Borrar Documentos No Terminados', 0, 82, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 83) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Tipos de Vehículos', 0, 83, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 84) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Stock de Establecimiento', 0, 84, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 85) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Cosecha', 0, 85, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 86) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Categoria', 0, 86, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 87) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Matricula', 0, 87, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 88) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Envio Camara Directo', 0, 88, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 89) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Aviso de Quiebre', 0, 89, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 90) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Nirs',0 , 90, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 91) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Historico de Camiones',0 , 91, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 92) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Estación Meteorológica', 0, 92, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 93) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Habilitación De Vehiculos', 0, 93, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 94) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Analisis Obligatorio', 0, 94, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 95) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Regla De Analisis Obligatorio', 0, 95, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 96) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Puesto de Comando Calado en Planta', 0, 96, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 97) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Cliente', 0, 97, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 98) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Transmision A Sap Manual', 0, 98, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 99) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('CP Otros Puertos', 0, 99, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 604) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Categoria Camiones',0,604, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 609) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Configuracion Sensores',0,609, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 701) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Configuracion Calle Hidraulica',0,701, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 702) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Excepcion Al Intermediario',0,702, NULL); END

IF NOT EXISTS (select 1 from Permiso where Codigo = 100) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Autorizar Descuentos Entregador', 1, 100, 'AutorizarDescuentosEntregador'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 101) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Autorizar Transportista Inhabilitado', 1, 101, 'AutorizarTransportistaInhabilitado'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 102) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Balanza A Cero', 1, 102, 'BalanzaACero'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 103) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Calado', 1, 103, 'Calado'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 104) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Cargar Precintos', 1, 104, 'CargarPrecintos'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 105) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Control De Peso Esperado', 1, 105, 'ControlDePesoEsperado'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 106) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Control Peso Maximo', 1, 106, 'ControlPesoMaximo'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 107) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Coordinacion', 1, 107, 'Coordinacion'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 108) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Documento De Ingreso', 1, 108, 'DocumentoDeIngreso'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 109) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Ingresar Orden Carga Interna', 1, 109, 'IngresarOrdenCargaInterna'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 110) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Ingreso De Observaciones', 1, 110, 'IngresoDeObservaciones'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 111) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Lista De Camiones', 1, 111, 'ListaDeCamiones'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 112) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Pesada', 1, 112, 'Pesada'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 113) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Salida De Centro', 1, 113, 'SalidaDeCentro'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 114) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Verificacion Camion Rechazado', 1, 114, 'VerificacionCamionRechazado'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 115) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Cargar Carta Porte', 1, 115, 'CargarCartaPorte'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 116) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Ingreso De Transportista', 1, 116, 'IngresoDeTransportista'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 117) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Baja De CTG', 1, 117, 'BajaCTG'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 118) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Confirmacion De Carga/Descarga', 1, 118, 'ConfirmacionDeCargaDescarga'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 119) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Analisis De Calidad', 1, 119, 'AnalisisDeCalidad'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 120) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Control de Peso Origen', 1, 120, 'ControlPesoOrigen'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 121) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Ingresar Carta de Porte Por Redespacho', 1, 121, 'IngresarCartaPorteRedespacho'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 122) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Alta De CTG', 1, 122, 'AltaCTG'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 123) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Movimiento Stock', 1, 123, 'ServicioSapMovAjuste'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 124) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Existe Pedido De Traslado', 1, 124, 'ExistePedidoDeTraslado'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 125) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Salida De Origen En Redespacho', 1, 125, 'ServicioSapMov975'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 126) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Llegada A Destino En Redespacho', 1, 126, 'ServicioSapMov305'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 127) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Ingreso Por Compra De Granos', 1, 127, 'ServicioSapFill_Z1000'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 128) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Control Peso Neto', 1, 128, 'ControlPesoNeto'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 129) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Cargar Orden De Descarga', 1, 129, 'CargarOrdenDeDescarga'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 130) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Ingresar Orden Carga Fas', 1, 130, 'IngresarOrdenCargaFas'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 131) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Romaneo', 1, 131, 'Romaneo'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 132) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Visteo', 1, 132, 'Visteo'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 133) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Egreso de Material no Productivo', 1, 133, 'ServicioSapEgresosNoProductivos'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 134) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Ingreso De Lote', 1, 134, 'IngresoDeLote'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 135) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Verificacion COT', 1, 135, 'VerificacionCot'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 136) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Validar Contrato', 1, 136, 'ValidarContrato'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 137) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Ingresar CP Redespacho Prestamo', 1, 137, 'IngresarCartaPorteRedespachoPrestamo'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 138) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Ingresar CP Prestamo', 1, 138, 'CargarCartaPortePrestamo'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 139) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Ingresar Orden de Descarga (Fason)', 1, 139, 'CargarOrdenDeDescargaFason'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 140) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Ingresar Orden de Carga Interna (Fason)', 1, 140, 'IngresarOrdenCargaInternaFason'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 141) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Baja CTG Definitivo', 1, 141, 'BajaCTGDefinitivo'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 142) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Control de Ingreso', 1, 142, 'ControlDeIngreso'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 143) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Ingresos Egresos Fazones', 1, 143, 'ServicioSapIngresosEgresosFazones'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 144) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Verifiacion Salida Flete', 1, 144, 'VerificacionSalidaFlete'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 145) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Pesa Neto', 1, 145, 'ServicioSapPesaNeto'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 146) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Ingresar Carta Porte Egreso por Desvio', 1, 146, 'CargarCartaPorteRedespachoDesvio'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 147) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Ingreso Número de COT', 1, 147, 'IngresoNumeroCot'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 148) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Fletes Doble Tramo', 1, 148, 'ServicioSap_Z4030'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 149) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Cargar Remito entre Plantas', 1, 149, 'CargarOrdenEntrePlantas'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 150) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Ingreso Remito', 1, 150, 'IngresoRemito'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 151) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Descarga Unidad', 1, 151, 'ControlPesoNetoDescargaUnidad'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 152) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Control Peso Neto Descarga Unidad', 1, 152, 'DescargaUnidad'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 153) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Cargar Orden de Carga por Contenedor', 1, 153, 'CargarOrdenDeCargaContenedor'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 154) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Ingreso Remito Terceros', 1, 154, 'IngresoRemitoTerceros'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 155) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Cargar Carta Porte Fasón', 1, 155, 'CargarCartaPorteFason'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 156) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Autorizar Tiempo En Tránsito', 1, 156, 'AutorizarTiempoEnTransito'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 157) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Imprime Recibo Municipal', 1, 157, 'ImprimeReciboMunicipal'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 158) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Puesto Comando', 1, 158, 'PuestoComando'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 159) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Control de Balanza', 1, 159, 'ControlDeBalanza'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 160) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Indianápolis', 1, 160, 'EnEsperaIndianapolis'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 161) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('En Espera Aduana', 1, 161, 'EnEsperaAduana'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 162) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Pesada Tara', 1, 162, 'PesadaTara'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 163) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Pesada Bruto', 1, 163, 'PesadaBruto'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 164) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad En Playa Externa', 1, 164, 'EnPlayaExterna'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 165) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad En Tránsito', 1, 165, 'EnTransito'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 166) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Egreso Sin Flete Fasones', 1, 166, 'ServicioSapMov291'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 167) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Ingreso CIU', 1, 167, 'IngresoCIU'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 168) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Remito Bodega Uva Propia', 1, 168, 'RemitoBodegaUvaPropia'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 169) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Autorizar Recepción de Uvas', 1, 169, 'AutorizarRecepcionUvas'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 170) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Distribución de almacenes', 1, 170, 'DistribucionDeAlmacenes'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 171) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Ingreso Bins de Salida', 1, 171, 'IngresoBinSalida'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 172) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Remito Bodega Uva Terceros', 1, 172, 'RemitoBodegaUvaTerceros'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 173) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Cargar Hoja de Ruta', 1, 173, 'CargarHojaDeRuta'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 174) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Cargar Hoja de Ruta Yerbatera', 1, 174, 'CargarHojaDeRutaYerbatera'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 175) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Asignacion de Establecimiento', 1, 175, 'AsignacionDeEstablecimiento'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 176) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Pesada Bruto Vagón', 1, 176, 'PesadaBrutoVagon'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 177) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Pesada Tara Vagón', 1, 177, 'PesadaTaraVagon'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 178) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Remito Bodega Vino',1,178,'RemitoBodegaVino'); END 
IF NOT EXISTS (select 1 from Permiso where Codigo = 179) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Verificar Impresión',1,179,'VerificarImpresion'); END 
IF NOT EXISTS (select 1 from Permiso where Codigo = 180) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Ingresos Bodega',1,180,'ServicioSapIngresosBodega'); END 
IF NOT EXISTS (select 1 from Permiso where Codigo = 181) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Forzar Cero',1,181,null); END 
IF NOT EXISTS (select 1 from Permiso where Codigo = 182) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Generar Archivo de Movimientos',1,182,null); END 
IF NOT EXISTS (select 1 from Permiso where Codigo = 183) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Ingreso de Datos de Exportación',1,183,'IngresoDeDatosDeExportacion'); END 
IF NOT EXISTS (select 1 from Permiso where Codigo = 184) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Carga de Cupo',1,184,'CargaDeCupo'); END 
IF NOT EXISTS (select 1 from Permiso where Codigo = 185) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Ingresar Carta Porte Redespacho Importaciones',1,185,'IngresarCartaPorteRedespachoImportaciones'); END 
IF NOT EXISTS (select 1 from Permiso where Codigo = 186) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Servicio Sap ZE7550',1,186,'ServicioSapZE7550'); END 
IF NOT EXISTS (select 1 from Permiso where Codigo = 187) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Paso Por Balanza', 1, 187, 'PasoPorBalanza'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 188) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Pesada Carga Exportacion', 1, 188, 'PesadaCargaExportacion'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 189) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Ingresar Carta Porte Redespacho Mercaderia',1,189,'IngresarCartaPorteRedespachoMercaderia'); END 
IF NOT EXISTS (select 1 from Permiso where Codigo = 190) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Verificar Limite De Credito Venta En SAP',1,190,'VerificarLimiteDeCreditoVentaEnSAP'); END 
IF NOT EXISTS (select 1 from Permiso where Codigo = 191) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Calado en planta',1,191,'CaladoEnPlanta'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 192) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Cancelar Pesada Carga Exportacion', 1, 192, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 193) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Calado Rechazar', 1, 193, 'Calado'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 194) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Pago de Recibo Municipal',1,194,null); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 195) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Devolución de Recibo Municipal',1,195,null); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 196) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Balanza Automatica',1,196,null); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 197) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Salida de centro Playa',1,197,'SalidaDeCentroPlaya'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 198) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Camion Demorado',1,198,'CamionDemorado'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 199) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('En Espera HB4',1,199,'EnEsperaHB4'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 700) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Espera Confirmacion', 1, 700, 'EsperaConfirmacion'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 703) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Alta CTGDG', 1, 703, 'AltaCTGDG'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 704) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Baja CTGDG', 1, 704, 'BajaCTGDG'); END

IF NOT EXISTS (select 1 from Permiso where Codigo = 200) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('IniciarWorkflow', 1, 200, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 201) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Armar Lote', 0, 201, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 202) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Buscar Lote', 0, 202, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 203) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Conversion Material', 0, 203, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 204) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Conversion Procedencia', 0, 204, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 205) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Conversion Grupo', 0, 205, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 206) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Conversion Caracteristica', 0, 206, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 207) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Ajustar Calidad', 0, 207, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 208) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Generación de Micromuestras', 0, 208, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 209) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Modifica Impresiones', 0, 209, 'Modifica Impresiones'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 210) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Ajuste de Stock', 0, 210, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 211) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actividad Asignación Tarjeta de Acceso ', 1, 211, 'AsignacionTarjetaDeAcceso'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 212) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reasignación de Tarjetas', 0, 212, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 213) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Imprimir Tarjetas de Acceso', 0, 213, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 214) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Imprimir Etiqueta Auditoría', 0, 214, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 215) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Ingresar Cot Manual', 0, 215, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 216) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Ejecución manual de Actividades', 0, 216, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 217) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Armar Lote de Biotecnología', 0, 217, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 218) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Ajuste y Stock Bines', 0, 218, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 219) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Conversion Centro', 0, 219, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 220) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Asignación Ticket Municipal', 0, 220, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 221) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Panel Server/AppPool', 0, 221, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 222) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Panel de Workflows', 0, 222, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 223) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Eliminar Workflows', 0, 223, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 224) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Workflow Movimiento de Bines', 0, 224, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 225) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Gráfico de Planta', 0, 225, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 226) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Panel de Control de Baja de CTG Definitiva', 0, 226, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 227) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Web Mobile', 0, 227, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 228) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Camiones Rechazados', 0, 228, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 229) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Asignacion de Contingencia', 0, 229, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 230) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Rangos de Redondeo', 0, 230, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 231) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Administración de operaciones de Balanza Puerto', 0, 231, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 232) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Administración de operaciones - Crear Carga', 0, 232, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 233) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Control de Calado', 0, 233, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 234) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Panel De Control Unrenport', 0, 234, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 235) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Actualizar Foto Carta de Porte', 0, 235, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 236) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Ver camiones Pendientes Mesa', 1, 236, 'Pendiente'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 237) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Puerto Destino', 0, 237, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 238) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Puerto Bodega', 0, 238, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 239) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Puerto Exportador', 0, 239, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 240) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Puerto Material', 0, 240, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 241) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Puerto EmbarqueLiquido', 0, 241, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 242) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Puerto Vapor', 0, 242, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 243) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Puerto Balanza', 0, 243, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 244) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Puerto Scato', 0, 244, 'Pendiente'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 245) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Lote Auditoria', 0, 245, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 246) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ModificarDatosExportacion', 0, 246, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 247) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('VisualizarVideoCamaras', 0, 247, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 248) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('EmbarquesPorBuques', 1, 248, 'Embarques por Buques'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 262) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Pinchazos por Calada', 0, 262, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 263) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('VisualizarVideoCamarasExportacion', 0, 263, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 611) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Armar Lote de Inase', 0, 611, NULL); END

IF NOT EXISTS (select 1 from Permiso where Codigo = 249) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('EstadoDeCalle', 1, 249, 'Estado De Calle'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 250) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('EstadoDeCalleLlamar', 1, 250, 'Estado De Calle - Llamar'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 251) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('EstadoDeCalleCancelar', 1, 251, 'Estado De Calle - Cancelar'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 252) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('EstadoDeCallePlayero', 1, 252, 'Estado De Calle - Playero'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 253) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('EstadoDeCalleCalador', 1, 253, 'Estado De Calle - Calador'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 254) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('EstadoDeCalleMoverRechazado',1,254, 'Estado de Calle - Mover Rechazado'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 255) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('EstadoDeCallePlayaInterna',1,255, 'Estado de Calle - Playa Interna'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 256) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('VisualizarVideoCamarasBalanza', 0, 256, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 257) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('EtiquetaPuerto', 1, 257, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 258) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Ver camiones Pendientes No Granos', 1, 258, 'CamionesPendientesNoGranos'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 259) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reasignacion de Calles PostCalado', 1, 259, 'ReasignacionCallesPostCalado'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 260) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Llamado De Filas Automatico', 1, 260, 'LlamadoDeFilasAutomatico'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 261) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Cambio De Material En Filas', 1, 261, 'CambioDeMaterialEnFilas'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 264) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Monitor CPEs Cacheadas', 0, 264, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 265) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Barrera', 0, 265, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 266) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Abm Configuracion Eficiencia Calado', 0, 266, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 267) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Visualizar Video Camaras Calador 1', 0, 267, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 268) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Visualizar Video Camaras Calador 2', 0, 268, NULL); END

IF NOT EXISTS (select 1 from Permiso where Codigo = 300) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Balanceros', 2, 300, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 301) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Administradores', 2, 301, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 302) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('PuestoComando', 2, 302, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 303) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('IngresoPlayaInterna', 2, 303, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 304) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Entregadores', 2, 304, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 305) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('NotificacionAplicacion', 2, 305, NULL); END

IF NOT EXISTS (select 1 from Permiso where Codigo = 400) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado de Arribos a Planta', 3, 400, 'Listado de Arribos a Planta'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 401) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Planilla F-515', 3, 401, 'Planilla F-515'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 402) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado de Ingreso de Granos', 3, 402, 'Listado de Ingreso de Granos'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 403) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Informe de Gestión Actual', 3, 403, 'Informe de Gestión Actual'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 404) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Informe de Gestión Histórico', 3, 404, 'Informe de Gestión Histórico'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 405) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte de Seguimiento y Control', 3, 405, 'Reporte de Seguimiento y Control'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 406) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado de Camiones Rechazados', 3, 406, 'Listado de Camiones Rechazados'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 407) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado de Diferencias de Peso en Redespacho', 3, 407, 'Listado de Diferencias de Peso en Redespacho'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 408) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado de Inconsistencias', 3, 408, 'Listado de Inconsistencias'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 409) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado de Pendientes de Armado de Lote', 3, 409, 'Listado de Pendientes de Armado de Lote'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 410) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Emisión de Detalle de Lote', 3, 410, 'Emisión de Detalle de Lote'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 411) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado de Bajas de CTG Manual', 3, 411, 'Listado de Bajas de CTG Manual'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 412) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado de Cambios Modalidad Balanza', 3, 412, 'Listado de Cambios Modalidad Balanza'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 413) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado de Excepciones de Control', 3, 413, 'Listado de Excepciones de Control'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 414) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado Ponderado por Característica', 3, 414, 'Listado Ponderado por Característica'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 415) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Consulta Datos de Vehículos', 3, 415, 'Consulta Datos de Vehículos'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 416) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Libro de Movimientos y existencia de granos', 3, 416, 'Libro de Movimientos y existencia de granos'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 417) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado de Resumen', 3, 417, 'Listado de Resumen'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 418) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Informe de Descarga', 3, 418, 'Informe de Descarga'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 419) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado de Romaneo', 3, 419, 'Listado de Romaneo'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 420) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado de Diferencia de Peso', 3, 420, 'Listado de Diferencia de Peso'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 421) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado de Rechazos', 3, 421, 'Listado de Rechazos'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 422) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte de Anexo INASE', 3, 422, 'Reporte de Anexo INASE'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 423) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte Resumen de Recepción', 3, 423, 'Reporte Resumen de Recepción'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 424) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte Control de Balanza', 3, 424, 'Reporte Control de Balanza'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 425) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte Resumen General por Bodega', 3, 425, 'Reporte Resumen General por Bodega'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 426) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte Listado de Cius por Variedad', 3, 426, 'Reporte Listado de Cius por Variedad'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 427) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte Listado de Cius por Viñatero', 3, 427, 'Reporte Listado de Cius por Viñatero'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 428) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte Listado de Cius por Viñedo y Variedad', 3, 428, 'Reporte Listado de Cius por Viñedo y Variedad'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 429) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte Listado de Recepciones de Bodegas', 3, 429, 'Reporte Listado de Recepciones de Bodegas'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 430) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte Ingreso de Uvas por Operacion y Variedad', 3, 430, 'Reporte Ingreso de Uvas por Operacion y Variedad'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 431) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte Ingreso de Uvas por Variedad', 3, 431, 'Reporte Ingreso de Uvas por Variedad'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 432) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte Ingreso de Uvas por Viñatero y Variedad', 3, 432, 'Reporte Ingreso de Uvas por Viñatero y Variedad'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 433) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte Ingreso de Uvas por Viñatero', 3, 433, 'Reporte Ingreso de Uvas por Viñatero'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 434) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado de Calidades', 3, 434, 'Listado de Calidades'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 436) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado de Quiebre de Barreras', 3, 436, 'Listado de Quiebre de Barreras'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 437) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado de Documentos Anulados', 3, 437, 'Listado de Documentos Anulados'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 438) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Registro de Uso de Tarjeta del Supervisor', 3, 438, 'Registro de Uso de Tarjeta del Supervisor'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 439) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado de Recibos Municipales', 3, 439, 'Listado de Recibos Municipales'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 440) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Log de Ajustes Oncca', 3, 440, 'Log de Ajustes Oncca'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 441) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado de Cambio de Modalidad Puesto de Trabajo', 3, 441, 'Listado de Cambio Modalidad Puesto de Trabajo'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 442) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado de Carta de Porte Fason', 3, 442, 'Listado de Carta de Porte Fason'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 443) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte de Muestras Monsanto', 3, 443, 'Reporte de Muestras Monsanto'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 444) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Log Humedimetro', 3,444,'Log Humedimetro');END
IF NOT EXISTS (select 1 from Permiso where Codigo = 445) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte Información Monsanto',3,445,'Reporte Información Monsanto'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 446) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Detalle Muestra Auditoria',3,446,'Detalle Muestra Auditoria'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 447) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Excepciones al Pago del Ticket Municipal',3,447,'Excepciones al Pago del Ticket Municipal'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 448) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado Cuenta Corriente Bins',3,448,'Listado Cuenta Corriente Bins'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 449) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Stock Detallado de Bines',3,449,'Stock Detallado de Bines'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 450) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte de Paso por Cero',3,450,'Reporte de Paso por Cero'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 451) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Log de Impresiones',3,451,'Log de Impresiones'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 452) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte de Calidad y Mermas',3,452,'Reporte de Calidad y Mermas'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 453) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte Listado Login Usuarios',3,453,'Reporte Listado Login Usuarios'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 454) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte de Stock EPA',3,454,'Reporte de Stock EPA'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 455) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado de Camiones Inhabilitados',3,455,'Listado de Camiones Inhabilitados'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 456) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado de Choferes Inhabilitados',3,456,'Listado de Choferes Inhabilitados'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 457) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Informe De Eficiencia De Hidraulicas',3,457,'Informe De Eficiencia De Hidraulicas'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 458) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado De CCPP Por Procedencia',3,458,'Listado De CCPP Por Procedencia'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 459) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Listado De Reasignacion De Tarjeta',3,459,'Listado De Reasignacion De Tarjeta'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 460) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Detalle De Movimientos',3,460,'DetalleDeMovimientoIngreso'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 461) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reconocimiento de Patentes',3,461,'ReconocimientoDePatentes'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 462) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Informe Tiempos De Pesada',3,462,null); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 463) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Autorizaciones por Exceso de Tiempo',3,463,'AutorizacionesExcesoDeTiempo'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 464) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte Contingencia',3,464,'Reporte Contingencia'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 465) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte Muestras Auditoria',3,465,'ReporteMuestrasAuditoria'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 466) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte CP Otros Puertos',3,466,'ReporteCpOtrosPuertos'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 467) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte CP Otros Puertos Sap',3,467,'ReporteCpOtrosPuertosSap'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 468) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Informe De Eficiencia De Hidraulicas Diario',3,468,'Informe De Eficiencia De Hidraulicas Diario'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 469) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte Modalidad Calador',3,469,'Reporte Modalidad Calador'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 470) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Comparacion Calidad',3,470,'ComparacionCalidad'); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 471) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Reporte Inactividad Calado',3,471,'ReporteInactividadCalado'); END


IF NOT EXISTS (select 1 from Permiso where Codigo = 600) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Pre line up',4,600, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 601) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Line up',4,601, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 602) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Line up lectura',4,602, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 603) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Line up exportar',4,603, NULL); END

IF NOT EXISTS (select 1 from Permiso where Codigo = 605) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Puesto Pausado',1,605, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 606) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Hidraulicas Especiales',1,606, NULL); END

IF NOT EXISTS (select 1 from Permiso where Codigo = 607) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Ver Balanzas Pesada',1,607, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 608) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Ver Revertir Rechazo Vagones',1,608, NULL); END

IF NOT EXISTS (select 1 from Permiso where Codigo = 610) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Envío Muestra INASE',1,610, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 612) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Confirmación Carga/Descarga', 1, 612, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 613) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Contingencia Pesos Excedentes', 1, 613, NULL); END

IF NOT EXISTS (select 1 from Permiso where Codigo = 614) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Crear Cliente Provisorio', 1, 614, NULL); END
IF NOT EXISTS (select 1 from Permiso where Codigo = 615) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('Edicion Configuracion Prebalanza', 0, 615, NULL); END


--IF NOT EXISTS (select 1 from Permiso where Codigo = 607) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('VisualizarVideoCamarasExportacion',0,607, NULL); END

IF NOT EXISTS (select 1 from MotivoReasignacionDeTarjeta where Descripcion = 'Tarjeta Extraviada') BEGIN insert into MotivoReasignacionDeTarjeta(Descripcion, DescripcionCorta) values ('Tarjeta Extraviada', 'Extrav.'); END
IF NOT EXISTS (select 1 from MotivoReasignacionDeTarjeta where Descripcion = 'Tarjeta Rota') BEGIN insert into MotivoReasignacionDeTarjeta(Descripcion, DescripcionCorta) values ('Tarjeta Rota', 'Rota'); END

-- Rol
if not exists(select 1 from rol where descripcion = 'Administrador General') begin insert into Rol(Descripcion) values ('Administrador General') end;

-- RolPermiso
insert into RolPermiso(Rol_Id, Permiso_Id) select r.id, p.id from Rol r, Permiso p where r.Descripcion = 'Administrador General' and p.id not in (select rp.Permiso_Id from RolPermiso rp where rp.Rol_Id = r.Id)
insert into RolPermiso(Rol_Id, Permiso_Id) select r.id, p.id from Rol r, Permiso p where r.Descripcion = 'Nivel 1' and p.id not in (select rp.Permiso_Id from RolPermiso rp where rp.Rol_Id = r.Id)

--Tipo Documento de Ingreso
IF NOT EXISTS (select 1 from TipoDocumentoIngreso where Descripcion = 'Orden de Carga Interna') BEGIN insert into TipoDocumentoIngreso(Id, Descripcion) values (0, 'Orden de Carga Interna'); END
IF NOT EXISTS (select 1 from TipoDocumentoIngreso where Descripcion = 'Carta de Porte') BEGIN insert into TipoDocumentoIngreso(Id, Descripcion) values (1, 'Carta de Porte'); END
IF NOT EXISTS (select 1 from TipoDocumentoIngreso where Descripcion = 'Orden de Descarga') BEGIN insert into TipoDocumentoIngreso(Id, Descripcion) values (2, 'Orden de Descarga'); END
IF NOT EXISTS (select 1 from TipoDocumentoIngreso where Descripcion = 'Orden de Carga Fas') BEGIN insert into TipoDocumentoIngreso(Id, Descripcion) values (3, 'Orden de Carga Fas'); END
IF NOT EXISTS (select 1 from TipoDocumentoIngreso where Descripcion = 'Orden de Descarga Fason') BEGIN insert into TipoDocumentoIngreso(Id, Descripcion) values (4, 'Orden de Descarga Fason'); END
IF NOT EXISTS (select 1 from TipoDocumentoIngreso where Descripcion = 'Orden de Carga Interna Fason') BEGIN insert into TipoDocumentoIngreso(Id, Descripcion) values (5, 'Orden de Carga Interna Fason'); END
IF NOT EXISTS (select 1 from TipoDocumentoIngreso where Descripcion = 'Remito entre Plantas') BEGIN insert into TipoDocumentoIngreso(Id, Descripcion) values (6, 'Remito entre Plantas'); END
IF NOT EXISTS (select 1 from TipoDocumentoIngreso where Descripcion = 'Remito') BEGIN insert into TipoDocumentoIngreso(Id, Descripcion) values (7, 'Remito'); END
IF NOT EXISTS (select 1 from TipoDocumentoIngreso where Descripcion = 'Orden de Carga Contenedor') BEGIN insert into TipoDocumentoIngreso(Id, Descripcion) values (8, 'Orden de Carga Contenedor'); END
IF NOT EXISTS (select 1 from TipoDocumentoIngreso where Descripcion = 'Remito Bodega Uva Propia') BEGIN insert into TipoDocumentoIngreso(Id, Descripcion) values (9, 'Remito Bodega Uva Propia'); END
IF NOT EXISTS (select 1 from TipoDocumentoIngreso where Descripcion = 'Remito Bodega Uva Terceros') BEGIN insert into TipoDocumentoIngreso(Id, Descripcion) values (10, 'Remito Bodega Uva Terceros'); END
IF NOT EXISTS (select 1 from TipoDocumentoIngreso where Descripcion = 'Remito Bodega Vino') BEGIN insert into TipoDocumentoIngreso(Id, Descripcion) values (11, 'Remito Bodega Vino'); END
IF NOT EXISTS (select 1 from TipoDocumentoIngreso where Descripcion = 'Hoja de Ruta') BEGIN insert into TipoDocumentoIngreso(Id, Descripcion) values (12, 'Hoja de Ruta'); END
IF NOT EXISTS (select 1 from TipoDocumentoIngreso where Descripcion = 'Hoja de Ruta Yerbatera') BEGIN insert into TipoDocumentoIngreso(Id, Descripcion) values (13, 'Hoja de Ruta Yerbatera'); END
IF NOT EXISTS (select 1 from TipoDocumentoIngreso where Descripcion = 'Embarque') BEGIN insert into TipoDocumentoIngreso(Id, Descripcion) values (14, 'Embarque'); END

--Tipo Comprobante Oncca
IF NOT EXISTS (select 1 from TipoComprobanteOncca where Descripcion = 'Carte de Porte') BEGIN insert into TipoComprobanteOncca(Descripcion, CodigoOncca) values ('Carte de Porte', '01'); END
IF NOT EXISTS (select 1 from TipoComprobanteOncca where Descripcion = 'Remito') BEGIN insert into TipoComprobanteOncca(Descripcion, CodigoOncca) values ('Remito', '02'); END
IF NOT EXISTS (select 1 from TipoComprobanteOncca where Descripcion = 'Comprobante Interno') BEGIN insert into TipoComprobanteOncca(Descripcion, CodigoOncca) values ('Comprobante Interno', '03'); END
IF NOT EXISTS (select 1 from TipoComprobanteOncca where Descripcion = 'Otros') BEGIN insert into TipoComprobanteOncca(Descripcion, CodigoOncca) values ('Otros', '04'); END

--Tipo Vehículo
IF NOT EXISTS (select 1 from TipoVehiculo where Descripcion = 'Camión') BEGIN insert into TipoVehiculo(Id, Descripcion) values (0, 'Camión'); END
IF NOT EXISTS (select 1 from TipoVehiculo where Descripcion = 'Tren') BEGIN insert into TipoVehiculo(Id, Descripcion) values (1, 'Tren'); END
IF NOT EXISTS (select 1 from TipoVehiculo where Descripcion = 'Bitren') BEGIN insert into TipoVehiculo(Id, Descripcion) values (2, 'Bitren'); END
IF NOT EXISTS (select 1 from TipoVehiculo where Descripcion = 'Camión C(55,5)') BEGIN insert into TipoVehiculo(Id, Descripcion) values (3, 'Camión C(55,5)'); END
IF NOT EXISTS (select 1 from TipoVehiculo where Descripcion = 'Camión D(52,5)') BEGIN insert into TipoVehiculo(Id, Descripcion) values (4, 'Camión D(52,5)'); END
IF NOT EXISTS (select 1 from TipoVehiculo where Descripcion = 'Camión E(49,5)') BEGIN insert into TipoVehiculo(Id, Descripcion) values (5, 'Camión E(49,5)'); END


--Tipo Pesada
IF NOT EXISTS (select 1 from TipoPesada where Descripcion = 'Manual') BEGIN insert into TipoPesada(Id, Descripcion) values (0, 'Manual'); END
IF NOT EXISTS (select 1 from TipoPesada where Descripcion = 'Automática') BEGIN insert into TipoPesada(Id, Descripcion) values (1, 'Automática'); END

--Funcion Sap
IF NOT EXISTS (select 1 from FuncionSap where Descripcion = 'Z_SDMF_Z1000N') BEGIN insert into FuncionSap(Id, Descripcion) values (0, 'Z_SDMF_Z1000N'); END
IF NOT EXISTS (select 1 from FuncionSap where Descripcion = 'Z_SDMF_MOV975') BEGIN insert into FuncionSap(Id, Descripcion) values (1, 'Z_SDMF_MOV975'); END
IF NOT EXISTS (select 1 from FuncionSap where Descripcion = 'Z_SDMF_MOV305') BEGIN insert into FuncionSap(Id, Descripcion) values (2, 'Z_SDMF_MOV305'); END
IF NOT EXISTS (select 1 from FuncionSap where Descripcion = 'Z_SDMF_MOVAJUSTE') BEGIN insert into FuncionSap(Id, Descripcion) values (3, 'Z_SDMF_MOVAJUSTE'); END
IF NOT EXISTS (select 1 from FuncionSap where Descripcion = 'Z_SDMF_RFC_EGRESOS_NO_PRODUCT') BEGIN insert into FuncionSap(Id, Descripcion) values (4, 'Z_SDMF_RFC_EGRESOS_NO_PRODUCT'); END
IF NOT EXISTS (select 1 from FuncionSap where Descripcion = 'Z_SDMF_FASON') BEGIN insert into FuncionSap(Id, Descripcion) values (5, 'Z_SDMF_FASON'); END
IF NOT EXISTS (select 1 from FuncionSap where Descripcion = 'Z_SDMF_RFC_PESANETO') BEGIN insert into FuncionSap(Id, Descripcion) values (6, 'Z_SDMF_RFC_PESANETO'); END
IF NOT EXISTS (select 1 from FuncionSap where Descripcion = 'Z_SDMF_MOV291') BEGIN insert into FuncionSap(Id, Descripcion) values (7, 'Z_SDMF_MOV291'); END

IF NOT EXISTS (select 1 from FuncionSap where Descripcion = 'Z_SDMF_Z4030') BEGIN insert into FuncionSap(Id, Descripcion) values (8, 'Z_SDMF_Z4030'); END

--TipoDeWorkflow
IF NOT EXISTS (select 1 from TipoDeWorkflow where Descripcion = 'Ingreso') BEGIN insert into TipoDeWorkflow(Id, Descripcion) values (0, 'Ingreso'); END
IF NOT EXISTS (select 1 from TipoDeWorkflow where Descripcion = 'Egreso') BEGIN insert into TipoDeWorkflow(Id, Descripcion) values (1, 'Egreso'); END

--Modalidad
IF NOT EXISTS (select 1 from Modalidad where Descripcion = 'Manual') BEGIN insert into Modalidad(Id, Descripcion) values (0, 'Manual'); END
IF NOT EXISTS (select 1 from Modalidad where Descripcion = 'Automática') BEGIN insert into Modalidad(Id, Descripcion) values (1, 'Automática'); END

--EstadoRomaneo
IF NOT EXISTS (select 1 from EstadoRomaneo where Descripcion = 'Pendiente') BEGIN insert into EstadoRomaneo(Id, Descripcion) values (0, 'Pendiente'); END
IF NOT EXISTS (select 1 from EstadoRomaneo where Descripcion = 'En Proceso') BEGIN insert into EstadoRomaneo(Id, Descripcion) values (1, 'En Proceso'); END
IF NOT EXISTS (select 1 from EstadoRomaneo where Descripcion = 'Finalizado') BEGIN insert into EstadoRomaneo(Id, Descripcion) values (2, 'Finalizado'); END

--MotivoExcepcionAlControl
IF NOT EXISTS (select 1 from MotivoExcepcionAlControl where Descripcion = 'A') BEGIN insert into MotivoExcepcionAlControl(Id, Descripcion) values (0, 'A'); END
IF NOT EXISTS (select 1 from MotivoExcepcionAlControl where Descripcion = 'B') BEGIN insert into MotivoExcepcionAlControl(Id, Descripcion) values (1, 'B'); END
IF NOT EXISTS (select 1 from MotivoExcepcionAlControl where Descripcion = 'M') BEGIN insert into MotivoExcepcionAlControl(Id, Descripcion) values (2, 'M'); END

-- Workflows
IF NOT EXISTS (select 1 from Workflow where Codigo = 'EgresoMaterialNoProductivo') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('EgresoMaterialNoProductivo', 'Egreso Material No Productivo', 1, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'IngresoPorCompraDeGranos') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('IngresoPorCompraDeGranos', 'Ingreso Por Compra de Granos', 0, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'EgresoPorRedespachoGranos') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('EgresoPorRedespachoGranos', 'Egreso Por Redespacho De Granos', 1, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'IngresoPorRedespachoDeGranos') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('IngresoPorRedespachoDeGranos', 'Ingreso Por Redespacho De Granos', 0, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'IngresoMercaderiaCarnica') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('IngresoMercaderiaCarnica', 'Ingreso De Mercadería Cárnica', 0, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'EgresoPorVentasFAS') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('EgresoPorVentasFAS', 'Egreso Por Ventas FAS', 1, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'RedespachoACentroDePrestamo') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('RedespachoACentroDePrestamo', 'Redespacho a Centro de Préstamo', 1, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'RecepcionRedespachoPrestamo') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('RecepcionRedespachoPrestamo', 'Recepción Redespacho Préstamo / Devolución', 0, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'RecepcionPorCompra') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('RecepcionPorCompra', 'Recepción por Compra / Devolución', 0, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'RecepcionClientesFason') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('RecepcionClientesFason', 'Recepción Cliente Fason', 0, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'EgresoClienteFason') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('EgresoClienteFason', 'Egreso Cliente Fason', 1, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'EgresoDeInsumosySubproductosPorRedespacho') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('EgresoDeInsumosySubproductosPorRedespacho', 'Egreso de Insumos y Subproductos por Redespacho a Plantas MRP', 1, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'RecepcionDeInsumosPorUnidad') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('RecepcionDeInsumosPorUnidad', 'Recepción de Insumos por Unidad', 0, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'TestStress_IngresoPorCompraDeGranos') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('TestStress_IngresoPorCompraDeGranos', 'Test Stress - Ingreso Por Compra de Granos', 0, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'RedespachoDobleTramo') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('RedespachoDobleTramo', 'Redespacho Doble Tramo Por Desvío', 0, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'RedespachoDobleTramoDesvio') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('RedespachoDobleTramoDesvio', 'Egreso Por Desvio', 1, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'RecepcionSubproductosMRP') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('RecepcionSubproductosMRP', 'Recepción de Subproductos MRP (Sin Viajes)', 0, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'EgresoMaterialNoProductivoContenedor') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('EgresoMaterialNoProductivoContenedor', 'Egreso Material No Productivo Por Contenedor', 0, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'EgresoClientesFasonConCP') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('EgresoClientesFasonConCP', 'Egreso a Clientes Fason con Carta de Porte', 1, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'RecepcionesVarias') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('RecepcionesVarias', 'Recepciones Varias', 0, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'EgresosVarios') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('EgresosVarios', 'Egresos Varios (Sin viajes)', 1, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'IngresoPorCompraDeGranosAutomatizado') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('IngresoPorCompraDeGranosAutomatizado', 'Ingreso Por Compra de Granos Automatizado', 0, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'EgresoPorExportaciones') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('EgresoPorExportaciones', 'Egreso Por Exportaciones', 1, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'RecepcionUvaPropia') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('RecepcionUvaPropia', 'Recepción de Uva Propia', 0, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'RecepcionUvaTerceros') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('RecepcionUvaTerceros', 'Recepción de Uva Terceros', 0, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'TransileInternoDeSemillas') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('TransileInternoDeSemillas', 'Transile Interno de Semillas', 0, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'RecepcionDeYerbaDeProductoresYTerceros') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('RecepcionDeYerbaDeProductoresYTerceros', 'Recepción de Yerba de Productores y Terceros', 0, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'RecepcionDeVinosDeTerceros') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('RecepcionDeVinosDeTerceros', 'Recepcion De Vino De Terceros', 0, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'IngresoPorRedespachoDeSemilla') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('IngresoPorRedespachoDeSemilla', 'Ingreso Por Redespacho de Semilla', 0, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'IngresoPorCompraDeSemilla') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('IngresoPorCompraDeSemilla', 'Ingreso Por Compra de Semilla', 0, (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Workflow where Codigo = 'EgresoPorRedespachoDeSemilla') BEGIN INSERT INTO [Workflow]([Codigo],[Descripcion],[TipoDeWorkflow],[Centro_Id]) VALUES ('EgresoPorRedespachoDeSemilla', 'Egreso Por Redespacho de Semilla', 1, (SELECT TOP 1 Id FROM Centro)) END


-- DocumentoDeImpresion
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'AsignacionDeRuta') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('AsignacionDeRuta', 'AsignacionDeRuta', 'AsignacionDeRuta') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'CertificacionDeCartaPorte') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('CertificacionDeCartaPorte', 'CertificacionDeCartaPorte', 'CertificacionDeCartaPorte') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'CertificadoDeAnalisis') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('CertificadoDeAnalisis', 'CertificadoDeAnalisis', 'CertificadoDeAnalisis') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'ConstanciaDeEntregaLaser') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('ConstanciaDeEntregaLaser', 'ConstanciaDeEntregaLaser', 'ConstanciaDeEntregaLaser') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'Formulario239') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('Formulario239', 'Formulario239', 'Formulario239') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'LoteACamara') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('LoteACamara', 'LoteACamara', 'LoteACamara') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'MuestraCalado') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('MuestraCalado', 'MuestraCalado', 'MuestraCalado') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'SolicitudDeAnalisis') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('SolicitudDeAnalisis', 'SolicitudDeAnalisis', 'SolicitudDeAnalisis') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'TicketPesada') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('TicketPesada', 'TicketPesada', 'TicketPesada') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'DeclaracionFosfina') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('DeclaracionFosfina', 'DeclaracionFosfina', 'DeclaracionFosfina') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'LibroMovimientosExistenciaGranos') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('LibroMovimientosExistenciaGranos', 'LibroMovimientosExistenciaGranos', 'LibroMovimientosExistenciaGranos') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'DocumentoEntrada') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('DocumentoEntrada', 'DocumentoEntrada', 'DocumentoEntrada') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'ResumenDeRecepcion') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('ResumenDeRecepcion', 'ResumenDeRecepcion', 'ResumenDeRecepcion') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'InformeDeRecepcion') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('InformeDeRecepcion', 'InformeDeRecepcion', 'InformeDeRecepcion') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'MuestraAuditoria') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('MuestraAuditoria', 'MuestraAuditoria', 'MuestraAuditoria') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'ConstanciaCIU') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('ConstanciaCIU', 'ConstanciaCIU', 'ConstanciaCIU') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'EtiquetaAuditoria') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('EtiquetaAuditoria', 'EtiquetaAuditoria', 'EtiquetaAuditoria') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'EtiquetaIntacta') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('EtiquetaIntacta', 'EtiquetaIntacta', 'EtiquetaIntacta') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'CartaDePorte') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('CartaDePorte', 'CartaDePorte', 'CartaDePorte') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'TicketPesadaAduana') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('TicketPesadaAduana', 'TicketPesadaAduana', 'TicketPesadaAduana') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'EtiquetaRubrosAnalizar') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('EtiquetaRubrosAnalizar', 'EtiquetaRubrosAnalizar', 'EtiquetaRubrosAnalizar') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'AsigRecorrCtrolCalid') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('AsigRecorrCtrolCalid', 'AsigRecorrCtrolCalid', 'AsigRecorrCtrolCalid') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'ImpGaritaSalida') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('ImpGaritaSalida', 'ImpGaritaSalida', 'ImpGaritaSalida') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'CartaDePorteElectronica') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('CartaDePorteElectronica', 'Impresion de Carta de Porte Electronica', 'CartaDePorteElectronica') END
IF NOT EXISTS (select 1 from DocumentoDeImpresion where Codigo = 'CartaPorteElectronicaDerivadoGranario') BEGIN INSERT INTO [DocumentoDeImpresion]([Codigo],[Descripcion],[DescripcionCorta]) VALUES ('CartaPorteElectronicaDerivadoGranario', 'Impresion de CPE Derivado Granario', 'CartaPorteElectronicaDerivadoGranario') END



--Tipo de Impresion
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Asignación de Ruta') BEGIN insert into TipoImpresion(Id, Descripcion) values (0, 'Asignación De Ruta'); END
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Certificado de Análisis') BEGIN insert into TipoImpresion(Id, Descripcion) values (1, 'Certificado De Análisis'); END
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Certificado de Carta de Porte') BEGIN insert into TipoImpresion(Id, Descripcion) values (2, 'Certificado de Carta de Porte'); END
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Constancia de Entrega Laser') BEGIN insert into TipoImpresion(Id, Descripcion) values (3, 'Constancia de Entrega Laser'); END
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Declaración Fosfina') BEGIN insert into TipoImpresion(Id, Descripcion) values (4, 'Declaración Fosfina'); END
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Documento de Entrada') BEGIN insert into TipoImpresion(Id, Descripcion) values (5, 'Documento de Entrada'); END
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Formulario 239') BEGIN insert into TipoImpresion(Id, Descripcion) values (6, 'Formulario 239'); END
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Identificación envío Lote a Cámara') BEGIN insert into TipoImpresion(Id, Descripcion) values (7, 'Identificación envío Lote a Cámara'); END
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Identificación Micromuestra') BEGIN insert into TipoImpresion(Id, Descripcion) values (8, 'Identificación Micromuestra'); END
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Identificación Muestra Calado') BEGIN insert into TipoImpresion(Id, Descripcion) values (9, 'Identificación Muestra Calado'); END
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Ticket Pesada') BEGIN insert into TipoImpresion(Id, Descripcion) values (10, 'Ticket Pesada'); END
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Solicitud de Análisis') BEGIN insert into TipoImpresion(Id, Descripcion) values (11, 'Solicitud de Análisis'); END
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Impresión Genérica') BEGIN insert into TipoImpresion(Id, Descripcion) values (12, 'Impresión Genérica'); END
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Resumen de Recepción') BEGIN insert into TipoImpresion(Id, Descripcion) values (13, 'Resumen de Recepción'); END
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Informe de Recepción') BEGIN insert into TipoImpresion(Id, Descripcion) values (14, 'Informe de Recepción'); END
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Recibo Municipal') BEGIN insert into TipoImpresion(Id, Descripcion) values (15, 'Recibo Municipal'); END
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Control de Carga') BEGIN insert into TipoImpresion(Id, Descripcion) values (16, 'Control de Carga'); END
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Identificación Muestra Auditoría') BEGIN insert into TipoImpresion(Id, Descripcion) values (17, 'Identificación Muestra Auditoría'); END
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Etiqueta Auditoría') BEGIN insert into TipoImpresion(Id, Descripcion) values (18, 'Etiqueta Auditoría'); END
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Etiqueta Intacta') BEGIN insert into TipoImpresion(Id, Descripcion) values (19, 'Etiqueta Intacta'); END
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Ticket Pesada Bodega') BEGIN insert into TipoImpresion(Id, Descripcion) values (20, 'Ticket Pesada Bodega'); END
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Ticket Pesada Aduana') BEGIN insert into TipoImpresion(Id, Descripcion) values (21, 'Ticket Pesada Aduana'); END
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Etiqueta Rubros a Analizar') BEGIN insert into TipoImpresion(Id, Descripcion) values (22, 'Etiqueta Rubros a Analizar'); END
IF NOT EXISTS (select 1 from TipoImpresion where Descripcion = 'Recibo Municipal Importacion') BEGIN insert into TipoImpresion(Id, Descripcion) values (23, 'Recibo Municipal Importacion'); END

-- Formato Papel
IF NOT EXISTS (select 1 from [FormatoDePapel] where [Descripcion] = 'A4') BEGIN INSERT INTO [FormatoDePapel]([Descripcion],[Ancho],[Alto]) VALUES ('A4', 210, 290) END
IF NOT EXISTS (select 1 from [FormatoDePapel] where [Descripcion] = 'Carta') BEGIN INSERT INTO [FormatoDePapel]([Descripcion],[Ancho],[Alto]) VALUES ('Carta', 216, 279) END
IF NOT EXISTS (select 1 from [FormatoDePapel] where [Descripcion] = 'Oficio') BEGIN INSERT INTO [FormatoDePapel]([Descripcion],[Ancho],[Alto]) VALUES ('Oficio', 216, 355) END

BEGIN UPDATE [FormatoDePapel] SET [CodigoTipoPapel] = 9 where [Descripcion] = 'A4' END
BEGIN UPDATE [FormatoDePapel] SET [CodigoTipoPapel] = 1 where [Descripcion] = 'Carta' END
BEGIN UPDATE [FormatoDePapel] SET [CodigoTipoPapel] = 5 where [Descripcion] = 'Oficio' END

-- Letra
IF NOT EXISTS (select 1 from [Letra] where [Descripcion] = 'Arial') BEGIN INSERT INTO [Letra]([Descripcion]) VALUES ('Arial') END
IF NOT EXISTS (select 1 from [Letra] where [Descripcion] = 'Times New Roman') BEGIN INSERT INTO [Letra]([Descripcion]) VALUES ('Times New Roman') END

-- FormatoDeImpresion
IF NOT EXISTS (select 1 from [FormatoDeImpresion] where [Descripcion] = 'ConstanciaCIU') BEGIN INSERT INTO [FormatoDeImpresion]([Descripcion],[FormatoDePapel_Id],[Posicion],[MargenIzquierdo],[MargenSuperior],[Filas],[Columnas]) VALUES ('ConstanciaCIU', (SELECT TOP 1 Id FROM [FormatoDePapel] where Descripcion = 'A4'), 0,10,10,22,2) END
IF NOT EXISTS (select 1 from [FormatoDeImpresion] where [Descripcion] = 'CartaDePorte') BEGIN INSERT INTO [FormatoDeImpresion]([Descripcion],[FormatoDePapel_Id],[Posicion],[MargenIzquierdo],[MargenSuperior],[Filas],[Columnas]) VALUES ('CartaDePorte', (SELECT TOP 1 Id FROM [FormatoDePapel] where Descripcion = 'A4'), 0,10,10,22,2) END
IF NOT EXISTS (select 1 from [FormatoDeImpresion] where [Descripcion] = 'LibroMovimientosExistenciaGranos') BEGIN INSERT INTO [FormatoDeImpresion]([Descripcion],[FormatoDePapel_Id],[Posicion],[MargenIzquierdo],[MargenSuperior],[Filas],[Columnas]) VALUES ('LibroMovimientosExistenciaGranos', (SELECT TOP 1 Id FROM [FormatoDePapel] where Descripcion = 'A4'), 1,10,10,32,9) END

-- Impresora
IF NOT EXISTS (select 1 from Impresora where Descripcion = 'ImpresoraDefault') BEGIN INSERT INTO [Impresora]([Descripcion],[Direccion],[Centro_Id]) VALUES ('ImpresoraDefault', '\\vicfsp02\PVicSiste010', (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from Impresora where Descripcion = 'ImpresoraTicketDefault') BEGIN INSERT INTO [Impresora]([Descripcion],[Direccion],[Centro_Id]) VALUES ('ImpresoraTicketDefault', '\\vicfsp02\PSLOSiste567', (SELECT TOP 1 Id FROM Centro)) END

-- DocumentoDeImpresionPorCentro
IF NOT EXISTS (select 1 from DocumentoDeImpresionPorCentro where DocumentoDeImpresion_Id = (SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'AsignacionDeRuta')) BEGIN INSERT INTO [DocumentoDeImpresionPorCentro]([DocumentoDeImpresion_Id],[Impresora_Id],[Centro_Id]) VALUES ((SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'AsignacionDeRuta'), (SELECT TOP 1 Id FROM Impresora where Descripcion = 'ImpresoraDefault'), (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from DocumentoDeImpresionPorCentro where DocumentoDeImpresion_Id = (SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'CertificacionDeCartaPorte')) BEGIN INSERT INTO [DocumentoDeImpresionPorCentro]([DocumentoDeImpresion_Id],[Impresora_Id],[Centro_Id]) VALUES ((SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'CertificacionDeCartaPorte'), (SELECT TOP 1 Id FROM Impresora where Descripcion = 'ImpresoraDefault'), (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from DocumentoDeImpresionPorCentro where DocumentoDeImpresion_Id = (SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'CertificadoDeAnalisis')) BEGIN INSERT INTO [DocumentoDeImpresionPorCentro]([DocumentoDeImpresion_Id],[Impresora_Id],[Centro_Id]) VALUES ((SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'CertificadoDeAnalisis'), (SELECT TOP 1 Id FROM Impresora where Descripcion = 'ImpresoraDefault'), (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from DocumentoDeImpresionPorCentro where DocumentoDeImpresion_Id = (SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'ConstanciaDeEntregaLaser')) BEGIN INSERT INTO [DocumentoDeImpresionPorCentro]([DocumentoDeImpresion_Id],[Impresora_Id],[Centro_Id]) VALUES ((SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'ConstanciaDeEntregaLaser'), (SELECT TOP 1 Id FROM Impresora where Descripcion = 'ImpresoraDefault'), (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from DocumentoDeImpresionPorCentro where DocumentoDeImpresion_Id = (SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'Formulario239')) BEGIN INSERT INTO [DocumentoDeImpresionPorCentro]([DocumentoDeImpresion_Id],[Impresora_Id],[Centro_Id]) VALUES ((SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'Formulario239'), (SELECT TOP 1 Id FROM Impresora where Descripcion = 'ImpresoraDefault'), (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from DocumentoDeImpresionPorCentro where DocumentoDeImpresion_Id = (SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'LoteACamara')) BEGIN INSERT INTO [DocumentoDeImpresionPorCentro]([DocumentoDeImpresion_Id],[Impresora_Id],[Centro_Id]) VALUES ((SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'LoteACamara'), (SELECT TOP 1 Id FROM Impresora where Descripcion = 'ImpresoraTicketDefault'), (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from DocumentoDeImpresionPorCentro where DocumentoDeImpresion_Id = (SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'MuestraCalado')) BEGIN INSERT INTO [DocumentoDeImpresionPorCentro]([DocumentoDeImpresion_Id],[Impresora_Id],[Centro_Id]) VALUES ((SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'MuestraCalado'), (SELECT TOP 1 Id FROM Impresora where Descripcion = 'ImpresoraTicketDefault'), (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from DocumentoDeImpresionPorCentro where DocumentoDeImpresion_Id = (SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'SolicitudDeAnalisis')) BEGIN INSERT INTO [DocumentoDeImpresionPorCentro]([DocumentoDeImpresion_Id],[Impresora_Id],[Centro_Id]) VALUES ((SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'SolicitudDeAnalisis'), (SELECT TOP 1 Id FROM Impresora where Descripcion = 'ImpresoraDefault'), (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from DocumentoDeImpresionPorCentro where DocumentoDeImpresion_Id = (SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'TicketPesada')) BEGIN INSERT INTO [DocumentoDeImpresionPorCentro]([DocumentoDeImpresion_Id],[Impresora_Id],[Centro_Id]) VALUES ((SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'TicketPesada'), (SELECT TOP 1 Id FROM Impresora where Descripcion = 'ImpresoraDefault'), (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from DocumentoDeImpresionPorCentro where DocumentoDeImpresion_Id = (SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'DeclaracionFosfina')) BEGIN INSERT INTO [DocumentoDeImpresionPorCentro]([DocumentoDeImpresion_Id],[Impresora_Id],[Centro_Id]) VALUES ((SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'DeclaracionFosfina'), (SELECT TOP 1 Id FROM Impresora where Descripcion = 'ImpresoraDefault'), (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from DocumentoDeImpresionPorCentro where DocumentoDeImpresion_Id = (SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'CartaDePorte')) BEGIN INSERT INTO [DocumentoDeImpresionPorCentro]([DocumentoDeImpresion_Id], [FormatoDeImpresion_Id],[Impresora_Id],[Centro_Id]) VALUES ((SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'CartaDePorte'), (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'CartaDePorte'), (SELECT TOP 1 Id FROM Impresora where Descripcion = 'ImpresoraDefault'), (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from DocumentoDeImpresionPorCentro where DocumentoDeImpresion_Id = (SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'LibroMovimientosExistenciaGranos')) BEGIN INSERT INTO [DocumentoDeImpresionPorCentro]([DocumentoDeImpresion_Id], [FormatoDeImpresion_Id],[Impresora_Id],[Centro_Id]) VALUES ((SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'LibroMovimientosExistenciaGranos'), (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos'), (SELECT TOP 1 Id FROM Impresora where Descripcion = 'ImpresoraDefault'), (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from DocumentoDeImpresionPorCentro where DocumentoDeImpresion_Id = (SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'DocumentoEntrada')) BEGIN INSERT INTO [DocumentoDeImpresionPorCentro]([DocumentoDeImpresion_Id],[Impresora_Id],[Centro_Id]) VALUES ((SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'DocumentoEntrada'), (SELECT TOP 1 Id FROM Impresora where Descripcion = 'ImpresoraDefault'), (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from DocumentoDeImpresionPorCentro where DocumentoDeImpresion_Id = (SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'InformeDeRecepcion')) BEGIN INSERT INTO [DocumentoDeImpresionPorCentro]([DocumentoDeImpresion_Id],[Impresora_Id],[Centro_Id]) VALUES ((SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'InformeDeRecepcion'), (SELECT TOP 1 Id FROM Impresora where Descripcion = 'ImpresoraDefault'), (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from DocumentoDeImpresionPorCentro where DocumentoDeImpresion_Id = (SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'MuestraAuditoria')) BEGIN INSERT INTO [DocumentoDeImpresionPorCentro]([DocumentoDeImpresion_Id],[Impresora_Id],[Centro_Id]) VALUES ((SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'MuestraAuditoria'), (SELECT TOP 1 Id FROM Impresora where Descripcion = 'ImpresoraDefault'), (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from DocumentoDeImpresionPorCentro where DocumentoDeImpresion_Id = (SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'ConstanciaCIU')) BEGIN INSERT INTO [DocumentoDeImpresionPorCentro]([DocumentoDeImpresion_Id],[Impresora_Id],[Centro_Id]) VALUES ((SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'ConstanciaCIU'), (SELECT TOP 1 Id FROM Impresora where Descripcion = 'ImpresoraDefault'), (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from DocumentoDeImpresionPorCentro where DocumentoDeImpresion_Id = (SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'EtiquetaAuditoria')) BEGIN INSERT INTO [DocumentoDeImpresionPorCentro]([DocumentoDeImpresion_Id],[Impresora_Id],[Centro_Id]) VALUES ((SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'EtiquetaAuditoria'), (SELECT TOP 1 Id FROM Impresora where Descripcion = 'ImpresoraDefault'), (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from DocumentoDeImpresionPorCentro where DocumentoDeImpresion_Id = (SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'EtiquetaRubrosAnalizar')) BEGIN INSERT INTO [DocumentoDeImpresionPorCentro]([DocumentoDeImpresion_Id],[Impresora_Id],[Centro_Id]) VALUES ((SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'EtiquetaRubrosAnalizar'), (SELECT TOP 1 Id FROM Impresora where Descripcion = 'ImpresoraDefault'), (SELECT TOP 1 Id FROM Centro)) END
IF NOT EXISTS (select 1 from DocumentoDeImpresionPorCentro where DocumentoDeImpresion_Id = (SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'AsigRecorrCtrolCalid')) BEGIN INSERT INTO [DocumentoDeImpresionPorCentro]([DocumentoDeImpresion_Id],[Impresora_Id],[Centro_Id]) VALUES ((SELECT TOP 1 Id FROM DocumentoDeImpresion where Codigo = 'AsigRecorrCtrolCalid'), (SELECT TOP 1 Id FROM Impresora where Descripcion = 'ImpresoraDefault'), (SELECT TOP 1 Id FROM Centro)) END


-- Campos
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'CTG') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('CTG','CTG') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'FechaEmision') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Fecha de Emisión','FechaEmision') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'FechaCP') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Fecha Carta de Porte','FechaCP') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'FechaVencimiento') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Fecha de Vencimiento','FechaVencimiento') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'TitularCP') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Titular Carta de Porte','TitularCP') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'CuitTitularCP') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Cuit Titular Carta de Porte','CuitTitularCP') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'Intermediario') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Intermediario','Intermediario') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'CuitIntermediario') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Cuit Intermediario','CuitIntermediario') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'RtteComercial') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Rtte Comercial','RtteComercial') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'CuitRtteComercial') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Cuit Rtte Comercial','CuitRtteComercial') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'Corredor') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Corredor','Corredor') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'CuitCorredor') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Cuit Corredor','CuitCorredor') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'Entregador') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Entregador','Entregador') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'CuitEntregador') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Cuit Entregador','CuitEntregador') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'Destinatario') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Destinatario','Destinatario') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'CuitDestinatario') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Cuit Destinatario','CuitDestinatario') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'Transportista') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Transportista','Transportista') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'CuitTransportista') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Cuit Transportista','CuitTransportista') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'Chofer') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Chofer','Chofer') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'CuitChofer') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Cuit Chofer','CuitChofer') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'Material') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Material','Material') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'Variedad') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Variedad','Variedad') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'Cosecha') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Cosecha','Cosecha') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'Procedencia') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Procedencia','Procedencia') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'CodigoEstablecimiento') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Codigo Establecimiento','CodigoEstablecimiento') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'PesoBrutoOrigen') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Peso Bruto Origen','PesoBrutoOrigen') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'PesoTaraOrigen') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Peso Tara Origen','PesoTaraOrigen') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'PesoNetoOrigen') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Peso Neto Origen','PesoNetoOrigen') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'Patente') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Patente','Patente') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'PatenteAcoplado') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Patente Acoplado','PatenteAcoplado') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'KmARecorrer') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Km a Recorrer','KmARecorrer') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'TarifaReferencia') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Tarifa Referencia','TarifaReferencia') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'TarifaTonelada') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Tarifa Tonelada','TarifaTonelada') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'CodigoAnexo') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Codigo Anexo','CodigoAnexo') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'Prestador') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Prestador','Prestador') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'CuitPrestador') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Cuit Prestador','CuitPrestador') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'BocaDestino') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Boca de Destino','BocaDestino') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'DomicilioBocaDestino') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Domicilio Boca de Destino','DomicilioBocaDestino') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'ProvinciaBocaDestino') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Provincia Boca de Destino','ProvinciaBocaDestino') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'LocalidadBocaDestino') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Localidad Boca de Destino','LocalidadBocaDestino') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'AcuerdoMarco') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Acuerdo Marco','AcuerdoMarco') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'Caratula') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Caratula','Caratula') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'PesoBruto') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Peso Bruto','PesoBruto') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'PesoTara') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Peso Tara','PesoTara') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'PesoNeto') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Peso Neto','PesoNeto') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'LocalidadCentroOrigen') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Localidad de Centro Origen','LocalidadCentroOrigen') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'ProvinciaCentroOrigen') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Provincia de Centro Origen','ProvinciaCentroOrigen') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'CodigoPostalCentroOrigen') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Codigo Postal de Centro Origen','CodigoPostalCentroOrigen') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'DireccionCentroOrigen') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Dirección de Centro Origen','DireccionCentroOrigen') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'Destino') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Centro Destino','Destino') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'CuitDestino') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Cuit Centro Destino','CuitDestino') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'LocalidadCentroDestino') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Localidad de Centro Destino','LocalidadCentroDestino') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'ProvinciaCentroDestino') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Provincia de Centro Destino','ProvinciaCentroDestino') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'CodigoPostaLCentroDestino') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Codigo Postal de Centro Destino','CodigoPostaLCentroDestino') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'DireccionCentroDestino') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Dirección de Centro Destino','DireccionCentroDestino') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'LocalidadClienteDestino') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Localidad de Cliente Destino','LocalidadClienteDestino') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'ProvinciaClienteDestino') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Provincia de Cliente Destino','ProvinciaClienteDestino') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'DireccionClienteDestino') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Dirección de Cliente Destino','DireccionClienteDestino') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'TipoDocumentoIngreso') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Tipo de Documento de Ingreso','TipoDocumentoIngreso') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'TipoDeComprobanteONCCA') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Tipo de Comprobante ONCCA','TipoDeComprobanteONCCA') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'NumeroDeDocumentoDeIngreso') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Numero de Documento de Ingreso','NumeroDeDocumentoDeIngreso') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'SaldosSTOCK') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Saldos STOCK','SaldosSTOCK') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'ObservacionesONCCA') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Observaciones ONCCA','ObservacionesONCCA') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'FletePagado') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Flete Pagado','FletePagado') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'FleteAPagar') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Flete a Pagar','FleteAPagar') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'Observaciones') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Observaciones','Observaciones') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'TextoLibre') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Texto Libre','TextoLibre') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'MaterialCodigoONCCA') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Material Codigo ONCCA', 'MaterialCodigoONCCA') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'MaterialDescCorta') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Material Desc Corta', 'MaterialDescCorta') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'PesoNetoSinHumedad') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Peso Neto Sin Humedad', 'PesoNetoSinHumedad') END

IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'NumeroCiu') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Número CIU', 'NumeroCiu') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'INVBodega') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('INV Bodega', 'INVBodega') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'CuitBodega') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Cuit Bodega', 'CuitBodega') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'IIBBBodega') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('IIBB Bodega', 'IIBBBodega') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'RazonSocialVinatero') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Razón Social Vinatero', 'RazonSocialVinatero') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'INVVinatero') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('INV Viñatero', 'INVVinatero') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'CuitViñatero') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Cuit Viñatero', 'CuitViñatero') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'IIBBViñatero') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('IIBB Viñatero', 'IIBBViñatero') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'EsCamion') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Es Camión', 'EsCamion') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'EsAcoplado') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Es Acoplado', 'EsAcoplado') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'EsBines') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Es Bines', 'EsBines') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'EsMoliendaEnVinedos') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Es Molienda en Viñedos', 'EsMoliendaEnVinedos') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'EsTractor') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Es Tractor', 'EsTractor') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'MarcaVehiculo') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Marca Vehiculo', 'MarcaVehiculo') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'ModeloVehiculo') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Modelo Vehiculo', 'ModeloVehiculo') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'INVVariedad') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('INV Variedad', 'INVVariedad') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'TenorAzucarino') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Tenor Azucarino', 'TenorAzucarino') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'EsUvaPropia') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Es Uva Propia', 'EsUvaPropia') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'EsUvaTerceros') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Es Uva Terceros', 'EsUvaTerceros') END

IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'RazonSocialBodega') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Razón Social Bodega', 'RazonSocialBodega') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'FechaPesoNetoBodega') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Fecha Peso Neto Bodega', 'FechaPesoNetoBodega') END

IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'CorredorVendedor') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Corredor Vendedor', 'CorredorVendedor') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'CuitCorredorVendedor') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Cuit Corredor Vendedor', 'CuitCorredorVendedor') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'IntermediarioFlete') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Intermediario Flete', 'IntermediarioFlete') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'MercadoATermino') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Mercado A Termino', 'MercadoATermino') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'CuitMercadoATermino') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Cuit Mercado A Termino', 'CuitMercadoATermino') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'CuitIntermediarioDelFlete') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('Cuit Intermediario Del Flete', 'CuitIntermediarioDelFlete') END
IF NOT EXISTS (select 1 from [Campo] where [Direccion] = 'CTGSap') BEGIN INSERT INTO [Campo]([Descripcion],[Direccion]) VALUES ('CTG SAP','CTGSap') END


-- Formato de Campos
IF NOT EXISTS (select 1 from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'NumeroDeDocumentoDeIngreso') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'CartaDePorte')) BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'NumeroDeDocumentoDeIngreso'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'CartaDePorte'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),0,1,0,10,0,0,0,0,0) END
IF NOT EXISTS (select 1 from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'TitularCP') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'CartaDePorte')) BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'TitularCP'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'CartaDePorte'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),0,1,1,10,0,0,0,0,0) END
IF NOT EXISTS (select 1 from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'Chofer') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'CartaDePorte')) BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'Chofer'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'CartaDePorte'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),0,2,0,10,0,0,0,0,0) END
IF NOT EXISTS (select 1 from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'Material') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'CartaDePorte')) BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'Material'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'CartaDePorte'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),0,2,1,10,0,0,0,0,0) END
IF NOT EXISTS (select 1 from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'Patente') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'CartaDePorte')) BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'Patente'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'CartaDePorte'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),0,3,0,10,0,0,0,0,0) END
--
IF NOT EXISTS (select 1 from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'FechaCP') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos')) BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'FechaCP'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),1,7,0,9,0,0,0,1,0) END
IF NOT EXISTS (select 1 from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'NumeroDeDocumentoDeIngreso') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos')) BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'NumeroDeDocumentoDeIngreso'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),1,7,1,9,0,0,0,1,0) END
IF NOT EXISTS (select 1 from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'CuitDestinatario') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos')) BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'CuitDestinatario'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),0,7,2,9,0,0,0,1,2) END
IF NOT EXISTS (select 1 from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'PesoBruto') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos')) BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'PesoBruto'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),0,7,4,9,0,0,0,1,1) END
IF NOT EXISTS (select 1 from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'PesoNeto') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos')) BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'PesoNeto'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),0,7,5,9,0,0,0,1,1) END
IF (select COUNT(id) from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'PesoNeto') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos')) < 2 BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'PesoNeto'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),0,7,6,9,0,0,0,1,2) END
IF NOT EXISTS (select 1 from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'TipoDeComprobanteONCCA') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos')) BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'TipoDeComprobanteONCCA'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),0,7,1,9,0,0,0,1,0) END
IF NOT EXISTS (select 1 from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'Destinatario') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos')) BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'Destinatario'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),0,7,3,9,0,0,0,1,2) END
IF NOT EXISTS (select 1 from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'Material') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos')) BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'Material'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),1,5,1,9,0,0,0,0,0) END
IF NOT EXISTS (select 1 from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'RtteComercial') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos')) BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'RtteComercial'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),0,7,3,9,0,0,0,1,1) END
IF NOT EXISTS (select 1 from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'CuitRtteComercial') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos')) BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'CuitRtteComercial'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),0,7,2,9,0,0,0,1,1) END
IF NOT EXISTS (select 1 from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'SaldosSTOCK') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos')) BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'SaldosSTOCK'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),0,7,7,9,0,0,0,1,0) END
IF NOT EXISTS (select 1 from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'ObservacionesONCCA') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos')) BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'ObservacionesONCCA'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),1,7,8,9,0,0,0,1,0) END
--
IF NOT EXISTS (select 1 from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'NumeroDeDocumentoDeIngreso') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'ConstanciaCIU')) BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'NumeroDeDocumentoDeIngreso'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'CartaDePorte'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),0,1,0,10,0,0,0,0,0) END
IF NOT EXISTS (select 1 from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'TitularCP') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'ConstanciaCIU')) BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'TitularCP'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'CartaDePorte'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),0,1,1,10,0,0,0,0,0) END
IF NOT EXISTS (select 1 from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'Chofer') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'ConstanciaCIU')) BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'Chofer'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'CartaDePorte'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),0,2,0,10,0,0,0,0,0) END
IF NOT EXISTS (select 1 from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'Material') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'ConstanciaCIU')) BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'Material'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'CartaDePorte'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),0,2,1,10,0,0,0,0,0) END
IF NOT EXISTS (select 1 from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'Patente') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'ConstanciaCIU')) BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'Patente'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'CartaDePorte'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),0,3,0,10,0,0,0,0,0) END
IF NOT EXISTS (select 1 from [FormatoDeCampo] where [Campo_Id] = (SELECT TOP 1 Id FROM Campo where Direccion = 'CTGSap') AND [FormatoDeImpresion_Id] =  (SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos')) BEGIN INSERT INTO [FormatoDeCampo]([Campo_Id],[FormatoDeImpresion_Id],[Letra_Id],[Alineacion],[Fila],[Columna],[Tamaño],[Negrita],[Cursiva],[Subrayado],[EsColumna],[TipoDeCampo]) VALUES ((SELECT TOP 1 Id FROM Campo where Direccion = 'CTGSap'),(SELECT TOP 1 Id FROM FormatoDeImpresion where Descripcion = 'LibroMovimientosExistenciaGranos'),(SELECT TOP 1 Id FROM Letra where Descripcion = 'Arial'),0,3,0,10,0,0,0,0,0) END

-- Variedad
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='ALICANT BOUCHET') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('ALICANT BOUCHET','126') END
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='AMEIS') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('AMEIS','266') END
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='ANCELLOTTA') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('ANCELLOTTA','152') END
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='ARAMON') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('ARAMON','304') END
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='ASPIRANT BOUCHET') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('ASPIRANT BOUCHET','125') END
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='BARBERA') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('BARBERA','103') END
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='BASTARDO') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('BASTARDO','142') END
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='BEQUIGNOL') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('BEQUIGNOL','131') END
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='BONARDA') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('BONARDA','104') END
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='BUONAMICO') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('BUONAMICO','302') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='CABERINTA') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('CABERINTA','144') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='CABERNET FRANC') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('CABERNET FRANC','132') END   
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='CABERNET SAUVIGNON') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('CABERNET SAUVIGNON','133') END
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='CANARI') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('CANARI','129') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='CANELA') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('CANELA','305') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='CARIGNAN') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('CARIGNAN','117') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='CEREZA') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('CEREZA','310') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='CESAR') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('CESAR','121') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='CG 13668') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('CG 13668','255') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='CG 1730') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('CG 1730','259') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='CG 2539') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('CG 2539','136') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='CG 26189') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('CG 26189','145') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='CG 26879') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('CG 26879','260') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='CG 34047') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('CG 34047','147') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='CG 4113') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('CG 4113','137') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='CG 4253') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('CG 4253','138') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='CG 45803') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('CG 45803','261') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='CHARDONNAY') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('CHARDONNAY','245') END           
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='CHENIN') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('CHENIN','249') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='CINZAUT') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('CINZAUT','135') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='COLEC.AMPELOGRAFICA') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('COLEC.AMPELOGRAFICA','990') END
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='CRIOLLA CHICA') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('CRIOLLA CHICA','301') END     
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='CRIOLLA GRANDE') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('CRIOLLA GRANDE','300') END   
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='CRIOLLA MEDIANA') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('CRIOLLA MEDIANA','303') END 
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='DOLCETTO') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('DOLCETTO','123') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='ELBLING') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('ELBLING','250') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='FAVORITA DIAZ') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('FAVORITA DIAZ','114') END     
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='FER') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('FER','128') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='FERRAL') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('FERRAL','306') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='FINTENDO') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('FINTENDO','149') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='FREISA') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('FREISA','108') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='GAMAY') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('GAMAY','134') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='GARNACHA') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('GARNACHA','307') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='GIBBI') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('GIBBI','263') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='GRACIANA') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('GRACIANA','119') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='GRECO NEGRO') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('GRECO NEGRO','122') END         
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='LAMBRUSCO MAESTRI') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('LAMBRUSCO MAESTRI','111') END
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='MACABEO') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('MACABEO','258') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='MALBECK') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('MALBECK','101') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='MATICHA') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('MATICHA','256') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='MELON') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('MELON','267') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='MERLOT') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('MERLOT','113') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='MEUNIER') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('MEUNIER','105') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='MONASTRELL') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('MONASTRELL','106') END           
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='MOSCATEL AMARILLO') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('MOSCATEL AMARILLO','264') END
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='MOSCATEL DE ALEJANDRIA') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('MOSCATEL DE ALEJANDRIA','251') END
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='MOSCATEL DE FRONTIGNAN') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('MOSCATEL DE FRONTIGNAN','252') END
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='MOSCATEL ROSADA') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('MOSCATEL ROSADA','311') END 
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='MOSCATO D'' ASTI') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('MOSCATO D'' ASTI','243') END 
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='NEBBIOLO') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('NEBBIOLO','109') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='OTRAS BLANCAS  VINIFICAR') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('OTRAS BLANCAS  VINIFICAR','262') END
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='OTRAS ROSADAS VINIFICAR') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('OTRAS ROSADAS VINIFICAR','313') END
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='OTRAS TINTAS DE VINIFICAR') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('OTRAS TINTAS DE VINIFICAR','150') END
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='PALOMINO') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('PALOMINO','241') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='PEDRO GIMENEZ') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('PEDRO GIMENEZ','231') END     
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='PINOT BLANCO') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('PINOT BLANCO','268') END       
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='PINOT GRIS') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('PINOT GRIS','314') END           
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='PINOT JOUBERTIN') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('PINOT JOUBERTIN','143') END 
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='PINOT NEGRO') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('PINOT NEGRO','130') END         
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='PROSECO') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('PROSECO','257') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='RABOSO VERONES') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('RABOSO VERONES','116') END   
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='RIESLINA (CG 38049)') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('RIESLINA (CG 38049)','254') END
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='RIESLING') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('RIESLING','239') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='RUBY CABERNET') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('RUBY CABERNET','139') END     
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='SAINT JEANNETT') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('SAINT JEANNETT','237') END   
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='SANGIOVESSE') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('SANGIOVESSE','110') END         
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='SAUVIGNON') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('SAUVIGNON','233') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='SAUVIGNON GRIS') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('SAUVIGNON GRIS','315') END   
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='SEMILLON') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('SEMILLON','232') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='SILVANER') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('SILVANER','253') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='SIRAH') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('SIRAH','120') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='SULTANINA BLANCA') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('SULTANINA BLANCA','701') END
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='TANNAT') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('TANNAT','112') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='TEMPRANILLA') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('TEMPRANILLA','118') END         
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='TOCAI FRIULANO') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('TOCAI FRIULANO','246') END   
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='TORRONTES MENDOCINO') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('TORRONTES MENDOCINO','240') END
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='TORRONTES RIOJANO') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('TORRONTES RIOJANO','247') END
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='TORRONTES SANJUANINO') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('TORRONTES SANJUANINO','248') END
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='TRAMINER') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('TRAMINER','309') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='UGNI BLANC') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('UGNI BLANC','236') END           
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='VALENCY') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('VALENCY','312') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='VERDOT') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('VERDOT','140') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='VIOGNIER') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('VIOGNIER','265') END             
IF NOT EXISTS (select 1 from [Variedad] where [Descripcion] ='ZINFANDEL') BEGIN INSERT INTO [Variedad]([Descripcion],[NumeroINV]) VALUES ('ZINFANDEL','107') END

-- Zonas --

IF NOT EXISTS (select 1 from Zona where Descripcion = '25 DE MAYO') BEGIN insert into Zona values ('25 DE MAYO') END;
IF NOT EXISTS (select 1 from Zona where Descripcion = 'ANGACO') BEGIN insert into Zona values ('ANGACO')END;
IF NOT EXISTS (select 1 from Zona where Descripcion = 'CAUCETE') BEGIN insert into Zona values ('CAUCETE')END;
IF NOT EXISTS (select 1 from Zona where Descripcion = 'EL MARCADO') BEGIN insert into Zona values ('EL MARCADO')END;
IF NOT EXISTS (select 1 from Zona where Descripcion = 'GUAYMALLEN') BEGIN insert into Zona values ('GUAYMALLEN')END;
IF NOT EXISTS (select 1 from Zona where Descripcion = 'JUNIN') BEGIN insert into Zona values ('JUNIN')END;
IF NOT EXISTS (select 1 from Zona where Descripcion = 'LA PAZ') BEGIN insert into Zona values ('LA PAZ')END;
IF NOT EXISTS (select 1 from Zona where Descripcion = 'LAVALLE') BEGIN insert into Zona values ('LAVALLE')END;
IF NOT EXISTS (select 1 from Zona where Descripcion = 'LUJAN DE CUYO') BEGIN insert into Zona values ('LUJAN DE CUYO')END;
IF NOT EXISTS (select 1 from Zona where Descripcion = 'MAIPU') BEGIN insert into Zona values ('MAIPU')END;
IF NOT EXISTS (select 1 from Zona where Descripcion = 'RIVADAVIA') BEGIN insert into Zona values ('RIVADAVIA')END;
IF NOT EXISTS (select 1 from Zona where Descripcion = 'SAN CARLOS') BEGIN insert into Zona values ('SAN CARLOS')END;
IF NOT EXISTS (select 1 from Zona where Descripcion = 'SAN MARTIN') BEGIN insert into Zona values ('SAN MARTIN')END;
IF NOT EXISTS (select 1 from Zona where Descripcion = 'SANTA ROSA') BEGIN insert into Zona values ('SANTA ROSA')END;
IF NOT EXISTS (select 1 from Zona where Descripcion = 'SARMIENTO') BEGIN insert into Zona values ('SARMIENTO')END;
IF NOT EXISTS (select 1 from Zona where Descripcion = 'TUNUYAN') BEGIN insert into Zona values ('TUNUYAN')END;
IF NOT EXISTS (select 1 from Zona where Descripcion = 'TUPUNGATO') BEGIN insert into Zona values ('TUPUNGATO')END;
IF NOT EXISTS (select 1 from Zona where Descripcion = 'ULLUM') BEGIN insert into Zona values ('ULLUM')END;
IF NOT EXISTS (select 1 from Zona where Descripcion = 'ZONDA') BEGIN insert into Zona values ('ZONDA')END;

-- SubZonas --

IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'LA CHIMBERA' and z.Descripcion = '25 DE MAYO') BEGIN insert into SubZona values ((select id from zona where Descripcion='25 DE MAYO'),'LA CHIMBERA')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'LAS CASUARINAS' and z.Descripcion = '25 DE MAYO') BEGIN insert into SubZona values ((select id from zona where Descripcion='25 DE MAYO'),'LAS CASUARINAS')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'POZO SALADO' and z.Descripcion = '25 DE MAYO') BEGIN insert into SubZona values ((select id from zona where Descripcion='25 DE MAYO'),'POZO SALADO')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'RINCON DE TUPELI' and z.Descripcion = '25 DE MAYO') BEGIN insert into SubZona values ((select id from zona where Descripcion='25 DE MAYO'),'RINCON DE TUPELI')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'VILLA BORJAS-LA CHIMBERA' and z.Descripcion = '25 DE MAYO') BEGIN insert into SubZona values ((select id from zona where Descripcion='25 DE MAYO'),'VILLA BORJAS-LA CHIMBERA')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'PLUMERILLO' and z.Descripcion = 'ANGACO') BEGIN insert into SubZona values ((select id from zona where Descripcion='ANGACO'),'PLUMERILLO')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'RINCON' and z.Descripcion = 'CAUCETE') BEGIN insert into SubZona values ((select id from zona where Descripcion='CAUCETE'),'RINCON')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = '12 DE OCTUBRE' and z.Descripcion = 'EL MARCADO') BEGIN insert into SubZona values ((select id from zona where Descripcion='EL MARCADO'),'12 DE OCTUBRE')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'COLONIA SEGOVIA' and z.Descripcion = 'GUAYMALLEN') BEGIN insert into SubZona values ((select id from zona where Descripcion='GUAYMALLEN'),'COLONIA SEGOVIA')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'JUNIN' and z.Descripcion = 'JUNIN') BEGIN insert into SubZona values ((select id from zona where Descripcion='JUNIN'),'JUNIN')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'LA COLONIA' and z.Descripcion = 'JUNIN') BEGIN insert into SubZona values ((select id from zona where Descripcion='JUNIN'),'LA COLONIA')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'MEDRANO' and z.Descripcion = 'JUNIN') BEGIN insert into SubZona values ((select id from zona where Descripcion='JUNIN'),'MEDRANO')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'PHILLIPS' and z.Descripcion = 'JUNIN') BEGIN insert into SubZona values ((select id from zona where Descripcion='JUNIN'),'PHILLIPS')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'VILLA NUEVA' and z.Descripcion = 'LA PAZ') BEGIN insert into SubZona values ((select id from zona where Descripcion='LA PAZ'),'VILLA NUEVA')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'COSTA DE ARAUJO' and z.Descripcion = 'LAVALLE') BEGIN insert into SubZona values ((select id from zona where Descripcion='LAVALLE'),'COSTA DE ARAUJO')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'EL VERGEL' and z.Descripcion = 'LAVALLE') BEGIN insert into SubZona values ((select id from zona where Descripcion='LAVALLE'),'EL VERGEL')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'JOCOLI' and z.Descripcion = 'LAVALLE') BEGIN insert into SubZona values ((select id from zona where Descripcion='LAVALLE'),'JOCOLI')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'LA HOLANDA' and z.Descripcion = 'LAVALLE') BEGIN insert into SubZona values ((select id from zona where Descripcion='LAVALLE'),'LA HOLANDA')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'AGRELO' and z.Descripcion = 'LUJAN DE CUYO') BEGIN insert into SubZona values ((select id from zona where Descripcion='LUJAN DE CUYO'),'AGRELO')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'CARRODILLA' and z.Descripcion = 'LUJAN DE CUYO') BEGIN insert into SubZona values ((select id from zona where Descripcion='LUJAN DE CUYO'),'CARRODILLA')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'CHACRAS DE CORIA' and z.Descripcion = 'LUJAN DE CUYO') BEGIN insert into SubZona values ((select id from zona where Descripcion='LUJAN DE CUYO'),'CHACRAS DE CORIA')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'EL CARRIZAL' and z.Descripcion = 'LUJAN DE CUYO') BEGIN insert into SubZona values ((select id from zona where Descripcion='LUJAN DE CUYO'),'EL CARRIZAL')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'LUJAN DE CUYO' and z.Descripcion = 'LUJAN DE CUYO') BEGIN insert into SubZona values ((select id from zona where Descripcion='LUJAN DE CUYO'),'LUJAN DE CUYO')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'PERDRIEL' and z.Descripcion = 'LUJAN DE CUYO') BEGIN insert into SubZona values ((select id from zona where Descripcion='LUJAN DE CUYO'),'PERDRIEL')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'UGARTECHE' and z.Descripcion = 'LUJAN DE CUYO') BEGIN insert into SubZona values ((select id from zona where Descripcion='LUJAN DE CUYO'),'UGARTECHE')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'VISTALBA' and z.Descripcion = 'LUJAN DE CUYO') BEGIN insert into SubZona values ((select id from zona where Descripcion='LUJAN DE CUYO'),'VISTALBA')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'BARRANCAS' and z.Descripcion = 'MAIPU') BEGIN insert into SubZona values ((select id from zona where Descripcion='MAIPU'),'BARRANCAS')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'COQUIMBITO' and z.Descripcion = 'MAIPU') BEGIN insert into SubZona values ((select id from zona where Descripcion='MAIPU'),'COQUIMBITO')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'FRAY LUIS BELTRAN' and z.Descripcion = 'MAIPU') BEGIN insert into SubZona values ((select id from zona where Descripcion='MAIPU'),'FRAY LUIS BELTRAN')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'LOS ALAMOS' and z.Descripcion = 'MAIPU') BEGIN insert into SubZona values ((select id from zona where Descripcion='MAIPU'),'LOS ALAMOS')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'RUSSELL' and z.Descripcion = 'MAIPU') BEGIN insert into SubZona values ((select id from zona where Descripcion='MAIPU'),'RUSSELL')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'ANDRADE' and z.Descripcion = 'RIVADAVIA') BEGIN insert into SubZona values ((select id from zona where Descripcion='RIVADAVIA'),'ANDRADE')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'CAMPAMENTOS' and z.Descripcion = 'RIVADAVIA') BEGIN insert into SubZona values ((select id from zona where Descripcion='RIVADAVIA'),'CAMPAMENTOS')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'LOS ARBOLES' and z.Descripcion = 'RIVADAVIA') BEGIN insert into SubZona values ((select id from zona where Descripcion='RIVADAVIA'),'LOS ARBOLES')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'MEDRANO' and z.Descripcion = 'RIVADAVIA') BEGIN insert into SubZona values ((select id from zona where Descripcion='RIVADAVIA'),'MEDRANO')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'RIVADAVIA' and z.Descripcion = 'RIVADAVIA') BEGIN insert into SubZona values ((select id from zona where Descripcion='RIVADAVIA'),'RIVADAVIA')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'EUGENIO BUSTOS' and z.Descripcion = 'SAN CARLOS') BEGIN insert into SubZona values ((select id from zona where Descripcion='SAN CARLOS'),'EUGENIO BUSTOS')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'LA CONSULTA' and z.Descripcion = 'SAN CARLOS') BEGIN insert into SubZona values ((select id from zona where Descripcion='SAN CARLOS'),'LA CONSULTA')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'CHIVILCOY' and z.Descripcion = 'SAN MARTIN') BEGIN insert into SubZona values ((select id from zona where Descripcion='SAN MARTIN'),'CHIVILCOY')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'DIVISADERO' and z.Descripcion = 'SAN MARTIN') BEGIN insert into SubZona values ((select id from zona where Descripcion='SAN MARTIN'),'DIVISADERO')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'DOS ACEQUIAS' and z.Descripcion = 'SAN MARTIN') BEGIN insert into SubZona values ((select id from zona where Descripcion='SAN MARTIN'),'DOS ACEQUIAS')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'MONTECASEROS' and z.Descripcion = 'SAN MARTIN') BEGIN insert into SubZona values ((select id from zona where Descripcion='SAN MARTIN'),'MONTECASEROS')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'TRES PORTEÑAS' and z.Descripcion = 'SAN MARTIN') BEGIN insert into SubZona values ((select id from zona where Descripcion='SAN MARTIN'),'TRES PORTEÑAS')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = '12 DE OCTUBRE' and z.Descripcion = 'SANTA ROSA') BEGIN insert into SubZona values ((select id from zona where Descripcion='SANTA ROSA'),'12 DE OCTUBRE')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'BARRIO DOCE DE OCTUBRE' and z.Descripcion = 'SANTA ROSA') BEGIN insert into SubZona values ((select id from zona where Descripcion='SANTA ROSA'),'BARRIO DOCE DE OCTUBRE')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'EL DIVISADERO' and z.Descripcion = 'SANTA ROSA') BEGIN insert into SubZona values ((select id from zona where Descripcion='SANTA ROSA'),'EL DIVISADERO')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'SANTA ROSA' and z.Descripcion = 'SANTA ROSA') BEGIN insert into SubZona values ((select id from zona where Descripcion='SANTA ROSA'),'SANTA ROSA')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'COCHAGUAL' and z.Descripcion = 'SARMIENTO') BEGIN insert into SubZona values ((select id from zona where Descripcion='SARMIENTO'),'COCHAGUAL')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'PEDERNAL' and z.Descripcion = 'SARMIENTO') BEGIN insert into SubZona values ((select id from zona where Descripcion='SARMIENTO'),'PEDERNAL')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'VILLA MEDIA AGUA' and z.Descripcion = 'SARMIENTO') BEGIN insert into SubZona values ((select id from zona where Descripcion='SARMIENTO'),'VILLA MEDIA AGUA')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'EL TOPON' and z.Descripcion = 'TUNUYAN') BEGIN insert into SubZona values ((select id from zona where Descripcion='TUNUYAN'),'EL TOPON')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'EL TOTORAL' and z.Descripcion = 'TUNUYAN') BEGIN insert into SubZona values ((select id from zona where Descripcion='TUNUYAN'),'EL TOTORAL')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'LOS ARBOLES' and z.Descripcion = 'TUNUYAN') BEGIN insert into SubZona values ((select id from zona where Descripcion='TUNUYAN'),'LOS ARBOLES')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'LOS CHACAYES' and z.Descripcion = 'TUNUYAN') BEGIN insert into SubZona values ((select id from zona where Descripcion='TUNUYAN'),'LOS CHACAYES')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'LOS SAUCES' and z.Descripcion = 'TUNUYAN') BEGIN insert into SubZona values ((select id from zona where Descripcion='TUNUYAN'),'LOS SAUCES')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'TUNUYAN' and z.Descripcion = 'TUNUYAN') BEGIN insert into SubZona values ((select id from zona where Descripcion='TUNUYAN'),'TUNUYAN')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'VILLA SECA' and z.Descripcion = 'TUNUYAN') BEGIN insert into SubZona values ((select id from zona where Descripcion='TUNUYAN'),'VILLA SECA')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'VISTA FLORES' and z.Descripcion = 'TUNUYAN') BEGIN insert into SubZona values ((select id from zona where Descripcion='TUNUYAN'),'VISTA FLORES')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'CAMPO VIDAL' and z.Descripcion = 'TUPUNGATO') BEGIN insert into SubZona values ((select id from zona where Descripcion='TUPUNGATO'),'CAMPO VIDAL')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'CORDON DEL PLATA' and z.Descripcion = 'TUPUNGATO') BEGIN insert into SubZona values ((select id from zona where Descripcion='TUPUNGATO'),'CORDON DEL PLATA')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'EL PERAL' and z.Descripcion = 'TUPUNGATO') BEGIN insert into SubZona values ((select id from zona where Descripcion='TUPUNGATO'),'EL PERAL')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'GUALTALLARY' and z.Descripcion = 'TUPUNGATO') BEGIN insert into SubZona values ((select id from zona where Descripcion='TUPUNGATO'),'GUALTALLARY')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'SAN JOSE' and z.Descripcion = 'TUPUNGATO') BEGIN insert into SubZona values ((select id from zona where Descripcion='TUPUNGATO'),'SAN JOSE')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'VILLA BASTIA' and z.Descripcion = 'TUPUNGATO') BEGIN insert into SubZona values ((select id from zona where Descripcion='TUPUNGATO'),'VILLA BASTIA')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'ZAMPAL' and z.Descripcion = 'TUPUNGATO') BEGIN insert into SubZona values ((select id from zona where Descripcion='TUPUNGATO'),'ZAMPAL')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'A DETERMINAR' and z.Descripcion = 'TUPUNGATO') BEGIN insert into SubZona values ((select id from zona where Descripcion='TUPUNGATO'),'A DETERMINAR')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'SIN ASIGNAR' and z.Descripcion = 'TUPUNGATO') BEGIN insert into SubZona values ((select id from zona where Descripcion='TUPUNGATO'),'SIN ASIGNAR')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'ULLUM' and z.Descripcion = 'ULLUM') BEGIN insert into SubZona values ((select id from zona where Descripcion='ULLUM'),'ULLUM')END;
IF NOT EXISTS (select 1 from SubZona s,Zona z where z.Id = s.Zona_Id and s.Descripcion = 'VILLA ZONDA' and z.Descripcion = 'ZONDA') BEGIN insert into SubZona values ((select id from zona where Descripcion='ZONDA'),'VILLA ZONDA')END;


-- MotivoHumedadManual --
IF NOT EXISTS (select 1 from MotivoHumedadManual where Descripcion = 'Decisión Comercial') BEGIN insert into MotivoHumedadManual values ('Decisión Comercial') END;
IF NOT EXISTS (select 1 from MotivoHumedadManual where Descripcion = 'Decisión de Gerencia') BEGIN insert into MotivoHumedadManual values ('Decisión de Gerencia')END;

--Provincia
update Provincia set DescripcionCollate_CI_AS = Descripcion

GO
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Soja Compra') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Soja Compra','Soja', 0, 0); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Soja 3ros.') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Soja 3ros.','', 0, 1); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Harina Low pro Venta') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Harina Low pro Venta','', 0, 2); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Harina HI PRO Otros') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Harina HI PRO Otros','', 0, 3); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Harina High Pro Venta') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Harina High Pro Venta','', 0, 4); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Harina High Pro EXPORTACION') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Harina High Pro EXPORTACION','', 0, 5); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Pecaso Venta') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Pecaso Venta','', 0, 6); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Pecaso EXPORTACION') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Pecaso EXPORTACION','', 0, 7); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Aceite Santa Clara') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Aceite Santa Clara','', 0, 8); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Aceite Avellaneda') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Aceite Avellaneda','', 0, 9); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Aceite Venta a 3ros') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Aceite Venta a 3ros','', 0, 10); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'ACEITE CRUDO DE SOJA EXPORTACION') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('ACEITE CRUDO DE SOJA EXPORTACION','', 0, 11); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Grasa Planta de Efluentes') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Grasa Planta de Efluentes','', 0, 12); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'ACEITE CRUDO SOJA VICENTIN') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('ACEITE CRUDO SOJA VICENTIN','', 0, 13); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Aceite Soja Crudo Retiro Fazon con 3°') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Aceite Soja Crudo Retiro Fazon con 3°','', 0, 14); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Pellet de Girasol') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Pellet de Girasol','PELLET DE GIRASOL', 0, 15); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Residuos Líquidos Urbanos') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Residuos Líquidos Urbanos','Residuos liquidos Urbanos', 0, 16); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Aceitecrudo de soja Calsa') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Aceitecrudo de soja Calsa','', 0, 17); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Residuos Organicos') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Residuos Organicos','RESIDUOS ORGANICOS', 0, 18); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Residuos Sólidos Urbanos') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Residuos Sólidos Urbanos','', 0, 19); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Chatarra') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Chatarra','CHATARRA', 0, 20); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Residuos Inorganicos') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Residuos Inorganicos','', 0, 21); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Residuos Peligrosos') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Residuos Peligrosos','RESIDUOS PELIGROSOS', 0, 22); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Fuel Oil') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Fuel Oil','FUEL OIL', 0, 23); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Aceite Girasol crudo ( alto Oleico)') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Aceite Girasol crudo ( alto Oleico)','', 0, 24); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Aceite Girasol Crudo  ') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Aceite Girasol Crudo  ','ACEITE GIRASOL CRUDO', 0, 25); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Aceite Soja Crudo Patagonia Bioenergia') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Aceite Soja Crudo Patagonia Bioenergia','', 0, 26); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Aceite Crudo de Maiz') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Aceite Crudo de Maiz','ACEITE CRUDO DE MAIZ', 0, 27); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Barro de tratamiento de Efuentes') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Barro de tratamiento de Efuentes','', 0, 28); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Goma de Extraccion') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Goma de Extraccion','GOMA EXTRACCION', 0, 30); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Lecitina  de Soja Venta') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Lecitina  de Soja Venta','', 0, 31); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Lecitina de Soja EXPORTACION') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Lecitina de Soja EXPORTACION','', 0, 32); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'Lecitina de Soja Redespacho') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Lecitina de Soja Redespacho','', 0, 33); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'MAIZ') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('MAIZ','Maiz Duro Colorado', 0, 34); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 0 and Descripcion = 'TRIGO') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('TRIGO','Trigo Pan', 0, 35); END

IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Soja Compra') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Soja Compra','Soja', 1, 0); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'SOJA SUSTENTABLE') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('SOJA SUSTENTABLE','Soja Sustentable', 1, 1); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Harina de Soja High Pro') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Harina de Soja High Pro','HARINA DE SOJA HIPRO', 1, 2); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Harina HI PRO RENOVA') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Harina HI PRO RENOVA','', 1, 3); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Aceite crudo Soja 3ros') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Aceite crudo Soja 3ros','ACEITE DE SOJA CRUDO A GRANEL', 1, 4); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Pecaso RENOVA') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Pecaso RENOVA','', 1, 5); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Aceite Bio Santa Clara') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Aceite Bio Santa Clara','', 1, 6); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Aceite Girasol Crudo') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Aceite Girasol Crudo','ACEITE GIRASOL CRUDO', 1, 7); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Aceite Girasol Crudo 3°') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Aceite Girasol Crudo 3°','', 1, 8); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Aceite Crudo de Soja RENOVA') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Aceite Crudo de Soja RENOVA','', 1, 9); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'TRIGO') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('TRIGO','Trigo Pan', 1, 10); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'BIODIESEL RENOVA') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('BIODIESEL RENOVA','', 1, 11); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Acido Sulfurico') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Acido Sulfurico','ACIDO SULFURICO', 1, 12); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Hexano') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Hexano','SOLVENTE HEXANO P/EXTRACCION DE ACEITE', 1, 13); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Fuel Oil') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Fuel Oil','FUEL OIL', 1, 14); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Cloro ( Hipoclorito de Sodio)') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Cloro ( Hipoclorito de Sodio)','HIPOCLORITO DE SODIO', 1, 15); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Aceite de Girasol Crudo Oleico') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Aceite de Girasol Crudo Oleico','', 1, 16); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Soda Caustica') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Soda Caustica','SODA CAUSTICA LIQUIDA BASE SECA GRANEL', 1, 17); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Sulfato de Aluminio') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Sulfato de Aluminio','SULFATO DE AL. DILUIDO AL 8 %', 1, 18); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Aceite refinado de girasol') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Aceite refinado de girasol','', 1, 19); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Operativos de Vagones') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Operativos de Vagones','', 1, 20); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'GIRASOL') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('GIRASOL','Semilla de Girasol', 1, 21); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'SOJA EXPORTACION/IMPORTACION') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('SOJA EXPORTACION/IMPORTACION','', 1, 22); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'BIODIESEL') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('BIODIESEL','BIODIESEL(METILESTER DE SOJA) A GRANEL', 1, 23); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Gas Licuado') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Gas Licuado','', 1, 24); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Nitrogeno') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Nitrogeno','NITROGENO', 1, 25); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Escoria') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Escoria','ESCORIA 10/30 G.FINO', 1, 26); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Acido Citrico') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Acido Citrico','Acido Citrico', 1, 27); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Aceite Cartamo alto oleico') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Aceite Cartamo alto oleico','', 1, 28); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Aceite Crudo de MAIZ de 3°') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Aceite Crudo de MAIZ de 3°','', 1, 29); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Aceite Girasol Crudo Alto Oleico 3°') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Aceite Girasol Crudo Alto Oleico 3°','', 1, 30); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'BIODIESEL de 3°') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('BIODIESEL de 3°','', 1, 31); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Residuos liquidos Urbanos') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Residuos liquidos Urbanos','Residuos liquidos Urbanos', 1, 32); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'Lecitina de soja') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('Lecitina de soja','Lecitina de soja', 1, 33); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'MAIZ ') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('MAIZ ','Maiz Duro Colorado', 1, 34); END
IF NOT EXISTS (select 1 from MaterialReporteDeMovimientos where Ingreso = 1 and Descripcion = 'SOJA EPA') BEGIN insert into MaterialReporteDeMovimientos(Descripcion,Material, Ingreso, Orden) values ('SOJA EPA','', 1, 35); END

GO
IF NOT EXISTS (select 1 from AlmacenReporteDeMovimientos where  Descripcion = 'Celda 1') BEGIN insert into AlmacenReporteDeMovimientos(Descripcion,Almacen,Orden) values ('Celda 1','Celda 1', 0); END
IF NOT EXISTS (select 1 from AlmacenReporteDeMovimientos where  Descripcion = 'Celda 2') BEGIN insert into AlmacenReporteDeMovimientos(Descripcion,Almacen,Orden) values ('Celda 2','Celda 2', 1); END
IF NOT EXISTS (select 1 from AlmacenReporteDeMovimientos where  Descripcion = 'Celda 29') BEGIN insert into AlmacenReporteDeMovimientos(Descripcion,Almacen,Orden) values ('Celda 29','Celda 29', 2); END
IF NOT EXISTS (select 1 from AlmacenReporteDeMovimientos where  Descripcion = 'Silo 3') BEGIN insert into AlmacenReporteDeMovimientos(Descripcion,Almacen,Orden) values ('Silo 3','Silo 3',  3); END
IF NOT EXISTS (select 1 from AlmacenReporteDeMovimientos where  Descripcion = 'Silo 4') BEGIN insert into AlmacenReporteDeMovimientos(Descripcion,Almacen,Orden) values ('Silo 4','Silo 4',  4); END
IF NOT EXISTS (select 1 from AlmacenReporteDeMovimientos where  Descripcion = 'Silo 5') BEGIN insert into AlmacenReporteDeMovimientos(Descripcion,Almacen,Orden) values ('Silo 5','Silo 5',  5); END
IF NOT EXISTS (select 1 from AlmacenReporteDeMovimientos where  Descripcion = 'Silo 6') BEGIN insert into AlmacenReporteDeMovimientos(Descripcion,Almacen,Orden) values ('Silo 6','Silo 6',  6); END
IF NOT EXISTS (select 1 from AlmacenReporteDeMovimientos where  Descripcion = 'Silo 18') BEGIN insert into AlmacenReporteDeMovimientos(Descripcion,Almacen,Orden) values ('Silo 18','Silo 18',  7); END
IF NOT EXISTS (select 1 from AlmacenReporteDeMovimientos where  Descripcion = 'Silo 19') BEGIN insert into AlmacenReporteDeMovimientos(Descripcion,Almacen,Orden) values ('Silo 19','Silo 19',  8); END
IF NOT EXISTS (select 1 from AlmacenReporteDeMovimientos where  Descripcion = 'Silo 9') BEGIN insert into AlmacenReporteDeMovimientos(Descripcion,Almacen,Orden) values ('Silo 9','Silo 9',  9); END
IF NOT EXISTS (select 1 from AlmacenReporteDeMovimientos where  Descripcion = 'Silo 25') BEGIN insert into AlmacenReporteDeMovimientos(Descripcion,Almacen,Orden) values ('Silo 25','Silo 25',  10); END
IF NOT EXISTS (select 1 from AlmacenReporteDeMovimientos where  Descripcion = 'Silo 26') BEGIN insert into AlmacenReporteDeMovimientos(Descripcion,Almacen,Orden) values ('Silo 26','Silo 26',  11); END
IF NOT EXISTS (select 1 from AlmacenReporteDeMovimientos where  Descripcion = 'Silo 16') BEGIN insert into AlmacenReporteDeMovimientos(Descripcion,Almacen,Orden) values ('Silo 16','Silo 16',  12); END
IF NOT EXISTS (select 1 from AlmacenReporteDeMovimientos where  Descripcion = 'Silo 17') BEGIN insert into AlmacenReporteDeMovimientos(Descripcion,Almacen,Orden) values ('Silo 17','Silo 17',  13); END
IF NOT EXISTS (select 1 from AlmacenReporteDeMovimientos where  Descripcion = 'Silo L3 A') BEGIN insert into AlmacenReporteDeMovimientos(Descripcion,Almacen,Orden) values ('Silo L3 A','Silo L3 A',  14); END
IF NOT EXISTS (select 1 from AlmacenReporteDeMovimientos where  Descripcion = 'Silo L3 B') BEGIN insert into AlmacenReporteDeMovimientos(Descripcion,Almacen,Orden) values ('Silo L3 B','Silo L3 B',  15); END
IF NOT EXISTS (select 1 from AlmacenReporteDeMovimientos where  Descripcion = 'Embarques') BEGIN insert into AlmacenReporteDeMovimientos(Descripcion,Almacen,Orden) values ('Embarques','Embarques',  16); END

GO
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Enerfo') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Enerfo') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='LDC') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('LDC') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='ADM') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('ADM') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='CJ Internacional') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('CJ Internacional') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='ECTP') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('ECTP') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Interpec') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Interpec') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Cefetra') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Cefetra') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Trafigura') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Trafigura') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Agravis') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Agravis') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Atlas') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Atlas') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Croosland') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Croosland') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Marubeni') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Marubeni') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Sodrugestvo') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Sodrugestvo') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Sojitz') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Sojitz') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Peter Cremer') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Peter Cremer') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Agrograin') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Agrograin') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Fisway') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Fisway') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Mercuria') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Mercuria') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Ypf') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Ypf') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Noble') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Noble') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Seabord') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Seabord') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Petrobras') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Petrobras') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Graincorp') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Graincorp') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Bunge') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Bunge') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Axxion') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Axxion') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='DLG') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('DLG') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Amaggi') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Amaggi') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Almarai') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Almarai') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Glencore') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Glencore') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Olam') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Olam') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='CIS') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('CIS') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Nidera') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Nidera') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Vitol') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Vitol') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='CAM') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('CAM') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Wilmar') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Wilmar') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Midstar') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Midstar') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Shell') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Shell') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Fondomonte South America') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Fondomonte South America') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='STI') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('STI') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Codrico') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Codrico') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Cofco') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Cofco') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Molinos Agro') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Molinos Agro') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Soprodi') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Soprodi') END
IF NOT EXISTS (select 1 from [CoordinadorPuerto] where [Nombre] ='Graneles Chile') BEGIN INSERT INTO [CoordinadorPuerto]([Nombre]) VALUES ('Graneles Chile') END

GO

IF NOT EXISTS (select 1 from [AgenciaMaritimaPuerto] where [Nombre] ='AMI') BEGIN INSERT INTO [AgenciaMaritimaPuerto]([Nombre]) VALUES ('AMI') END
IF NOT EXISTS (select 1 from [AgenciaMaritimaPuerto] where [Nombre] ='Faroship') BEGIN INSERT INTO [AgenciaMaritimaPuerto]([Nombre]) VALUES ('Faroship') END
IF NOT EXISTS (select 1 from [AgenciaMaritimaPuerto] where [Nombre] ='ISA') BEGIN INSERT INTO [AgenciaMaritimaPuerto]([Nombre]) VALUES ('ISA') END
IF NOT EXISTS (select 1 from [AgenciaMaritimaPuerto] where [Nombre] ='WAVE') BEGIN INSERT INTO [AgenciaMaritimaPuerto]([Nombre]) VALUES ('WAVE') END
IF NOT EXISTS (select 1 from [AgenciaMaritimaPuerto] where [Nombre] ='CLIPPER') BEGIN INSERT INTO [AgenciaMaritimaPuerto]([Nombre]) VALUES ('CLIPPER') END
IF NOT EXISTS (select 1 from [AgenciaMaritimaPuerto] where [Nombre] ='ALPEMAR') BEGIN INSERT INTO [AgenciaMaritimaPuerto]([Nombre]) VALUES ('ALPEMAR') END
IF NOT EXISTS (select 1 from [AgenciaMaritimaPuerto] where [Nombre] ='EUROAMERICA') BEGIN INSERT INTO [AgenciaMaritimaPuerto]([Nombre]) VALUES ('EUROAMERICA') END
IF NOT EXISTS (select 1 from [AgenciaMaritimaPuerto] where [Nombre] ='RIOPLAT') BEGIN INSERT INTO [AgenciaMaritimaPuerto]([Nombre]) VALUES ('RIOPLAT') END
IF NOT EXISTS (select 1 from [AgenciaMaritimaPuerto] where [Nombre] ='FERTIMPORT') BEGIN INSERT INTO [AgenciaMaritimaPuerto]([Nombre]) VALUES ('FERTIMPORT') END
IF NOT EXISTS (select 1 from [AgenciaMaritimaPuerto] where [Nombre] ='CHRISTOPHERSEN') BEGIN INSERT INTO [AgenciaMaritimaPuerto]([Nombre]) VALUES ('CHRISTOPHERSEN') END
IF NOT EXISTS (select 1 from [AgenciaMaritimaPuerto] where [Nombre] ='SUPERMAR') BEGIN INSERT INTO [AgenciaMaritimaPuerto]([Nombre]) VALUES ('SUPERMAR') END
IF NOT EXISTS (select 1 from [AgenciaMaritimaPuerto] where [Nombre] ='HEINLEIN') BEGIN INSERT INTO [AgenciaMaritimaPuerto]([Nombre]) VALUES ('HEINLEIN') END
IF NOT EXISTS (select 1 from [AgenciaMaritimaPuerto] where [Nombre] ='ABBEY SEA') BEGIN INSERT INTO [AgenciaMaritimaPuerto]([Nombre]) VALUES ('ABBEY SEA') END
IF NOT EXISTS (select 1 from [AgenciaMaritimaPuerto] where [Nombre] ='NABSA') BEGIN INSERT INTO [AgenciaMaritimaPuerto]([Nombre]) VALUES ('NABSA') END
IF NOT EXISTS (select 1 from [AgenciaMaritimaPuerto] where [Nombre] ='MARSA') BEGIN INSERT INTO [AgenciaMaritimaPuerto]([Nombre]) VALUES ('MARSA') END
IF NOT EXISTS (select 1 from [AgenciaMaritimaPuerto] where [Nombre] ='CONSULTORES MARITIMOS') BEGIN INSERT INTO [AgenciaMaritimaPuerto]([Nombre]) VALUES ('CONSULTORES MARITIMOS') END
IF NOT EXISTS (select 1 from [AgenciaMaritimaPuerto] where [Nombre] ='BALTZER') BEGIN INSERT INTO [AgenciaMaritimaPuerto]([Nombre]) VALUES ('BALTZER') END
IF NOT EXISTS (select 1 from [AgenciaMaritimaPuerto] where [Nombre] ='DULCE') BEGIN INSERT INTO [AgenciaMaritimaPuerto]([Nombre]) VALUES ('DULCE') END
IF NOT EXISTS (select 1 from [AgenciaMaritimaPuerto] where [Nombre] ='OCEAN WAY') BEGIN INSERT INTO [AgenciaMaritimaPuerto]([Nombre]) VALUES ('OCEAN WAY') END
IF NOT EXISTS (select 1 from [AgenciaMaritimaPuerto] where [Nombre] ='AT PORT') BEGIN INSERT INTO [AgenciaMaritimaPuerto]([Nombre]) VALUES ('AT PORT') END
IF NOT EXISTS (select 1 from [AgenciaMaritimaPuerto] where [Nombre] ='B2B') BEGIN INSERT INTO [AgenciaMaritimaPuerto]([Nombre]) VALUES ('B2B') END
IF NOT EXISTS (select 1 from [AgenciaMaritimaPuerto] where [Nombre] ='JNL') BEGIN INSERT INTO [AgenciaMaritimaPuerto]([Nombre]) VALUES ('JNL') END

GO

IF NOT EXISTS (select 1 from [Estiba] where [Nombre] ='GONZALO' and [Apellido] ='FIGUEROA') BEGIN INSERT INTO [Estiba]([Nombre],[Apellido]) VALUES ('GONZALO','FIGUEROA' ) END
IF NOT EXISTS (select 1 from [Estiba] where [Nombre] ='RAUL' and [Apellido] ='RONCAGLIA') BEGIN INSERT INTO [Estiba]([Nombre],[Apellido]) VALUES ('RAUL','RONCAGLIA' ) END
IF NOT EXISTS (select 1 from [Estiba] where [Nombre] ='DANIEL' and [Apellido] ='CALVET') BEGIN INSERT INTO [Estiba]([Nombre],[Apellido]) VALUES ('DANIEL','CALVET' ) END

GO

IF NOT EXISTS (select 1 from [AgenciaControlPrivado] where [Nombre] ='Trust Control') BEGIN INSERT INTO [AgenciaControlPrivado]([Nombre]) VALUES ('Trust Control') END
IF NOT EXISTS (select 1 from [AgenciaControlPrivado] where [Nombre] ='Superinspect') BEGIN INSERT INTO [AgenciaControlPrivado]([Nombre]) VALUES ('Superinspect') END
IF NOT EXISTS (select 1 from [AgenciaControlPrivado] where [Nombre] ='Black Belt') BEGIN INSERT INTO [AgenciaControlPrivado]([Nombre]) VALUES ('Black Belt') END
IF NOT EXISTS (select 1 from [AgenciaControlPrivado] where [Nombre] ='Seaport') BEGIN INSERT INTO [AgenciaControlPrivado]([Nombre]) VALUES ('Seaport') END
IF NOT EXISTS (select 1 from [AgenciaControlPrivado] where [Nombre] ='Krudo') BEGIN INSERT INTO [AgenciaControlPrivado]([Nombre]) VALUES ('Krudo') END
IF NOT EXISTS (select 1 from [AgenciaControlPrivado] where [Nombre] ='Euroamerica') BEGIN INSERT INTO [AgenciaControlPrivado]([Nombre]) VALUES ('Euroamerica') END
IF NOT EXISTS (select 1 from [AgenciaControlPrivado] where [Nombre] ='Redflint') BEGIN INSERT INTO [AgenciaControlPrivado]([Nombre]) VALUES ('Redflint') END
IF NOT EXISTS (select 1 from [AgenciaControlPrivado] where [Nombre] ='Alex Stewart') BEGIN INSERT INTO [AgenciaControlPrivado]([Nombre]) VALUES ('Alex Stewart') END
IF NOT EXISTS (select 1 from [AgenciaControlPrivado] where [Nombre] ='Bureau Veritas') BEGIN INSERT INTO [AgenciaControlPrivado]([Nombre]) VALUES ('Bureau Veritas') END
IF NOT EXISTS (select 1 from [AgenciaControlPrivado] where [Nombre] ='Schutter') BEGIN INSERT INTO [AgenciaControlPrivado]([Nombre]) VALUES ('Schutter') END
IF NOT EXISTS (select 1 from [AgenciaControlPrivado] where [Nombre] ='Control internacional') BEGIN INSERT INTO [AgenciaControlPrivado]([Nombre]) VALUES ('Control internacional') END
IF NOT EXISTS (select 1 from [AgenciaControlPrivado] where [Nombre] ='Inspectorate de argentina') BEGIN INSERT INTO [AgenciaControlPrivado]([Nombre]) VALUES ('Inspectorate de argentina') END
IF NOT EXISTS (select 1 from [AgenciaControlPrivado] where [Nombre] ='Intertek testing Services') BEGIN INSERT INTO [AgenciaControlPrivado]([Nombre]) VALUES ('Intertek testing Services') END
IF NOT EXISTS (select 1 from [AgenciaControlPrivado] where [Nombre] ='Control union') BEGIN INSERT INTO [AgenciaControlPrivado]([Nombre]) VALUES ('Control union') END
IF NOT EXISTS (select 1 from [AgenciaControlPrivado] where [Nombre] ='SGS') BEGIN INSERT INTO [AgenciaControlPrivado]([Nombre]) VALUES ('SGS') END
IF NOT EXISTS (select 1 from [AgenciaControlPrivado] where [Nombre] ='Cotecna') BEGIN INSERT INTO [AgenciaControlPrivado]([Nombre]) VALUES ('Cotecna') END
IF NOT EXISTS (select 1 from [AgenciaControlPrivado] where [Nombre] ='Fides control') BEGIN INSERT INTO [AgenciaControlPrivado]([Nombre]) VALUES ('Fides control') END
IF NOT EXISTS (select 1 from [AgenciaControlPrivado] where [Nombre] ='Hl Service control') BEGIN INSERT INTO [AgenciaControlPrivado]([Nombre]) VALUES ('Hl Service control') END
IF NOT EXISTS (select 1 from [AgenciaControlPrivado] where [Nombre] ='RRMG') BEGIN INSERT INTO [AgenciaControlPrivado]([Nombre]) VALUES ('RRMG') END

GO

--Tipo de Buque Puerto
IF NOT EXISTS (select 1 from TipoDeBuquePuerto where Nombre = 'Handy-sized') BEGIN insert into TipoDeBuquePuerto(Nombre) values ('Handy-sized'); END
IF NOT EXISTS (select 1 from TipoDeBuquePuerto where Nombre = 'Handy-max') BEGIN insert into TipoDeBuquePuerto(Nombre) values ('Handy-max'); END
IF NOT EXISTS (select 1 from TipoDeBuquePuerto where Nombre = 'Wood-chip carriers') BEGIN insert into TipoDeBuquePuerto(Nombre) values ('Wood-chip carriers'); END
IF NOT EXISTS (select 1 from TipoDeBuquePuerto where Nombre = 'WPanamax') BEGIN insert into TipoDeBuquePuerto(Nombre) values ('WPanamax'); END

GO

--Ubicación de Buque Puerto
IF NOT EXISTS (select 1 from UbicacionDeBuquePuerto where Nombre = 'Zarpó') BEGIN insert into UbicacionDeBuquePuerto(Nombre, Orden) values ('Zarpó', 1); END
IF NOT EXISTS (select 1 from UbicacionDeBuquePuerto where Nombre = 'Muelle de Carga') BEGIN insert into UbicacionDeBuquePuerto(Nombre, Orden) values ('Muelle de Carga', 2); END
IF NOT EXISTS (select 1 from UbicacionDeBuquePuerto where Nombre = 'Rada de Carga') BEGIN insert into UbicacionDeBuquePuerto(Nombre, Orden) values ('Rada de Carga', 3); END
IF NOT EXISTS (select 1 from UbicacionDeBuquePuerto where Nombre = 'Otra Rada') BEGIN insert into UbicacionDeBuquePuerto(Nombre, Orden) values ('Otra Rada', 4); END
IF NOT EXISTS (select 1 from UbicacionDeBuquePuerto where Nombre = 'Otro Muelle') BEGIN insert into UbicacionDeBuquePuerto(Nombre, Orden) values ('Otro Muelle', 5); END
IF NOT EXISTS (select 1 from UbicacionDeBuquePuerto where Nombre = 'Subiendo') BEGIN insert into UbicacionDeBuquePuerto(Nombre, Orden) values ('Subiendo', 6); END
IF NOT EXISTS (select 1 from UbicacionDeBuquePuerto where Nombre = 'Recalada') BEGIN insert into UbicacionDeBuquePuerto(Nombre, Orden) values ('Recalada', 7); END
IF NOT EXISTS (select 1 from UbicacionDeBuquePuerto where Nombre = 'En Viaje') BEGIN insert into UbicacionDeBuquePuerto(Nombre, Orden) values ('En Viaje', 8); END

GO

IF NOT EXISTS (SELECT 1 FROM MensajeCartelLed mcl WHERE mcl.Codigo = 'GaritaIngresoAsignarCalle')
BEGIN
	INSERT INTO MensajeCartelLed (Codigo, Orden, Mensaje, Programa, Trama, Variable, SegundosDeEspera, DescripcionFormatoMensaje, Habilitado) VALUES('GaritaIngresoAsignarCalle', 1, '{1}', '01', '01', '00', 0, '{1}: patente del camion', 1)
	INSERT INTO MensajeCartelLed (Codigo, Orden, Mensaje, Programa, Trama, Variable, SegundosDeEspera, DescripcionFormatoMensaje, Habilitado) VALUES('GaritaIngresoAsignarCalle', 2, '{0}', '01', '01', '01', 0, '{0}: nombre de calle asignada', 1)
	INSERT INTO MensajeCartelLed (Codigo, Orden, Mensaje, Programa, Trama, Variable, SegundosDeEspera, DescripcionFormatoMensaje, Habilitado) VALUES('GaritaIngresoAsignarCalle', 3, 'PARE', '01', '02', '00', 0, 'Se limpia el cartel', 1)
END

IF NOT EXISTS (SELECT 1 FROM MensajeCartelLed mcl WHERE mcl.Codigo = 'CartelPrecaladoCalador1' AND mcl.Orden = 1) BEGIN INSERT INTO MensajeCartelLed (Codigo, Orden, Mensaje, Programa, Trama, Variable, SegundosDeEspera, DescripcionFormatoMensaje, Habilitado) VALUES('CartelPrecaladoCalador1', 1, '{1}', '01', '01', '00', 0, '{1}: patente del camion', 1) END
IF NOT EXISTS (SELECT 1 FROM MensajeCartelLed mcl WHERE mcl.Codigo = 'CartelPrecaladoCalador1' AND mcl.Orden = 2) BEGIN INSERT INTO MensajeCartelLed (Codigo, Orden, Mensaje, Programa, Trama, Variable, SegundosDeEspera, DescripcionFormatoMensaje, Habilitado) VALUES('CartelPrecaladoCalador1', 2, '{0}', '01', '01', '01', 0, '{0}: nombre de calle asignada', 1) END
IF NOT EXISTS (SELECT 1 FROM MensajeCartelLed mcl WHERE mcl.Codigo = 'CartelPrecaladoCalador1' AND mcl.Orden = 3) BEGIN INSERT INTO MensajeCartelLed (Codigo, Orden, Mensaje, Programa, Trama, Variable, SegundosDeEspera, DescripcionFormatoMensaje, Habilitado) VALUES('CartelPrecaladoCalador1', 3, 'PARE', '01', '02', '00', 0, 'Se limpia el cartel', 1) END

IF NOT EXISTS (SELECT 1 FROM MensajeCartelLed mcl WHERE mcl.Codigo = 'CartelPrecaladoCalador2' AND mcl.Orden = 1) BEGIN INSERT INTO MensajeCartelLed (Codigo, Orden, Mensaje, Programa, Trama, Variable, SegundosDeEspera, DescripcionFormatoMensaje, Habilitado) VALUES('CartelPrecaladoCalador2', 1, '{1}', '01', '01', '00', 0, '{1}: patente del camion', 1) END
IF NOT EXISTS (SELECT 1 FROM MensajeCartelLed mcl WHERE mcl.Codigo = 'CartelPrecaladoCalador2' AND mcl.Orden = 2) BEGIN INSERT INTO MensajeCartelLed (Codigo, Orden, Mensaje, Programa, Trama, Variable, SegundosDeEspera, DescripcionFormatoMensaje, Habilitado) VALUES('CartelPrecaladoCalador2', 2, '{0}', '01', '01', '01', 0, '{0}: nombre de calle asignada', 1) END
IF NOT EXISTS (SELECT 1 FROM MensajeCartelLed mcl WHERE mcl.Codigo = 'CartelPrecaladoCalador2' AND mcl.Orden = 3) BEGIN INSERT INTO MensajeCartelLed (Codigo, Orden, Mensaje, Programa, Trama, Variable, SegundosDeEspera, DescripcionFormatoMensaje, Habilitado) VALUES('CartelPrecaladoCalador2', 3, 'PARE', '01', '02', '00', 0, 'Se limpia el cartel', 1) END

IF NOT EXISTS (SELECT 1 FROM MensajeCartelLed mcl WHERE mcl.Codigo = 'CartelPreBalanza' AND mcl.Orden = 1) BEGIN INSERT INTO MensajeCartelLed (Codigo, Orden, Mensaje, Programa, Trama, Variable, SegundosDeEspera, DescripcionFormatoMensaje, Habilitado) VALUES('CartelPreBalanza', 1, '', '01', '01', '01', 0, '', 1) END
IF NOT EXISTS (SELECT 1 FROM MensajeCartelLed mcl WHERE mcl.Codigo = 'CartelPreBalanza' AND mcl.Orden = 2) BEGIN INSERT INTO MensajeCartelLed (Codigo, Orden, Mensaje, Programa, Trama, Variable, SegundosDeEspera, DescripcionFormatoMensaje, Habilitado) VALUES('CartelPreBalanza', 2, '', '01', '01', '02', 0, '', 1) END
IF NOT EXISTS (SELECT 1 FROM MensajeCartelLed mcl WHERE mcl.Codigo = 'CartelPreBalanza' AND mcl.Orden = 3) BEGIN INSERT INTO MensajeCartelLed (Codigo, Orden, Mensaje, Programa, Trama, Variable, SegundosDeEspera, DescripcionFormatoMensaje, Habilitado) VALUES('CartelPreBalanza', 3, '', '01', '01', '03', 0, '', 1) END
IF NOT EXISTS (SELECT 1 FROM MensajeCartelLed mcl WHERE mcl.Codigo = 'CartelPreBalanza' AND mcl.Orden = 4) BEGIN INSERT INTO MensajeCartelLed (Codigo, Orden, Mensaje, Programa, Trama, Variable, SegundosDeEspera, DescripcionFormatoMensaje, Habilitado) VALUES('CartelPreBalanza', 4, '', '01', '01', '04', 0, '', 1) END
IF NOT EXISTS (SELECT 1 FROM MensajeCartelLed mcl WHERE mcl.Codigo = 'CartelPreBalanza' AND mcl.Orden = 5) BEGIN INSERT INTO MensajeCartelLed (Codigo, Orden, Mensaje, Programa, Trama, Variable, SegundosDeEspera, DescripcionFormatoMensaje, Habilitado) VALUES('CartelPreBalanza', 5, '', '01', '01', '05', 0, '', 1) END
IF NOT EXISTS (SELECT 1 FROM MensajeCartelLed mcl WHERE mcl.Codigo = 'CartelPreBalanza' AND mcl.Orden = 6) BEGIN INSERT INTO MensajeCartelLed (Codigo, Orden, Mensaje, Programa, Trama, Variable, SegundosDeEspera, DescripcionFormatoMensaje, Habilitado) VALUES('CartelPreBalanza', 6, '', '01', '01', '06', 0, '', 1) END
IF NOT EXISTS (SELECT 1 FROM MensajeCartelLed mcl WHERE mcl.Codigo = 'CartelPreBalanza' AND mcl.Orden = 7) BEGIN INSERT INTO MensajeCartelLed (Codigo, Orden, Mensaje, Programa, Trama, Variable, SegundosDeEspera, DescripcionFormatoMensaje, Habilitado) VALUES('CartelPreBalanza', 7, '', '01', '01', '07', 0, '', 1) END
IF NOT EXISTS (SELECT 1 FROM MensajeCartelLed mcl WHERE mcl.Codigo = 'CartelPreBalanza' AND mcl.Orden = 8) BEGIN INSERT INTO MensajeCartelLed (Codigo, Orden, Mensaje, Programa, Trama, Variable, SegundosDeEspera, DescripcionFormatoMensaje, Habilitado) VALUES('CartelPreBalanza', 8, '', '01', '01', '08', 0, '', 1) END

IF NOT EXISTS (SELECT 1 FROM MensajeCartelLed mcl WHERE mcl.Codigo = 'BalanzaLimpiarCartelLed') BEGIN INSERT INTO MensajeCartelLed (Codigo, Orden, Mensaje, Programa, Trama, Variable, SegundosDeEspera, DescripcionFormatoMensaje, Habilitado) VALUES('BalanzaLimpiarCartelLed', 1, '', '01', '01', '00', 0, 'Se limpia el cartel', 1) END
IF NOT EXISTS (SELECT 1 FROM MensajeCartelLed mcl WHERE mcl.Codigo = 'BalanzaAvanzarCamion') BEGIN INSERT INTO MensajeCartelLed (Codigo, Orden, Mensaje, Programa, Trama, Variable, SegundosDeEspera, DescripcionFormatoMensaje, Habilitado) VALUES('BalanzaAvanzarCamion', 1, 'Avance, no esta bien posicionado', '01', '01', '00', 0, '', 1) END
IF NOT EXISTS (SELECT 1 FROM MensajeCartelLed mcl WHERE mcl.Codigo = 'BalanzaRetrocederCamion') BEGIN INSERT INTO MensajeCartelLed (Codigo, Orden, Mensaje, Programa, Trama, Variable, SegundosDeEspera, DescripcionFormatoMensaje, Habilitado) VALUES('BalanzaRetrocederCamion', 1, 'Retroceda, no esta bien posicionado', '01', '01', '00', 0, '', 1) END
IF NOT EXISTS (SELECT 1 FROM MensajeCartelLed mcl WHERE mcl.Codigo = 'LlamadoCallePrecaladoACalar') BEGIN INSERT INTO MensajeCartelLed (Codigo, Orden, Mensaje, Programa, Trama, Variable, SegundosDeEspera, DescripcionFormatoMensaje, Habilitado) VALUES('LlamadoCallePrecaladoACalar', 1, 'asignado a', '01', '01', '00', 0, '', 1) END
IF NOT EXISTS (SELECT 1 FROM MensajeCartelLed mcl WHERE mcl.Codigo = 'LlamadoCallePostcalado') BEGIN INSERT INTO MensajeCartelLed (Codigo, Orden, Mensaje, Programa, Trama, Variable, SegundosDeEspera, DescripcionFormatoMensaje, Habilitado) VALUES('LlamadoCallePostcalado', 1, 'Avance a garita de salida - ', '01', '01', '00', 0, '', 1) END
IF NOT EXISTS (SELECT 1 FROM MensajeCartelLed mcl WHERE mcl.Codigo = 'LlamadoAutomaticoVolcadoras') BEGIN INSERT INTO MensajeCartelLed (Codigo, Orden, Mensaje, Programa, Trama, Variable, SegundosDeEspera, DescripcionFormatoMensaje, Habilitado) VALUES('LlamadoAutomaticoVolcadoras', 1, '', '01', '01', '00', 0, '', 1) END

GO

--Celdas de Mano de Embarque
IF NOT EXISTS (select 1 from CeldaManoDeEmbarque where Nombre = '7') BEGIN insert into CeldaManoDeEmbarque(Nombre, Posicion) values ('7', 1); END
IF NOT EXISTS (select 1 from CeldaManoDeEmbarque where Nombre = '20') BEGIN insert into CeldaManoDeEmbarque(Nombre, Posicion) values ('20', 2); END
IF NOT EXISTS (select 1 from CeldaManoDeEmbarque where Nombre = '23') BEGIN insert into CeldaManoDeEmbarque(Nombre, Posicion) values ('23', 3); END
IF NOT EXISTS (select 1 from CeldaManoDeEmbarque where Nombre = '30') BEGIN insert into CeldaManoDeEmbarque(Nombre, Posicion) values ('30', 4); END
IF NOT EXISTS (select 1 from CeldaManoDeEmbarque where Nombre = 'Silo 31') BEGIN insert into CeldaManoDeEmbarque(Nombre, Posicion) values ('Silo 31', 5); END
IF NOT EXISTS (select 1 from CeldaManoDeEmbarque where Nombre = 'Silo 32') BEGIN insert into CeldaManoDeEmbarque(Nombre, Posicion) values ('Silo 32', 6); END
IF NOT EXISTS (select 1 from CeldaManoDeEmbarque where Nombre = 'Silo Logística') BEGIN insert into CeldaManoDeEmbarque(Nombre, Posicion) values ('Silo Logística', 7); END
IF NOT EXISTS (select 1 from CeldaManoDeEmbarque where Nombre = 'PVO5') BEGIN insert into CeldaManoDeEmbarque(Nombre, Posicion) values ('PVO5', 11); END
IF NOT EXISTS (select 1 from CeldaManoDeEmbarque where Nombre = 'PV06/7') BEGIN insert into CeldaManoDeEmbarque(Nombre, Posicion) values ('PV06/7', 12); END
GO

--Sentidos de Mano de Embarque
IF NOT EXISTS (select 1 from SentidoManoDeEmbarque where Nombre = 'Norte a sur') BEGIN insert into SentidoManoDeEmbarque(Nombre, Posicion) values ('Norte a sur', 1); END
IF NOT EXISTS (select 1 from SentidoManoDeEmbarque where Nombre = 'Sur a norte') BEGIN insert into SentidoManoDeEmbarque(Nombre, Posicion) values ('Sur a norte', 2); END
IF NOT EXISTS (select 1 from SentidoManoDeEmbarque where Nombre = 'Centro a sur') BEGIN insert into SentidoManoDeEmbarque(Nombre, Posicion) values ('Centro a sur', 3); END
IF NOT EXISTS (select 1 from SentidoManoDeEmbarque where Nombre = 'Centro a norte') BEGIN insert into SentidoManoDeEmbarque(Nombre, Posicion) values ('Centro a norte', 4); END
IF NOT EXISTS (select 1 from SentidoManoDeEmbarque where Nombre = 'Oeste a este') BEGIN insert into SentidoManoDeEmbarque(Nombre, Posicion) values ('Oeste a este', 5); END
IF NOT EXISTS (select 1 from SentidoManoDeEmbarque where Nombre = 'Este a oeste') BEGIN insert into SentidoManoDeEmbarque(Nombre, Posicion) values ('Este a oeste', 6); END
IF NOT EXISTS (select 1 from SentidoManoDeEmbarque where Nombre = 'Centro a este') BEGIN insert into SentidoManoDeEmbarque(Nombre, Posicion) values ('Centro a este', 7); END
IF NOT EXISTS (select 1 from SentidoManoDeEmbarque where Nombre = 'Centro a Oeste') BEGIN insert into SentidoManoDeEmbarque(Nombre, Posicion) values ('Centro a Oeste', 8); END
IF NOT EXISTS (select 1 from SentidoManoDeEmbarque where Nombre = 'Gravedad') BEGIN insert into SentidoManoDeEmbarque(Nombre, Posicion) values ('Gravedad', 9); END
IF NOT EXISTS (select 1 from SentidoManoDeEmbarque where Nombre = 'Descarga') BEGIN insert into SentidoManoDeEmbarque(Nombre, Posicion) values ('Descarga', 10); END
IF NOT EXISTS (select 1 from SentidoManoDeEmbarque where Nombre = 'Producción') BEGIN insert into SentidoManoDeEmbarque(Nombre, Posicion) values ('Producción', 11); END
GO

--Motivos Horas a la Espera de Limpieza
IF NOT EXISTS (select 1 from MotivosLimpieza where Nombre = 'Falta de Limpieza') BEGIN insert into MotivosLimpieza(Nombre) values ('Falta de Limpieza'); END
IF NOT EXISTS (select 1 from MotivosLimpieza where Nombre = 'Insectos Vivos') BEGIN insert into MotivosLimpieza(Nombre) values ('Insectos Vivos'); END
IF NOT EXISTS (select 1 from MotivosLimpieza where Nombre = 'Cascarones') BEGIN insert into MotivosLimpieza(Nombre) values ('Cascarones'); END
IF NOT EXISTS (select 1 from MotivosLimpieza where Nombre = 'Resto de Óxido') BEGIN insert into MotivosLimpieza(Nombre) values ('Resto de Óxido'); END
IF NOT EXISTS (select 1 from MotivosLimpieza where Nombre = 'Resto de Carga Anterior') BEGIN insert into MotivosLimpieza(Nombre) values ('Resto de Carga Anterior'); END
IF NOT EXISTS (select 1 from MotivosLimpieza where Nombre = 'Otros') BEGIN insert into MotivosLimpieza(Nombre) values ('Otros'); END
GO

--Motivos Fallas de Balanzas
IF NOT EXISTS (select 1 from MotivosFallasBalanza where Nombre = 'Falta Eléctrica') BEGIN insert into MotivosFallasBalanza(Nombre, Siglas) values ('Falta Eléctrica', 'E'); END
IF NOT EXISTS (select 1 from MotivosFallasBalanza where Nombre = 'Falta Mecánica') BEGIN insert into MotivosFallasBalanza(Nombre, Siglas) values ('Falta Mecánica', 'M'); END
IF NOT EXISTS (select 1 from MotivosFallasBalanza where Nombre = 'Operativas Puerto') BEGIN insert into MotivosFallasBalanza(Nombre, Siglas) values ('Operativas Puerto', 'OP'); END
IF NOT EXISTS (select 1 from MotivosFallasBalanza where Nombre = 'Operativas Comercial') BEGIN insert into MotivosFallasBalanza(Nombre, Siglas) values ('Operativas Comercial', 'OC'); END
IF NOT EXISTS (select 1 from MotivosFallasBalanza where Nombre = 'Operativas Buque') BEGIN insert into MotivosFallasBalanza(Nombre, Siglas) values ('Operativas Buque', 'OB'); END
IF NOT EXISTS (select 1 from MotivosFallasBalanza where Nombre = 'Espera Determinante') BEGIN insert into MotivosFallasBalanza(Nombre, Siglas) values ('Espera Determinante', 'ED'); END
GO

--Motivos De Corte
IF NOT EXISTS (select 1 from MotivosDeCorte where Nombre = 'Falta Eléctrica') BEGIN insert into MotivosDeCorte(Nombre, Siglas) values ('Falta Eléctrica', 'E'); END
IF NOT EXISTS (select 1 from MotivosDeCorte where Nombre = 'Falta Mecánica') BEGIN insert into MotivosDeCorte(Nombre, Siglas) values ('Falta Mecánica', 'M'); END
IF NOT EXISTS (select 1 from MotivosDeCorte where Nombre = 'Operativas Puerto') BEGIN insert into MotivosDeCorte(Nombre, Siglas) values ('Operativas Puerto', 'OP'); END
IF NOT EXISTS (select 1 from MotivosDeCorte where Nombre = 'Operativas Comercial') BEGIN insert into MotivosDeCorte(Nombre, Siglas) values ('Operativas Comercial', 'OC'); END
IF NOT EXISTS (select 1 from MotivosDeCorte where Nombre = 'Operativas Buque') BEGIN insert into MotivosDeCorte(Nombre, Siglas) values ('Operativas Buque', 'OB'); END
IF NOT EXISTS (select 1 from MotivosDeCorte where Nombre = 'Espera Determinante') BEGIN insert into MotivosDeCorte(Nombre, Siglas) values ('Espera Determinante', 'ED'); END
GO

--Turnos de Puerto
IF NOT EXISTS (select 1 from TurnoPuerto where Nombre = '00-06') BEGIN insert into TurnoPuerto(Nombre, Orden) values ('00-06', 1); END
IF NOT EXISTS (select 1 from TurnoPuerto where Nombre = '06-12') BEGIN insert into TurnoPuerto(Nombre, Orden) values ('06-12', 2); END
IF NOT EXISTS (select 1 from TurnoPuerto where Nombre = '12-18') BEGIN insert into TurnoPuerto(Nombre, Orden) values ('12-18', 3); END
IF NOT EXISTS (select 1 from TurnoPuerto where Nombre = '18-24') BEGIN insert into TurnoPuerto(Nombre, Orden) values ('18-24', 4); END
GO

 --Material puerto
IF NOT EXISTS (select 1 from MaterialPuerto where Descripcion = 'SEMILLA DE GIRASOL' and CodigoSap = '19908018') BEGIN insert into MaterialPuerto(Descripcion, CodigoSap, Almacen_Id,EsLiquido) values ('SEMILLA DE GIRASOL','19908018', null, 0); END
IF NOT EXISTS (select 1 from MaterialPuerto where Descripcion = 'POROTO DE SOJA' and CodigoSap = '19908017' and Almacen_Id = 1 and DescripcionCorta = 'PDS') BEGIN insert into MaterialPuerto(Descripcion, DescripcionCorta, CodigoSap, Almacen_Id, EsLiquido) values ('POROTO DE SOJA','PDS', '19908017', null, 0); END
IF NOT EXISTS (select 1 from MaterialPuerto where Descripcion = 'SEMILLA DE GIRASOL OLEICO' and CodigoSap = '19908019') BEGIN insert into MaterialPuerto(Descripcion, CodigoSap, Almacen_Id, EsLiquido) values ('SEMILLA DE GIRASOL OLEICO','19908019', null, 1); END
IF NOT EXISTS (select 1 from MaterialPuerto where Descripcion = 'SEMILLA DE MAIZ' and CodigoSap = '19908036') BEGIN insert into MaterialPuerto(Descripcion, CodigoSap, Almacen_Id, EsLiquido) values ('SEMILLA DE MAIZ','19908036', null, 0); END
IF NOT EXISTS (select 1 from MaterialPuerto where Descripcion = 'MAIZ' and DescripcionCorta = 'MAIZ' and CodigoSap = '99108' and Almacen_Id = 251 and EsLiquido = 0) BEGIN insert into MaterialPuerto(Descripcion, DescripcionCorta, CodigoSap, Almacen_Id, EsLiquido) values ('MAIZ','MAIZ','99108', 251, 0); END
IF NOT EXISTS (select 1 from MaterialPuerto where Descripcion = 'BIODIESEL' and DescripcionCorta = 'BIODIESEL' and CodigoSap = '99319' and Almacen_Id = 328 and EsLiquido = 1) BEGIN insert into MaterialPuerto(Descripcion, DescripcionCorta, CodigoSap, Almacen_Id, EsLiquido) values ('BIODIESEL','BIODIESEL','99319', 328, 1); END
IF NOT EXISTS (select 1 from MaterialPuerto where Descripcion = 'SEMILLA DE TRIGO' and CodigoSap = '19908027') BEGIN insert into MaterialPuerto(Descripcion, CodigoSap, Almacen_Id, EsLiquido) values ('SEMILLA DE TRIGO','19908027', null, 0); END
GO
GO

--Estados del buque
IF NOT EXISTS (select 1 from EstadoBuque where Descripcion = 'PreOperativo' and Id = 1) BEGIN insert into EstadoBuque(Descripcion, Id) values ('PreOperativo',1); END
IF NOT EXISTS (select 1 from EstadoBuque where Descripcion = 'Cargando' and Id = 2) BEGIN insert into EstadoBuque(Descripcion, Id) values ('Cargando',2); END
IF NOT EXISTS (select 1 from EstadoBuque where Descripcion = 'ControlCalidad' and Id = 3) BEGIN insert into EstadoBuque(Descripcion, Id) values ('ControlCalidad',3); END
IF NOT EXISTS (select 1 from EstadoBuque where Descripcion = 'PostOperativo' and Id = 4) BEGIN insert into EstadoBuque(Descripcion, Id) values ('PostOperativo',4); END
GO

--Sentidos de Mano de Embarque
IF NOT EXISTS (select 1 from SentidoManoDeEmbarque where Nombre = 'Norte a sur') BEGIN insert into SentidoManoDeEmbarque(Nombre) values ('Norte a sur'); END
IF NOT EXISTS (select 1 from SentidoManoDeEmbarque where Nombre = 'Sur a norte') BEGIN insert into SentidoManoDeEmbarque(Nombre) values ('Sur a norte'); END
IF NOT EXISTS (select 1 from SentidoManoDeEmbarque where Nombre = 'Centro a sur') BEGIN insert into SentidoManoDeEmbarque(Nombre) values ('Centro a sur'); END
IF NOT EXISTS (select 1 from SentidoManoDeEmbarque where Nombre = 'Centro a norte') BEGIN insert into SentidoManoDeEmbarque(Nombre) values ('Centro a norte'); END
IF NOT EXISTS (select 1 from SentidoManoDeEmbarque where Nombre = 'Oeste a este') BEGIN insert into SentidoManoDeEmbarque(Nombre) values ('Oeste a este'); END
IF NOT EXISTS (select 1 from SentidoManoDeEmbarque where Nombre = 'Este a oeste') BEGIN insert into SentidoManoDeEmbarque(Nombre) values ('Este a oeste'); END
IF NOT EXISTS (select 1 from SentidoManoDeEmbarque where Nombre = 'Centro a este') BEGIN insert into SentidoManoDeEmbarque(Nombre) values ('Centro a este'); END
IF NOT EXISTS (select 1 from SentidoManoDeEmbarque where Nombre = 'Centro a Oeste') BEGIN insert into SentidoManoDeEmbarque(Nombre) values ('Centro a Oeste'); END
GO

GO
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPSOJHUM' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (4,'HUMEDAD','MPSOJHUM',1); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPSOJGQU' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (4,'GRANOS QUEBRADOS','MPSOJGQU',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPSOJGDA' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (4,'DAÑADOS','MPSOJGDA',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPSOJCEX' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (4,'CUERPOS EXTRAÑOS','MPSOJCEX',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPSOJMCHA' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (4,'CHAMICO','MPSOJMCHA',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPSOJTIE' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (4,'TIERRA','MPSOJTIE',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPSOJOLO' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (4,'OLOR','MPSOJOLO',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPSOJREV' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (4,'REVOLCADOS EN TIERRA','MPSOJREV',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPSOJMOH' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (4,'AMOHOSADOS','MPSOJMOH',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPSOJAVE' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (4,'AVERIADOS','MPSOJAVE',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPSOJGVE' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (4,'GRANOS VERDES','MPSOJGVE',0); END

IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPMAZGRA' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (386,'GRADO','MPMAZGRA',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPMAZPHE' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (386,'PESO HECTOLITRICO','MPMAZPHE',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPMAZGDA' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (386,'DAÑADOS','MPMAZGDA',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPMAZQUE' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (386,'GRANOS QUEBRADOS','MPMAZQUE',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPMAZMEX' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (386,'CUERPOS EXTRAÑOS','MPMAZMEX',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPMAZTIP' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (386,'TIPO','MPMAZTIP',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPMAZHUM' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (386,'HUMEDAD','MPMAZHUM',1); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPMAZOLO' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (386,'OLOR','MPMAZOLO',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPMAZAMO' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (386,'AMOHOSADOS','MPMAZAMO',0); END

IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPTRPGRA' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (13,'GRADO','MPTRPGRA',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPTRPPHE' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (13,'PESO HECTOLITRICO','MPTRPPHE',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPTRPCEX' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (13,'CUERPOS EXTRAÑOS','MPTRPCEX',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPTRPARD' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (13,'ARDIDOS','MPTRPARD',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPTRPTDA' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (13,'DAÑADOS','MPTRPTDA',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPTRPCAR' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (13,'GRANOS CON CARBON','MPTRPCAR',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPTRPPZB' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (13,'PANZA BLANCA','MPTRPPZB',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPTRPQUE' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (13,'GRANOS QUEBRADOS','MPTRPQUE',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPTRPPIC' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (13,'GRANOS PICADOS','MPTRPPIC',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPTRPHM' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (13,'HUMEDAD','MPTRPHM',1); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPTRPINS' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (13,'INSECTOS VIVOS','MPTRPINS',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPTRPOLO' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (13,'OLOR','MPTRPOLO',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPTRPPST' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (13,'PSP TIERRA','MPTRPPST',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPTRPRTI' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (13,'REVOLCADOS EN TIERRA','MPTRPRTI',0); END

IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPGIRACI' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (5,'ACIDEZ','MPGIRACI',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPGIRAOL' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (5,'ALTO_OLEICO','MPGIRAOL',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPGIRCEX' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (5,'CUERPOS EXTRAÑOS','MPGIRCEX',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPGIRCHA' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (5,'CHAMICO','MPGIRCHA',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPGIRGAR' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (5,'ARDIDOS','MPGIRGAR',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPGIRHUM' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (5,'HUMEDAD','MPGIRHUM',1); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPGIRINV' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (5,'INSECTOS VIVOS','MPGIRINV',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPGIRMGR' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (5,'MATERIAS GRASAS','MPGIRMGR',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPGIRMOH' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (5,'AMOHOSADOS','MPGIRMOH',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPGIROLO' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (5,'OLOR','MPGIROLO',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPGIRREV' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (5,'REVOLCADOS EN TIERRA','MPGIRREV',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPGIRTEM' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (5,'TEMPERATURA','MPGIRTEM',0); END
IF NOT EXISTS (select 1 from EquivalenciasCaracteristicasCpOtrosPuertos where CodigoSap = 'MPGIRTIE' ) BEGIN insert into EquivalenciasCaracteristicasCpOtrosPuertos ( Material_Id,CodigoExterno,CodigoSap,EsHumedad) values (5,'TIERRA','MPGIRTIE',0); END
GO

--Configuracion Etapas Automaticas
IF NOT EXISTS (SELECT 1 FROM ConfiguracionAutomatizacionEtapas ca WHERE ca.Actividad = 'Coordinacion' and ca.CentroId = 5) BEGIN INSERT INTO ConfiguracionAutomatizacionEtapas (Descripcion, CentroId, Actividad, MinutosEjecucion, FechaCreacion, Deshabilitada) VALUES('Pase automatico - Etapa Coordinacion', 5, 'Coordinacion', 60, '2021-09-27 08:00:00.000', 0) END
IF NOT EXISTS (SELECT 1 FROM ConfiguracionAutomatizacionEtapas ca WHERE ca.Actividad = 'AutorizarDescuentosEntregador' and ca.CentroId = 5) BEGIN INSERT INTO ConfiguracionAutomatizacionEtapas (Descripcion, CentroId, Actividad, MinutosEjecucion, FechaCreacion, Deshabilitada) VALUES('Pase automatico - Etapa Autorizar Descuentos Entregador', 5, 'AutorizarDescuentosEntregador', 60, '2021-09-27 08:00:00.000', 0) END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[NumeroCPESeq]') AND type = 'SO')
BEGIN
CREATE SEQUENCE NumeroCPESeq
START WITH 1
INCREMENT BY 1
MINVALUE 1
MAXVALUE 9999
CYCLE
;
END
GO

--Estados del buque
IF NOT EXISTS (select 1 from ConfiguracionMail where TemplateMail = 'PlanillaDeTurnos') BEGIN insert into ConfiguracionMail(TemplateMail, Direcciones) values ('PlanillaDeTurnos','SupervisoresPuertosanLorenzo@molinosagro.com.ar;sebastian.bolger@molinosagro.com.ar,fabricio.herrera@molinosagro.com.ar;jose.luis.gomez@molinosagro.com.ar;marcelo.gustavo.lopez@molinosagro.com.ar;sebastian.muniz@molinosagro.com.ar;german.turcutto@molinosagro.com.ar;Pablo.Yturres@molinosagro.com.ar;mauro.mir@molinosagro.com.ar;nestor.abalos@molinosagro.com.ar'); END
GO

--Configuracion Ramal Ferroviario AFIP
IF NOT EXISTS (SELECT 1 FROM RamalFerroviario rf WHERE rf.CodigoAfip = 1) BEGIN INSERT INTO RamalFerroviario (CodigoAfip, Descripcion, FechaCreacion, Deshabilitada) VALUES(1, 'Roca', '2021-11-01 19:30:00.000', 0) END
IF NOT EXISTS (SELECT 1 FROM RamalFerroviario rf WHERE rf.CodigoAfip = 2) BEGIN INSERT INTO RamalFerroviario (CodigoAfip, Descripcion, FechaCreacion, Deshabilitada) VALUES(2, 'Sarmiento', '2021-11-01 19:30:00.000', 0) END
IF NOT EXISTS (SELECT 1 FROM RamalFerroviario rf WHERE rf.CodigoAfip = 3) BEGIN INSERT INTO RamalFerroviario (CodigoAfip, Descripcion, FechaCreacion, Deshabilitada) VALUES(3, 'Mitre', '2021-11-01 19:30:00.000', 0) END
IF NOT EXISTS (SELECT 1 FROM RamalFerroviario rf WHERE rf.CodigoAfip = 4) BEGIN INSERT INTO RamalFerroviario (CodigoAfip, Descripcion, FechaCreacion, Deshabilitada) VALUES(4, 'Urquiza', '2021-11-01 19:30:00.000', 0) END
IF NOT EXISTS (SELECT 1 FROM RamalFerroviario rf WHERE rf.CodigoAfip = 5) BEGIN INSERT INTO RamalFerroviario (CodigoAfip, Descripcion, FechaCreacion, Deshabilitada) VALUES(5, 'Belgrano', '2021-11-01 19:30:00.000', 0) END
IF NOT EXISTS (SELECT 1 FROM RamalFerroviario rf WHERE rf.CodigoAfip = 6) BEGIN INSERT INTO RamalFerroviario (CodigoAfip, Descripcion, FechaCreacion, Deshabilitada) VALUES(6, 'San Martín', '2021-11-01 19:30:00.000', 0) END
IF NOT EXISTS (SELECT 1 FROM RamalFerroviario rf WHERE rf.CodigoAfip = 99) BEGIN INSERT INTO RamalFerroviario (CodigoAfip, Descripcion, FechaCreacion, Deshabilitada) VALUES(99, 'Otro', '2021-11-01 19:30:00.000', 0) END
GO

-- Motivos inactividad
IF NOT EXISTS (select 1 from MotivoInactividad where Descripcion = 'Almuerzo') BEGIN insert into MotivoInactividad (Descripcion) values ('Almuerzo'); END
IF NOT EXISTS (select 1 from MotivoInactividad where Descripcion = 'Cena') BEGIN insert into MotivoInactividad (Descripcion) values ('Cena'); END
IF NOT EXISTS (select 1 from MotivoInactividad where Descripcion = 'Desayuno – Merienda') BEGIN insert into MotivoInactividad (Descripcion) values ('Desayuno – Merienda'); END
IF NOT EXISTS (select 1 from MotivoInactividad where Descripcion = 'Cambio de turno') BEGIN insert into MotivoInactividad (Descripcion) values ('Cambio de turno'); END
IF NOT EXISTS (select 1 from MotivoInactividad where Descripcion = 'Rotación perito') BEGIN insert into MotivoInactividad (Descripcion) values ('Rotación perito'); END
IF NOT EXISTS (select 1 from MotivoInactividad where Descripcion = 'Rotación caladores') BEGIN insert into MotivoInactividad (Descripcion) values ('Rotación caladores'); END
IF NOT EXISTS (select 1 from MotivoInactividad where Descripcion = 'Presencia de Carga No Homogénea') BEGIN insert into MotivoInactividad (Descripcion) values ('Presencia de Carga No Homogénea'); END
IF NOT EXISTS (select 1 from MotivoInactividad where Descripcion = 'Intervención Mecánico J1') BEGIN insert into MotivoInactividad (Descripcion) values ('Intervención Mecánico J1'); END
IF NOT EXISTS (select 1 from MotivoInactividad where Descripcion = 'Falta camiones Pre Calado') BEGIN insert into MotivoInactividad (Descripcion) values ('Falta camiones Pre Calado'); END
IF NOT EXISTS (select 1 from MotivoInactividad where Descripcion = 'Post calado lleno por gestión de filas') BEGIN insert into MotivoInactividad (Descripcion) values ('Post calado lleno por gestión de filas'); END
IF NOT EXISTS (select 1 from MotivoInactividad where Descripcion = 'Intervención eléctrica J1') BEGIN insert into MotivoInactividad (Descripcion) values ('Intervención eléctrica J1'); END
IF NOT EXISTS (select 1 from MotivoInactividad where Descripcion = 'Intervención Mecánica J2') BEGIN insert into MotivoInactividad (Descripcion) values ('Intervención Mecánica J2'); END
IF NOT EXISTS (select 1 from MotivoInactividad where Descripcion = 'Intervención eléctrica J2') BEGIN insert into MotivoInactividad (Descripcion) values ('Intervención eléctrica J2'); END
IF NOT EXISTS (select 1 from MotivoInactividad where Descripcion = 'Mantenimiento Jorgensen J1') BEGIN insert into MotivoInactividad (Descripcion) values ('Mantenimiento Jorgensen J1'); END
IF NOT EXISTS (select 1 from MotivoInactividad where Descripcion = 'Mantenimiento Jorgensen J2') BEGIN insert into MotivoInactividad (Descripcion) values ('Mantenimiento Jorgensen J2'); END
IF NOT EXISTS (select 1 from MotivoInactividad where Descripcion = 'Limpieza filtros') BEGIN insert into MotivoInactividad (Descripcion) values ('Limpieza filtros'); END
IF NOT EXISTS (select 1 from MotivoInactividad where Descripcion = 'Sonda tapada') BEGIN insert into MotivoInactividad (Descripcion) values ('Sonda tapada'); END
IF NOT EXISTS (select 1 from MotivoInactividad where Descripcion = 'Limpieza calles') BEGIN insert into MotivoInactividad (Descripcion) values ('Limpieza calles'); END
IF NOT EXISTS (select 1 from MotivoInactividad where Descripcion = 'Sanitarios') BEGIN insert into MotivoInactividad (Descripcion) values ('Sanitarios'); END
IF NOT EXISTS (select 1 from MotivoInactividad where Descripcion = 'Camión roto') BEGIN insert into MotivoInactividad (Descripcion) values ('Camión roto'); END

GO

--Entidades
IF NOT EXISTS (SELECT 1 FROM Entidad WHERE Codigo = 'EVIST') BEGIN INSERT INTO Entidad VALUES('EVIST', 'Visteo') END
GO

--Tipos De Actividad
IF NOT EXISTS (SELECT 1 FROM TipoDeActividad WHERE Codigo = 'TRECH') BEGIN INSERT INTO TipoDeActividad VALUES('TRECH', 'Rechazar') END
GO

--Entidades con Tipos De Actividad
IF NOT EXISTS (SELECT 1 FROM EntidadTipoDeActividad WHERE Entidad_Id = (SELECT Id FROM Entidad WHERE Codigo = 'EVIST') AND TipoDeActividad_Id = (SELECT Id FROM TipoDeActividad WHERE Codigo = 'TRECH')) BEGIN INSERT INTO EntidadTipoDeActividad VALUES((SELECT Id FROM Entidad WHERE Codigo = 'EVIST'), (SELECT Id FROM TipoDeActividad WHERE Codigo = 'TRECH')) END
GO

--Configuraciones Generales
IF NOT EXISTS (SELECT 1 FROM ConfiguracionGeneral WHERE Pantalla = 'AFIP' AND  Nombre = 'ConsultasParalelas' AND  Centro_Id IS NULL) 
BEGIN 
INSERT INTO ConfiguracionGeneral(Pantalla,Nombre,Valor,Centro_Id,FechaCreacion,UsuarioCreacion) VALUES ('AFIP', 'ConsultasParalelas', '1', NULL, GETDATE(), 'SCATO')
END

IF NOT EXISTS (SELECT 1 FROM ConfiguracionGeneral WHERE Pantalla = 'EficienciaCalado' AND  Nombre = 'EficienciaCalles' AND  Centro_Id = 5) 
BEGIN 
INSERT INTO ConfiguracionGeneral(Pantalla,Nombre,Valor,Centro_Id,FechaCreacion,UsuarioCreacion) VALUES ('EficienciaCalado', 'EficienciaCalles', '[]', 5, GETDATE(), 'SCATO')
END

IF NOT EXISTS (SELECT 1 FROM ConfiguracionGeneral WHERE Pantalla = 'EficienciaCalado' AND  Nombre = 'HorarioTurno' AND  Centro_Id = 5) 
BEGIN 
INSERT INTO ConfiguracionGeneral(Pantalla,Nombre,Valor,Centro_Id,FechaCreacion,UsuarioCreacion) VALUES ('EficienciaCalado', 'HorarioTurno', '{"HoraEntrada":"08:00","HoraSalida":"20:00"}', 5, GETDATE(), 'SCATO')
END

IF NOT EXISTS (SELECT 1 FROM ConfiguracionGeneral WHERE Pantalla = 'EstadoDeCallePreCalado' AND  Nombre = 'LimiteFilasLlamadas' AND  Centro_Id IS NULL) 
BEGIN 
INSERT INTO ConfiguracionGeneral(Pantalla,Nombre,Valor,Centro_Id,FechaCreacion,UsuarioCreacion) VALUES ('EstadoDeCallePreCalado', 'LimiteFilasLlamadas', '6', NULL, GETDATE(), 'SCATO')
END

IF NOT EXISTS (SELECT 1 FROM ConfiguracionGeneral WHERE Pantalla = 'MuestraInase' AND  Centro_Id IS NULL) 
BEGIN INSERT INTO ConfiguracionGeneral(Pantalla,Nombre,Valor,Centro_Id,FechaCreacion,UsuarioCreacion) 
VALUES	('MuestraInase', 'MailDestino', 'info@bolsatech.com.ar', NULL, GETDATE(), 'SCATO')
END

IF NOT EXISTS (SELECT 1 FROM ConfiguracionGeneral WHERE Pantalla = 'MuestraInase' AND  Centro_Id = 5) 
BEGIN INSERT INTO ConfiguracionGeneral(Pantalla,Nombre,Valor,Centro_Id,FechaCreacion,UsuarioCreacion) 
VALUES	('MuestraInase', 'MailDestinatarios', 'carlos.cortina@molinosagro.com.ar;sergio.schoeder@molinosagro.com.ar;sergio.schoeder@molinosagro.com.ar', 5, GETDATE(), 'SCATO'),
		('MuestraInase', 'Responsable', 'Carlos Cortina', 5, GETDATE(), 'SCATO'),
		('MuestraInase', 'Contacto', 'Email: carlos.cortina@molinosagro.com.ar', 5, GETDATE(), 'SCATO'),
		('MuestraInase', 'Horario', 'L a V de 8 a 14hs', 5, GETDATE(), 'SCATO')
END

IF NOT EXISTS (SELECT 1 FROM ConfiguracionGeneral WHERE Pantalla = 'EstadoDeCallePreCalado' AND  Nombre = 'CartelLedCalador' AND  Centro_Id IS NULL) 
BEGIN 
INSERT INTO ConfiguracionGeneral(Pantalla,Nombre,Valor,Centro_Id,FechaCreacion,UsuarioCreacion) VALUES ('EstadoDeCallePreCalado', 'CartelLedCalador', '', NULL, GETDATE(), 'SCATO')
END

IF NOT EXISTS (SELECT 1 FROM ConfiguracionGeneral WHERE Pantalla = 'EstadoDeCallePreCalado' AND  Nombre = 'Calador 1' AND  Centro_Id IS NULL) 
BEGIN 
INSERT INTO ConfiguracionGeneral(Pantalla,Nombre,Valor,Centro_Id,FechaCreacion,UsuarioCreacion) VALUES ('EstadoDeCallePreCalado', 'Calador 1', 'Calle 1 o 2', NULL, GETDATE(), 'SCATO')
END

IF NOT EXISTS (SELECT 1 FROM ConfiguracionGeneral WHERE Pantalla = 'EstadoDeCallePreCalado' AND  Nombre = 'Calador 2' AND  Centro_Id IS NULL) 
BEGIN 
INSERT INTO ConfiguracionGeneral(Pantalla,Nombre,Valor,Centro_Id,FechaCreacion,UsuarioCreacion) VALUES ('EstadoDeCallePreCalado', 'Calador 2', 'Calle 3 o 4', NULL, GETDATE(), 'SCATO')
END

IF NOT EXISTS (SELECT 1 FROM ConfiguracionGeneral WHERE Pantalla = 'PreLote' AND  Nombre = 'HorarioNocturnoDesde' AND  Centro_Id IS NULL) 
BEGIN 
INSERT INTO ConfiguracionGeneral(Pantalla,Nombre,Valor,Centro_Id,FechaCreacion,UsuarioCreacion) VALUES ('PreLote', 'HorarioNocturnoDesde', '21', NULL, GETDATE(), 'SCATO')
END

IF NOT EXISTS (SELECT 1 FROM ConfiguracionGeneral WHERE Pantalla = 'PreLote' AND  Nombre = 'HorarioNocturnoHasta' AND  Centro_Id IS NULL) 
BEGIN 
INSERT INTO ConfiguracionGeneral(Pantalla,Nombre,Valor,Centro_Id,FechaCreacion,UsuarioCreacion) VALUES ('PreLote', 'HorarioNocturnoHasta', '7', NULL, GETDATE(), 'SCATO')
END

IF NOT EXISTS (SELECT 1 FROM ConfiguracionGeneral WHERE Pantalla = 'EstadoDeCallePostCalado' AND  Nombre = 'CartelLedPostCalado' AND  Centro_Id IS NULL) 
BEGIN 
INSERT INTO ConfiguracionGeneral(Pantalla,Nombre,Valor,Centro_Id,FechaCreacion,UsuarioCreacion) VALUES ('EstadoDeCallePostCalado', 'CartelLedPostCalado', '', NULL, GETDATE(), 'SCATO')
END

IF NOT EXISTS (SELECT 1 FROM ConfiguracionGeneral WHERE Pantalla = 'EstadoPlayaInterna' AND  Nombre = 'CartelLedPreBalanza' AND  Centro_Id IS NULL) 
BEGIN 
INSERT INTO ConfiguracionGeneral(Pantalla,Nombre,Valor,Centro_Id,FechaCreacion,UsuarioCreacion) VALUES ('EstadoPlayaInterna', 'CartelLedPreBalanza', '', NULL, GETDATE(), 'SCATO')
END

IF NOT EXISTS (SELECT 1 FROM ConfiguracionGeneral WHERE Pantalla = 'EstadoVolcadoras' AND  Nombre = 'CartelLedIntervalo' AND  Centro_Id IS NULL) 
BEGIN 
INSERT INTO ConfiguracionGeneral(Pantalla,Nombre,Valor,Centro_Id,FechaCreacion,UsuarioCreacion) VALUES ('EstadoVolcadoras', 'CartelLedIntervalo', '5000', NULL, GETDATE(), 'SCATO')
END

IF NOT EXISTS (SELECT 1 FROM ConfiguracionGeneral WHERE Pantalla = 'Establecimiento' AND  Nombre = 'RangoMax' AND  Centro_Id IS NULL) 
BEGIN 
INSERT INTO ConfiguracionGeneral(Pantalla,Nombre,Valor,Centro_Id,FechaCreacion,UsuarioCreacion) VALUES ('Establecimiento', 'RangoMax', '730000', NULL, GETDATE(), 'SCATO')
END

IF NOT EXISTS (SELECT 1 FROM ConfiguracionGeneral WHERE Pantalla = 'Establecimiento' AND  Nombre = 'RangoMin' AND  Centro_Id IS NULL) 
BEGIN 
INSERT INTO ConfiguracionGeneral(Pantalla,Nombre,Valor,Centro_Id,FechaCreacion,UsuarioCreacion) VALUES ('Establecimiento', 'RangoMin', '700000', NULL, GETDATE(), 'SCATO')
END


IF NOT EXISTS (SELECT 1 FROM ConfiguracionGeneral WHERE Pantalla = 'EstadoPlayaInterna' AND  Nombre = 'LlamadoAutomaticoPrebalanza' AND  Centro_Id IS NULL) 
BEGIN 
INSERT INTO ConfiguracionGeneral(Pantalla,Nombre,Valor,Centro_Id,FechaCreacion,UsuarioCreacion) VALUES ('EstadoPlayaInterna', 'LlamadoAutomaticoPrebalanza', 'True', NULL, GETDATE(), 'SCATO')
END

IF NOT EXISTS (SELECT 1 FROM ConfiguracionGeneral WHERE Pantalla = 'EstadoPlayaInterna' AND  Nombre = 'PaseDirecto' AND  Centro_Id IS NULL) 
BEGIN 
INSERT INTO ConfiguracionGeneral(Pantalla,Nombre,Valor,Centro_Id,FechaCreacion,UsuarioCreacion) VALUES ('EstadoPlayaInterna', 'PaseDirecto', '0', NULL, GETDATE(), 'SCATO')
END

IF NOT EXISTS (SELECT 1 FROM ConfiguracionGeneral WHERE Pantalla = 'EstadoPlayaInterna' AND  Nombre = 'MaterialesPaseDirecto' AND  Centro_Id IS NULL) 
BEGIN 
INSERT INTO ConfiguracionGeneral(Pantalla,Nombre,Valor,Centro_Id,FechaCreacion,UsuarioCreacion) VALUES ('EstadoPlayaInterna', 'MaterialesPaseDirecto', '386,13,4,5', NULL, GETDATE(), 'SCATO')
END


--Equivalencias de Característica de Calidad
IF NOT EXISTS (SELECT 1 FROM EquivalenciasCaracteristicasCpOtrosPuertos WHERE CodigoSap = 'MPSOJVOL') 
BEGIN 
INSERT INTO EquivalenciasCaracteristicasCpOtrosPuertos VALUES (4,'MPSOJVOL','MPSOJVOL',0)
END

IF NOT EXISTS (SELECT 1 FROM EquivalenciasCaracteristicasCpOtrosPuertos WHERE CodigoSap = 'MPTRPVOL') 
BEGIN 
INSERT INTO EquivalenciasCaracteristicasCpOtrosPuertos VALUES (13,'MPTRPVOL','MPTRPVOL',0)
END

IF NOT EXISTS (SELECT 1 FROM EquivalenciasCaracteristicasCpOtrosPuertos WHERE CodigoSap = 'MPMAZVOL') 
BEGIN 
INSERT INTO EquivalenciasCaracteristicasCpOtrosPuertos VALUES (386,'MPMAZVOL','MPMAZVOL',0)
END

IF NOT EXISTS (SELECT 1 FROM EquivalenciasCaracteristicasCpOtrosPuertos WHERE CodigoSap = 'MPGIRVOL') 
BEGIN 
INSERT INTO EquivalenciasCaracteristicasCpOtrosPuertos VALUES (5,'MPGIRVOL','MPGIRVOL',0)
END

IF NOT EXISTS (SELECT 1 FROM EquivalenciasCaracteristicasCpOtrosPuertos WHERE CodigoSap = 'MPSOJINV') 
BEGIN 
INSERT INTO EquivalenciasCaracteristicasCpOtrosPuertos VALUES (4,'MPSOJINV','MPSOJINV',0)
END

IF NOT EXISTS (SELECT 1 FROM EquivalenciasCaracteristicasCpOtrosPuertos WHERE CodigoSap = 'MPMAZINV') 
BEGIN 
INSERT INTO EquivalenciasCaracteristicasCpOtrosPuertos VALUES (386,'MPMAZINV','MPMAZINV',0)
END

-- Variedades de Material
IF NOT EXISTS (select 1 from TipoVariedad where Codigo = 'SUS')
BEGIN
  insert into TipoVariedad (Descripcion, Codigo, Activo, FechaCreacion, CreadoPor)
  values ('Sustentable', 'SUS', 1, GETDATE(), 'lguedez'); 
END

IF NOT EXISTS (select 1 from TipoVariedad where Codigo = 'EPA')
BEGIN
  insert into TipoVariedad (Descripcion, Codigo, Activo, FechaCreacion, CreadoPor)
  values ('Epa', 'EPA', 1, GETDATE(), 'lguedez');
END

IF NOT EXISTS (select 1 from TipoVariedad where Codigo = 'IMP') 
BEGIN
  insert into TipoVariedad (Descripcion, Codigo, Activo, FechaCreacion, CreadoPor)
  values ('Import', 'IMP', 1, GETDATE(), 'lguedez');  
END