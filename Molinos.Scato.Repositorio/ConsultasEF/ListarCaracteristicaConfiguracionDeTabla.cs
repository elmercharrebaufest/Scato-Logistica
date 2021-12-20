using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;

namespace Molinos.Scato.Repositorio.ConsultasEF
{
    public class ListarCaracteristicaConfiguracionDeTabla : IConsulta<string>
    {
        private readonly int materialId;
        private readonly int centroId;
        private readonly string usuario;

        public ListarCaracteristicaConfiguracionDeTabla(int materialId, int centroId, string usuario)
        {
            this.materialId = materialId;
            this.centroId = centroId;
            this.usuario = usuario;
        }

        List<string> IConsulta<string>.Ejecutar(DbContext contexto)
        {
            var configuraciones = contexto.Set<ConfiguracionDeTabla>().Where(x => (materialId == 0 || x.Material.Id == materialId) && x.Centro.Id == centroId && x.Usuario.NombreUsuario == usuario);

            return configuraciones.SelectMany(x => x.CaracteristicasDeCalidad).Select(x => x.CaracteristicaDeCalidadMaestro.Descripcion).ToList();
        }
    }
}
