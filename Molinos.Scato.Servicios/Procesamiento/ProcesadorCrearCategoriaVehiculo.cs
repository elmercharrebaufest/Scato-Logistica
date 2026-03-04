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
    public class ProcesadorCrearCategoriaVehiculo : ProcesadorCrear<CrearCategoriaVehiculo, CategoriaVehiculo>
    {
        public ProcesadorCrearCategoriaVehiculo(IRepositorio repositorio, IConversor conversor, ILogger log) : base(repositorio, conversor, log)
        {
        }

        protected override CategoriaVehiculo CrearEntidad(CrearCategoriaVehiculo comando)
        {
            return new CategoriaVehiculo
            {
                Patente = comando.Dto.Patente,
                PatenteAcoplado = !string.IsNullOrEmpty(comando.Dto.PatenteAcoplado) ? comando.Dto.PatenteAcoplado : string.Empty,
                PatenteAcoplado2 = !string.IsNullOrEmpty(comando.Dto.PatenteAcoplado2) ? comando.Dto.PatenteAcoplado2 : string.Empty,
                TipoVehiculo = comando.Dto.TipoVehiculo
            };
        }

        protected override void Validar(CrearCategoriaVehiculo comando, Resultado resultado)
        {
            Expression<Func<CategoriaVehiculo, bool>> filter =
                x => x.Patente == comando.Dto.Patente
                    && (string.IsNullOrEmpty(comando.Dto.PatenteAcoplado) || x.PatenteAcoplado == comando.Dto.PatenteAcoplado)
                    && (string.IsNullOrEmpty(comando.Dto.PatenteAcoplado2) || x.PatenteAcoplado2 == comando.Dto.PatenteAcoplado2);

            if (Repositorio.Existe(filter))
                resultado.Error(nameof(CategoriaVehiculo.Patente), Textos.CategoriaCamiones_PatenteExistente);
        }
    }
}
