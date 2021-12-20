using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorEliminarConversionCentro: ProcesadorEliminar<EliminarConversionCentro,ConversionCentro>
    {
        public ProcesadorEliminarConversionCentro(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override int IdEntidad(EliminarConversionCentro comando)
        {
            return comando.Id;
        }

        protected override void Validar(EliminarConversionCentro comando, Resultado resultado)
        {
        }
    }
}
