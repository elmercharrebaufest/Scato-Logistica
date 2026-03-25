using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearLogIngresoPorPuesto : ProcesadorCrear<CrearLogIngresoPorPuesto, LogIngresoPorPuesto>
    {
        public ProcesadorCrearLogIngresoPorPuesto(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override LogIngresoPorPuesto CrearEntidad(CrearLogIngresoPorPuesto comando)
        {
            var recorrido = Repositorio.Obtener<Recorrido>(comando.Dto.RecorridoId);
            var puestoDeTrabajo = Repositorio.Obtener<PuestoDeTrabajo>(comando.Dto.PuestoDeTrabajoId);

            var logIngreso = new LogIngresoPorPuesto
            {
                Recorrido = recorrido,
                PuestoDeTrabajo = puestoDeTrabajo,
                TipoIngreso = comando.Dto.TipoIngreso,
                FechaHora = comando.Dto.FechaHora
            };

            return logIngreso;
        }

        protected override void Validar(CrearLogIngresoPorPuesto comando, Resultado resultado)
        {
            if (!Repositorio.Existe<Recorrido>(x => x.Id == comando.Dto.RecorridoId))
                resultado.Error(nameof(LogIngresoPorPuestoDto.RecorridoId), Textos.Error_RecorridoNoEncontrado);

            if (!Repositorio.Existe<PuestoDeTrabajo>(x => x.Id == comando.Dto.PuestoDeTrabajoId))
                resultado.Error(nameof(LogIngresoPorPuestoDto.PuestoDeTrabajoId), Textos.PuestoDeTrabajo_NoEncontrado);
        }
    }
}
