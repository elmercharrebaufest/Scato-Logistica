# Referencia: Refactor de actividades de Impresión

## El problema

Las actividades `ImpresionGenerica`, `ImpresionReciboMunicipal` y similares tienen 
**más de 80 `InArgument` individuales** que representan campos del mismo documento.
Esto hace imposible testearlas con parámetros explícitos y ensucia el XAMLX.

---

## Estrategia: DTO de documento + actividad fina

### Paso 1 — Identificar los grupos de campos

| Grupo | Campos | DTO sugerido |
|---|---|---|
| Intervinientes | Titular, Intermediario, RtteComercial, Corredor, Entregador, Destinatario, Transportista, Chofer | `IntervinientesImpresionDto` |
| Destino | Destino, DireccionDestino, LocalidadDestino, ProvinciaDestino, CodigoPostalDestino | Puede ser parte del DTO principal |
| Pesos | PesoBrutoOrigen, PesoTaraOrigen, PesoNetoOrigen, PesoBruto, PesoTara, PesoNeto | `PesosImpresionDto` |
| Vehículo | Patente, PatenteAcoplado, TipoDeVehiculoId, MarcaVehiculo, ModeloVehiculo | |
| Metadata workflow | WorkflowId, PuestoDeTrabajoId, CentroId, CodigoDeImpresion | Mantener como InArguments separados |

### Paso 2 — Crear el DTO principal

```csharp
// Molinos.Scato.Dominio/Dto/DatosImpresionCartaPorteDto.cs
public class DatosImpresionCartaPorteDto
{
    public string CTG                   { get; set; }
    public DateTime? FechaEmision       { get; set; }
    public DateTime? FechaCP            { get; set; }
    public DateTime? FechaVencimiento   { get; set; }

    // Intervinientes
    public string TitularCP             { get; set; }
    public string CuitTitularCP         { get; set; }
    public string Intermediario         { get; set; }
    public string CuitIntermediario     { get; set; }
    public string RtteComercial         { get; set; }
    public string CuitRtteComercial     { get; set; }
    public string Corredor              { get; set; }
    public string CuitCorredor          { get; set; }
    public string Entregador            { get; set; }
    public string CuitEntregador        { get; set; }
    public string Destinatario          { get; set; }
    public string CuitDestinatario      { get; set; }
    public string Transportista         { get; set; }
    public string CuitTransportista     { get; set; }
    public string Chofer                { get; set; }
    public string CuitChofer            { get; set; }

    // Destino
    public string Destino               { get; set; }
    public string DireccionDestino      { get; set; }
    public string LocalidadDestino      { get; set; }
    public string ProvinciaDestino      { get; set; }
    public string CodigoPostalDestino   { get; set; }

    // Grano / mercadería
    public string Variedad              { get; set; }
    public string Cosecha               { get; set; }
    public string Procedencia           { get; set; }
    public string CodigoEstablecimiento { get; set; }

    // Pesos origen
    public string PesoBrutoOrigen       { get; set; }
    public string PesoTaraOrigen        { get; set; }
    public string PesoNetoOrigen        { get; set; }

    // Pesos destino
    public string PesoBruto             { get; set; }
    public string PesoTara              { get; set; }
    public string PesoNeto              { get; set; }

    // Vehículo
    public string Patente               { get; set; }
    public string PatenteAcoplado       { get; set; }
    public int    TipoDeVehiculoId      { get; set; }
    public string MarcaVehiculo         { get; set; }
    public string ModeloVehiculo        { get; set; }

    // Comercial
    public string KmARecorrer           { get; set; }
    public string TarifaReferencia      { get; set; }
    public string TarifaTonelada        { get; set; }
    public string CodigoAnexo           { get; set; }
    public string AcuerdoMarco          { get; set; }
    public string Caratula              { get; set; }
    public bool   FletePagado           { get; set; }

    // Observaciones y otros
    public string NumeroDeDocumentoDeIngreso { get; set; }
    public string Observaciones              { get; set; }
    public string ObservacionesONCCA         { get; set; }
    public string TipoDeComprobanteONCCA     { get; set; }
    public string SaldosSTOCK                { get; set; }

    // Campos específicos bodega/uva
    public string   NumeroCiu             { get; set; }
    public string   RazonSocialVinatero   { get; set; }
    public string   INVVinatero           { get; set; }
    public string   CuitVinatero          { get; set; }
    public string   IIBBVinatero          { get; set; }
    public bool     EsUva                 { get; set; }
    public bool     EsUvaPropia           { get; set; }
    public DateTime? FechaPesoNetoBodega  { get; set; }
}
```

