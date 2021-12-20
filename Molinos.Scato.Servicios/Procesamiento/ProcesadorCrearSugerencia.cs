using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearSugerencia : ProcesadorCrear<CrearSugerencia, Sugerencia>
    {
        public ProcesadorCrearSugerencia(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override Sugerencia CrearEntidad(CrearSugerencia comando)
        {
            var control = Conversor.Convertir<SugerenciaDto, Sugerencia>(comando.Dto);
            control.Fecha = DateTime.Now;
            return control;
        }

        protected override void Validar(CrearSugerencia comando, Resultado resultado)
        {
        }
    }
}
