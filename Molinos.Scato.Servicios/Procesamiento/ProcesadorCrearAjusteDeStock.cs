using System;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearAjusteDeStock : ProcesadorCrear<CrearAjusteDeStock, AjusteDeStock>
    {
        public ProcesadorCrearAjusteDeStock(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override AjusteDeStock CrearEntidad(CrearAjusteDeStock comando)
        {
            var ajuste = Conversor.Convertir<AjusteDeStockDto, AjusteDeStock>(comando.Dto);
            ajuste.Material = Repositorio.Obtener<Material>(comando.Dto.MaterialId);
            ajuste.Centro = Repositorio.Obtener<Centro>(comando.Dto.CentroId);
            ajuste.TipoComprobanteOncca = Repositorio.Obtener<TipoComprobanteOncca>(comando.Dto.TipoComprobanteOnccaId);

            Repositorio.Agregar(new LogAjusteDeStock
            {
                Centro = ajuste.Centro,
                Fecha = DateTime.Now,
                FechaAjuste = ajuste.Fecha,
                Material = ajuste.Material,
                NumeroDocumentoIngreso = ajuste.NumeroDocumentoIngreso,
                Observaciones = ajuste.Observaciones,
                PesoBrutoIngreso = ajuste.PesoBrutoIngreso,
                PesoNetoEgreso = ajuste.PesoNetoEgreso,
                PesoNetoIngreso = ajuste.PesoNetoIngreso,
                Tipo = "Alta",
                TipoComprobanteOncca = ajuste.TipoComprobanteOncca,
                NombreUsuario = comando.Dto.NombreUsuario
            });
            return ajuste;
        }

        protected override void Validar(CrearAjusteDeStock comando, Resultado resultado)
        {
        }
    }
}
