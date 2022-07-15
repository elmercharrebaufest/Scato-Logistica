using System;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Repositorio;
using Ninject.Extensions.Logging;

namespace Molinos.Scato.Servicios.Impl
{
    public class FirmaProvider : IFirmaProvider
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger log;
        private readonly ICache cache;
        private FirmaDto firma;
        private Byte[] logo;
        private Byte[] favicon;

        public FirmaProvider(IRepositorio repositorio, ILogger log, ICache cache)
        {
            this.repositorio = repositorio;
            this.log = log;
            this.cache = cache;
            RefrescarFirma();
        }

        public FirmaDto ObtenerFirmaSinLogo()
        {
            return cache.Existe("FirmaProvider:firma") ? cache.Obtener<FirmaDto>("FirmaProvider:firma") : firma;
        }

        public Byte[] ObtenerLogo()
        {
            return cache.Existe("FirmaProvider:logo") ? cache.Obtener<Byte[]>("FirmaProvider:logo") : logo;
        }

        public Byte[] ObtenerFavicon()
        {
            return cache.Existe("FirmaProvider:favicon") ? cache.Obtener<Byte[]>("FirmaProvider:favicon") : favicon;
        }

        public void RefrescarFirma()
        {
            if (!cache.Existe("FirmaProvider:firma") || !cache.Existe("FirmaProvider:logo") || !cache.Existe("FirmaProvider:favicon"))
            {
                cache.RemoverTodos();
                var firmaCompleta = repositorio.ObtenerProyeccion((Firma x) => true, x => new { 
                    Firma = new FirmaDto
                    {
                        Ciudad = x.Ciudad,
                        CodigoSAP = x.CodigoSAP,
                        Cuit = x.Cuit,
                        Descripcion = x.Descripcion,
                        DescripcionCorta = x.DescripcionCorta,
                        Direccion = x.Direccion,
                        RazonSocial = x.RazonSocial,
                        FechaDeInicio = x.FechaDeInicio,
                        IngBrutosConvMultilateral = x.IngBrutosConvMultilateral
                    }
                , x.Logo ,x.Favicon});
                firma = firmaCompleta.Firma ?? new FirmaDto();
                cache.Agregar("FirmaProvider:firma", firma, DateTimeOffset.Now.AddHours(12));
                logo = firmaCompleta.Logo ?? new byte[0];
                cache.Agregar("FirmaProvider:logo", logo, DateTimeOffset.Now.AddHours(12));
                favicon = firmaCompleta.Favicon ?? new byte[0];
                cache.Agregar("FirmaProvider:favicon", favicon, DateTimeOffset.Now.AddHours(12));
            }
        }
    }
}
