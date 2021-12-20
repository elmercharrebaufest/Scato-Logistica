using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarCasilleros : ProcesadorComando<ModificarCasilleros>
    {
        public ProcesadorModificarCasilleros(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ModificarCasilleros comando)
        {
            var resultado = new Resultado();
            bool hayErrorCapacidad = false;
            foreach (var casillero in comando.Dto)
            {
                try
                {
                    if (Repositorio.Listar<MicroMuestrasPorCasillero>(x => x.Casillero.Id == casillero.Id).Count <= comando.Capacidad)
                    {
                        casillero.Capacidad = comando.Capacidad;
                        ModificarEntidad(casillero);
                    }
                    else
                    {
                        hayErrorCapacidad = true;
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "Errod al modificar los casilleros");
                    resultado.Error("", Textos.Casillero_ErrorEnLaCarga);
                }
            }

            if (hayErrorCapacidad)
            {
                resultado.Error("", Textos.Casillero_ErrorCapacidad);
            }

            if (!resultado.HayErrores)
            {
                Repositorio.GuardarCambios();
            }

            return resultado;
        }

        protected void ModificarEntidad(CasilleroDto casillero)
        {
            var casilleroEditado = Repositorio.Obtener<Casillero>(casillero.Id);
            Conversor.Convertir(casillero, casilleroEditado);
        }
    }
}
