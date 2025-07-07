using Molinos.Scato.Dominio;
using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Enums;
using Molinos.Scato.Dominio.Recursos;
using System;
using System.Collections.Generic;

namespace Molinos.Scato.Servicios.Impl
{
    public class ReglaExcepcionSojaImpo: IReglaExcepcionTasaMunicipal
    {
        private readonly IServicioRepositorio _servicioRepositorio;
        private readonly IServicioComandos _servicioComandos;

        public ReglaExcepcionSojaImpo(IServicioRepositorio servicioRepositorio, IServicioComandos servicioComandos)
        {
            _servicioRepositorio = servicioRepositorio ?? throw new ArgumentNullException(nameof(servicioRepositorio), "El servicio repositorio no puede ser nulo.");
            _servicioComandos = servicioComandos as IServicioComandos ?? throw new ArgumentNullException(nameof(servicioComandos), "El servicio comandos no puede ser nulo.");
        }
        public bool Aplica(DatosExcepcionTasaMunicipal datos)
        {
            bool aplica = false;
            if (!string.IsNullOrWhiteSpace(datos.Ctg)&&!string.IsNullOrWhiteSpace(datos.CodigoEstablecimiento))
            {
                aplica = ValidarPorCartaPorteElectronica(datos);
            }
            else if (datos.InstanceId.HasValue && datos.InstanceId.Value != Guid.Empty)
            {
                aplica = ValidarConRecorrido(datos);
            }

            return aplica;
        }

        public Dictionary<TipoValidacionPagoTasaMunicipal, bool> ValidarExcepcion(DatosExcepcionTasaMunicipal datos)
        {
            return new Dictionary<TipoValidacionPagoTasaMunicipal, bool>()
            {
                {TipoValidacionPagoTasaMunicipal.Abonado, true }
            };
        }

        private bool ValidarConRecorrido(DatosExcepcionTasaMunicipal datos)
        {
            var recorrido = _servicioRepositorio.ObtenerRecorridoPorGuid(datos.InstanceId.Value);
            if (!recorrido.Material.EsGrano)
                return false;

            if (recorrido?.Workflow == null || string.IsNullOrEmpty(recorrido.Patente) || recorrido.Id <= 0)
                throw new Exception("Recorrido inválido.");

            var cartaPorte = _servicioRepositorio.ObtenerCartaDePortePorrecorrido(recorrido.Id);
            if (cartaPorte == null)
                throw new ArgumentNullException(nameof(cartaPorte), "La carta de porte no puede ser nula.");

            return ValidarEsSojaImpo(cartaPorte?.TitularCartaPorteCodigoSap, cartaPorte?.CodEstab);
        }


        private bool ValidarPorCartaPorteElectronica(DatosExcepcionTasaMunicipal datos)
        {
            var cartaPorte =_servicioRepositorio.ObtenerCartaPorteElectronicaPorCTG(datos.Ctg);
            var cuitOrigen = cartaPorte?.CuitOrigen.ToString();
            var codigoSAPtitularCP = _servicioRepositorio.ObtenerProveedorPorCuit(ConvertirCuil(cuitOrigen), new TiposProveedor { PR = true }).CodigoSap;
            
           return ValidarEsSojaImpo(codigoSAPtitularCP, datos.CodigoEstablecimiento);
        }

        private bool ValidarEsSojaImpo(string titularCartaPorteCodigoSap, string codEstab)
        {
            if (!string.IsNullOrEmpty(titularCartaPorteCodigoSap)
                           && (titularCartaPorteCodigoSap == Constantes.ValoresPorDefecto.CodigoSapTPR
                               || (titularCartaPorteCodigoSap == Constantes.ValoresPorDefecto.CodigoSapACA
                                   && !string.IsNullOrEmpty(codEstab) && codEstab == Constantes.ValoresPorDefecto.EstablecimientoACA)))

                return true;
            else 
                return false;
        }

        private string ConvertirCuil(string cuil)
        {
            if (String.IsNullOrEmpty(cuil))
            {
                return "";
            }
            if (cuil.Length != 11)
            {
                throw new ArgumentException(Textos.DatoConLongitudIncorrecta);
            }

            string validador1 = cuil.Substring(0, 2);
            string documento = cuil.Substring(2, 8);
            string validador2 = cuil.Substring(10, 1);
            return validador1 + "-" + documento + "-" + validador2;
        }
    }

}
