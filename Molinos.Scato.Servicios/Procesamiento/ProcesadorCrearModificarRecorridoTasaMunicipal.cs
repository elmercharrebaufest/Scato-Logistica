using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearModificarRecorridoTasaMunicipal : ProcesadorComando<CrearModificarRecorridoTasaMunicipal>
    {
        public ProcesadorCrearModificarRecorridoTasaMunicipal(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearModificarRecorridoTasaMunicipal comando)
        {
            var resultado = new Resultado();

            try
            {
                var recorridoTasaMunicipal = Repositorio.Obtener<RecorridoTasaMunicipal>(comando.Id);
                if (recorridoTasaMunicipal == null)
                {
                    recorridoTasaMunicipal = new RecorridoTasaMunicipal()
                    {
                        Id = comando.Id,
                        Exceptuado = comando.TieneExcepcion,
                        MotivoExceptuado = comando.MotivoExcepcion
                    };
                    Repositorio.Agregar(recorridoTasaMunicipal);
                }
                else
                {
                    recorridoTasaMunicipal.Exceptuado = comando.TieneExcepcion;
                    recorridoTasaMunicipal.MotivoExceptuado = comando.MotivoExcepcion;
                }

                Repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex);
                resultado.Error(string.Empty, Textos.Error_ActualizarGenerico);
            }

            return resultado;
        }
    }
}