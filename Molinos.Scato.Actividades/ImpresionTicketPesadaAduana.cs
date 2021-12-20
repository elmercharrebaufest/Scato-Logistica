using System;
using System.Activities;
using System.Globalization;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ImpresionTicketPesadaAduana : CodeActivity<Resultado>
    {
        public InArgument<string> CodigoDeImpresion { get; set; }
        [RequiredArgument]
        public InArgument<int> CantCopias { get; set; }
        [RequiredArgument]
        public InArgument<string> NumeroDocumento { get; set; }
        [RequiredArgument]
        public InArgument<string> Patente { get; set; }

        public InArgument<string> PatenteAcoplado { get; set; }
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<string> NumeroIngreso { get; set; }
        public InArgument<string> Observaciones { get; set; }
        [RequiredArgument]
        public InArgument<string> TipoDocumento { get; set; }
        [RequiredArgument]
        public InArgument<int> PesoBruto { get; set; }
        [RequiredArgument]
        public InArgument<int> PesoTara { get; set; }
        [RequiredArgument]
        public InArgument<int> PesoNeto { get; set; }
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }
        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();

            var resultado = new Resultado();

            var workflowId = WorkflowId.Get<Guid>(context);
            var numeroDocumento = NumeroDocumento.Get<string>(context);
            var patente = Patente.Get<string>(context);
            var patenteAcoplado = PatenteAcoplado.Get<string>(context);
            var centroId = CentroId.Get<int>(context);
            var numeroIngreso = NumeroIngreso.Get<string>(context);
            var codigo = CodigoDeImpresion.Get<string>(context);
            var cantCopias = CantCopias.Get<int>(context);
            var observaciones = Observaciones.Get<string>(context);
            var tipoDocumento = TipoDocumento.Get<string>(context);
            var pesoBruto = PesoBruto.Get<int>(context);
            var pesoTara = PesoTara.Get<int>(context);
            var pesoNeto = PesoNeto.Get<int>(context);
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);
            
            var logActividad = new LogActividadDto
                {
                    Actividad = "Impresion Ticket Pesada Aduana",
                    ActividadXaml = "ImpresionTicketPesadaAduana",
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
                var recorrido = repositorio.ObtenerRecorridoPorGuid(workflowId);
                var datos = repositorio.ObtenerIngresoDeDatosDeExportacionPorRecorrido(recorrido.Id);

                var material = recorrido.Material != null ? recorrido.Material.Descripcion : "";
                var chofer = recorrido.Chofer;
                var transportista = recorrido.Transportista;
                var centro = recorrido.Centro;
                var fechaTara = recorrido.PesoTaraFecha;
                var fechaBruto = recorrido.PesoBrutoFecha;

                var balanzaid = recorrido.BalanzaTaraId ?? recorrido.BalanzaBrutoId ?? 0;
                var balanza = repositorio.ObtenerBalanza(balanzaid);

                var documento = repositorio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, centroId, puestoDeTrabajoId);
                if (documento == null) { throw new Exception(String.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, codigo)); }
                
                var dto = new ImpTicketPesadaAduanaDto
                {
                    Impresora = documento.ImpresoraDireccion ?? "",
                    Codigo = codigo,
                    Observaciones = observaciones,
                    PatenteAcoplado = patenteAcoplado ?? "",
                    NumeroDocumento = numeroDocumento,
                    Patente = patente,
                    Material = material,
                    PesoBruto = pesoBruto.ToString(CultureInfo.InvariantCulture),
                    PesoTara = pesoTara.ToString(CultureInfo.InvariantCulture),
                    PesoNeto = pesoNeto.ToString(CultureInfo.InvariantCulture),
                    CuitTransportista = transportista != null ? transportista.Cuit : "",
                    Transportista = datos == null ? "" : datos.Transportista,
                    WorkflowId = workflowId,
                    ChoferApellido = chofer != null ? chofer.Apellido : "",
                    ChoferNombre = chofer != null ? chofer.Nombre : "",
                    ChoferNumero = chofer != null ? chofer.NumeroDeDocumento : "",
                    ChoferTipoDoc = chofer != null ? chofer.DescripcionCorta : "",
                    CertificadoDeHabilitacion = balanza != null ? balanza.CertificadoDeHabilitacion : "",
                    CodigoAduana = centro.CodigoDeAduana,
                    CuitExportador = datos == null ? "" : datos.FirmaCuit,
                    FechaEgreso = recorrido.FechaEgreso ?? DateTime.Now,
                    FechaImpresion = DateTime.Now,
                    FechaHoraPesoBruto = fechaBruto,
                    FechaHoraPesoTara = fechaTara,
                    FechaInicio = recorrido.FechaInicio,
                    IdentificadorDeContenedor = datos == null ? "" : datos.IdentificadorContenedor,
                    Nacionalidad = datos == null ? "" : datos.Nacionalidad,
                    NumeroIngreso = numeroIngreso,
                    PermisoDeEmbarque = datos == null ? "" : datos.PermisoEmbarque,
                    RazonSocialExportador = datos == null ? "" : datos.FirmaRazonSocial,
                    VencimientoDeCertificacion = balanza != null ? balanza.VencimientoDeCertificado : null,
                    Latitud = balanza != null ? balanza.CodigoLatitud : "",
                    Longitud = balanza != null ? balanza.CodigoLongitud : "",
                    BalanzaNombre = balanza != null ? balanza.Nombre : "",
                    Lot = balanza != null ? balanza.CodigoLot : ""
                };

                resultado = servicio.Ejecutar(new ImprimirTicketPesadaAduana { Dto = dto, CantidadCopias = cantCopias });
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }

            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ImpresionTicketPesadaAduana", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }

            return resultado;
        }
    }
}
