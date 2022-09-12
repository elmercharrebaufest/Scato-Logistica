using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearBajaCTG : ProcesadorCrear<CrearBajaCTG, BajaCTG>
    {
        public ProcesadorCrearBajaCTG(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override BajaCTG CrearEntidad(CrearBajaCTG comando)
        {
            var baja = Repositorio.Obtener<BajaCTG>(x => x.WorkflowId == comando.Dto.WorkflowId);

            if (baja == null)
            {
                baja = Conversor.Convertir<BajaCTGDto, BajaCTG>(comando.Dto);
            }
            else
            {
                baja.CodigoDeBaja = comando.Dto.CodigoDeBaja;
                baja.Fecha = DateTime.Now;
            }

            baja.CartaPorte = Repositorio.Obtener<CartaPorte>(x => x.Id == comando.Dto.CartaPorteId);
            return baja;
        }

        protected override void Validar(CrearBajaCTG comando, Resultado resultado)
        {
        }
    }
}