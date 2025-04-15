using log4net.Core;
using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Dto.OperacionesAPI;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Newtonsoft.Json;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;


namespace Molinos.Scato.Servicios
{
    internal class ServicioSapMServicioOperacionesMock : IServicioOperaciones
    {
        protected IRepositorio Repositorio { get; private set; }
        protected IConversor Conversor { get; private set; }

        public ServicioSapMServicioOperacionesMock(IRepositorio repositorio, IConversor conversor)
        {
            Repositorio = repositorio;
            Conversor = conversor;
        }


        public void InformarViajeOrdenesDeCargaFason(IngresosEgresosFasonesDto ingresosEgresosFasonesDto)
        {
            throw new NotImplementedException();
        }

        public void InformarViajeOrdenesResiduos(IngresosEgresosResiduosDto ingresosEgresosResiduosDto)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OrdenDeCargaDto> ObtenerOrdenesDeCarga(string patente)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OrdenDeCargaDto> ObtenerOrdenesDeCargaFas(string patente)
        {

            var confiOperacionesDummy = Repositorio.Obtener<Dominio.Entidades.ConfiguracionGeneral>(x => x.Pantalla == Constantes.ConfiguracionGeneral.Pantalla.ServicioOperaciones && x.Nombre == Constantes.ConfiguracionGeneral.Servicios.OperacionesDummyResponse && x.CentroId == null);

            if (string.IsNullOrEmpty(confiOperacionesDummy.Valor))
            {
                return Enumerable.Empty<OrdenDeCargaDto>();
            }

            byte[] bytes = Convert.FromBase64String(confiOperacionesDummy.Valor);
            string jsonMock = Encoding.UTF8.GetString(bytes);
           
            jsonMock = Regex.Replace(jsonMock, @"\s+", "");

            return JsonConvert.DeserializeObject<IEnumerable<OrdenDeCargaDto>>(jsonMock);
        }

        public IEnumerable<OrdenResiduosDto> ObtenerOrdenesResiduos(string patente)
        {
            throw new NotImplementedException();
        }
    }
}
