using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarCasilleros : ProcesadorComando<EliminarCasilleros>
    {
        public ProcesadorEliminarCasilleros(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(EliminarCasilleros comando)
        {
            var resultado = new Resultado();
            bool hayCasilleroConMuestras = false;
            foreach (var id in comando.Id)
            {
                try
                {
                    if (Repositorio.Listar<MicroMuestrasPorCasillero>(x => x.Casillero.Id == id).Count > 0)
                    {
                        hayCasilleroConMuestras = true;
                    }
                    else
                    {
                        Repositorio.Remover<Casillero>(id);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "Error al eliminar el casillero {0}", comando.Id);
                    resultado.Error("", Textos.Casillero_ErrorEnLaCarga);
                }
            }

            if (hayCasilleroConMuestras)
            {
                resultado.Error("", Textos.Casillero_Ocupado);
            }

            if (!resultado.HayErrores)
            {
                Repositorio.GuardarCambios();
            }

            return resultado;
        }
    }
}
