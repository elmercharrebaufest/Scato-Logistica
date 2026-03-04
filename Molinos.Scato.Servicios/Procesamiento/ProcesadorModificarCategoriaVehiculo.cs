using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;
using System.Linq.Expressions;


namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorModificarCategoriaVehiculo : ProcesadorModificar<ModificarCategoriaVehiculo>
    {
        public ProcesadorModificarCategoriaVehiculo(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override void ModificarEntidad(ModificarCategoriaVehiculo comando)
        {
            var vehiculo = Repositorio.Obtener<CategoriaVehiculo>(comando.Dto.Id);
            vehiculo.Patente = comando.Dto.Patente;
            vehiculo.PatenteAcoplado = !string.IsNullOrEmpty(comando.Dto.PatenteAcoplado) ? comando.Dto.PatenteAcoplado : string.Empty;
            vehiculo.PatenteAcoplado2 = !string.IsNullOrEmpty(comando.Dto.PatenteAcoplado2) ? comando.Dto.PatenteAcoplado2 : string.Empty;
            vehiculo.TipoVehiculo = comando.Dto.TipoVehiculo;
        }

        protected override void Validar(ModificarCategoriaVehiculo comando, Resultado resultado)
        {
            if (!Repositorio.Existe<CategoriaVehiculo>(x => x.Id == comando.Dto.Id))
            {
                resultado.Error(nameof(CategoriaVehiculo.Id), Textos.Error_NoExistenDatos);
                return;
            }

            Expression<Func<CategoriaVehiculo, bool>> filter =
                x => x.Id != comando.Dto.Id
                    && x.Patente == comando.Dto.Patente
                    && (string.IsNullOrEmpty(comando.Dto.PatenteAcoplado) || x.PatenteAcoplado == comando.Dto.PatenteAcoplado)
                    && (string.IsNullOrEmpty(comando.Dto.PatenteAcoplado2) || x.PatenteAcoplado2 == comando.Dto.PatenteAcoplado2);

            if (Repositorio.Existe(filter))
                resultado.Error(nameof(CategoriaVehiculo.Patente), Textos.CategoriaCamiones_PatenteExistente);
        }
    }
}
