--Creacion Centros Virtuales - San Lorenzo
insert into Centro values ('1029','San Lorenzo',	NULL	,NULL,	NULL,	NULL,	NULL,	NULL,	NULL,	0,	0	,0,	0,	0	,0	,0	,0,0,	NULL,	'30-71511877-3'	,NULL,	NULL,	NULL,	NULL,	2,	21,	17536,	'BENIELLI 398'	,1,	'404189'	,'S2200AAA',	'0',	0	,NULL,	NULL	,NULL,	1	,0)
insert into UsuarioCentro (Usuario_Id,Centro_Id) select ur.usuario_id,c.id  from centro c ,UsuarioRol ur inner join Rol r on ur.Rol_Id = r.id where r.Descripcion = 'Nivel 1' and c.CodigoSAP='1029'

--
--Centro CHIVILCOY
--

declare @CentroSap nvarchar(100),@WorkflowCodigo nvarchar(100), @WorkflowCodigoAntiguo nvarchar(100)
set @CentroSap = '1034'

--Eliminar material Semilla de Soja (19908017) de Ingreso por Redespacho, Recepción Chivilcoy, 
												--Redespacho de Chivilcoy a Acopios/Plantas, Redespacho de Chivilcoy a Centros de Prestamo
delete MaterialPorWorkflow 
where id in (select id from MaterialPorWorkflow 
				where centro_id = (select id from centro where codigosap = @CentroSap) 
				and Material_Id = (select Id from Material where CodigoSAP = '19908017')
				and Workflow_Id in (select Id from Workflow where Codigo in ('1034-IngresoPorRedespacho', 'IngresoCompra',
																			 'RedespAcopiosPlantas', 'RedespCentrosPrestamo')))

delete TransaccionSAP 
where id in (select t.id from TransaccionSAP t inner join material m on m.id=t.material_id and m.codigosap='19908017' 
inner join centro c on c.id=t.CentroOrigen_Id and c.CodigoSAP = @CentroSap 
inner join workflowtipocomercial wtp on wtp.tipocomercial_id = t.tipocomercial_id
inner join workflow w on w.id = wtp.workflow_id and w.codigo in ('1034-IngresoPorRedespacho', 'IngresoCompra',
																 'RedespAcopiosPlantas', 'RedespCentrosPrestamo'))

--Workflows Nuevos
--Recepción Mercadería MOA – Carta de Porte
set @WorkflowCodigoAntiguo = 'IngresoCompra'
set @WorkflowCodigo = '1034-RecepcionMercaderiaMOA'
insert into Workflow select @WorkflowCodigo,'Recepción Mercadería MOA – Carta de Porte',(select TipoDeWorkflow from Workflow where Codigo = @WorkflowCodigoAntiguo),(select id from centro where codigosap=@CentroSap),1

insert into workflowdefinicion select top 1 [Workflow_Id]
      ,[FechaCreacion]
      ,[Comentario]
      ,[NombreUsuario]
      ,[Activa]
      ,[FechaActivacion]
      ,[ActividadInicial]
      ,[Definicion] from workflowdefinicion wd inner join Workflow w on w.id=wd.workflow_id and w.codigo = @WorkflowCodigoAntiguo order by wd.id desc

update workflowdefinicion set Workflow_Id = (select id from Workflow where codigo = @WorkflowCodigo) where id = (SELECT SCOPE_IDENTITY())
insert into MaterialPorWorkflow (Centro_Id,Material_Id,Workflow_Id) select c.id,m.id,w.id from centro c,material m,Workflow w where m.CodigoSAP in ('19908017') 
and c.codigosap = @CentroSap and w.id = (select id from workflow where codigo = @WorkflowCodigo)


--Salida de Mercadería a Plantas MOA – Carta de Porte
set @WorkflowCodigoAntiguo = 'RedespAcopiosPlantas'
set @WorkflowCodigo = '1034-SalidaMercaderiaPlantasMOA'
insert into Workflow select @WorkflowCodigo,'Salida de Mercadería a Plantas MOA – Carta de Porte',(select TipoDeWorkflow from Workflow where Codigo = @WorkflowCodigoAntiguo),(select id from centro where codigosap=@CentroSap),1

insert into workflowdefinicion select top 1 [Workflow_Id]
      ,[FechaCreacion]
      ,[Comentario]
      ,[NombreUsuario]
      ,[Activa]
      ,[FechaActivacion]
      ,[ActividadInicial]
      ,[Definicion] from workflowdefinicion wd inner join Workflow w on w.id=wd.workflow_id and w.codigo = @WorkflowCodigoAntiguo order by wd.id desc

update workflowdefinicion set Workflow_Id = (select id from Workflow where codigo = @WorkflowCodigo) where id = (SELECT SCOPE_IDENTITY())
insert into MaterialPorWorkflow (Centro_Id,Material_Id,Workflow_Id) select c.id,m.id,w.id from centro c,material m,Workflow w where m.CodigoSAP in ('19908017') 
and c.codigosap = @CentroSap and w.id = (select id from workflow where codigo = @WorkflowCodigo)

