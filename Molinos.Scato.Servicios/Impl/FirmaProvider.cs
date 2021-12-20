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
        private FirmaDto firma;
        private Byte[] logo;
        private Byte[] favicon;

        public FirmaProvider(IRepositorio repositorio, ILogger log)
        {
            this.repositorio = repositorio;
            this.log = log;
            RefrescarFirma();
        }

        public FirmaDto ObtenerFirmaSinLogo()
        {
            return firma;
        }

        public Byte[] ObtenerLogo()
        {
            return logo;
        }

        public Byte[] ObtenerFavicon()
        {
            return favicon;
        }

        public void RefrescarFirma()
        {
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
            logo = firmaCompleta.Logo ?? new byte[0];
            favicon = firmaCompleta.Favicon ?? new byte[0];
        }
    }
}
