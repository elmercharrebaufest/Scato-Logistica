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
    public class ProcesadorBloquearTarjeta : ProcesadorCrear<BloquearTarjeta, TarjetaBloqueada>
    {
        public ProcesadorBloquearTarjeta(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override TarjetaBloqueada CrearEntidad(BloquearTarjeta comando)
        {
            var editado = Conversor.Convertir<TarjetaBloqueadaDto, TarjetaBloqueada>(comando.Dto);
            editado.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            return editado;
        }

        protected override void Validar(BloquearTarjeta comando, Resultado resultado)
        {
            var date = DateTime.Now.Date;
            if (Repositorio.Existe<TarjetaBloqueada>(e => e.Numero == comando.Dto.Numero && (e.Id != comando.Dto.Id) && e.Centro.Id == comando.Dto.CentroId))
            {
                resultado.Error("Numero", Textos.TarjetaBloqueada_ErrorTarjetaIngresada);
            }
            else if (!Repositorio.Existe<TarjetaRango>(t => ((t.Codigo == comando.Dto.Numero.Substring(0, 5)) & ((t.RangoDesde.CompareTo(comando.Dto.Numero.Substring(5, 5)) <= 0) && (t.RangoHasta.CompareTo(comando.Dto.Numero.Substring(5, 5)) >= 0))
                & (t.ValidoDesde <= date && t.ValidoHasta >= date)) && t.Centro.Id == comando.Dto.CentroId))
            {
                resultado.Error("Numero", Textos.TarjetaBloqueada_ErrorInexistenteNoVigente);
            }
        }
    }
}
