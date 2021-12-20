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
    public class ProcesadorCrearCiuAnulado : ProcesadorCrear<CrearCiuAnulado, CiuAnulado>
    {
        public ProcesadorCrearCiuAnulado(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override CiuAnulado CrearEntidad(CrearCiuAnulado comando)
        {
            return Conversor.Convertir<CiuAnuladoDto, CiuAnulado>(comando.Dto);
        }

        protected override void Validar(CrearCiuAnulado comando, Resultado resultado)
        {
            if (comando.Dto.Fecha > DateTime.Today)
            {
                resultado.Error("Fecha", Textos.CiuAnulado_ErrorFecha);
            }
            if (Repositorio.Existe<CiuAnulado>(e => e.Numero == comando.Dto.Numero && (e.Id != comando.Dto.Id)))
            {
                resultado.Error("Numero", Textos.CiuAnulado_NumeroExistente);
            }
        }
    }
}
