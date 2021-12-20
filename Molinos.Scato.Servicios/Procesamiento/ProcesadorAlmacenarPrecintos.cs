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
    public class ProcesadorAlmacenarPrecintos : ProcesadorComando<AlmacenarPrecintos>
    {
        public ProcesadorAlmacenarPrecintos(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(AlmacenarPrecintos comando)
        {
            var resultado = new Resultado();
            try
            {
                foreach (var precintoDto in comando.Precintos)
                {
                    if (precintoDto.Id != 0)
                    {
                        if (precintoDto.Eliminar)
                        {
                            Repositorio.Remover<Precinto>(precintoDto.Id);
                        }
                    }
                    else
                    {
                        if (!precintoDto.Eliminar)
                        {
                            var entidad = CrearEntidad(precintoDto);
                            Repositorio.Agregar(entidad);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Info(e, "Error en la carga de precintos");
                resultado.Error("", Textos.Precinto_ErrorEnLaCarga);
            }
            if (!resultado.HayErrores)
            {
                Repositorio.GuardarCambios();                
            }
            return resultado;
        }

        protected Precinto CrearEntidad(PrecintoDto precinto)
        {
           return Conversor.Convertir<PrecintoDto, Precinto>(precinto);
        }
    }
}
