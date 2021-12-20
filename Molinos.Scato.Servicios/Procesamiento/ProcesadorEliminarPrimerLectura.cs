using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarPrimerLectura : ProcesadorComando<EliminarPrimerLectura>
    {
        public ProcesadorEliminarPrimerLectura(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(EliminarPrimerLectura comando)
        {
            var resultado = new ResultadoEliminarPrimerLectura();

            try
            {
                var lecturaActual = Repositorio.ObtenerMenor<LecturaDeTarjeta, int>(x => x.PuestoDeTrabajo.Id == comando.PuestoDeTrabajoId, x => x.Id);
                if (lecturaActual != null)
                {
                    Repositorio.Remover(lecturaActual);
                    Repositorio.GuardarCambios();
                }
                var lecturaSiguiente = Repositorio.ObtenerMenor<LecturaDeTarjeta, int>(x => x.PuestoDeTrabajo.Id == comando.PuestoDeTrabajoId, x => x.Id);
                if (lecturaSiguiente != null)
                {
                    resultado.ProximaLectura = lecturaSiguiente.Lectura;
                    resultado.ProximaPatente = lecturaSiguiente.Patente;
                    resultado.ProximaPatenteLeida = lecturaSiguiente.PatenteLeida;
                    resultado.OcrActivo = lecturaSiguiente.OcrActivo;
                    resultado.ReconocimientoExitoso = lecturaSiguiente.ReconocimientoExitoso;
                }
            }
            catch (EntidadReferenciadaException)
            {
                resultado.Error("", Textos.Error_EliminarReferenciado);
            }
            catch (Exception e)
            {
                Log.Error(e, "Ocurrio al eliminar la entidad del tipo {0} - Id {1}", typeof(LecturaDeTarjeta).Name, comando.PuestoDeTrabajoId);
                resultado.Error("", Textos.Error_ActualizarGenerico);
            }

            return resultado;
        }
    }
}
