using System;
using System.Activities;
using System.Configuration;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ImpresionCertificadoDeAnalisis : CodeActivity<Resultado>
    {
        public InArgument<string> CodigoDeImpresion { get; set; }
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<string> NumeroIngreso { get; set; }
        public InArgument<string> Observaciones { get; set; }
        public InArgument<AnalisisPorCaracteristicaDto[]> Analisis { get; set; }
        public InArgument<CaladoPorCaracteristicaDto[]> Calado { get; set; }
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }
        public InArgument<int?> CantCopias { get; set; }
        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }

        public InArgument<string> DestinatarioCodigoSap { get; set; }
        public InArgument<string> Destinatario { get; set; }
        public InArgument<string> RtteComercial { get; set; }
        public InArgument<string> TitularCartaPorte { get; set; }
        public InArgument<string> Corredor { get; set; }
        public InArgument<string> Patente { get; set; }
        public InArgument<string> PatenteAcoplado { get; set; }
        public InArgument<string> NumeroDocumento { get; set; }
        public InArgument<string> Entregador { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var firmaProvider = context.GetExtension<IFirmaProvider>();

            var resultado = new Resultado();

            var workflowId = WorkflowId.Get<Guid>(context);
            var centroId = CentroId.Get<int>(context);
            var numeroIngreso = NumeroIngreso.Get<string>(context);
            var codigo = CodigoDeImpresion.Get<string>(context);
            var analisis = Analisis.Get<AnalisisPorCaracteristicaDto[]>(context);
            var calado = Calado.Get<CaladoPorCaracteristicaDto[]>(context);
            var observaciones = Observaciones.Get<string>(context);
            var cantCopias = CantCopias.Get<int?>(context) ?? 1;
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);

            var destinatarioCodigoSap = DestinatarioCodigoSap.Get<string>(context);
            var destinatario = Destinatario.Get<string>(context);
            var rtteComercial = RtteComercial.Get<string>(context);
            var titularCartaPorte = TitularCartaPorte.Get<string>(context);
            var corredor = Corredor.Get<string>(context);
            var patente = Patente.Get<string>(context);
            var patenteAcoplado = PatenteAcoplado.Get<string>(context);
            var numeroDocumento = NumeroDocumento.Get<string>(context);
            var entregador = Entregador.Get<string>(context);

            var centro = repositorio.ObtenerCentro(centroId);

            var logActividad = new LogActividadDto
            {
                Actividad = "Impresion Certificado De Analisis",
                ActividadXaml = "ImpresioCertificadoDeAnalisis",
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
                string vendedor;
                if (destinatarioCodigoSap != firmaProvider.ObtenerFirmaSinLogo().CodigoSAP)
                {
                    vendedor = destinatario;
                }
                else
                {
                    vendedor = rtteComercial ?? titularCartaPorte;
                }
                var humedad = calado.FirstOrDefault(x => x.EsHumedad);

                var documento = repositorio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, centroId, puestoDeTrabajoId);
                if (documento == null) { throw new Exception(String.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, codigo)); }
                var cartaPorte = repositorio.ObtenerCartaPortePorInstanceId(workflowId);
                var numeroDeTarjetaAsignada = repositorio.ObtenerTarjetaRFIDAsignada(TipoDocumentoIngreso.CartaPorte, numeroDocumento);
                var fechaYhoraDeIngreso = cartaPorte.FechaEmision.ToString();
                var materialDesc = cartaPorte.Material;
                var esSustentable = repositorio.EsRecorridoSustentable(workflowId);
                var ctg = cartaPorte.CTG; 
                var titularDeCartaDePorteCuit = cartaPorte.TitularCartaPorteCuil;
                var titularDeCartaDePorteRazon = cartaPorte.TitularCartaPorte;
                var intermediarioCuit = cartaPorte.IntermediarioCuit;
                var intermediarioRazon = cartaPorte.Intermediario;
                var remitenteComercialCuit = cartaPorte.RtteComercialCuit;
                var remitenteComercialRazon = cartaPorte.RtteComercial;
                var corredorCuit = cartaPorte.CorredorCuil;
                var vendedorCuit = cartaPorte.CorredorVendedorCuil;
                var entregadorCuit = cartaPorte.EntregadorCuit;
                var agenteDeComprasCuit = cartaPorte.AgenteComprasCuil;
                var agenteDeComprasRazon = cartaPorte.AgenteCompras;
                var destinatarioCuit = cartaPorte.DestinatarioCuil;
                var destinatarioRazon = cartaPorte.Destinatario;
                var destinoCuit = cartaPorte.DestinatarioCuil;
                var destinoRazon = cartaPorte.Destinatario;
                var transportistaCuit = cartaPorte.TransportistaCUIT;
                var transportistaRazon = cartaPorte.Transportista;
                var choferCuit = cartaPorte.Chofer.Cuil;
                var choferNombre = cartaPorte.Chofer.NombreCompleto;
                var procedenciaDeLaMercanderiaCodAfip = cartaPorte.ProcedenciaCodigoSap;
                var procedenciaDeLaMercanderiaDesc = cartaPorte.Procedencia;

                var dto = new ImpCertificadoDeAnalisisDto
                {
                    Impresora = documento.ImpresoraDireccion ?? "",
                    Centro = centro.Descripcion,
                    Codigo = codigo,
                    Observaciones = observaciones,
                    AnalisisPorCaracteristicas = analisis,
                    CaladoPorCaracteristicas = calado,
                    HumedadDescripcion = humedad == null ? "" : humedad.Caracteristica,
                    WorkflowId = workflowId,
                    NumeroDeTarjetaAsignada = numeroDeTarjetaAsignada ?? "",
                    FechaYhoraDeIngreso = fechaYhoraDeIngreso ?? "",
                    MaterialDesc = materialDesc ?? "",
                    EsSustentable = esSustentable,
                    NumeroDocumento = numeroDocumento ?? "",
                    NumeroIngreso = numeroIngreso ?? "",
                    Patente = patente ?? "",
                    PatenteAcoplado = patenteAcoplado ?? "",
                    CTG = ctg ?? "",
                    TitularDeCartaDePorteCuit = titularDeCartaDePorteCuit ?? "",
                    TitularDeCartaDePorteRazon = titularDeCartaDePorteRazon ?? "",
                    IntermediarioCuit = intermediarioCuit ?? "",
                    IntermediarioRazon = intermediarioRazon ?? "",
                    RemitenteComercialCuit = remitenteComercialCuit ?? "",
                    RemitenteComercialRazon = remitenteComercialRazon ?? "",
                    CorredorCuit = corredorCuit ?? "",
                    Corredor = corredor ?? "",
                    VendedorCuit = vendedorCuit ?? "",
                    Vendedor = vendedor ?? "",
                    EntregadorCuit = entregadorCuit ?? "",
                    Entregador = entregador ?? "",
                    AgenteDeComprasCuit = agenteDeComprasCuit ?? "",
                    AgenteDeComprasRazon = agenteDeComprasRazon ?? "",
                    DestinatarioCuit = destinatarioCuit ?? "",
                    DestinatarioRazon = destinatarioRazon ?? "",
                    DestinoCuit = destinoCuit ?? "",
                    DestinoRazon = destinoRazon ?? "",
                    TransportistaCuit = transportistaCuit ?? "",
                    TransportistaRazon = transportistaRazon ?? "",
                    ChoferCuit = choferCuit ?? "",
                    ChoferNombre = choferNombre ?? "",
                    ProcedenciaDeLaMercanderiaCodAfip = procedenciaDeLaMercanderiaCodAfip ?? "",
                    ProcedenciaDeLaMercanderiaDesc = procedenciaDeLaMercanderiaDesc ?? "",
                    Cupo =  cartaPorte.Cupo ?? ""
                };

                resultado = servicio.Ejecutar(new ImprimirCertificadoDeAnalisis { Dto = dto, CantidadCopias = cantCopias });
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }

            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ImpresioCertificadoDeAnalisis", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }

            return resultado;
        }
    }
}