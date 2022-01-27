using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarEstadoMaterial : ProcesadorComando<ActualizarEstadoMaterial>
    {

        public ProcesadorActualizarEstadoMaterial(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ActualizarEstadoMaterial comando)
        {
            var id = Repositorio.Listar(
                x => new MaterialEstadoDto { Descripcion = x.Material.Descripcion, DescripcionWebMobile = x.DescripcionWebMobile, EsGrano = x.Material.EsGrano, MaterialId = x.Material.Id, Orden = x.Orden, CentroId = x.Centro.Id },
                (MaterialPorCentro x) =>x.MostrarEnWebMobile).OrderBy(x => x.Orden == null).ThenBy(x => x.Orden).ThenBy(x => x.DescripcionWebMobile);
            var estadoViejo = Repositorio.Listar<EstadoMaterial>();
            CrearEstado(id, true);
            CrearEstado(id, false);
            Repositorio.RemoverTodos(estadoViejo);
            Repositorio.GuardarCambios();
            return new Resultado();
        }


        public void CrearEstado(IOrderedEnumerable<MaterialEstadoDto> id, bool mostrarIngresos)
        {
            foreach (var materialesPorCentro in id.GroupBy(x => x.CentroId))
            {
                foreach (var materialPorCentro in materialesPorCentro)
                {
                    var camionesDescargados = ObtenerCantidadDeCamionesDescargados(materialPorCentro.CentroId, materialPorCentro.MaterialId, mostrarIngresos);
                    var camionesEnPlanta = ObtenerCantidadDeCamionesEnPlanta(materialPorCentro.CentroId, materialPorCentro.MaterialId, mostrarIngresos);
                    var vagonesDescargados = ObtenerCantidadDeVagonesDescargados(materialPorCentro.CentroId, materialPorCentro.MaterialId, mostrarIngresos);
                    var vagonesEnPlanta = ObtenerCantidadDeVagonesEnPlanta(materialPorCentro.CentroId, materialPorCentro.MaterialId, mostrarIngresos);

                    Repositorio.Agregar(new EstadoMaterial
                    {
                        CamionesEnElDia = camionesDescargados,
                        CamionesEnPlanta = camionesEnPlanta,
                        TotalIngresosEnElDia = camionesDescargados + camionesEnPlanta,
                        Peso = (ObtenerTotalIngreso(materialPorCentro.CentroId, materialPorCentro.MaterialId, mostrarIngresos) / 1000),
                        Rechazados = ObtenerCantiddadDeCamionesRechazadosPorMaterial(materialPorCentro.CentroId, materialPorCentro.MaterialId),
                        Material = !string.IsNullOrEmpty(materialPorCentro.DescripcionWebMobile) ? materialPorCentro.DescripcionWebMobile : materialPorCentro.Descripcion,
                        CentroId = materialPorCentro.CentroId,
                        EsGrano = materialPorCentro.EsGrano,
                        EsIngreso = mostrarIngresos,
                        VagonesEnElDia = vagonesDescargados,
                        VagonesEnPlanta = vagonesEnPlanta,
                        TotalIngresosVagonesEnElDia = vagonesDescargados + vagonesEnPlanta,
                        VagonesRechazados = ObtenerCantidadDeVagonesRechazadosPorMaterial(materialPorCentro.CentroId, materialPorCentro.MaterialId)
                    });
                }
            }
        }

        public int ObtenerCantiddadDeCamionesRechazadosPorMaterial(int centroId, int Id)
        {
            var date = DateTime.Now.Date;
            return Repositorio.Contar<Recorrido>(x => x.Centro.Id == centroId && x.Material.Id == Id && x.Rechazado && (!x.Terminado || (x.FechaEgreso.HasValue && x.FechaEgreso > date)) && (x.TipoVehiculo != TipoVehiculo.Tren));
        }

        public int ObtenerTotalIngreso(int centroId, int Id, bool mostrarIngresos)
        {
            var date = DateTime.Now.Date;
            return (int)Repositorio.Listar<Recorrido>(x => x.Terminado && x.PesoTaraFecha.HasValue && x.PesoTaraFecha.Value > date && x.Centro.Id == centroId && x.Workflow.TipoDeWorkflow == (mostrarIngresos == true ? TipoDeWorkflow.Ingreso : TipoDeWorkflow.Egreso) && x.PesoBruto.HasValue && x.PesoTara.HasValue && x.Material.Id == Id && !x.Rechazado).Sum(x => x.PesoBruto - x.PesoTara);
        }

        public int ObtenerCantidadDeCamionesDescargados(int centroId, int Id, bool mostrarIngresos)
        {
            var date = DateTime.Now.Date;
            return Repositorio.Contar<Recorrido>(x => x.Terminado && x.PesoTaraFecha.HasValue && x.PesoTaraFecha >= date && x.Centro.Id == centroId && x.Workflow.TipoDeWorkflow == (mostrarIngresos == true ? TipoDeWorkflow.Ingreso : TipoDeWorkflow.Egreso) && x.Material.Id == Id && !x.Rechazado && (x.TipoVehiculo != TipoVehiculo.Tren));
        }

        public int ObtenerCantidadDeCamionesEnPlanta(int centroId, int Id, bool mostrarIngresos)
        {
            return Repositorio.Contar<Recorrido>(x => !x.Terminado && x.Centro.Id == centroId && x.Material.Id == Id && !x.Rechazado && x.Workflow.TipoDeWorkflow == (mostrarIngresos == true ? TipoDeWorkflow.Ingreso : TipoDeWorkflow.Egreso) && (x.TipoVehiculo != TipoVehiculo.Tren));
        }

        public int ObtenerCantidadDeVagonesRechazadosPorMaterial(int centroId, int Id)
        {
            var date = DateTime.Now.Date;
            return Repositorio.Contar<Recorrido>(x => x.Centro.Id == centroId && x.Material.Id == Id && x.Rechazado && (!x.Terminado || (x.FechaEgreso.HasValue && x.FechaEgreso > date)) && (x.TipoVehiculo == TipoVehiculo.Tren));
        }

        public int ObtenerCantidadDeVagonesDescargados(int centroId, int Id, bool mostrarIngresos)
        {
            var date = DateTime.Now.Date;
            return Repositorio.Contar<Recorrido>(x => x.Terminado && x.PesoTaraFecha.HasValue && x.PesoTaraFecha >= date && x.Centro.Id == centroId && x.Workflow.TipoDeWorkflow == (mostrarIngresos == true ? TipoDeWorkflow.Ingreso : TipoDeWorkflow.Egreso) && x.Material.Id == Id && !x.Rechazado && (x.TipoVehiculo == TipoVehiculo.Tren));
        }

        public int ObtenerCantidadDeVagonesEnPlanta(int centroId, int Id, bool mostrarIngresos)
        {
            return Repositorio.Contar<Recorrido>(x => !x.Terminado && x.Centro.Id == centroId && x.Material.Id == Id && !x.Rechazado && x.Workflow.TipoDeWorkflow == (mostrarIngresos == true ? TipoDeWorkflow.Ingreso : TipoDeWorkflow.Egreso) && (x.TipoVehiculo == TipoVehiculo.Tren));
        }
    }
}