# WFEditor Web

## Ejecucion
Compilar utilizando msbuild

### Firma
El proyecto está firmado con ClickOnce. Renueve el certificado de prueba, en caso de estar vencido. Sino, bloqueará la compilación. Incluso en entornos de integración.

Para renovar el certificado, seguir los siguientes pasos:

1. Abrir las propiedades del proyecto
1. Seccion `Signing`
1. Marcar la opcion `Sign de ClickOnce manifests`
1. Crear nuevo certificado `Create test certificate`
1. Actualiza nuevo certificado en el repositorio.


### Error al levantar el cliente

```
rundll32 dfshim CleanOnlineAppCache
```