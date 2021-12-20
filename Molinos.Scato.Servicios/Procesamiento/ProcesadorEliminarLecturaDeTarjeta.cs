using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarLecturaDeTarjeta : ProcesadorComando<EliminarLecturaDeTarjeta>
    {
        public ProcesadorEliminarLecturaDeTarjeta(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public sealed override Resultado Ejecutar(EliminarLecturaDeTarjeta comando)
        {
            var resultado = new Resultado();
            Validar(comando, resultado);
            if (!resultado.HayErrores)
            {
                var lectura = Repositorio.ObtenerMenor<LecturaDeTarjeta, int>(x => x.PuestoDeTrabajo.Id == comando.Id, x => x.Id);
                Repositorio.Remover<LecturaDeTarjeta>(lectura);

                try
                {
                    Repositorio.GuardarCambios();
                }
                catch (EntidadReferenciadaException)
                {
                    resultado.Error("", Textos.Error_EliminarReferenciado);
                }
                catch (Exception e)
                {
                    Log.Error(e, "Ocurrio al eliminar la entidad del tipo {0} - Id {1}", typeof(LecturaDeTarjeta).Name, lectura);
                    resultado.Error("", Textos.Error_ActualizarGenerico);
                }
            }

            return resultado;
        }

        protected void Validar(EliminarLecturaDeTarjeta comando, Resultado resultado)
        {
            if (!Repositorio.Existe<LecturaDeTarjeta>(x => x.PuestoDeTrabajo.Id == comando.Id))
            {
                resultado.Error("", Textos.Error_Invalido);
            }
        }
    }
}