### Paso 3 — Actividad refactorizada

```csharp
public sealed class ImpresionGenerica : CodeActivity<Resultado>
{
    // Argumentos de metadata workflow — se mantienen como InArgument individuales
    [RequiredArgument] public InArgument<string> CodigoDeImpresion  { get; set; }
    [RequiredArgument] public InArgument<Guid>   WorkflowId         { get; set; }
    [RequiredArgument] public InArgument<int>    PuestoDeTrabajoId  { get; set; }
    [RequiredArgument] public InArgument<int>    CentroId           { get; set; }
    [RequiredArgument] public InArgument<bool>   EsDestinoCliente   { get; set; }

    // ✅ Un solo argumento para los datos del documento
    [RequiredArgument] public InArgument<DatosImpresionCartaPorteDto> Datos { get; set; }

    // Opcionales de impresión
    public InArgument<int> BocaDestinoId { get; set; }
    public InArgument<int> MaterialId    { get; set; }

    protected override Resultado Execute(CodeActivityContext context)
    {
        var datos    = Datos.Get(context);
        var resultado = new Resultado();
        try
        {
            var servicio = context.GetExtension<IServicioImpresion>();
            if (servicio == null)
            {
                resultado.Errores.Add("Servicio", Textos.Error_ActualizarGenerico);
                return resultado;
            }
            // ... lógica de impresión usando 'datos'
        }
        catch (Exception ex)
        {
            resultado.Errores.Add("ErrorImpresion", ex.Message);
        }
        return resultado;
    }
}
```

---

## Prompt para ejecutar este refactor en Copilot

```
Refactoriza ImpresionGenerica.cs para reemplazar todos los InArgument 
individuales de campos del documento por un único InArgument<DatosImpresionCartaPorteDto> Datos.
Pasos:
1. Crea DatosImpresionCartaPorteDto en Molinos.Scato.Dominio/Dto/ con todas las propiedades 
   de los InArguments que se eliminan (mantén los tipos y nombres exactos de propiedades).
2. Reemplaza los InArgument individuales del documento en ImpresionGenerica por 
   InArgument<DatosImpresionCartaPorteDto> Datos con [RequiredArgument].
3. Actualiza el cuerpo de Execute() para leer las propiedades desde datos.Get(context).
4. Mantén sin cambios: CodigoDeImpresion, WorkflowId, PuestoDeTrabajoId, CentroId, 
   EsDestinoCliente, BocaDestinoId, MaterialId — estos siguen como InArguments individuales.
5. Actualiza ImpresionGenericaTest.cs para construir el DatosImpresionCartaPorteDto en el SetUp.
No modifiques las actividades ImpresionReciboMunicipal u otras — solo ImpresionGenerica.cs.
```

---

## Actividades de impresión ordenadas por complejidad de refactor

| Actividad | Líneas | Campos InArgument | Prioridad |
|---|---|---|---|
| `ImpresionGenerica` | 374 | ~80 | 🔴 Alta |
| `ImpresionReciboMunicipal` | 314 | ~60 | 🔴 Alta |
| `ImpresionAsigRecorrCtrolCalid` | 230 | ~40 | 🟡 Media |
| `ImpresionCertificadoDeAnalisis` | 193 | ~35 | 🟡 Media |
| `ImpresionDeclaracionFosfinaV2` | 178 | ~30 | 🟡 Media |
| `ImpresionGenericaFile` | 174 | ~25 | 🟢 Baja |
