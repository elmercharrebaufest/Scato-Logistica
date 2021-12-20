using Molinos.Scato.Dominio.Comandos;
using Molinos.Scato.Dominio.Dto;
using Molinos.Scato.Dominio.Entidades;
using Molinos.Scato.Dominio.Recursos;
using Molinos.Scato.Repositorio;
using Molinos.Scato.Servicios.Conversiones;
using Ninject.Extensions.Logging;
using System;

namespace Molinos.Scato.Servicios.Procesamiento
{
    public class ProcesadorCrearAltaCTG : ProcesadorCrear<CrearAltaCTG, AltaCTG>
    {
        public ProcesadorCrearAltaCTG(IRepositorio repositorio, IConversor conversor, ILogger log)
            : base(repositorio, conversor, log)
        {
        }

        protected override AltaCTG CrearEntidad(CrearAltaCTG comando)
        {
            var dto = Conversor.Convertir<AltaCTGDto, AltaCTG>(comando.Dto);
            var cartaPorte = Repositorio.Obtener<CartaPorte>(comando.Dto.CartaPorteId);
            dto.CartaPorte = cartaPorte;
            cartaPorte.CTG = comando.Dto.CodigoCTG;
            cartaPorte.TarifaReferencia = comando.Dto.TarifaReferencia;
            if (comando.Dto.Cpe)
            {
                var recorrido = Repositorio.Obtener<Recorrido>(comando.Dto.RecorridoId);
                recorrido.NumeroDocumentoIngreso = comando.Dto.CodigoCTG;
                cartaPorte.Sucursal = Convert.ToInt32(comando.Dto.Sucursal);
                cartaPorte.CTG = comando.Dto.NroOrden;
                cartaPorte.NroCartaPorte = comando.Dto.CodigoCTG;
            }
            return dto;
        }

        protected override void Validar(CrearAltaCTG comando, Resultado resultado)
        {
            if (string.IsNullOrEmpty(comando.Dto.CodigoCTG))
            {
                resultado.Error("CodigoCTG", string.Format(Textos.Error_Requerido, "CodigoCTG"));
            }

            if(comando.Dto.Cpe)
            {
                if (string.IsNullOrEmpty(comando.Dto.Sucursal))
                {
                    resultado.Error("Sucursal", string.Format(Textos.Error_Requerido, "Sucursal"));
                }
                if (string.IsNullOrEmpty(comando.Dto.NroOrden))
                {
                    resultado.Error("NroOrden", string.Format(Textos.Error_Requerido, "NroOrden"));
                }
            }
        }
    }
}
