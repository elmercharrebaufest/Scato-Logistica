using System;
using System.Activities;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ImpresionGaritaSalida : CodeActivity<Resultado>
    {

        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<int> MaterialId { get; set; }
        [RequiredArgument]
        public InArgument<string> CodigoDeImpresion { get; set; }
        [RequiredArgument]
        public InArgument<int> PuestoDeTrabajoId { get; set; }
        public InArgument<int?> CantCopias { get; set; }
        [RequiredArgument]
        public InArgument<string> Patente { get; set; }
        [RequiredArgument]
        public InArgument<string> PatenteAcoplado { get; set; }
        [RequiredArgument]
        public InArgument<string> Calidad { get; set; }
        public InArgument<string> NumeroDocumento { get; set; }


        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new Resultado();
            var workflowId = context.WorkflowInstanceId;

            var centroId = CentroId.Get<int>(context);
            var codigo = CodigoDeImpresion.Get<string>(context);
            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);
            var cantCopias = CantCopias.Get<int?>(context) ?? 1;
            var calidad = Calidad.Get<string>(context);
            var patente = Patente.Get<string>(context);
            var patenteAcoplado = PatenteAcoplado.Get<string>(context);
            var materialId = MaterialId.Get<int>(context);
            var numeroDocumento = NumeroDocumento.Get<string>(context);

            var logActividad = new LogActividadDto
                {
                    Actividad = "Impresion Garita Salida",
                    ActividadXaml = "ImpresionGaritaSalida",
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
                var caladoRuta = repositorio.ObtenerCaladoPorGuid(workflowId);
                var analisisRuta = caladoRuta != null ? repositorio.ObtenerAnalisisDeCalidadPorCaladoId(caladoRuta.Id) : null;
                var asignacion = repositorio.ObtenerAsignacionDePuestoComando(workflowId.ToString());
                var almacen = repositorio.ObtenerAlmacenDescripcion(asignacion.AlmacenId);
                var hidraulicas = asignacion.HidraulicasId != null ? asignacion.HidraulicasId.Select(repositorio.ObtenerHidraulicaNombre) : new List<string>();
                var calle = repositorio.ObtenerCalle(asignacion.CalleId);
                var material = repositorio.ObtenerMaterial(materialId);
                var centro = repositorio.ObtenerCentro(centroId);
                var numeroDeTarjetaAsignada = repositorio.ObtenerTarjetaRFIDAsignada(TipoDocumentoIngreso.CartaPorte, numeroDocumento);
                var esSustentable = repositorio.EsRecorridoSustentable(workflowId);
                var caracteristicasAnalizadas = material.CodigoSAP == "19908027" ? repositorio.ObtenerCaracteristicasAnalizadasPorInstanceId(workflowId) : null;

                var documento = repositorio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, centroId, puestoDeTrabajoId);
                if (documento == null) { throw new Exception(String.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, codigo)); }

                var dto = new ImpGaritaSalidaDto
                {
                    Impresora = documento.ImpresoraDireccion ?? "",
                    Codigo = codigo,
                    FechaCalado = analisisRuta != null ? analisisRuta.FechaCreacion.ToString("dd/MM/yyyy HH:mm") : caladoRuta != null && caladoRuta.FechaCreacion.HasValue ? caladoRuta.FechaCreacion.Value.ToString("dd/MM/yyyy HH:mm") : null,
                    FechaImpresion = DateTime.Now,
                    Almacen = almacen,
                    Calidad = calidad,
                    Patente = patente,
                    Calle = calle != null ? calle.Nombre : "",
                    Hidraulicas = hidraulicas != null ? hidraulicas.ToList() : new List<string>(),
                    Humedad = caladoRuta != null ? caladoRuta.CaladosPorCaracteristica.Where(x => x.EsHumedad).Select(x => x.ValorCalado).FirstOrDefault().ToString() : "",
                    MaterialDesc = material != null ? material.Descripcion : "",
                    PatenteAcoplado = patenteAcoplado,
                    WorkflowId = workflowId,
                    Centro = centro.Descripcion,
                    NumeroDeTarjetaAsignada = numeroDeTarjetaAsignada ?? "",
                    EsSustentable = esSustentable,
                    NumeroDocumento = numeroDocumento ?? "",
                    ProteinaAlta = caracteristicasAnalizadas != null && caracteristicasAnalizadas.EsProteinaAlta ? "ALTA" : "",
                    ProteinaBaja = caracteristicasAnalizadas != null && caracteristicasAnalizadas.EsProteinaBaja ? "BAJA" : "",
                };
                resultado = servicio.Ejecutar(new ImprimirGaritaSalida { Dto = dto, CantidadCopias = cantCopias });
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }

            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ImpresionReciboMunicipal", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }

            return resultado;
        }
    }
}
