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
    public class ProcesadorCrearCasilleros : ProcesadorComando<CrearCasilleros>
    {
        public ProcesadorCrearCasilleros(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(CrearCasilleros comando)
        {
            var resultado = new Resultado();
            bool existeCasillero = false;
            foreach (var casillero in comando.Dto)
            {
                try
                {
                    if (Repositorio.Obtener<Casillero>(x => x.Numero == casillero.Numero && x.Centro.Id == casillero.CentroId) != null)
                    {
                        existeCasillero = true;
                    }
                    else
                    {
                        var entidad = CrearEntidad(casillero);
                        Repositorio.Agregar(entidad);
                    }
                }
                catch (Exception)
                {
                    resultado.Error("", Textos.Casillero_ErrorEnLaCarga);
                }
            }

            if (existeCasillero)
            {
                resultado.Error("", Textos.Casillero_ExisteCasillero);
            }

            if (!resultado.HayErrores)
            {
                Repositorio.GuardarCambios();
            }
            return resultado;
        }

        protected Casillero CrearEntidad(CasilleroDto casillero)
        {
            var casilleroNuevo = Conversor.Convertir<CasilleroDto, Casillero>(casillero);
            casilleroNuevo.Centro = Repositorio.Obtener<Centro>(casillero.CentroId);

            return casilleroNuevo;
        }
    }
}
