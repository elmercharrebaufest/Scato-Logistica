using System.Collections.Generic;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarAnalisisObligatorio : ProcesadorModificar<ModificarAnalisisObligatorio>
    {
        public ProcesadorModificarAnalisisObligatorio(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarAnalisisObligatorio comando)
        {
            var analisisObligatorioEditado = Repositorio.Obtener<AnalisisObligatorio>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, analisisObligatorioEditado);

            analisisObligatorioEditado.PuestosDeTrabajoAsociados.Clear();
            IList<PuestoDeTrabajo> puestosActuales = comando.Dto.PuestosDeTrabajoAsociados.Select(centroDto => Repositorio.ObtenerUnchanged<PuestoDeTrabajo>(centroDto.Id)).ToList();

            analisisObligatorioEditado.PuestosDeTrabajoAsociados = puestosActuales;
        }

        protected override void Validar(ModificarAnalisisObligatorio comando, Resultado resultado)
        {
            if (Repositorio.Existe<AnalisisObligatorio>(e => e.Material.Id == comando.Dto.MaterialId && e.Centro.Id == comando.Dto.CentroId && (e.Id != comando.Dto.Id)))
            {
                resultado.Error("MaterialDescripcion", Textos.AnalisisObligatorio_Existente);
            }
        }
    }
}
