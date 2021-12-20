using System;
using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearTarjetaSupervisor : ProcesadorCrear<CrearTarjetaSupervisor, TarjetaSupervisor>
    {
        public ProcesadorCrearTarjetaSupervisor(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override TarjetaSupervisor CrearEntidad(CrearTarjetaSupervisor comando)
        {
            var editado = Conversor.Convertir<TarjetaSupervisorDto, TarjetaSupervisor>(comando.Dto);
            IList<PuestoDeTrabajo> puestosActuales = comando.Dto.PuestosDeTrabajoAsociados.Select(puestoDto => Repositorio.Obtener<PuestoDeTrabajo>(puestoDto.Id)).ToList();
            editado.PuestosDeTrabajoAsociados = puestosActuales;

            editado.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            return editado;
        }

        protected override void Validar(CrearTarjetaSupervisor comando, Resultado resultado)
        {
            if (Repositorio.Existe<TarjetaSupervisor>(x => x.Numero == comando.Dto.Numero && (x.Id != comando.Dto.Id) && x.Centro.Id == comando.Dto.CentroId))
            {
                resultado.Error("Numero", Textos.TarjetaSupervisor_NumeroExistente);                
            }
            var date = DateTime.Now.Date;
            if (!Repositorio.Existe<TarjetaRango>(x => ((((x.Codigo + x.RangoDesde).CompareTo(comando.Dto.Numero) <= 0) && (x.Codigo + x.RangoHasta).CompareTo(comando.Dto.Numero) >= 0))
                & (x.ValidoDesde <= date && x.ValidoHasta >= date) && x.Centro.Id == comando.Dto.CentroId))
            {
                resultado.Error("Numero", Textos.TarjetaSupervisor_NumeroFueraDeRangoONoVigente);
            }
        }
    }
}
