using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarTarjetaSupervisor : ProcesadorModificar<ModificarTarjetaSupervisor>
    {
        public ProcesadorModificarTarjetaSupervisor(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarTarjetaSupervisor comando)
        {
            var tarjetaEditada = Repositorio.Obtener<TarjetaSupervisor>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, tarjetaEditada);

            tarjetaEditada.PuestosDeTrabajoAsociados.Clear();
            IList<PuestoDeTrabajo> puestosActuales = comando.Dto.PuestosDeTrabajoAsociados.Select(puestoDto => Repositorio.ObtenerUnchanged<PuestoDeTrabajo>(puestoDto.Id)).ToList();

            tarjetaEditada.PuestosDeTrabajoAsociados = puestosActuales;
        }

        protected override void Validar(ModificarTarjetaSupervisor comando, Resultado resultado)
        {
            if (Repositorio.Existe<TarjetaSupervisor>(e => e.Numero == comando.Dto.Numero && (e.Id != comando.Dto.Id)))
            {
                resultado.Error("Descripcion", Textos.TarjetaSupervisor_NumeroExistente);
            }

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
