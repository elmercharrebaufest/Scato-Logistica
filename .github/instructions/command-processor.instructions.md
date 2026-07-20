---
applyTo: "**/Molinos.Scato.Dominio/Comandos/**/*.cs"
---

# Reglas de la capa Comandos (Dominio)

## Principio fundamental
Los comandos son objetos de datos puros que describen **qué operación ejecutar**. Se serializan vía WCF y se envían al servicio `IServicioComandos.Ejecutar(Comando)` del host `Molinos.Scato.ServiciosWeb`. Cada comando tiene un procesador correspondiente en `Molinos.Scato.Servicios/Procesamiento/`.

## Punto de entrada WCF
El contrato es:
```csharp
[ServiceContract(Namespace = "http://scato.molinos.com.ar")]
public interface IServicioComandos
{
    [OperationContract]
    Resultado Ejecutar(Comando comando);
}
```
La clase `Comando` base usa `[KnownType("TiposDeComandos")]` para descubrir automáticamente todas las subclases del assembly. No es necesario registrar los comandos manualmente.

## Estructura de un comando

```csharp
namespace Molinos.Scato.Dominio.Comandos
{
    [LoguearEntidad]   // Agregar solo si el ABM debe quedar en el log de auditoría
    public class CrearNuevoConcepto : Comando
    {
        public NuevoConceptoDto Dto { get; set; }
    }
}
```

- **Hereda de `Comando`** (que ya tiene `[DataContract]` y `[KnownType]`).
- **Las propiedades no necesitan `[DataMember]`** si heredan de `Comando` (el contrato está en la clase base). Agregarlos solo si el comando se serializa fuera del canal WCF estándar.
- **Sin lógica**: solo propiedades, sin métodos de negocio.
- El campo `Usuario` está en la clase base `Comando`; no redefinirlo.

## Convenciones de naming

| Tipo de operación | Prefijo | Ejemplo |
|-------------------|---------|---------|
| Creación ABM | `Crear` | `CrearAlmacen` |
| Modificación ABM | `Modificar` | `ModificarAlmacen` |
| Eliminación ABM | `Eliminar` | `EliminarAlmacen` |
| Validación de negocio | `Validar` | `ValidarOrdenCargaInternaFas` |
| Operación con servicios externos | verbo propio | `AutorizarCpe`, `ConfirmarArribo` |
| Impresión | `Imprimir` | `ImprimirTicketPesada` |

## Clases de resultado

Los resultados viven en el mismo folder `Comandos/` y heredan de `Resultado`:

```csharp
// Resultado genérico (ya existente): úsalo directamente si el procesador no retorna datos adicionales
// Resultado resultado = new Resultado();

// Para operaciones de creación: úsalo si necesitás devolver el Id generado
public class ResultadoCrear : Resultado
{
    public int Id { get; set; }
}

// Resultado específico cuando hay datos adicionales
public class ResultadoConsultarAlgo : Resultado
{
    public string DatoEspecifico { get; set; }
    public IList<ItemDto> Items { get; set; }
}
```

- `Resultado` tiene `HayErrores` y `Error(string clave, string descripcion)`.
- Usar `ResultadoCrear` para alta de entidades (ya tiene la propiedad `Id`).
- Crear `Resultado{Concepto}` específico solo si el cliente necesita datos adicionales.
- Los resultados que agrupan datos de servicio externo van en `ResultadoServicio/`.

## Atributo `[LoguearEntidad]`

Agregar a comandos de tipo Crear/Modificar/Eliminar que deben quedar registrados en `LogABM`. El procesador base leerá este atributo y guardará el XML del comando automáticamente.

## Prohibido en comandos
- Lógica de negocio o validaciones.
- Referencias a `IRepositorio`, `DbContext`, servicios, o cualquier infraestructura.
- Herencia entre comandos concretos (hereda siempre directo de `Comando`).
- Constructores con parámetros (Ninject/WCF necesitan constructor sin parámetros).
