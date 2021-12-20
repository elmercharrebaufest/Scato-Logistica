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
    public class ProcesadorCrearAnalisisObligatorio : ProcesadorCrear<CrearAnalisisObligatorio, AnalisisObligatorio>
    {
        public ProcesadorCrearAnalisisObligatorio(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override AnalisisObligatorio CrearEntidad(CrearAnalisisObligatorio comando)
        {
            var usarioEditado = Conversor.Convertir<AnalisisObligatorioDto, AnalisisObligatorio>(comando.Dto);

            IList<PuestoDeTrabajo> rolesActuales = comando.Dto.PuestosDeTrabajoAsociados.Select(x => Repositorio.Obtener<PuestoDeTrabajo>(x.Id)).ToList();
            usarioEditado.PuestosDeTrabajoAsociados = rolesActuales;
            usarioEditado.Material = Repositorio.Obtener<Material>(comando.Dto.MaterialId);
            usarioEditado.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);

            return usarioEditado;
        }

        protected override void Validar(CrearAnalisisObligatorio comando, Resultado resultado)
        {
            if (Repositorio.Existe<AnalisisObligatorio>(e => e.Material.Id == comando.Dto.MaterialId && e.Centro.Id == comando.Dto.CentroId ))
            {
                resultado.Error("MaterialDescripcion", Textos.AnalisisObligatorio_Existente);
            }
        }
    }
}
