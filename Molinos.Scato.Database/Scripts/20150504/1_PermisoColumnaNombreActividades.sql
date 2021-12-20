UPDATE Permiso 
SET NombreActividad = logAct.Actividad
FROM (Select la.actividad,la.ActividadXaml from LogActividad la Inner Join Permiso p
on la.ActividadXaml = p.ActividadWorkflow) as logAct
where logAct.ActividadXaml = Permiso.ActividadWorkflow


UPDATE Permiso
  SET  Permiso.NombreActividad = 'Autorizar Transportista Inhabilitado' 
  WHERE Permiso.ActividadWorkflow = 'AutorizarTransportistaInhabilitado'

  UPDATE Permiso
  SET  Permiso.NombreActividad = 'Autorizar Recepción de Uvas' 
  WHERE Permiso.ActividadWorkflow = 'AutorizarRecepcionUvas'

  UPDATE Permiso
  SET Permiso.NombreActividad = 'Cargar Carta Porte' 
  WHERE Permiso.ActividadWorkflow= 'CargarCartaPorte'

  UPDATE Permiso
  SET  Permiso.NombreActividad = 'Cargar Carta Porte Fasón' 
  WHERE Permiso.ActividadWorkflow = 'CargarCartaPorteFason'
  
  UPDATE Permiso
  SET  Permiso.NombreActividad = 'Cargar Carta Porte Prestamo' 
  WHERE Permiso.ActividadWorkflow = 'CargarCartaPortePrestamo'
  
  UPDATE Permiso
  SET  Permiso.NombreActividad = 'Ingresar Carta de Porte Egreso por Desvío' 
  WHERE ActividadWorkflow = 'CargarCartaPorteRedespachoDesvio'

  UPDATE Permiso
  SET Permiso.NombreActividad = 'Cargar Hoja de Ruta' 
  WHERE Permiso.ActividadWorkflow = 'CargarHojaDeRuta'

  UPDATE Permiso
  SET  Permiso.NombreActividad = 'Cargar Hoja de Ruta Yerbatera' 
  WHERE ActividadWorkflow = 'CargarHojaDeRutaYerbatera'

  UPDATE Permiso
  SET  Permiso.NombreActividad = 'Cargar Orden de Carga por Contenedor' 
  WHERE Permiso.ActividadWorkflow = 'CargarOrdenDeCargaContenedor'

  UPDATE Permiso
  SET  Permiso.NombreActividad = 'Ingresar Orden de Descarga' 
  WHERE Permiso.ActividadWorkflow ='CargarOrdenDeDescarga'

  UPDATE Permiso
  SET Permiso.NombreActividad = 'Aprobar Diferencia Peso Neto Descarga de Unidad' 
  WHERE Permiso.ActividadWorkflow = 'ControlPesoNetoDescargaUnidad'

  UPDATE Permiso
  SET  Permiso.NombreActividad = 'Control Peso Neto Romaneo' 
  WHERE Permiso.ActividadWorkflow = 'ControlPesoNetoRomaneo'

  UPDATE Permiso
  SET Permiso.NombreActividad = 'Descarga Por Unidad' 
  WHERE Permiso.ActividadWorkflow = 'DescargaUnidad'

  UPDATE Permiso
  SET  Permiso.NombreActividad = 'Distribución de almacenes' 
  WHERE Permiso.ActividadWorkflow = 'DistribucionDeAlmacenes'

  UPDATE Permiso
  SET  Permiso.NombreActividad = 'Documento De Ingreso' 
  WHERE Permiso.ActividadWorkflow = 'DocumentoDeIngreso'

  UPDATE Permiso
  SET  Permiso.NombreActividad = 'Ingresar Carta Porte Redespacho Prestamo' 
  WHERE Permiso.ActividadWorkflow = 'IngresarCartaPorteRedespachoPrestamo'

  UPDATE Permiso
  SET  Permiso.NombreActividad = 'Ingreso Bins de Salida' 
  WHERE Permiso.ActividadWorkflow = 'IngresoBinSalida'

  UPDATE Permiso
  SET Permiso.NombreActividad = 'Ingreso CIU' 
  WHERE Permiso.ActividadWorkflow = 'IngresoCIU'

  UPDATE Permiso
  SET Permiso.NombreActividad = 'Ingreso Número de COT' 
  WHERE Permiso.ActividadWorkflow = 'IngresoNumeroCot'

  UPDATE Permiso
  SET Permiso.NombreActividad = 'Lista de Tareas' 
  WHERE Permiso.ActividadWorkflow = 'ListaDeCamiones'

  UPDATE Permiso
  SET  Permiso.NombreActividad = 'Pesada' 
  WHERE Permiso.ActividadWorkflow = 'Pesada'
  
  UPDATE Permiso
  SET  Permiso.NombreActividad = 'Pesada Bruto Vagón' 
  WHERE Permiso.ActividadWorkflow = 'PesadaBrutoVagon'
  
  UPDATE Permiso
  SET  Permiso.NombreActividad = 'Pesada Tara Vagón' 
  WHERE Permiso.ActividadWorkflow = 'PesadaTaraVagon'
  
  UPDATE Permiso
  SET  Permiso.NombreActividad = 'Remito Bodega Uva Propia' 
  WHERE Permiso.ActividadWorkflow = 'RemitoBodegaUvaPropia'
  
  UPDATE Permiso
  SET  Permiso.NombreActividad = 'Remito Bodega Uva Terceros' 
  WHERE Permiso.ActividadWorkflow = 'RemitoBodegaUvaTerceros'
  
  UPDATE Permiso
  SET Permiso.NombreActividad = 'Romaneo' 
  WHERE Permiso.ActividadWorkflow = 'Romaneo'
  
  UPDATE Permiso
  SET  Permiso.NombreActividad = 'Fletes Doble Tramo' 
  WHERE Permiso.ActividadWorkflow = 'ServicioSap_Z4030'