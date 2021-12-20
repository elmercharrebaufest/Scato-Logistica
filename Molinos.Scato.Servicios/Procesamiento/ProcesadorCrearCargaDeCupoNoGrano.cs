using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Molinos.Scato.Servicios.Orquestador;
using Ninject.Extensions.Logging;
using System;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearCargaDeCupoNoGrano : ProcesadorCrear<CrearCargaDeCupoNoGrano, CargaDeCupo>
    {
        private readonly IServicioComandos servicioComandos;
        private readonly IServicioOrquestador servicioOrquestador;

        public ProcesadorCrearCargaDeCupoNoGrano(IRepositorio repositorio, IConversor conversor, ILogger log, IServicioComandos servicioComandos, IServicioOrquestador servicioOrquestador)
            : base(repositorio, conversor, log)
        {
            this.servicioComandos = servicioComandos;
            this.servicioOrquestador = servicioOrquestador;
        }

        protected override CargaDeCupo CrearEntidad(CrearCargaDeCupoNoGrano comando)
        {
            EliminarDuplicadas(comando);
            var cupo = Conversor.Convertir<CargaDeCupoDto, CargaDeCupo>(comando.Dto);
            cupo.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            cupo.Material = Repositorio.Obtener<Material>(comando.Dto.MaterialId);
            cupo.PuestoDeTrabajo = Repositorio.Obtener<PuestoDeTrabajo>(comando.Dto.PuestoDeTrabajoId);

            return cupo;
        }

        protected void EliminarDuplicadas(CrearCargaDeCupoNoGrano comando)
        {
            var entidadDuplicada = Repositorio.Listar<CargaDeCupo>(x => x.Numero == comando.Dto.Numero && comando.Dto.Numero != "" && comando.Dto.Numero != null && x.Recorrido == null && x.Centro.Id == comando.Dto.CentroId);

            if (entidadDuplicada != null)
            {
                foreach (var i in entidadDuplicada)
                {
                    Repositorio.Remover(i);
                }
            }

            var entidadDuplicada3 = Repositorio.Listar<CargaDeCupo>(x => x.Patente == comando.Dto.Patente && comando.Dto.Patente != "" 
            && comando.Dto.Patente != null && x.Recorrido == null && x.Centro.Id == comando.Dto.CentroId);

            if (entidadDuplicada3 != null)
            {
                foreach (var i in entidadDuplicada3)
                {
                    Repositorio.Remover(i);
                }
            }
        }

        protected override void Validar(CrearCargaDeCupoNoGrano comando, Resultado resultado)
        {
            if (comando.Dto.Patente != "" && 
                comando.Dto.Patente != null && 
                Repositorio.Existe<CargaDeCupo>(
                e => e.Patente == comando.Dto.Patente
                && e.Centro.Id == comando.Dto.CentroId
                && (e.Recorrido != null && (!e.Recorrido.Rechazado || (e.Recorrido.Rechazado && !e.Recorrido.Terminado)))))
            {
                resultado.Error("Cupo1", $"El camion {comando.Dto.Patente} ya se encuentra en circuito");
            }
        }
    }
}
