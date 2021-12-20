using System.Collections.Generic;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearModificarEstacionMeteorologica : ProcesadorModificar<CrearModificarEstacionMeteorologica>
    {
        public ProcesadorCrearModificarEstacionMeteorologica(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(CrearModificarEstacionMeteorologica comando)
        {
            var centro = Repositorio.Obtener<Centro>(comando.Dto.Id);
            var centroDto = Conversor.Convertir<Centro, CentroDto>(centro);
            centroDto.CodigoEstacionMeteorologica = comando.Dto.CodigoEstacionMeteorologica;
            Conversor.Convertir(centroDto, centro);
        }

        protected override void Validar(CrearModificarEstacionMeteorologica comando, Resultado resultado)
        {
        }
    }
}