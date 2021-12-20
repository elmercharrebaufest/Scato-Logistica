using System;
using System.Activities;
using System.Globalization;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ImpresionDeclaracionFosfinaV2 : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<string> TipoDocumento { get; set; }
        public InArgument<string> CodigoDeImpresion { get; set; }
        [RequiredArgument]
        public InArgument<int> CantCopias { get; set; }
        [RequiredArgument]
        public InArgument<DateTime> FechaCarga { get; set; }
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        public InArgument<string> Observaciones { get; set; }
        public InArgument<int> PesoNeto { get; set; }
        [RequiredArgument]
        public InArgument<string> Patente { get; set; }
        public InArgument<string> PatenteAcoplado { get; set; }
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }
        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }
        public InArgument<string> Provincia { get; set; }
        public InArgument<string> ProvinciaRemitenteLegal { get; set; }
        public InArgument<string> LocalidadRemitenteLegal { get; set; }
        public InArgument<string> DomicilioRemitenteLegal { get; set; }
        public InArgument<string> TelefonoRemitente { get; set; }
        public InArgument<string> ProvinciaDestinatario { get; set; }
        public InArgument<string> LocalidadDestinatario { get; set; }
        public InArgument<string> DomicilioDestinatario { get; set; }
        public InArgument<int?> TransportistaId { get; set; }
        public InArgument<string> NroCartaPorte { get; set; }
        public InArgument<string> Ctg { get; set; }
        public InArgument<string> Material { get; set; }
        public InArgument<string> KmRecorrer { get; set; }
        public InArgument<string> ChoferCuil { get; set; }
        public InArgument<string> ChoferNombreCompleto { get; set; }
        public InArgument<string> DestinoDireccion { get; set; }
        public InArgument<string> DestinoLocalidad { get; set; }
        public InArgument<string> DestinoProvincia { get; set; }
        public InArgument<string> CuitDestinatario { get; set; }
        public InArgument<string> NombreDestinatario { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var tipoDocumento = TipoDocumento.Get<string>(context);
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new Resultado();
            var fechaCarga = FechaCarga.Get<DateTime>(context);
            var workflowId = WorkflowId.Get<Guid>(context);
            var centroId = CentroId.Get<int>(context);
            var codigo = CodigoDeImpresion.Get<string>(context);
            var cantCopias = CantCopias.Get<int>(context);
            var observaciones = Observaciones.Get<string>(context);
            var pesoNeto = PesoNeto.Get<int>(context);
            var patente = Patente.Get<string>(context);
            var patenteAcoplado = PatenteAcoplado.Get<string>(context);
            var provincia = Provincia.Get<string>(context);
            var provinciaRemitente = ProvinciaRemitenteLegal.Get<string>(context);
            var domicilioRemitente = DomicilioRemitenteLegal.Get<string>(context);
            var localidadRemitente = LocalidadRemitenteLegal.Get<string>(context);
            var telefono = TelefonoRemitente.Get<string>(context);
            var domicilioDestinatario = DomicilioDestinatario.Get<string>(context);
            var localidadDestinatario = LocalidadDestinatario.Get<string>(context);
            var provinciaDestinatario = ProvinciaDestinatario.Get<string>(context);
            var transportistaId = TransportistaId.Get<int?>(context);
            var nroCartaPorte = NroCartaPorte.Get<string>(context);
            var ctg = Ctg.Get<string>(context);
            var material = Material.Get<string>(context);
            var kmRecorrer = KmRecorrer.Get<string>(context);
            var choferCuil = ChoferCuil.Get<string>(context);
            var choferNombreCompleto = ChoferNombreCompleto.Get<string>(context);
            var destinoDireccion = DestinoDireccion.Get<string>(context);
            var destinoLocalidad = DestinoLocalidad.Get<string>(context);
            var destinoProvincia = DestinoProvincia.Get<string>(context);
            var cuitDestinatario = CuitDestinatario.Get<string>(context);
            var nombreDestinatario = NombreDestinatario.Get<string>(context);

            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);
            var logActividad = new LogActividadDto
                {
                    Actividad = "Impresión Declaración Fosfina",
                    ActividadXaml = "ImpresionDeclaracionFosfina",
                    WorkflowInstanceId = workflowId,
                    Fecha = DateTime.Now
                };
            try
            {
                resultado = servicio.Ejecutar(new CrearLogActividad { Dto = logActividad });
            }
            catch (Exception)
            {
                resultado.Errores.Add("", Textos.LogActividad_ErrorEnLaCarga);
            }
            try
            {
                var centro = repositorio.ObtenerCentro(centroId);
                var transportista = repositorio.ObtenerTransportista(transportistaId ?? 0) ?? new TransportistaDto();
                var documento = repositorio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, centroId, puestoDeTrabajoId);
                if (documento == null) { throw new Exception(String.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, codigo)); }

                var recorridoDto = repositorio.ObtenerRecorridoValoresSapPorGuid(workflowId);
                nroCartaPorte = (String.IsNullOrEmpty(recorridoDto.NumeroDeDocumentoSap) ? nroCartaPorte : recorridoDto.NumeroDeDocumentoSap).Replace("R", "-");
                tipoDocumento = String.IsNullOrEmpty(recorridoDto.NumeroDeDocumentoSap) ? tipoDocumento : "RTO";

                var dto = new ImpDeclaracionFosfinaDto
                {
                    Impresora = documento.ImpresoraDireccion ?? "",
                    Codigo = codigo,
                    TipoDocumento = tipoDocumento,
                    NumeroDocumento = nroCartaPorte.Contains("-") ? nroCartaPorte : nroCartaPorte.Insert(4, "-"),
                    Ctg = ctg,
                    FechaDeCarga = fechaCarga,
                    Nombre = centro.RazonSocial,
                    Cuit = centro.Cuit,
                    Telefono = telefono,
                    Establecimiento = centro.Descripcion,
                    Material = material,
                    Observaciones = observaciones,
                    Peso = pesoNeto.ToString(CultureInfo.InvariantCulture),
                    DomicilioCarga = centro.Direccion,
                    LocalidadCarga = centro.LocalidadDesc,
                    ProvinciaCarga = centro.ProvinciaDesc,
                    NombreTransporte = transportista.RazonSocial,
                    CuitTransporte = transportista.Cuit,
                    DomicilioTransporte = transportista.Domicilio,
                    ProvinciaTransporte = transportista.Provincia,
                    LocalidadTrasnporte = transportista.Localidad,
                    Patente = patente,
                    PatenteAcoplado = patenteAcoplado ?? "",
                    WorkflowId = workflowId,
                    KmsARecorrer = kmRecorrer,
                    CuitChofer = choferCuil,
                    NombreChofer = choferNombreCompleto,
                    CuitDestinatario = cuitDestinatario,
                    NombreDestinatario = nombreDestinatario,
                    LocalidadDestinatario = localidadDestinatario,
                    DomicilioDestinatario = domicilioDestinatario,
                    ProvinciaDestinatario = provinciaDestinatario,
                    DomicilioDestino = destinoDireccion,
                    LocalidadDestino = destinoLocalidad,
                    ProvinciaDestino = destinoProvincia,
                    FechaImpresion = DateTime.Now,
                    DomicilioLegal = domicilioRemitente,
                    DomicilioReal = centro.Direccion,
                    LocalidadLegal = localidadRemitente,
                    LocalidadReal = centro.LocalidadDesc,
                    Provincia = provincia ?? "Santa Fe",
                    ProvinciaLegal = provinciaRemitente,
                    ProvinciaReal = centro.ProvinciaDesc
                };
                resultado = servicio.Ejecutar(new ImprimirDeclaracionFosfina { Dto = dto, CantidadCopias = cantCopias });
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }
            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ImpresionDeclaracionFosfina", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }
            return resultado;
        }
    }
}
