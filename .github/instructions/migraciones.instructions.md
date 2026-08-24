---
applyTo: "**/Molinos.Scato.Database/**/*.sql"
---
## Objetivo
Cambios SQL para `Molinos.Scato.Database` (SSDT) que se despliegan con Publish. Priorizar definición por objeto y scripts de datos idempotentes.

## Hacer
- Para cambios de esquema, editar el objeto correspondiente en `Molinos.Scato.Database/dbo/Tables`, `Molinos.Scato.Database/dbo/Views`, `Molinos.Scato.Database/dbo/Stored Procedures`, etc.
- Para datos de referencia/backfill, usar scripts de `Scripts\Post-Deployment\` con guardas `IF NOT EXISTS` / `IF EXISTS`.
- Mantener scripts de datos idempotentes: ejecutar N veces no debe cambiar el estado tras la primera ejecución.
- Proteger **todos** los INSERT con `IF NOT EXISTS (SELECT 1 FROM ... WHERE ...)` antes del `BEGIN`
- Proteger CREATE con `IF NOT EXISTS` / proteger DROP con `IF EXISTS`
- Usar `GETDATE()` para columnas de fecha y `'SCATO'` como `UsuarioCreacion` en inserts de referencia
- Si se agrega un script nuevo de post-deploy, asegurar que quede incluido por el `.sqlproj` (directo o vía script agregador).

## No hacer
- No usar `DROP TABLE` sin `IF EXISTS`
- No hardcodear IDs numéricos en inserts de datos de referencia — usar subselects o variables
- No crear scripts versionados tipo `R.XX.YY.ZZ-...` como flujo principal de despliegue
- No asumir ejecución por orden alfabético de archivos: Publish despliega según modelo SSDT + pre/post deploy configurado

## Ejemplo mínimo
```sql
IF NOT EXISTS (
    SELECT 1 FROM ConfiguracionGeneral
    WHERE Pantalla = 'AFIP'
      AND Nombre   = 'NuevoParametro'
      AND Centro_Id IS NULL
)
BEGIN
    INSERT INTO ConfiguracionGeneral (Pantalla, Nombre, Valor, Centro_Id, FechaCreacion, UsuarioCreacion)
    VALUES ('AFIP', 'NuevoParametro', '1', NULL, GETDATE(), 'SCATO')
END
```
