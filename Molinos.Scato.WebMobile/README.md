# SCATO Mobile

## Ejecución

- La app se conecta un servicio ADFS para autenticarse. Para ello, se debe configurar el archivo web.config con los datos de conexión.

### Entorno local

- Para el entorno local, se utiliza un dummy que evita la conexión con el ADFS y permite simular un usurio y sus roles. `\Molinos.Scato.WebMobile\Seguridad\DummyAuthenticationModule.cs`