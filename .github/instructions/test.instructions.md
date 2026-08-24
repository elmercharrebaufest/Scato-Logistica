---
applyTo: "**/Molinos.Scato.Test/**/*.cs"
---
## Objetivo
Escribir tests NUnit 2.6.3 + Moq que validen comportamiento aislado por capa sin dependencias reales.
## Hacer
- Usar `[TestFixture]`, `[Test]` y `[SetUp]`
- Configurar mocks con `Mock<T>` + `.Setup(...)` y validar interacciones con `.Verify(...)`
- Para actividades WF4.5 usar `WorkflowInvokerTest.Create(target)` y `host.Extensions.Add(...)`
- Usar nombres de test descriptivos y una sola intención por test
- Escribir aserciones con `Assert.That(..., Is....)`
- Reusar `FactoryContext` en tests de controllers
## No hacer
- No usar `Assert.AreEqual` ni `Assert.IsNotNull`
- No escribir tests `async` en NUnit 2.6.3
- No crear mocks innecesarios
- No compartir estado mutable entre tests
## Ejemplo mínimo
```csharp
[TestFixture] public class MiActividadTest {
    [Test] public void TestEjecucionExitosaRetornaResultado() {
        var srv = new Mock<IServicioComandos>();
        srv.Setup(x => x.Ejecutar(It.IsAny<MiComando>())).Returns(new Resultado());
        var host = WorkflowInvokerTest.Create(new MiActividad()); host.Extensions.Add(srv.Object); host.InArguments.RecorridoId = 1;
        var r = host.TestActivity();
        Assert.That(r, Is.Not.Null);
    }
}
```