--Configuracion de Materiales

--Campo nuevo ingresado en Molinos Agro
update MaterialPorCentro set MaterialDeTerceros = 1
where Id = (select mp.Id 
			from MaterialPorCentro mp 
			inner join Material m on m.id = mp.Material_Id and m.codigosap = '19908017'
			inner join Centro c on c.Id = mp.Centro_Id and c.CodigoSAP = @CentroSap)

--
--Centro LUCCHETTI
--

--Workflows Nuevos

--Ingreso de Mercaderia de Plantas MOA – Carta de Porte:

set @CentroSap = '1018'
set @WorkflowCodigoAntiguo = '1018-IngresoPorRedespacho'
set @WorkflowCodigo = '1018-IngresoMercaderiaPlantasCartaPorteMOA'

insert into Workflow select @WorkflowCodigo,'Ingreso de Mercaderia de Plantas MOA – Carta de Porte',(select TipoDeWorkflow from Workflow where Codigo = @WorkflowCodigoAntiguo),(select id from centro where codigosap=@CentroSap),1

insert into workflowdefinicion select top 1 [Workflow_Id]
      ,[FechaCreacion]
      ,[Comentario]
      ,[NombreUsuario]
      ,[Activa]
      ,[FechaActivacion]
      ,[ActividadInicial]
      ,[Definicion] from workflowdefinicion wd inner join Workflow w on w.id=wd.workflow_id and w.codigo = @WorkflowCodigoAntiguo order by wd.id desc

update workflowdefinicion set Workflow_Id = (select id from Workflow where codigo = @WorkflowCodigo) where id = (SELECT SCOPE_IDENTITY())

insert into MaterialPorWorkflow (Centro_Id,Material_Id,Workflow_Id) 
select c.id,m.id,w.id 
from centro c,material m,Workflow w 
where m.CodigoSAP in ('19908121') 
and c.codigosap = @CentroSap and w.id = (select id from workflow where codigo = @WorkflowCodigo)

insert into WorkflowTipoComercial (Workflow_Id,TipoComercial_Id)
select w.id,tc.id from workflow w,tipocomercial tc where tc.id in (select wtc.tipocomercial_id from workflowtipocomercial wtc inner join workflow w on w.id = wtc.workflow_id and w.codigo = @WorkflowCodigoAntiguo)
and w.id = (select id from workflow where codigo = @WorkflowCodigo )

--Ingreso de Mercaderia de Plantas MOA – Remito

set @WorkflowCodigoAntiguo = '1018-IngresoPorRedespachoSemilla'
set @WorkflowCodigo = '1018-IngresoMercaderiaPlantasRemitoMOA'
insert into Workflow select @WorkflowCodigo,'Ingreso de Mercaderia de Plantas MOA – Remito',(select TipoDeWorkflow from Workflow where Codigo = @WorkflowCodigoAntiguo),(select id from centro where codigosap=@CentroSap),1

insert into workflowdefinicion select top 1 [Workflow_Id]
      ,[FechaCreacion]
      ,[Comentario]
      ,[NombreUsuario]
      ,[Activa]
      ,[FechaActivacion]
      ,[ActividadInicial]
      ,[Definicion] from workflowdefinicion wd inner join Workflow w on w.id=wd.workflow_id and w.codigo = @WorkflowCodigoAntiguo order by wd.id desc

update workflowdefinicion set Workflow_Id = (select id from Workflow where codigo = @WorkflowCodigo) where id = (SELECT SCOPE_IDENTITY())

insert into MaterialPorWorkflow (Centro_Id,Material_Id,Workflow_Id) 
select c.id,m.id,w.id 
from centro c,material m,Workflow w 
where m.CodigoSAP in ('19908022', '19908023', '19908040', '19908041', '19908042', '19908043', '19908044', 
						'19908045', '19908046', '19908047', '19908048', '19908049', '19908050', '19908051', 
						'19908052', '19908053', '19908054', '19908055', '19910172', '19910317', '19910318', 
						'19910320', '19910321', '19910322', '19910323', '19910324', '19910325', '19910326', 
						'19910351', '19910352', '19910355', '19910356', '19910360', '19911026', '19911047', 
						'19911500', '19911501', '19911502', '19911503', '19911504', '19911505', '19911506') 
and c.codigosap = @CentroSap and w.id = (select id from workflow where codigo = @WorkflowCodigo)

insert into WorkflowTipoComercial (Workflow_Id,TipoComercial_Id)
select w.id,tc.id from workflow w,tipocomercial tc where tc.id in (select wtc.tipocomercial_id from workflowtipocomercial wtc inner join workflow w on w.id = wtc.workflow_id and w.codigo = @WorkflowCodigoAntiguo)
and w.id = (select id from workflow where codigo = @WorkflowCodigo )