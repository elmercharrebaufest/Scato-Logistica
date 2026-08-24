---
applyTo: "**/Molinos.Scato.Actividades/**/*.cs"
---
## Objetivo
Definir actividades WF4.5 que ejecutan una unidad de trabajo del workflow logístico con contratos estables para xamlx.
## Hacer
- Heredar de `CodeActivity` o `NativeActivity` según necesidad de bookmarks/children
- Declarar `sealed` en actividades simples
- Resolver dependencias con `context.GetExtension<T>()`
- Marcar argumentos obligatorios con `[RequiredArgument]`
- Usar `InOutArgument<T>` solo cuando se lee y escribe el mismo valor
- Mapear errores a `Resultado.Errores` y devolverlos por `OutArgument<Resultado>`
## No hacer
- No usar `async/await` en actividades WF4.5
- No inyectar servicios por constructor o propiedad pública
- No acceder directo a `ScatoDbContext`; usar servicios/repositorio vía extensión
- No dejar excepciones sin capturar en `Execute`
## Ejemplo mínimo
```csharp
public sealed class MiActividad : CodeActivity {
    [RequiredArgument] public InArgument<int> RecorridoId { get; set; }
    [RequiredArgument] public OutArgument<Resultado> Resultado { get; set; }
    protected override void Execute(CodeActivityContext c) {
        try { Resultado.Set(c, c.GetExtension<IServicioComandos>().Ejecutar(new MiComando { RecorridoId = RecorridoId.Get(c) })); }
        catch (Exception e) { var r = new Resultado(); r.Errores.Add(string.Empty, e.Message); Resultado.Set(c, r); }
    }
}
```
