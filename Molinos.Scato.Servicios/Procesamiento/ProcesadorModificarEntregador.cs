using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarEntregador : ProcesadorModificar<ModificarEntregador>
    {
        public ProcesadorModificarEntregador(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarEntregador comando)
        {
            var entregador = Repositorio.Obtener<Entregador>(comando.Dto.Id);
            Conversor.Convertir(comando.Dto, entregador);
            if (comando.Dto.PaisId.HasValue && (entregador.Pais == null || entregador.Pais.Id != comando.Dto.PaisId))
            {
                entregador.Pais = Repositorio.Obtener<Pais>(comando.Dto.PaisId);
            }
            if (comando.Dto.ProvinciaId.HasValue &&
                (entregador.Provincia == null || entregador.Provincia.Id != comando.Dto.ProvinciaId))
            {
                entregador.Provincia = Repositorio.Obtener<Provincia>(comando.Dto.ProvinciaId);
            }
            if (comando.Dto.LocalidadId.HasValue &&
                (entregador.Localidad == null || entregador.Localidad.Id != comando.Dto.LocalidadId))
            {
                entregador.Localidad = Repositorio.Obtener<Localidad>(comando.Dto.LocalidadId);
            }
        }

        protected override void Validar(ModificarEntregador comando, Resultado resultado)
        {
            if (
                Repositorio.Existe<Entregador>(
                    x =>
                    x.Id != comando.Dto.Id && (x.DescripcionCorta == comando.Dto.DescripcionCorta)))
            {
                resultado.Error("DescripcionCorta", Textos.Entregador_DescripcionCortaExistente);
            }

            if (
                Repositorio.Existe<Entregador>(
                    x =>
                    x.Id != comando.Dto.Id && (x.RazonSocial == comando.Dto.RazonSocial)))
            {
                resultado.Error("RazonSocial", Textos.Entregador_RazonSocial);
            }

            if (
                Repositorio.Existe<Entregador>(
                    x =>
                    x.Id != comando.Dto.Id && x.Cuil == comando.Dto.Cuil))
            {
                resultado.Error("Cuil", Textos.Entregador_Cuil);
            }
        }
    }
}
