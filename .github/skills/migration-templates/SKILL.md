---
name: migration-templates
description: Plantillas SQL listas para cambios de esquema en Molinos.Scato.Database (SSDT Publish). Usar al modificar tablas/índices y scripts post-deploy idempotentes.
---

# Migration Templates — SSDT Publish + SQL Server

## Flujo base

1. Cambios de estructura: editar objeto en `Molinos.Scato.Database/dbo/...`.
2. Datos de referencia/backfill: script en `Molinos.Scato.Database/Scripts/Post-Deployment/`.
3. Asegurar idempotencia para DML (`IF NOT EXISTS` / `IF EXISTS`).
4. Verificar impacto en entidad/mapeo EF5 (`Dominio` + `ScatoDbContext`) cuando aplique.

---

## Agregar columna nullable (archivo de tabla en `dbo/Tables/{Tabla}.sql`)

```sql
ALTER TABLE [dbo].[MiTabla]
    ADD [NuevaColumna] NVARCHAR(200) NULL;
```

## Agregar columna NOT NULL con default

```sql
ALTER TABLE [dbo].[MiTabla]
    ADD [NuevaColumna] INT CONSTRAINT [DF_MiTabla_NuevaColumna] DEFAULT ((0)) NOT NULL;
```

## Crear tabla nueva

```sql
CREATE TABLE [dbo].[MiTabla] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Nombre] NVARCHAR(200) NOT NULL,
    [FechaCreacion] DATETIME NOT NULL CONSTRAINT [DF_MiTabla_FechaCreacion] DEFAULT (GETDATE()),
    [UsuarioCreacion] NVARCHAR(100) NOT NULL,
    CONSTRAINT [PK_MiTabla] PRIMARY KEY CLUSTERED ([Id] ASC)
);
```

> **EF5**: nombre de tabla = nombre de la entidad (sin plural). Agregar `DbSet<MiTabla>` y mapeo en `ScatoDbContext.OnModelCreating`.

## Agregar índice no agrupado

```sql
CREATE NONCLUSTERED INDEX [IX_MiTabla_Campo]
    ON [dbo].[MiTabla]([Campo] ASC)
    WITH (PAD_INDEX = ON, FILLFACTOR = 90, ONLINE = ON, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON);
```

## Agregar índice con INCLUDE

```sql
CREATE NONCLUSTERED INDEX [IX_MiTabla_Campo_incl]
    ON [dbo].[MiTabla]([Campo1] ASC, [Campo2] ASC)
    INCLUDE ([Campo3], [Campo4])
    WITH (PAD_INDEX = ON, FILLFACTOR = 90, ONLINE = ON);
```

## Insertar dato de configuración (Post-Deployment)

```sql
IF NOT EXISTS (
    SELECT 1 FROM ConfiguracionGeneral
    WHERE Pantalla = 'SECCION' AND Nombre = 'ClaveSetting' AND Centro_Id IS NULL
)
BEGIN
    INSERT INTO ConfiguracionGeneral (Pantalla, Nombre, Valor, Centro_Id, FechaCreacion, UsuarioCreacion)
    VALUES ('SECCION', 'ClaveSetting', 'valor_default', NULL, GETDATE(), 'SCATO');
END
```

## Actualizar datos existentes sin duplicar (Post-Deployment)

```sql
UPDATE dbo.MiTabla
SET NombreNuevo = NombreViejo
WHERE NombreNuevo IS NULL
  AND NombreViejo IS NOT NULL;
```

## Nunca hacer

- Usar `Molinos.Scato.Migrations` como flujo principal de despliegue.
- Scripts de datos sin guardas de idempotencia.
- `ALTER TABLE ADD NOT NULL` sin `DEFAULT` en tablas con datos existentes.
- `DROP COLUMN` sin confirmación explícita del equipo.
- Lógica de negocio en scripts — solo DDL/DML de estructura y datos de referencia.
