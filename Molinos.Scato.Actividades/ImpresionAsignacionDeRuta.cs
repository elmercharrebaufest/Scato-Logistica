using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Actividades.Helpers;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Servicios;

namespace Molinos.Scato.Actividades
{
    public class ImpresionAsignacionDeRuta : CodeActivity<Resultado>
    {
        [RequiredArgument]
        public InArgument<int> CentroId { get; set; }
        [RequiredArgument]
        public InArgument<int> MaterialId { get; set; }
        [RequiredArgument]
        public InArgument<CaladoDto> Calado { get; set; }
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
        public InArgument<string> Observacion { get; set; }

        protected override Resultado Execute(CodeActivityContext context)
        {
            var servicio = context.GetExtension<IServicioComandos>();
            var repositorio = context.GetExtension<IServicioRepositorio>();
            var resultado = new Resultado();

            var workflowId = WorkflowId.Get<Guid>(context);
            var centroId = CentroId.Get<int>(context);
            var materialId = MaterialId.Get<int>(context);
            
            var calado = Calado.Get<CaladoDto>(context);
            var codigo = CodigoDeImpresion.Get<string>(context);
            var numeroDeOrden = NumeroDeOrden.Get<string>(context);
            var patente = Patente.Get<string>(context);
            var patenteAcoplado = PatenteAcoplado.Get<string>(context);
            var calidad = Calidad.Get<string>(context);
            var material = repositorio.ObtenerMaterial(materialId);
            var cantCopias = CantCopias.Get<int?>(context) ?? 1;
            var observacion = Observacion.Get<string>(context);

            var puestoDeTrabajoId = PuestoDeTrabajoId.Get<int>(context);

            var logActividad = new LogActividadDto
            {
                Actividad = "Impresion Asignacion De Ruta",
                ActividadXaml = "ImpresionAsignacionDeRuta",
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
                var asignacion = repositorio.ObtenerAsignacionDePuestoComando(workflowId.ToString());

                var almacen = repositorio.ObtenerAlmacenDescripcion(asignacion.AlmacenId);
                var balanzaBruto = repositorio.ObtenerBalanzaNombre(asignacion.BalanzaBrutoId ?? 0);
                var balanzaTara = repositorio.ObtenerBalanzaNombre(asignacion.BalanzaTaraId ?? 0);
                var calle = repositorio.ObtenerCalle(asignacion.CalleId);
                var hidraulicas = asignacion.HidraulicasId != null ? asignacion.HidraulicasId.Select(repositorio.ObtenerHidraulicaNombre) : new List<string>();
                var analisis = calado != null ? repositorio.ObtenerAnalisisDeCalidadPorCaladoId(calado.Id) : null;
                
                var documento = repositorio.ObtenerDocumentoDeImpresionPorCentroCodigoPuestoDeTrabajo(codigo, centroId, puestoDeTrabajoId);
                if (documento == null) 
                    throw new Exception(string.Format(Textos.Error_DocumentoDeImpresionNoEncontrado, codigo)); 

                var dto = new ImpAsignacionDeRutaDto
                {
                    Impresora = documento.ImpresoraDireccion ?? "",
                    Codigo = codigo,
                    FechaCalado = analisis != null ? analisis.FechaCreacion : calado != null ? calado.FechaCreacion : null,
                    FechaImpresion = DateTime.Now,
                    Almacen = almacen,
                    BalanzaBruto = balanzaBruto,
                    BalanzaTara = balanzaTara,
                    Calidad = calidad,
                    NumeroDeOrden = numeroDeOrden,
                    Patente = patente,
                    Calle = calle != null ? calle.Nombre : "",
                    Hidraulicas = hidraulicas != null ? hidraulicas.ToList() : new List<string>(),
                    Humedad = calado != null ? calado.CaladosPorCaracteristica.Where(x => x.EsHumedad).Select(x => x.ValorCalado).FirstOrDefault().ToString() : "",
                    Material = material != null ? material.Descripcion : "",
                    MaterialCodigoSap = material != null ? material.CodigoSAP : "",
                    PatenteAcoplado = patenteAcoplado,
                    WorkflowId = workflowId,
                    Observacion = observacion,
                    TipoVehiculo = asignacion.TipoVehiculo.DisplayEnum(),
                    MaterialId = materialId,
                    CentroId = centroId,
                };

                resultado = servicio.Ejecutar(new ImprimirAsignacionDeRuta { Dto = dto, CantidadCopias = cantCopias });
            }
            catch (Exception e)
            {
                resultado.Errores.Add("1", e.Message);
            }

            try
            {
                resultado = servicio.Ejecutar(new FinDeActividad { InstanceId = workflowId, Actividad = "ImpresionAsignacionDeRuta", PuestoDeTrabajoId = puestoDeTrabajoId });
            }
            catch (Exception)
            {
                resultado.Errores.Add("2", Textos.FinDeActividad_ErrorEnLaCarga);
            }
            return resultado;
        }
    }
}
