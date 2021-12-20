using System;
using System.Linq;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Filtros;
using Molinos.Scato.Dominio.Helpers;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarTodoMaterialPorCentro : ProcesadorComando<ModificarTodoMaterialPorCentro>
    {
        public ProcesadorModificarTodoMaterialPorCentro(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        public override Resultado Ejecutar(ModificarTodoMaterialPorCentro comando)
        {
            var resultado = new Resultado();
            try
            {
                var materialEditado = Repositorio.Listar<MaterialPorCentro>(x => x.Centro.Id == comando.MaterialPorCentroDto.CentroId && !x.IgnoraContingencia);
                foreach (var mat in materialEditado)
                {
                    mat.ImprimeReciboMunicipal = comando.MaterialPorCentroDto.ImprimeReciboMunicipal;
                }
                Repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                resultado.Error("", e.Message);
            }
            return resultado;
        }

    }
}
