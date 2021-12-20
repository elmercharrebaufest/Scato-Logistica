using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorActualizarEstadoConexion : ProcesadorComando<ActualizarEstadoConexion>
    {
        public ProcesadorActualizarEstadoConexion(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ActualizarEstadoConexion comando)
        {
            var resultado = new ResultadoActualizarEstadoConexion();
            resultado.EstadoConexionDto = new List<EstadoConexionDto>();
            Validar(comando, resultado);
            if (!resultado.HayErrores)
            {
                ModificarEntidad(comando, resultado);
                Repositorio.GuardarCambios();
            }

            return resultado;
        }

        protected void ModificarEntidad(ActualizarEstadoConexion comando, ResultadoActualizarEstadoConexion resultado)
        {
            var puestosDeTrabajo = Repositorio.Listar<PuestoDeTrabajo>(x => x.Lector == comando.Dto.Dispositivo);
            if (!puestosDeTrabajo.Any())
            {
                return;
            }
            foreach(var puestoDeTrabajo in puestosDeTrabajo)
            {
                resultado.EstadoConexionDto.Add(new EstadoConexionDto() {
                    PuestoDeTrabajoId = puestoDeTrabajo.Id,
                    CentroId = puestoDeTrabajo.Centro.Id,
                    Estado = comando.Dto.Estado,
                    Mensaje = comando.Dto.Mensaje
                });
                

                var estado = puestoDeTrabajo.Estados.LastOrDefault();
                if (estado == null)
                {
                    estado = new EstadoConexion
                    {
                        PuestoDeTrabajo = puestoDeTrabajo,
                    };

                    Repositorio.Agregar(estado);
                }
                estado.Estado = comando.Dto.Estado;
                estado.Mensaje = comando.Dto.Mensaje;
            }
            
        }

        protected void Validar(ActualizarEstadoConexion comando, ResultadoActualizarEstadoConexion resultado)
        {
        }
    }
}