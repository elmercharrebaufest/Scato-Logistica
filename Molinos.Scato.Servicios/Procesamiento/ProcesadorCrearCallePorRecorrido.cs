using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearCallePorRecorrido : ProcesadorComando<CrearCallePorRecorrido>
    {
        private IAdministradorDeCalles administradorDeCalles;
        private IServicioComandos servicioComandos;

        public ProcesadorCrearCallePorRecorrido(IRepositorio repositorio, IConversor conversor, ILogger log, IAdministradorDeCalles administradorDeCalles, IServicioComandos servicioComandos)
            : base(repositorio, conversor, log)
        {
            this.administradorDeCalles = administradorDeCalles;
            this.servicioComandos = servicioComandos;
        }

        public override Resultado Ejecutar(CrearCallePorRecorrido comando)
        {
            var resultado = new ResultadoCrearCalle();
            try
            {
                Validar(comando, resultado);
                if (!resultado.HayErrores)
                {
                    var entidad = CrearEntidad(comando, resultado);
                    Repositorio.Agregar(entidad);
                    Repositorio.GuardarCambios();
                    resultado.Id = entidad.Id;
                    Finally(comando, entidad.Id);
                }
                if(resultado.Errores.Count == 1 && resultado.Errores.ContainsKey("Recorrido"))
                {
                    var entidad = ActualizarEntidad(comando, resultado);
                    Repositorio.GuardarCambios();
                    resultado.Id = entidad.Id;
                }
            }
            catch (CrearException e)
            {
                Log.Error(e, e.Message);
                resultado.Error("", e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrió un error al crear la entidad");
                resultado.Error("", Textos.Error_ActualizarGenerico);
            }
            return resultado;
        }

        protected CallePorRecorrido CrearEntidad(CrearCallePorRecorrido comando, ResultadoCrearCalle resultado)
        {
            var entidadNueva = new CallePorRecorrido
            {
                Recorrido = comando.InstanciaWorkflow != null ? Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.InstanciaWorkflow) : null,
                CargaDeCupo = comando.CargaDeCupoId > 0 ? Repositorio.Obtener<CargaDeCupo>(x => x.Id == comando.CargaDeCupoId) : null,
                FechaIngeso = DateTime.Now
            };
            var material = entidadNueva.Recorrido != null ? entidadNueva.Recorrido.Material : entidadNueva.CargaDeCupo != null ? entidadNueva.CargaDeCupo.Material : null;
            var calidad = entidadNueva.Recorrido != null && entidadNueva.Recorrido.CaracteristicasAnalizadas != null ? entidadNueva.Recorrido.CaracteristicasAnalizadas.Calidad : Dominio.Enums.TipoCalidad.Desconocida;


            entidadNueva.Calle = comando.FlagReasignacionCalle ? comando.CalleReasignacion : administradorDeCalles.AsignarCalle(comando.TipoCalle, material?.Id, calidad, comando.CentroId, comando.TurnoActivo, comando.InstanciaWorkflow);
            if (entidadNueva.Calle == null)
            {
                var calidadstr = calidad == Dominio.Enums.TipoCalidad.Desconocida ? string.Empty : calidad.ToString();
                throw new CrearException($"No hay calles disponibles para {comando.TipoCalle} - {material?.Descripcion} - {calidadstr}");
            }
            if (entidadNueva.Calle.TipoCalle != TipoCalle.Circular)
            {
                entidadNueva.Calle.Bloqueada = false;
                entidadNueva.Calle.FechaLLamada = null;
            }
            
            resultado.Disponibilidad = administradorDeCalles.ObtenerEspacioDisponible(comando.TipoCalle, material != null ? material.Id : 0, calidad) - 1; //porque aun no se guarda la asignacion actual
            resultado.DisponibilidadCalles = administradorDeCalles.ObtenerEspacioDisponibleEnCalle(entidadNueva.Calle.Id);
            resultado.CentroId = entidadNueva.Calle.CentroId;
            resultado.CalleId = entidadNueva.Calle.Id;
            resultado.Calidad = calidad;
            Log.Debug($"Espacio en calle {entidadNueva.Calle.Nombre}: {resultado.Disponibilidad}, {(resultado.DisponibilidadCalles ? "Hay" : "No hay")} espacio en calle");
            var materialDescripcion = entidadNueva.Calle.Material != null ? entidadNueva.Calle.Material.DescripcionCorta : "";
            Log.Debug($"{entidadNueva.Calle.Nombre} de {entidadNueva.Calle.TipoCalle} y  {calidad} o Material  {materialDescripcion}");
            return entidadNueva;
        }

        protected CallePorRecorrido ActualizarEntidad(CrearCallePorRecorrido comando, ResultadoCrearCalle resultado)
        {
            var recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.InstanciaWorkflow);
            var entidad = Repositorio.Obtener<CallePorRecorrido>(x=> x.CargaDeCupo.Patente == recorrido.Patente && x.FechaEgreso == null && x.Recorrido == null);
            entidad.Recorrido = recorrido;
            var material = recorrido.Material;
            var calidad = recorrido.CaracteristicasAnalizadas != null ? recorrido.CaracteristicasAnalizadas.Calidad : Dominio.Enums.TipoCalidad.Desconocida;

            resultado.Disponibilidad = administradorDeCalles.ObtenerEspacioDisponible(comando.TipoCalle, material != null ? material.Id : 0, calidad) - 1; //porque aun no se guarda la asignacion actual
            resultado.DisponibilidadCalles = administradorDeCalles.ObtenerEspacioDisponibleEnCalle(entidad.Calle.Id);
            resultado.CentroId = entidad.Calle.CentroId;
            resultado.CalleId = entidad.Calle.Id;
            resultado.Calidad = calidad;
            Log.Debug($"Espacio en calle {entidad.Calle.Nombre}: {resultado.Disponibilidad}, {(resultado.DisponibilidadCalles ? "Hay" : "No hay")} espacio en calle");
            var materialDescripcion = entidad.Calle.Material != null ? entidad.Calle.Material.DescripcionCorta : "";
            Log.Debug($"Actualizando Calle por recorrido para {entidad.Calle.Nombre} con recorrido {recorrido} de {entidad.Calle.TipoCalle} y  {calidad} o Material  {materialDescripcion}");
            return entidad;
        }

        protected void Finally(CrearCallePorRecorrido comando, int id)
        {
            servicioComandos.Ejecutar(new DesasignarCalle
            {
                UltimaAsignacionId = id,
                InstanciaWorkflow = comando.InstanciaWorkflow,
            });
        }


        protected void Validar(CrearCallePorRecorrido comando, Resultado resultado)
        {
            if (!Repositorio.Existe<Recorrido>(x => x.InstanciaWorkflow == comando.InstanciaWorkflow)
                && !Repositorio.Existe<CargaDeCupo>(x => x.Id == comando.CargaDeCupoId))
            {
                resultado.Error("InstanciaWorkflow", Textos.Recorrido_Inexistente);
            }

            var recorrido = Repositorio.Obtener<Recorrido>(x => x.InstanciaWorkflow == comando.InstanciaWorkflow);
            if (recorrido != null && comando.TipoCalle == TipoCalle.NoGranos && 
                Repositorio.Existe<CallePorRecorrido>(x=>x.FechaEgreso == null &&
                (x.CargaDeCupo.Patente == recorrido.Patente || x.Recorrido.Patente == recorrido.Patente )))
            {
                resultado.Error("Recorrido", Textos.CallePorRecorrido_YaAsignado);
            }
        }
    }
}
