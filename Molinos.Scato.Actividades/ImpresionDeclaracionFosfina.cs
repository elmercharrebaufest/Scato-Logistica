using System;
using System.Activities;
using System.Globalization;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ImpresionDeclaracionFosfina : CodeActivity<Resultado>
    {
        public InArgument<string> CodigoDeImpresion { get; set; }
        [RequiredArgument]
        public InArgument<int> CantCopias { get; set; }
        [RequiredArgument]
        public InArgument<CartaPorteDto> Orden { get; set; }
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

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new Resultado();
            var orden = Orden.Get<CartaPorteDto>(context);
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
                var transportista = repositorio.ObtenerTransportista(orden.TransportistaId ?? 0) ?? new TransportistaDto();
                var destinatario = repositorio.ObtenerProveedor(orden.DestinatarioId);
                var documento = repositorio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, centroId, puestoDeTrabajoId);
                if (documento == null) { throw new Exception(String.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, codigo)); }
                var dto = new ImpDeclaracionFosfinaDto
                {
                    Impresora = documento.ImpresoraDireccion ?? "",
                    Codigo = codigo,
                    TipoDocumento = "CP",
                    NumeroDocumento = orden.NroCartaPorte.Contains("-") ? orden.NroCartaPorte : orden.NroCartaPorte.Insert(4, "-"),
                    Ctg = orden.CTG,
                    FechaDeCarga = fechaCarga,
                    Nombre = centro.RazonSocial,
                    Cuit = centro.Cuit,
                    Telefono = telefono,
                    Establecimiento = centro.Descripcion,
                    Material = orden.Material,
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
                    KmsARecorrer = orden.KmRecorrer.HasValue ? orden.KmRecorrer.Value.ToString(CultureInfo.InvariantCulture) : string.Empty,
                    CuitChofer = orden.Chofer.Cuil,
                    NombreChofer = orden.Chofer.NombreCompleto,
                    CuitDestinatario = destinatario.Cuil,
                    NombreDestinatario = destinatario.RazonSocial,
                    LocalidadDestinatario = localidadDestinatario,
                    DomicilioDestinatario = domicilioDestinatario,
                    ProvinciaDestinatario = provinciaDestinatario,
                    DomicilioDestino = orden.DestinoDireccion,
                    LocalidadDestino = orden.DestinoLocalidad,
                    ProvinciaDestino = orden.DestinoProvincia,
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
