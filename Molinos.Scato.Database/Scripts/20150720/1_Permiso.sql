IF NOT EXISTS (select 1 from Permiso where Codigo = 73) BEGIN INSERT INTO [Permiso]([Descripcion],[TipoPermiso],[Codigo],[ActividadWorkflow]) VALUES ('ABM Transmisión a sap', 0, 73, NULL); END
