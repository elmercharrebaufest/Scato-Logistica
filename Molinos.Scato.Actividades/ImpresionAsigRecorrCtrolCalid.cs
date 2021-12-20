using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ImpresionAsigRecorrCtrolCalid : CodeActivity<Resultado>
    {

        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<int> MaterialId { get; set; }
        [RequiredArgument]
        public InArgument<string> NumeroDeOrden { get; set; }
        [RequiredArgument]
        public InArgument<string> Patente { get; set; }
        [RequiredArgument]
        public InArgument<string> PatenteAcoplado { get; set; }
        [RequiredArgument]
        public InArgument<string> Calidad { get; set; }
        [RequiredArgument]
        public InArgument<Guid> WorkflowId { get; set; }
        [RequiredArgument]
        public InArgument<string> CodigoDeImpresion { get; set; }
        public InArgument<int?> CantCopias { get; set; }
        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }
        [RequiredArgument]
        public InArgument<string> NumeroIngreso { get; set; }
        public InArgument<string> Observaciones { get; set; }
        public InArgument<string> DestinatarioCodigoSap { get; set; }
        public InArgument<string> Destinatario { get; set; }
        public InArgument<string> RtteComercial { get; set; }
        public InArgument<string> TitularCartaPorte { get; set; }
        public InArgument<string> Corredor { get; set; }
        public InArgument<string> NumeroDocumento { get; set; }
        public InArgument<string> Entregador { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var firmaProvider = context.GetExtension<IFirmaProvider>();
            var numeroIngreso = NumeroIngreso.Get<string>(context);
            var observaciones = Observaciones.Get<string>(context);
            var destinatarioCodigoSap = DestinatarioCodigoSap.Get<string>(context);
            var destinatario = Destinatario.Get<string>(context);
            var rtteComercial = RtteComercial.Get<string>(context);
            var titularCartaPorte = TitularCartaPorte.Get<string>(context);
            var corredor = Corredor.Get<string>(context);
            var numeroDocumento = NumeroDocumento.Get<string>(context);
            var entregador = Entregador.Get<string>(context);       
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new Resultado();
            var workflowId = WorkflowId.Get<Guid>(context);
            var centroId = CentroId.Get<int>(context);
            var materialId = MaterialId.Get<int>(context);
            var centro = repositorio.ObtenerCentro(centroId);
            var codigo = CodigoDeImpresion.Get<string>(context);
            var numeroDeOrden = NumeroDeOrden.Get<string>(context);
            var patente = Patente.Get<string>(context);
            var patenteAcoplado = PatenteAcoplado.Get<string>(context);
            var calidad = Calidad.Get<string>(context);
            var material = repositorio.ObtenerMaterial(materialId);
            var cantCopias = CantCopias.Get<int?>(context) ?? 1;

            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);

            var logActividad = new LogActividadDto
            {
                Actividad = "Impresion Asignacion De Recorrido Control Calidad",
                ActividadXaml = "ImpresionAsigRecorrCtrolCalid",
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
                var documento = repositorio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, centroId, puestoDeTrabajoId);
                if (documento == null) { throw new Exception(String.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, codigo)); }
                var cartaPorte = repositorio.ObtenerCartaPortePorInstanceId(workflowId);
                var numeroDeTarjetaAsignada = repositorio.ObtenerTarjetaRFIDAsignada(TipoDocumentoIngreso.CartaPorte, numeroDocumento);
                var fechaYhoraDeIngreso = cartaPorte.FechaEmision.ToString();
                var materialDesc = cartaPorte.Material;
                var caracteristicasAnalizadas = material.CodigoSAP == "19908027" ? repositorio.ObtenerCaracteristicasAnalizadasPorInstanceId(workflowId) : null;
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
                var asignacion = repositorio.ObtenerAsignacionDePuestoComando(workflowId.ToString());
                var almacen = repositorio.ObtenerAlmacenDescripcion(asignacion.AlmacenId);
                var balanzaBruto = repositorio.ObtenerBalanzaNombre(asignacion.BalanzaBrutoId ?? 0);
                var balanzaTara = repositorio.ObtenerBalanzaNombre(asignacion.BalanzaTaraId ?? 0);
                var calle = repositorio.ObtenerCalle(asignacion.CalleId);
                var hidraulicas = asignacion.HidraulicasId != null ? asignacion.HidraulicasId.Select(repositorio.ObtenerHidraulicaNombre) : new List<string>();
                var caladoRuta = repositorio.ObtenerCaladoPorGuid(workflowId);
                var analisisRuta = caladoRuta != null ? repositorio.ObtenerAnalisisDeCalidadPorCaladoId(caladoRuta.Id) : null;
                var humedad = caladoRuta != null ? caladoRuta.CaladosPorCaracteristica.FirstOrDefault(x => x.EsHumedad) : null;

                var valorMateriaGrasa = caladoRuta != null ? caladoRuta.CaladosPorCaracteristica.Where(a=>a.CaracteristicaCodigoSap== "MPGIRMGR").Select(x => x.ValorCalado).FirstOrDefault() : null;
               
                var dto = new ImpAsigRecorrCtrolCalidDto

                {
                    Impresora = documento.ImpresoraDireccion ?? "",
                    Codigo = codigo,
                    FechaCalado = analisisRuta != null ? analisisRuta.FechaCreacion : caladoRuta != null ? caladoRuta.FechaCreacion : null,
                    FechaImpresion = DateTime.Now,
                    Almacen = almacen,
                    BalanzaBruto = balanzaBruto,
                    BalanzaTara = balanzaTara,
                    Calidad = calidad,
                    NumeroDeOrden = numeroDeOrden,
                    Patente = patente,
                    Calle = calle != null ? calle.Nombre : "",
                    Hidraulicas = hidraulicas != null ? hidraulicas.ToList() : new List<string>(),
                    Humedad = caladoRuta != null ? caladoRuta.CaladosPorCaracteristica.Where(x => x.EsHumedad).Select(x => x.ValorCalado).FirstOrDefault().ToString() : "",
                    Material = material != null ? material.Descripcion : "",
                    MaterialCodigoSap = material != null ? material.CodigoSAP : "",
                    PatenteAcoplado = patenteAcoplado,
                    WorkflowId = workflowId,
                    Centro = centro.Descripcion,
                    Observaciones = observaciones,
                    AnalisisPorCaracteristicas = analisisRuta != null ? analisisRuta.CaracteristicasAnalizadas : null,
                    CaladoPorCaracteristicas = caladoRuta != null ? caladoRuta.CaladosPorCaracteristica : null,
                    HumedadDescripcion = humedad == null ? "" : humedad.Caracteristica,
                    NumeroDeTarjetaAsignada = numeroDeTarjetaAsignada ?? "",
                    FechaYhoraDeIngreso = fechaYhoraDeIngreso ?? "",
                    MaterialDesc = materialDesc ?? "",
                    EsSustentable = esSustentable,
                    NumeroDocumento = numeroDocumento ?? "",
                    NumeroIngreso = numeroIngreso ?? "",
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
                    Cupo = cartaPorte.Cupo ?? "",
                    ProteinaAlta = caracteristicasAnalizadas != null && caracteristicasAnalizadas.EsProteinaAlta ? "true" : "",
                    ProteinaBaja = caracteristicasAnalizadas != null && caracteristicasAnalizadas.EsProteinaBaja ? "true" : "",
                    MateriaGrasa = valorMateriaGrasa.HasValue ? valorMateriaGrasa.Value.ToString("0.00").Replace(".", ",") : ""
                };

                resultado = servicio.Ejecutar(new ImprimirAsigRecorrCtrolCalid { Dto = dto, CantidadCopias = cantCopias });

            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }

            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ImpresionAsigRecorrCtrolCalid", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }
            return resultado;
        }
    }
}
